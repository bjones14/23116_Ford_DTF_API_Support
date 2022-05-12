// <copyright file="ISubsystemView.cs" company="Jacobs Technology, Inc.">
//     Copyright (c) Jacobs Technology, Inc. All rights reserved.
// </copyright>
// <summary> This interface defines the basic view for interacting with the ChannelPresenter in this plugin. </summary>

namespace Customer.TestSLATE.Mnemonic.UI
{
    using Customer.TestSLATE.Mnemonic.Presenters;

    /// <summary>
    ///   Interface that defines a generic view.
    /// </summary>
    public interface ISubsystemView
    {
        /// <summary>
        ///   Gets or sets the AI data source.
        /// </summary>
        /// <value>
        ///   The AI data source.
        /// </value>
        object AIData { get; set; }

        /// <summary>
        ///   Gets or sets the AO data source.
        /// </summary>
        /// <value>
        ///   The AO data source.
        /// </value>
        object AOData { get; set; }

        /// <summary>
        ///   Gets or sets the DI data source.
        /// </summary>
        /// <value>
        ///   The DI data source.
        /// </value>
        object DIData { get; set; }

        /// <summary>
        ///   Gets or sets the DO data source.
        /// </summary>
        /// <value>
        ///   The DO data source.
        /// </value>
        object DOData { get; set; }

        /// <summary>
        ///   Gets or sets the RFM Config data source.
        /// </summary>
        /// <value>
        ///   The RFM Config data source.
        /// </value>
        object RFMData { get; set; }

        /// <summary>
        /// Gets or sets the presenter.
        /// </summary>
        /// <value>
        /// The presenter.
        /// </value>
        ChannelPresenter ChannelPresenter { get; set; }

        /// <summary>
        /// Gets or sets the presenter.
        /// </summary>
        /// <value>
        /// The presenter.
        /// </value>
        EditBoxPresenter RFMConfigPresenter { get; set; }

        /// <summary>
        /// Gets or sets the presenter.
        /// </summary>
        /// <value>
        /// The presenter.
        /// </value>
        EditBoxPresenter CircularBufferConfigPresenter { get; set; }

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