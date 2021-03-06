// <copyright file="EditBoxPresenter.cs" company="Jacobs Technology, Inc.">
//     Copyright (c) Jacobs Technology, Inc. All rights reserved.
// </copyright>
// <summary>
// The presenter handles the interactions between the UI layer and the data/valdiation model.
// The presenter responds to the users' requests and executes commands against the model and then updates the view with the latest data.
// It separates the view from the model allowing the view to be changed out as desired.
// </summary>

namespace Customer.TestSLATE.Mnemonic.Presenters
{
    using System.Diagnostics;
    using Jacobs.TestSLATE.Commands.History;
    using Customer.TestSLATE.Mnemonic.Models;
    using Customer.TestSLATE.Mnemonic.UI;

    /// <summary>
    /// The presenter handles the interactions between the UI layer and the data/valdiation model.
    /// </summary>
    public class EditBoxPresenter
    {
        /// <summary>
        /// The command history.
        /// </summary>
        private readonly CommandHistory _history;

        /// <summary>
        /// Initializes a new instance of the <see cref="EditBoxPresenter"/> class.
        /// </summary>
        public EditBoxPresenter()
        {
            this._history = new CommandHistory();
        }

        /// <summary>
        /// Gets or sets the index for the current view.
        /// </summary>
        /// <value>
        /// The index for the current view.
        /// </value>
        public int CurrentViewIndex { get; set; }

        /// <summary>
        /// Gets or sets the view.
        /// </summary>
        /// <value>
        /// The view.
        /// </value>
        public ISubsystemView View { get; set; }

        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        /// <value>
        /// The Item model.
        /// </value>
        public IItemCollectionModel Model { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is data valid.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is data valid; otherwise, <c>false</c>.
        /// </value>
        public bool IsValid { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is dirty.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is dirty; otherwise, <c>false</c>.
        /// </value>
        public bool IsDirty
        {
            get { return this.Model.IsDirty; }
            set { this.Model.IsDirty = value; }
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public void Initialize()
        {
            // Set the View's data = the model's data
            this.View.RFMData = this.Model;
            this.View.RefreshData();
            this.Model.CollectionChanged += OnCollectionChanged;
            this.Model.IsDirty = false;
        }

        /// <summary>
        /// Called when [collection changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        public void OnCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // In case we want to watch for changes to the collection. 
            this.View.RefreshData();
            this.View.ShowUnsavedChanges();
        }

        /// <summary>
        /// Saves this instance.
        /// </summary>
        /// <returns></returns>
        public bool Save()
        {
            this.Model.Save();
            this._history.Clear();
            this.View.ShowUnsavedChanges();
            return true;
        }

        /// <summary>
        /// Undoes this instance.
        /// </summary>
        public void Undo()
        {
            this._history.Undo();
        }

        /// <summary>
        /// Redoes this instance.
        /// </summary>
        public void Redo()
        {
            this._history.Redo();
        }

        /// <summary>
        /// Gets the error message.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="propertyName">The propertyName.</param>
        /// <returns></returns>
        public string GetErrorMessage(int index, string propertyName)
        {
            return this.Model.GetErrorMessage(index, propertyName);
        }

        /// <summary>
        /// Validates the specified RFM Config Item.
        /// </summary>
        /// <returns></returns>
        public bool Validate(int index)
        {
            return this.Model.IsValid;
        }

        /// <summary>
        /// Validates all balances.
        /// </summary>
        /// <returns></returns>
        public bool ValidateAll()
        {
            return this.Model.Validate();
        }

        /// <summary>
        /// Validates all balances.
        /// </summary>
        /// <returns></returns>
        public void DataChanged(int index, string stringValue, int intValue, float floatValue)
        {
            this.Model.UpdateValue(index, stringValue, intValue, floatValue);
            return;
        }
    }
}
