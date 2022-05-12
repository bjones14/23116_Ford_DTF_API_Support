// <copyright file="ExtensionDocManager.cs" company="Jacobs Technology, Inc.">
//     Copyright (c) Jacobs Technology, Inc. All rights reserved.
// </copyright>
// <summary>
//          This class is a sample Extension Doc Manager. It is a sealed (static) class that contains methods for 
//          Loading data from an extension doc into a data model and for saving the data model to an extension doc into the database.
//          Extension docs require your data model to implement [Serializable]
// </summary>

namespace Customer.TestSLATE.Mnemonic.Interop.Database
{
    using System.Linq;
    using Jacobs.TestSLATE.ExtensionDoc;
    using Customer.TestSLATE.Mnemonic.Models;
    using TSConfig = Jacobs.TestSLATE.Cell.DomainLayer.Configurations.ObjectClass;
    using TSSource = Jacobs.TestSLATE.Cell.DomainLayer.Sources.ObjectClass;

    /// <summary>
    ///   This class manages access to the Extension Docs
    /// </summary>
    public sealed class ExtensionDocManager
    {
        ///<summary>
        ///  Loads a collection of NamedConfigItems from the extension Docs
        ///  Modify return type to match your object class.
        ///</summary>
        public static RFMConfig LoadFromExtensionDoc(TSSource source, ref ExtensionDoc extensionDoc)
        {
            int index;
            NamedConfigItem NamedConfigItem;
            IExtensionDocRepository docRepository = new ExtensionDocRepository();
            var documents = docRepository.GetByExtends(typeof(TSSource).AssemblyQualifiedName, source.GetTSUID());
            if (documents.Count > 0)
            {
                // This example uses RFMConfig class and NamedConfigItem class to retrieve Reflective Memory Configuration
                // from ExtensionDocs
                var RFMConfig = new RFMConfig(source.CellID, source.SourceID);
                index = 0;
                foreach (var doc in documents.Where(doc => doc != null))
                {
                    extensionDoc = doc;
                    // If you're not expecting multiple extension docs, you will return here

                    // Bundle thie NamedConfigItem object into RFMConfig Collection.
                    NamedConfigItem = (NamedConfigItem)doc.GetObjectFromData(typeof(NamedConfigItem));
                    RFMConfig._doc[index] = doc;
                    RFMConfig.Add(NamedConfigItem);
                    index++;
                }
                return (RFMConfig);
            }

            return null;
        }

        /// <summary>
        ///   Saves NamedConfigItem to extension doc.
        /// </summary>
        public static void SaveToExtensionDoc(TSSource source, ref ExtensionDoc extensionDoc, NamedConfigItem item)
        {
            IExtensionDocRepository docRepository = new ExtensionDocRepository();
            if (extensionDoc == ExtensionDoc.Empty)
            {
                var newDoc = new ExtensionDoc
                {
                    ExtendsType = typeof(TSSource).AssemblyQualifiedName,
                    ExtendsKey = source.GetTSUID()
                };

                newDoc.SetDataAsXml(item);
                docRepository.Add(newDoc);
            }
            else
            {
                extensionDoc.SetDataAsXml(item);
                docRepository.Update(extensionDoc);
            }
        }
        /// <summary>
        ///   Deletes all ExtensionDocs associated with specified source.
        /// </summary>
        public static void DeleteFromExtensionDoc(TSSource source)
        {
            IExtensionDocRepository docRepository = new ExtensionDocRepository();
            var documents = docRepository.GetByExtends(typeof(TSSource).AssemblyQualifiedName, source.GetTSUID());
            if (documents.Count > 0)
            {
                foreach (var doc in documents.Where(doc => doc != null))
                {
                    docRepository.Remove(doc);
                }
            }
        }
    }
}