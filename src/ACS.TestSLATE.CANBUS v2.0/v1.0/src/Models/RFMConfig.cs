// <copyright file="RFMConfig.cs" company="Jacobs Technology, Inc.">
//     Copyright (c) Jacobs Technology, Inc. All rights reserved.
// </copyright>
// <summary> This class implements a collections of NamedConfigItems which are stored in ExtensionDocs
// Collections utililzing .NET 4.0 can inherit from ObservableCollection, if the dll has to use .NET 3.5, then
//          collections will need to inherit from CollectionBase and implement IBindingList.
//          Doing so will make collection's changes obseverable by the grid through event notifications.
// </summary>

using Customer.TestSLATE.Mnemonic.Presenters;

namespace Customer.TestSLATE.Mnemonic.Models
{
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using Jacobs.TestSLATE.Commands.Interfaces;
    using Jacobs.TestSLATE.DB;
    using System;
    using System.Xml.Serialization;
    using Customer.TestSLATE.Mnemonic.Interop.Database;
    using TSSource = Jacobs.TestSLATE.Cell.DomainLayer.Sources.ObjectClass;
    using Jacobs.TestSLATE.ExtensionDoc;
    using Customer.TestSLATE.Mnemonic.Properties;

    /// <summary>
    /// A collection of Reflective Memory Configuration Items (NamedConfigItem)
    /// </summary>
    [Serializable]
    [XmlRoot]
    public class RFMConfig : ObservableCollection<NamedConfigItem>, IItemCollectionModel 
    {
        /// <summary>
        /// The Cell Id.
        /// </summary>
        private readonly int _cellId;

        /// <summary>
        /// The Source Id.
        /// </summary>
        private readonly int _srcId;

        ///<summary>
        ///</summary>
        public ExtensionDoc []_doc;

        private TSSource _source;

        private bool _IsDirty;



        /// <summary>
        /// Initializes a new instance of the <see cref="Channels&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="cellId">The cell id.</param>
        /// <param name="srcId">The Source id.</param>
        public RFMConfig(int cellId, int srcId)
        {
            this._cellId = cellId;
            this._srcId = srcId;
            var environment = EnvironmentFactory.CellEnvironment();
            var tsSources = environment.Sources.SelectByCellIDAndSourceID(this._cellId, this._srcId);

            foreach (var source in tsSources)
            {
                 _source = source;
            }
            this._doc = new ExtensionDoc[19];
            for (int i = 0; i < 19;i++ )
                this._doc[i] = ExtensionDoc.Empty;
            this._IsDirty = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Channels&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="cellId">The cell id.</param>
        /// <param name="srcId">The Source id.</param>
        /// <param name="value"></param>
        public RFMConfig(int cellId, int srcId, string value)
        {
            NamedConfigItem NamedConfigItem;
            this._cellId = cellId;
            this._srcId = srcId;
            this._doc = new ExtensionDoc[19];
            for (int i = 0; i < 19;i++ )
                this._doc[i] = ExtensionDoc.Empty;
            var environment = EnvironmentFactory.CellEnvironment();
            var tsSources = environment.Sources.SelectByCellIDAndSourceID(this._cellId, this._srcId);

            foreach (var source in tsSources)
            {
                _source = source;
            }
            NamedConfigItem = new NamedConfigItem("0","0");
            this.Add(NamedConfigItem);
            this._IsDirty = false;
        }
        ///<summary>
        ///</summary>
        ///<param name="index"></param>
        ///<returns></returns>
        public NamedConfigItem GetItemAt (int index)
        {
            int nCount;
            nCount = this.Count;
            if (index < nCount)
            {
                var NamedConfigItem = this.ElementAt(index);
                return NamedConfigItem;
            }
            return null;
        }

        /// <summary>
        /// Gets a value indicating whether this instance has error.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has error; otherwise, <c>false</c>.
        /// </value>
        public bool HasError
        {
            get { return this.Any(channel => !channel.Validator.IsValid); }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is dirty.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is dirty; otherwise, <c>false</c>.
        /// </value>
        public bool IsDirty
        {
            get { return _IsDirty; }
            set { this._IsDirty = value; }
        }

        /// <summary>
        /// Adds the new.
        /// </summary>
        /// <returns></returns>
        public UndoableCommand AddNew(NamedConfigItem NamedConfigItem)
        {
            this.Add(NamedConfigItem);
            return null;
        }

        /// <summary>
        /// Updates the value at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="stringValue"></param>
        /// <param name="intValue"></param>
        /// <param name="floatValue"></param>
        /// <returns></returns>
        public UndoableCommand UpdateValue(int index, string stringValue, int intValue, float floatValue)
        {
            var NamedConfigItem = this.GetItemAt(index);
            NamedConfigItem.StringValue = stringValue;
            NamedConfigItem.IntValue = intValue;
            NamedConfigItem.FloatValue = floatValue;
            this.SetItem(index, NamedConfigItem);
            this.IsDirty = true;
            return null;
        }


        /// <summary>
        /// Saves the specified cell id.
        /// </summary>
        public void Save()
        {
            int index;
            //            TSSource source;
            var environment = EnvironmentFactory.CellEnvironment();
            var tsSources = environment.Sources.SelectByCellIDAndSourceID(this._cellId, this._srcId);

            foreach (var source  in tsSources)
            {
                index = 0;
                foreach (var b in this)
                {
                    ExtensionDocManager.SaveToExtensionDoc(source, ref this._doc[index], b);
                    index++;
                }
            }
            this._IsDirty = false;
        }

        /// <summary>
        /// Gets the error message.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        public string GetErrorMessage(int index, string propertyName)
        {
            return this[index].Validator[propertyName];
        }

        /// <summary>
        /// Gets a value indicating whether this instance is valid.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is valid; otherwise, <c>false</c>.
        /// </value>
        public bool IsValid
        {
            get { return this.All(channel => channel.Validator.IsValid); }
        }

        /// <summary>
        /// Validates the Collection of Channels
        /// </summary>
        /// <returns></returns>
        public bool Validate()
        {
            return this.All(channel => channel.Validator.IsValid);
        }
    }
}
