// <copyright file="Plugin.cs" company="Jacobs Technology, Inc.">
//     Copyright (c) Jacobs Technology, Inc. All rights reserved.
// </copyright>
// <summary>This class is what loads the plugin into Test SLATE.</summary>


namespace Jacobs.TestSLATE.PluginTemplate.Interop.TestSLATE
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows.Forms;
    using Jacobs.TestSLATE.PluginCore;
    using Jacobs.TestSLATE.Commands.Commands.DomainLayer;
    using Jacobs.TestSLATE.DB;
    using Jacobs.TestSLATE.PluginTemplate.Properties;
    using Jacobs.TestSLATE.UX.Core;
    using Jacobs.TestSLATE.UX.SystemExplorer;
    using Environment = Jacobs.TestSLATE.Cell.DomainLayer.Environment;
    using TSCell = Jacobs.TestSLATE.Cell.DomainLayer.Cells.ObjectClass;
    using TSChannels = Jacobs.TestSLATE.Cell.DomainLayer.Channels.ObjectClass;
    using TSCmdDelProcess = Jacobs.TestSLATE.Cell.DomainLayer.CmdDelProcesses.ObjectClass;
    using TSMnemonic = Jacobs.TestSLATE.Cell.DomainLayer.Mnemonics.ObjectClass;
    using TSPermission = Jacobs.TestSLATE.Cell.DomainLayer.SecurityPermissions.ObjectClass;
    using TSRole = Jacobs.TestSLATE.Cell.DomainLayer.RolePermissions.ObjectClass;
    using TSSource = Jacobs.TestSLATE.Cell.DomainLayer.Sources.ObjectClass;

    /// <summary>
    ///   This class is what loads the plugin into Test SLATE.
    /// </summary>
    public sealed class ConfigPlugin : IPlugin
    {
        /// <summary>
        ///   Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return Resources.PluginName; }
        }

        /// <summary>
        /// Gets the description.
        /// </summary>
        public string Description
        {
            get { return Resources.PluginName; }
        }

        /// <summary>
        ///   Gets the version.
        /// </summary>
        /// <value>The version.</value>
        public string Version
        {
            get { return Resources.PluginVersion; }
        }

        /// <summary>
        ///   Gets the path.
        /// </summary>
        /// <value>The path.</value>
        public string Path
        {
            // The Path will the the place in the System Explorer Tree where this Plugin adds itself under.
            get { return Resources.ConfigLevelPluginPath; }
//            get { return Resources.SourceLevelPluginPath; }
        }

        /// <summary>
        /// Gets the compatible with.
        /// </summary>
        public string CompatibleWith
        {
            get { return Resources.CompatibleWithVersions; }
        }

        /// <summary>
        ///   Initializes the specified parent.
        /// </summary>
        /// <param name = "parent">The parent.</param>
        /// <returns></returns>
        public bool Initialize(object parent)
        {
            var parentNode = parent as SystemExplorerNode;
            if (parentNode == null) return false;

            var environment = EnvironmentFactory.CellEnvironment();
            if (environment == null) return false;

            CreateNode(parentNode);
            CreateContextMenuItems(parentNode);
            return true;
        }

        /// <summary>
        /// Creates the context menu items.
        /// </summary>
        /// <param name="parentNode">The parent node.</param>
        private static void CreateContextMenuItems(SystemExplorerNode parentNode)
        {
            if (parentNode == null) return;
            if (parentNode.ContextMenuStrip == null)
            {
                parentNode.ContextMenuStrip = new ContextMenuStrip();
            }

            // The ONLY allowed command from the Plugin is ADD
            // ADD
            var menuItem = new TSMenuItem
                               {
                                   Tag = parentNode.Tag,
                                   Text = Resources.AddTemplate,
                                   Permission = Resources.AddSourcePermission
                               };

            menuItem.Click += OnAddClick;
            parentNode.AddMenuItem(menuItem);
        }

        /// <summary>
        /// Called when [add click].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private static void OnAddClick(object sender, EventArgs e)
        {
            var menuItem = sender as ToolStripMenuItem;
            if (menuItem == null) return;

            var cell = menuItem.Tag as TSCell;
            if (cell == null) return;

            var environment = EnvironmentFactory.CellEnvironment();
            if (environment == null) return;

            var mnemonic = CreateMnemonic(environment);
            CreateErrorCodes(mnemonic);
            var source = CreateNewSource(environment, cell);
            CreateCommandDeliveryProcess(environment, cell, mnemonic, source);

            var systemExplorer = UXMap.Get(Resources.SystemExplorer) as SystemExplorer;
            if (systemExplorer == null) return;

            var root = systemExplorer.GetNodeAtTSUID(string.Format("SourcesRoot;{0}", cell.GetTSUID()));
            if (root == null) return;

            CreateChildNode(source, root);
            root.Expand();
        }

        /// <summary>
        /// Creates the child node.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="root">The root.</param>
        private static void CreateChildNode(TSSource source, SystemExplorerNode root)
        {
            var node = new Node(source);
            root.AddChildNode(node);
        }

        /// <summary>
        /// Creates the new source.
        /// </summary>
        /// <param name="environment">The environment.</param>
        /// <param name="cell">The cell.</param>
        private static TSSource CreateNewSource(Environment environment, TSCell cell)
        {
            // Create the new Source here
            var source = new TSSource(environment)
                             {
                                 // Set additional source properties as needed.
                                 CellID = cell.CellID,
                                 Mnemonic = Resources.Mnemonic,
                                 EUDType = Resources.EUDType,
                                 Manufacturer = Resources.Manufacturer,
                                 Model = Resources.SourceName,
                                 SerialNumber = Resources.SerialNumber,
                                 SourceID = Resources.SourceID,
                                 SourceName = Resources.SourceName,
                                 CalBy = DateTime.Today.ToString(),
                                 CalDate = DateTime.Today,
                                 RequiredSource = Resources.RequiredSource,
                                 ChannelsList = new BindingList<TSChannels>(),
                                 SourceDescr = Resources.SourceName
                             };
            source.Save();
            return source;
        }

        /// <summary>
        /// Creates the error codes.
        /// </summary>
        /// <param name="mnemonic">The mnemonic.</param>
        private static void CreateErrorCodes(TSMnemonic mnemonic)
        {
            // Error Codes should be the Command Delivery ID + an integer.
            var errorCode = new AddErrorCode(mnemonic.CommandDeliveryID + 1, Resources.ErrorMessage0001);
            errorCode.Execute();
        }

        /// <summary>
        /// Creates the command delivery process.
        /// </summary>
        /// <param name="environment">The environment.</param>
        /// <param name="cell">The cell.</param>
        /// <param name="mnemonic">The mnemonic.</param>
        /// <param name="source">The source.</param>
        private static void CreateCommandDeliveryProcess(Environment environment, TSCell cell, TSMnemonic mnemonic, TSSource source)
        {
            if (mnemonic == null) return;
            var cmdDelProcess = environment.CmdDelProcesses.SelectByKeyItems(cell.CellID, Resources.ProcessName);
            if (cmdDelProcess != null) return;

            // Create the new Command Delivery Process here 
            cmdDelProcess = new TSCmdDelProcess(environment)
                                    {
                                        CellID = cell.CellID,
                                        CommandDeliveryID = mnemonic.CommandDeliveryID,
                                        Acq = mnemonic.Acq,
                                        Cal = mnemonic.Cal,
                                        Diag = mnemonic.Diag,
                                        ExecCode = mnemonic.ExecCode,
                                        ExecName = Resources.ExecName,
                                        Init = mnemonic.Init,
                                        HighRFMEnd = Resources.HighRFMEnd,
                                        HighRFMStart = Resources.HighRFMStart,
                                        ListenForAll = mnemonic.ListenForAll,
                                        Mnemonic = mnemonic.Mnemonic,
                                        PCName = Resources.PCName,
                                        PerformanceFunctionsVI = mnemonic.PerformanceFunctionsVI,
                                        Port = Resources.PortNumber,
                                        ProcessName = Resources.ProcessName,
                                        ReloadDB = mnemonic.ReloadDB,
                                        Required = mnemonic.Required,
                                        SourceID = source.SourceID,
                                        Start = mnemonic.Start,
                                        Stop = mnemonic.Stop,
                                        TVKill = mnemonic.TVKill,
                                        WritesToRFM = Resources.WritesToRFM
                                    };
            cmdDelProcess.Save();
        }

        /// <summary>
        /// Creates the mnemonic.
        /// </summary>
        /// <param name="environment">The environment.</param>
        private static TSMnemonic CreateMnemonic(Environment environment)
        {
            var mnemonic = environment.Mnemonics.SelectByKeyItems(Resources.Mnemonic);
            if (mnemonic != null) return mnemonic;

            // Obtains the highest available Command Delivery ID in the Mnemonics Table and increments it by 10k.
            var cmdDelId = GetCommandDeliveryId(environment);
            mnemonic = new TSMnemonic(environment)
                                {
                                    Acq = Resources.Acquire,
                                    Cal = Resources.Calibrate,
                                    CommandDeliveryID = cmdDelId,
                                    Diag = Resources.Diagnostics,
                                    ExecCode = Resources.ExecCode,
                                    Init = Resources.Initialize,
                                    ListenForAll = Resources.ListenForAll,
                                    Mnemonic = Resources.Mnemonic,
                                    PerformanceFunctionsVI = Resources.PerformanceFunctionsPath,
                                    ReloadDB = Resources.ReloadDB,
                                    Required = Resources.Required,
                                    Start = Resources.Start,
                                    Stop = Resources.Stop,
                                    TVKill = Resources.TVKill
                                };

            mnemonic.Save();
            return mnemonic;
        }

        /// <summary>
        /// Gets the command delivery id.
        /// </summary>
        /// <param name="environment">The environment.</param>
        /// <returns></returns>
        private static int GetCommandDeliveryId(Environment environment)
        {
            var mnemonics = environment.Mnemonics.SelectAll();
            var cmdDelId = mnemonics.Aggregate(0, (current, m) => Math.Max(m.CommandDeliveryID, current));
            cmdDelId += 10000;
            return cmdDelId;
        }

        /// <summary>
        ///   Creates the node.
        /// </summary>
        /// <param name = "parentNode">The parent node.</param>
        private static void CreateNode(SystemExplorerNode parentNode)
        {
            var node = new Node {Tag = parentNode.Tag};
            parentNode.AddChildNode(node);
            PluginManager.InitializePluginsAtPath(Resources.InitializeAtPath, node);
        }
    }
}