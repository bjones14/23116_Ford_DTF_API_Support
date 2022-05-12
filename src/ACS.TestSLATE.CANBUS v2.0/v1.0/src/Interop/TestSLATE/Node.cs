// <copyright file="Node.cs" company="Jacobs Technology, Inc.">
//     Copyright (c) Jacobs Technology, Inc. All rights reserved.
// </copyright>
// <summary>This class is an example of a basic node for configuration based editors.</summary>


namespace Jacobs.TestSLATE.PluginTemplate.Interop.TestSLATE
{
    using System;
    using System.Windows.Forms;
    using Jacobs.TestSLATE.DB;
    using Jacobs.TestSLATE.PluginTemplate.Models;
    using Jacobs.TestSLATE.PluginTemplate.Presenters;
    using Jacobs.TestSLATE.PluginTemplate.Properties;
    using Jacobs.TestSLATE.PluginTemplate.UI;
    using Jacobs.TestSLATE.UX.Core;
    using Jacobs.TestSLATE.UX.SystemExplorer;
    using TSConfig = Jacobs.TestSLATE.Cell.DomainLayer.Configurations.ObjectClass;
    using TSSource = Jacobs.TestSLATE.Cell.DomainLayer.Sources.ObjectClass;
    using Jacobs.TestSLATE.PluginTemplate.Interop.LabVIEW;

    /// <summary>
    ///   This class is basic node for the config nodes and editors.
    /// </summary>
    public sealed class Node : SystemExplorerNode
    {
        /// <summary>
        /// The Source.
        /// </summary>
        private readonly TSSource _source;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "Node" /> class.
        /// </summary>
        public Node()
        {
            this.UpdateName();
            this.CreateMenuItems();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Node"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        public Node(TSSource source)
        {
            this._source = source;
        }

        /// <summary>
        ///   Create the Menu Items associated with this node.
        /// </summary>
        private void CreateMenuItems()
        {
            // The ONLY allowed commands from the Node is EDIT and REMOVE
            // IF this is a DISPLAY NODE, then you have 2 other options available: RENAME and SHOW
            if (this.ContextMenuStrip == null)
            {
                this.ContextMenuStrip = new ContextMenuStrip();
            }

            // EDIT
            var edit = new TSMenuItem
                           {
                               Name = Resources.EditTemplate,
                               Text = Resources.EditTemplate,
                               Permission = Resources.EditTemplate
                           };

            edit.Click += this.OnEditClick;
            this.ContextMenuStrip.Items.Add(edit);
            this.DoubleClickItem = edit;

            // REMOVE
            var remove = new TSMenuItem
                             {
                                 Name = Resources.RemoveTemplate,
                                 Text = Resources.RemoveTemplate,
                                 Permission = Resources.RemoveTemplate
                             };

            remove.Click += this.OnRemoveClick;
            this.ContextMenuStrip.Items.Add(remove);
        }

        /// <summary>
        ///   Refreshes this instance.
        /// </summary>
        public override void Refresh()
        {
            this.UpdateData();
            this.UpdateName();
        }

        /// <summary>
        ///   Updates the data.
        /// </summary>
        public override void UpdateData()
        {
            if (this.Tag == null) return;

            var config = this.Tag as TSConfig;
            if (config == null) return;

            var environment = EnvironmentFactory.CellEnvironment();
            if (environment == null) return;

            config = environment.Configurations.SelectByKey(config);
            if (config == null) return;

            this.Tag = config;
        }

        /// <summary>
        ///   Updates the name.
        /// </summary>
        public override void UpdateName()
        {
            this.Name = Resources.NodeName;
            this.Text = this.Name;
        }

        /// <summary>
        ///   Called when [on Edit clicked].
        /// </summary>
        /// <param name = "sender">The sender.</param>
        /// <param name = "e">The event arguments.</param>
        private void OnEditClick(object sender, EventArgs e)
        {
            if (this.Tag == null) return;

            var config = this.Tag as TSConfig;
            if (config == null) return;

            this.LaunchUI(config);
        }

        /// <summary>
        /// Called when [remove click].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnRemoveClick(object sender, EventArgs e)
        {
            // Will need to do other clean up here. If you created a set of channels or other items, those will need to be deleted here as well.
            this.Delete();
        }

        /// <summary>
        ///   Opens the editor.
        /// </summary>
        /// <param name = "config">The config.</param>
        private void LaunchUI(TSConfig config)
        {
            // IF you're going to call just a LabVIEW Editor, use the appropriate VIWrapper from the Template for your source.
            //var lvEditor = new VIWrapper {CellId = config.CellID, CfgId = config.CfgID};
            //lvEditor.Run();

            if (UXMap.CurrentUser.Cannot(Resources.EditTemplate)) return;

            // For .NET Editors...
            // Create the Editor, pass parameters, set properties & events, call methods as needed.
            var editor = OpenEditor();

            // Do stuff here to get the model to load its data from the db
            var balances = GetData(config);

            // Create the presenter and assign to the editor and initialize
            InitPresenter(editor, balances);
        }

        /// <summary>
        /// Inits the presenter.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="model">The model.</param>
        private static void InitPresenter(IView view, IModel model)
        {
            var presenter = new GridPresenter
                                {
                                    View = view, 
                                    Model = model
                                };

            presenter.Initialize();
            view.Presenter = presenter;
        }

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <param name="config">The config.</param>
        /// <returns></returns>
        private static IModel GetData(TSConfig config)
        {
            var environment = EnvironmentFactory.CellEnvironment();
            var tsBalances = environment.Balances.SelectByCellIDAndCfgID(config.CellID, config.CfgID);
            var balances = new Balances(config.CellID, config.CfgID);
            foreach (var bal in tsBalances)
            {
                balances.Add(new Balance(bal));
            }

            return balances;
        }

        /// <summary>
        /// Opens the editor.
        /// </summary>
        /// <returns></returns>
        private IView OpenEditor()
        {
            var editor = new Editor
                             {
                                 Manager = UXMap.DockManager,
                                 ToolTipText = this.TSLocation
                             };

            editor.TabViewSavingEvent += this.OnSaving;
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
            // This event can be powerful.
            // When an editor saves, this event fires, and can trigger other editors to reload their settings.
            this.Refresh();
        }
    }
}