// <copyright file="SubsystemNode.cs" company="Jacobs Technology, Inc.">
//     Copyright (c) Jacobs Technology, Inc. All rights reserved.
// </copyright>
// <summary>This class is an example of a basic node for subsystem based editors.</summary>
//   Remove this file from Project if editor does not support subsystem node.

#region References

using System;
using System.Windows.Forms;
using Jacobs.TestSLATE.Cell.DomainLayer;
using Jacobs.TestSLATE.Cell.DomainLayer.Sources;
using Jacobs.TestSLATE.DB;
using Customer.TestSLATE.Mnemonic.Interop.Database;
using Customer.TestSLATE.Mnemonic.Models;
using Customer.TestSLATE.Mnemonic.Presenters;
using Customer.TestSLATE.Mnemonic.Properties;
using Customer.TestSLATE.Mnemonic.UI;
using Jacobs.TestSLATE.ExtensionDoc;
using Jacobs.TestSLATE.UX.Core;
using Jacobs.TestSLATE.UX.SystemExplorer;

#endregion

namespace Customer.TestSLATE.Mnemonic.Interop.TestSLATE
{
    #region References

    using TSSource = ObjectClass;
    using TSCell = Jacobs.TestSLATE.Cell.DomainLayer.Cells.ObjectClass;

    #endregion

    /// <summary>
    ///   This class is basic node for the subsystem nodes and editors.
    /// </summary>
    public sealed class SubsystemNode : SystemExplorerNode
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref = "SubsystemNode" /> class.
        /// </summary>
        public SubsystemNode()
        {
            UpdateName();
            CreateMenuItems();
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "SubsystemNode" /> class.
        /// </summary>
        /// <param name = "source">The source.</param>
        public SubsystemNode(IObjectClass source)
        {
            Tag = source;
            UpdateName();
            UpdateData();
            CreateMenuItems();
        }

        /// <summary>
        ///   Create the Menu Items associated with this node.
        /// </summary>
        private void CreateMenuItems()
        {
            // The ONLY allowed commands from the Node is EDIT and REMOVE and RENAME
            // IF this is a DISPLAY NODE, then you have 1 other option available: SHOW
            if (ContextMenuStrip == null)
            {
                ContextMenuStrip = new ContextMenuStrip();
            }

            // EDIT
            var edit = new TSMenuItem
                           {
                               Name = Resources.EditTemplate,
                               Text = Resources.EditTemplate,
                               Permission = Resources.EditTemplate
                           };

            edit.Click += OnEditClick;
            ContextMenuStrip.Items.Add(edit);
            DoubleClickItem = edit;

            // REMOVE
            var remove = new TSMenuItem
                             {
                                 Name = Resources.RemoveTemplate,
                                 Text = Resources.RemoveTemplate,
                                 Permission = Resources.RemoveTemplate
                             };

            remove.Click += OnRemoveClick;
            ContextMenuStrip.Items.Add(remove);

            // RENAME
            var rename = new TSMenuItem
                             {
                                 Name = Resources.RenameTemplate,
                                 Text = Resources.RenameTemplate,
                                 Permission = Resources.RenameTemplate
                             };

            rename.Click += OnRenameClick;
            ContextMenuStrip.Items.Add(rename);
        }

        /// <summary>
        ///   Refreshes this instance.
        /// </summary>
        public override void Refresh()
        {
            UpdateData();
            UpdateName();
        }

        /// <summary>
        ///   Updates the data.
        /// </summary>
        public override void UpdateData()
        {
            if (Tag == null) return;
            var source = Tag as TSSource;
            if (source == null) return;
            source = EnvironmentFactory.CellEnvironment().Sources.SelectByKey(source);
            if (source == null) return;
            Tag = source;
        }

        /// <summary>
        ///   Updates the name.
        /// </summary>
        public override void UpdateName()
        {
            if (Tag == null) return;
            var source = Tag as TSSource;
            if (source == null) return;
            Name = source.SourceName;
            Text = Name;
        }

        /// <summary>
        ///   Called when [on Edit clicked].
        /// </summary>
        /// <param name = "sender">The sender.</param>
        /// <param name = "e">The event arguments.</param>
        private void OnEditClick(object sender, EventArgs e)
        {
            if (Tag == null) return;
            var source = Tag as TSSource;
            if (source == null) return;
            LaunchUI(source);
        }

        /// <summary>
        ///   Called when [remove click].
        /// </summary>
        /// <param name = "sender">The sender.</param>
        /// <param name = "e">The <see cref = "System.EventArgs" /> instance containing the event data.</param>
        private void OnRemoveClick(object sender, EventArgs e)
        {
            if (Tag == null) return;
            var source = Tag as TSSource;
            if (source == null) return;

            // Delete will remove the source (which will trigger deletion of channels & cmd del entries via Triggers in the DB)
            Delete();
            ExtensionDocManager.DeleteFromExtensionDoc(source);
        }

        /// <summary>
        ///   Called when [rename click].
        /// </summary>
        /// <param name = "sender">The sender.</param>
        /// <param name = "e">The <see cref = "System.EventArgs" /> instance containing the event data.</param>
        private void OnRenameClick(object sender, EventArgs e)
        {
            if (Tag == null) return;
            var source = Tag as TSSource;
            if (source == null) return;
            var editSource = source;
            var dlg = new Rename(editSource);
            var ret = dlg.ShowDialog();
            if (ret != DialogResult.OK) return;
            source = editSource;
            source.Save();
            var cmdDelProcesses = EnvironmentFactory.CellEnvironment().CmdDelProcesses.SelectByCellIDAndSourceID(source.CellID, source.SourceID);
            foreach (var cmdDelProcess in cmdDelProcesses)
            {
                var newcmdDelProcess = cmdDelProcess;
                cmdDelProcess.Delete();
                newcmdDelProcess.ProcessName = source.SourceName;
                newcmdDelProcess.Save();
            }

            Refresh();
        }

        /// <summary>
        ///   Opens the editor.
        /// </summary>
        private void LaunchUI(TSSource source)
        {
            var editor = OpenEditor(source);

            // Get the Channel model to load its AI, AO, DI, and DO data from the db
            // Don't place these strings into Resources File since we don't want them to change
            // with Language.
            var aiChannels = GetData(source, "AI");
            var aoChannels = GetData(source, "AO");
            var diChannels = GetData(source, "DI");
            var doChannels = GetData(source, "DO");

            // Get the TCP Config Information from ExtensionDoc table in Db.
            var doc = new ExtensionDoc();
            var rfmSetup = GetRFMConfigItems(source, ref doc);

            // Create the Channel presenter and assign to the editor and initialize
            InitChannelPresenter(editor, aiChannels, aoChannels, diChannels, doChannels);

            // Create the RFM Config presenter and assign to the editor and initialize
            InitRFMConfigPresenter(editor, rfmSetup);

            // Create the Source Config presenter and assign to the editor and initialize
        }

        /// <summary>
        ///   Inits the presenter.
        /// </summary>
        /// <param name = "view">The view.</param>
        /// <param name = "AI">The AI model.</param>
        /// <param name = "AO">The AO model.</param>
        /// <param name = "DI">The DI model.</param>
        /// <param name = "DO">The DO model.</param>
        private static void InitChannelPresenter(ISubsystemView view, IChannelModel AI, IChannelModel AO, IChannelModel DI, IChannelModel DO)
        {
            var presenter = new ChannelPresenter
                                {
                                    View = view,
                                    ChnlModels = new IChannelModel[4],
                                };

            presenter.ChnlModels[0] = AI;
            presenter.ChnlModels[1] = AO;
            presenter.ChnlModels[2] = DI;
            presenter.ChnlModels[3] = DO;
            for (var i = 0; i < 4; i++) presenter.ChnlModels[i].SetPresenter(presenter);
            presenter.CurrentViewIndex = 0; // Initialize to point to first tab view (AI).
            presenter.Initialize();
            view.ChannelPresenter = presenter;
        }

        /// <summary>
        ///   Inits the presenter.
        /// </summary>
        /// <param name = "view">The view.</param>
        /// <param name = "rfmSetup">The RFM Config model.</param>
        private static void InitRFMConfigPresenter(ISubsystemView view, IItemCollectionModel rfmSetup)
        {
            var presenter = new EditBoxPresenter
                                {
                                    View = view,
                                };
            view.RFMConfigPresenter = presenter;
            presenter.Model = rfmSetup;
            presenter.CurrentViewIndex = 0; // Initialize to point to first tab view (AI).
            presenter.Initialize();
        }

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="channelType">The Channel Type we want retrieved.</param>
        /// <returns></returns>
        private static IChannelModel GetData(ISource source, String channelType)
        {
            var environment = EnvironmentFactory.CellEnvironment();
            var tsChannels = environment.Channels.SelectByCellIDAndSourceID(source.CellID, source.SourceID);
            var analogchannels = new Channels<AnalogChannel>(source.CellID, source.SourceID, channelType);
            var channels = new Channels<Channel>(source.CellID, source.SourceID, channelType);
            foreach (var chnl in tsChannels)
            {
                if ((channelType == "AI") || (channelType == "AO"))
                {
                    if (chnl.ChannelType == channelType)
                    {
                        analogchannels.Add(new AnalogChannel(chnl));
                    }
                }
                else
                {
                    {
                        if (chnl.ChannelType == channelType)
                        {
                            channels.Add(new Channel(chnl));
                        }
                    }
                }
            }

            if ((channelType == "AI") || (channelType == "AO")) return analogchannels;
            return channels;
        }

        /// <summary>
        /// Gets the RFM Config data.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="doc">The doc.</param>
        /// <returns></returns>
        private static IItemCollectionModel GetRFMConfigItems(TSSource source, ref ExtensionDoc doc)
        {
            var environment = EnvironmentFactory.CellEnvironment();
            environment.CmdDelProcesses.SelectByKeyItems(source.CellID, Resources.ProcessName);
            var rfmConfig = ExtensionDocManager.LoadFromExtensionDoc(source, ref doc) ??
                            new RFMConfig(source.CellID, source.SourceID, "0");
            return rfmConfig;
        }

        /// <summary>
        ///   Opens the editor.
        /// </summary>
        /// <returns></returns>
        private ISubsystemView OpenEditor(ObjectClass source)
        {
            var editor = new SubsystemEditor(source)
                             {
                                 Manager = UXMap.DockManager,
                                 ToolTipText = TSLocation
                             };

            editor.TabViewSavingEvent += OnSaving;
            editor.Open();
            editor.Activate();
            return editor;
        }

        /// <summary>
        ///   Called when [on Saving]
        /// </summary>
        /// <param name = "dockWindow">The dock window that is saving.</param>
        private void OnSaving(TSTabbedDocument dockWindow)
        {
            Refresh();
        }
    }
}