// <copyright file="Balances.cs" company="Jacobs Technology, Inc.">
//     Copyright (c) Jacobs Technology, Inc. All rights reserved.
// </copyright>
// <summary>Collections utililzing .NET 4.0 can inherit from ObservableCollection, if the dll has to use .NET 3.5, then
//          collections will need to inherit from CollectionBase and implement IBindingList.
//          Doing so will make collection's changes obseverable by the grid through event notifications.
// </summary>

namespace Customer.TestSLATE.Mnemonic.Models
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using Jacobs.TestSLATE.Commands.Commands.Collection;
    using Jacobs.TestSLATE.Commands.Interfaces;
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// A collection of Balances
    /// </summary>
    [Serializable]
    [XmlRoot]
    public class Balances : ObservableCollection<Balance>, IModel
    {
        /// <summary>
        /// The Cell Id.
        /// </summary>
        private readonly int _cellId;

        /// <summary>
        /// The Config Id.
        /// </summary>
        private readonly int _configId;

        private bool _Dirty;

        /// <summary>
        /// Initializes a new instance of the <see cref="Balances"/> class.
        /// </summary>
        /// <param name="cellId">The cell id.</param>
        /// <param name="cfgId">The CFG id.</param>
        public Balances(int cellId, int cfgId)
        {
            this._cellId = cellId;
            this._configId = cfgId;
        }

        /// <summary>
        /// Gets a value indicating whether this instance has error.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has error; otherwise, <c>false</c>.
        /// </value>
        public bool HasError
        {
            get { return this.Any(balance => !balance.Validator.IsValid); }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is dirty.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is dirty; otherwise, <c>false</c>.
        /// </value>
        public bool IsDirty
        {
            get { return this.Any(balance => balance.Dirty); }
            set { this._Dirty = value; }
        }

        /// <summary>
        /// Adds the new.
        /// </summary>
        /// <returns></returns>
        public UndoableCommand AddNew()
        {
            var addItem = new AddItem(this, Balance.CreateNewBalance());
            addItem.Execute();
            return addItem;
        }

        /// <summary>
        /// Inserts the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public UndoableCommand Insert(int index)
        {
            var insertItem = new InsertItem(this, Balance.CreateNewBalance(), index);
            insertItem.Execute();
            return insertItem;
        }

        /// <summary>
        /// Removes the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public UndoableCommand Remove(int index)
        {
            var removeItem = new RemoveItem(this, this[index]);
            removeItem.Execute();
            return removeItem;
        }

        /// <summary>
        /// Saves the specified cell id.
        /// </summary>
        public void Save()
        {
            foreach (var b in this)
            {
                b.Save(this._cellId, this._configId);
            }
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
            get { return this.All(balance => balance.Validator.IsValid); }
        }

        /// <summary>
        /// Validates the Collection of Balances
        /// </summary>
        /// <returns></returns>
        public bool Validate()
        {
            return this.All(balance => balance.Validator.IsValid);
        }

        /// <summary>
        /// Swaps the positions of two balances.
        /// </summary>
        /// <param name="sourceRow">The source row.</param>
        /// <param name="targetRow">The target row.</param>
        public void SwapItems(int sourceRow, int targetRow)
        {
            var tmp = this[sourceRow];
            this[sourceRow] = this[targetRow];
            this[targetRow] = tmp;
        }
    }
}
