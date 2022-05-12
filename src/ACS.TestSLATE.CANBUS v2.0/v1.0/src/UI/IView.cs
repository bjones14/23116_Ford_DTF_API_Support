// <copyright file="IView.cs" company="Jacobs Technology, Inc.">
//     Copyright (c) Jacobs Technology, Inc. All rights reserved.
// </copyright>
// <summary> This interface defines the basic view for interacting with the presenter in this plugin. </summary>

namespace Customer.TestSLATE.Mnemonic.UI
{
    using Customer.TestSLATE.Mnemonic.Presenters;

    /// <summary>
    ///   Interface that defines a generic view.
    /// </summary>
    public interface IView
    {
        /// <summary>
        ///   Gets or sets the data source.
        /// </summary>
        /// <value>
        ///   The data source.
        /// </value>
        object Data { get; set; }

        /// <summary>
        /// Gets or sets the presenter.
        /// </summary>
        /// <value>
        /// The presenter.
        /// </value>
        GridPresenter Presenter { get; set; }

        /// <summary>
        ///   Refreshes the data.
        /// </summary>
        void RefreshData();

        /// <summary>
        ///   Shows the unsaved changes.
        /// </summary>
        void ShowUnsavedChanges();
    }
}