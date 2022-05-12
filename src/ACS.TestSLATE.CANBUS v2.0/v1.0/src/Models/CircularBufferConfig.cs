// <copyright file="CircularBufferConfig.cs" company="Jacobs Technology, Inc.">
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
    /// A collection of Circular Buffer Configuration Items (NamedConfigItem)
    /// </summary>
    [Serializable]
    [XmlRoot]
    public class CircularBufferConfig : ObservableCollection<NamedConfigItem>, IItemCollectionModel 
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
        public CircularBufferConfig(int cellId, int srcId)
        {
            this._cellId = cellId;
            this._srcId = srcId;
            var environment = EnvironmentFactory.CellEnvironment();
            var tsSources = environment.Sources.SelectByCellIDAndSourceID(this._cellId, this._srcId);

            foreach (var source in tsSources)
            {
                 _source = source;
            }
            this.AddNew(new NamedConfigItem("SourceDataRate", _source.SourceDataRate));
            this.AddNew(new NamedConfigItem("DataBufferSampleSize", _source.DataBufferSampleSize));
            float CircularBufferInSeconds = _source.DataBufferSampleSize/_source.SourceDataRate;
            this.AddNew(new NamedConfigItem("CircularBufferInSeconds", CircularBufferInSeconds));
            this._IsDirty = false;
        }

        ///<summary>
        ///</summary>
        ///<param name="index"></param>
        ///<returns></returns>
        public NamedConfigItem GetItemAt(int index)
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

            // Now calculate Buffers to Fill based on modified Source Rate and Circular Buffer In Seconds.
            float SourceRate = this.GetItemAt(0).FloatValue;
            float CircularBufferInSeconds = this.GetItemAt(2).FloatValue;
            int BuffersToFill = (int)(SourceRate*CircularBufferInSeconds);

            NamedConfigItem = this.GetItemAt(1);
            NamedConfigItem.IntValue = BuffersToFill;
            this.SetItem(1, NamedConfigItem);
            return null;
        }


        /// <summary>
        /// Saves the specified cell id.
        /// </summary>
        public void Save()
        {
            _source.DataBufferSampleSize = GetItemAt(2).IntValue;
            _source.SourceDataRate = GetItemAt(0).FloatValue;
            _source.Save();
            this._IsDirty = false;
        }

        /// <summary>
        /// </summary>
        /// Gets the error message. />
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
