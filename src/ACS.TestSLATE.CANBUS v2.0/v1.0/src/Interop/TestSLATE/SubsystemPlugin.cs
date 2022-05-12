// <copyright file="Plugin.cs" company="Jacobs Technology, Inc.">
//     Copyright (c) Jacobs Technology, Inc. All rights reserved.
// </copyright>
// <summary>This class is what loads the plugin into Test SLATE.</summary>

#region References

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Jacobs.TestSLATE.Cell.DomainLayer;
using Jacobs.TestSLATE.Cell.DomainLayer.Cells;
using Jacobs.TestSLATE.Commands.Commands.DomainLayer;
using Jacobs.TestSLATE.DB;
using Jacobs.TestSLATE.ExtensionDoc;
using Jacobs.TestSLATE.PluginCore;
using Customer.TestSLATE.Mnemonic.Interop.Database;
using Customer.TestSLATE.Mnemonic.Models;
using Customer.TestSLATE.Mnemonic.Properties;
using Jacobs.TestSLATE.UX.Core;
using Jacobs.TestSLATE.UX.SystemExplorer;
using Environment = Jacobs.TestSLATE.Cell.DomainLayer.Environment;

#endregion

namespace Customer.TestSLATE.Mnemonic.Interop.TestSLATE
{
    #region References

    using TSCell = ObjectClass;
    using TSChannels = Jacobs.TestSLATE.Cell.DomainLayer.Channels.ObjectClass;
    using TSCmdDelProcess = Jacobs.TestSLATE.Cell.DomainLayer.CmdDelProcesses.ObjectClass;
    using TSMnemonic = Jacobs.TestSLATE.Cell.DomainLayer.Mnemonics.ObjectClass;
    using TSPermission = Jacobs.TestSLATE.Cell.DomainLayer.SecurityPermissions.ObjectClass;
    using TSRole = Jacobs.TestSLATE.Cell.DomainLayer.RolePermissions.ObjectClass;
    using TSSource = Jacobs.TestSLATE.Cell.DomainLayer.Sources.ObjectClass;

    #endregion

    /// <summary>
    ///   This class is what loads the plugin into Test SLATE.
    /// </summary>
    public sealed class SubsystemPlugin : IPlugin
    {
        #region IPlugin Members

        /// <summary>
        ///   Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return Resources.PluginName; }
        }

        /// <summary>
        ///   Gets the description.
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
            get { return Resources.SourceLevelPluginPath; }
        }

        /// <summary>
        ///   Gets the compatible with.
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

        #endregion

        /// <summary>
        ///   Creates the context menu items.
        /// </summary>
        /// <param name = "parentNode">The parent node.</param>
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
        ///   Called when [add click].
        /// </summary>
        /// <param name = "sender">The sender.</param>
        /// <param name = "e">The <see cref = "System.EventArgs" /> instance containing the event data.</param>
        private static void OnAddClick(object sender, EventArgs e)
        {
            var menuItem = sender as ToolStripMenuItem;
            if (menuItem == null) return;
            var cell = menuItem.Tag as TSCell;
            if (cell == null) return;
            var environment = EnvironmentFactory.CellEnvironment();
            if (environment == null) return;

            // Create all the database tables associated with this source.
            var source = CreateTemplate(cell);
            var systemExplorer = UXMap.Get(Resources.SystemExplorer) as SystemExplorer;
            if (systemExplorer == null) return;
            var root = systemExplorer.GetNodeAtTSUID(string.Format("SourcesRoot;{0}", cell.GetTSUID()));
            if (root == null) return;
            CreateChildNode(source, root);
            root.Expand();
        }

        /// <summary>
        ///   Creates the TACS.
        /// </summary>
        private static TSSource CreateTemplate(TSCell cell)
        {
            var environment = EnvironmentFactory.CellEnvironment();
            if (environment == null) return null;

            var source = CreateNewSource(environment, cell);
            var mnemonic = CreateMnemonic(environment);
            CreateCommandDeliveryProcess(environment, cell, mnemonic, source);
            CreatePermissions();
            CreateErrorCodes(mnemonic);

            // Delete this call if not defining default channels.
            CreateDefaultChannels(source, cell);

            // Delete this call if not using ExtensionDocs.
            CreateRFMExtensionDocs(source);
            return source;
        }

        /// <summary>
        /// Creates the child node.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="root">The root.</param>
        private static void CreateChildNode(TSSource source, SystemExplorerNode root)
        {
            root.AddChildNode(new SubsystemNode(source));
        }

        /// <summary>
        ///   Creates the new source.
        /// </summary>
        /// <param name = "environment">The environment.</param>
        /// <param name = "cell">The cell.</param>
        private static TSSource CreateNewSource(Environment environment, TSCell cell)
        {
            // See if any other sources exist of this Source Type and same Cell ID. 
            // If so create unique Source Name.
            var numSources = environment.Sources.SelectByCellID(cell.CellID).Count(s => s.Mnemonic == Resources.Mnemonic);

            // Calculate the number of bytes of extra RFM Memory required by this source.
            // 4-byte align data.
            var rfmExtraMemoryInBytes = Convert.ToInt32(Resources.RFMExtraMemoryInBytes);
            var nRemainder = rfmExtraMemoryInBytes % 4;
            if (nRemainder != 0)
            {
                rfmExtraMemoryInBytes += (4 - nRemainder);
            }

            // Create the new Source here
            var sourceDataRate = Convert.ToSingle(Resources.DataSourceRate);
            var circularBufferInSeconds = Convert.ToSingle(Resources.CircularBufferInSeconds);
            var dataBufferSampleSize = (int)(circularBufferInSeconds * sourceDataRate);
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
                                 SourceName = String.Format("{0}_{1}", Resources.SourceName, numSources.ToString().PadLeft(3, '0')),
                                 CalBy = DateTime.Today.ToString(),
                                 CalDate = DateTime.Today,
                                 RequiredSource = Resources.RequiredSource,
                                 ChannelsList = new BindingList<TSChannels>(),
                                 SourceDescr = Resources.SourceName,
                                 SourceDataRate = sourceDataRate,
                                 RFMExtraMemory = rfmExtraMemoryInBytes,
                                 DataBufferSampleSize = dataBufferSampleSize
                             };

            source.Save();
            return source;
        }

        /// <summary>
        ///   Create permissions.
        /// </summary>
        private static void CreatePermissions()
        {
            // Add any required permissions your source might need to the database using this method.
            var addPermission = new AddPermission(Resources.AddSourcePermission);
            addPermission.Execute();
        }

        /// <summary>
        ///   Creates the error codes.
        /// </summary>
        /// <param name = "mnemonic">The mnemonic.</param>
        private static void CreateErrorCodes(TSMnemonic mnemonic)
        {
            // Error Codes should be the Command Delivery ID + an integer > 1000.
            var errorCode = new AddErrorCode(mnemonic.CommandDeliveryID + 1001, Resources.ErrorMessage0001);
            errorCode.Execute();
        }

        /// <summary>
        ///   Creates the command delivery process.
        /// </summary>
        /// <param name = "environment">The environment.</param>
        /// <param name = "cell">The cell.</param>
        /// <param name = "mnemonic">The mnemonic.</param>
        /// <param name = "source">The source.</param>
        private static void CreateCommandDeliveryProcess(Environment environment, TSCell cell, TSMnemonic mnemonic, ISource source)
        {
            if (mnemonic == null) return;
            var cmdDelProcess = environment.CmdDelProcesses.SelectByKeyItems(cell.CellID, source.SourceName);

            // Initialize to the defaults 
            var exec = Resources.ExecName;
            var pc = Resources.PCName;
            if (cmdDelProcess != null)
            {
                // We have an existing entry, save it's location 
                exec = cmdDelProcess.ExecName;
                pc = cmdDelProcess.PCName;

                // Delete the existing entry since we want to make sure the plugin controls the rest of the settings 
                cmdDelProcess.Delete();
            }

            // Create the new Command Delivery Process here 
            cmdDelProcess = new TSCmdDelProcess(environment)
                                {
                                    CellID = cell.CellID,
                                    CommandDeliveryID = mnemonic.CommandDeliveryID,
                                    Acq = mnemonic.Acq,
                                    Cal = mnemonic.Cal,
                                    Diag = mnemonic.Diag,
                                    ExecCode = mnemonic.ExecCode,
                                    ExecName = exec,
                                    Init = mnemonic.Init,
                                    HighRFMEnd = Resources.HighRFMEnd,
                                    HighRFMStart = Resources.HighRFMStart,
                                    ListenForAll = mnemonic.ListenForAll,
                                    Mnemonic = mnemonic.Mnemonic,
                                    PCName = pc,
                                    PerformanceFunctionsVI = mnemonic.PerformanceFunctionsVI,
                                    Port = Resources.PortNumber,
                                    ProcessName = source.SourceName,
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
        ///   Creates the mnemonic.
        /// </summary>
        /// <param name = "environment">The environment.</param>
        private static TSMnemonic CreateMnemonic(Environment environment)
        {
            var mnemonic = environment.Mnemonics.SelectByKeyItems(Resources.Mnemonic);
            
            // Obtains the highest available Command Delivery ID in the Mnemonics Table and increments it by 10k.
            var cmdDelId = mnemonic != null ? mnemonic.CommandDeliveryID : GetCommandDeliveryId(environment);
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
        ///   Creates the default Template channels.
        /// </summary>
        /// <param name = "source">The source.</param>
        /// <param name = "cell">The cell.</param>
        private static void CreateDefaultChannels(ISource source, TSCell cell)
        {
            var defChannels = new DefaultChannels();
            var environment = EnvironmentFactory.CellEnvironment();
            var tsChannels = environment.Channels.SelectByCellIDAndSourceID(cell.CellID, source.SourceID);
            foreach (var channel in from defChannel in defChannels
                                    where !tsChannels.Any(existingChnl => (existingChnl.ChannelName == defChannel.ChannelName) && 
                                                                          (existingChnl.ChannelType == defChannel._channel.ChannelType))
                                    select Channel.CreateNewChannel(defChannel))
            {
                channel.Save(cell.CellID, source.SourceID);
            }
        }

        /// <summary>
        ///   Creates the default RFM ExtensionDocs Table.
        /// </summary>
        /// <param name = "source">The source.</param>
        private static void CreateRFMExtensionDocs(TSSource source)
        {
            // Define default values for new NamedConfigItems
            var defRfMs = new List<NamedConfigItem>
                              {
                                  new NamedConfigItem("AIStartAddr", "1"),
                                  new NamedConfigItem("AIEndAddr", "2"),
                                  new NamedConfigItem("AOStartAddr", "3"),
                                  new NamedConfigItem("AOEndAddr", "4"),
                                  new NamedConfigItem("DIStartAddr", "5"),
                                  new NamedConfigItem("DIEndAddr", "6"),
                                  new NamedConfigItem("DOStartAddr", "7"),
                                  new NamedConfigItem("DOEndAddr", "8"),
                                  new NamedConfigItem("BalStartAddr", "9"),
                                  new NamedConfigItem("BalEndAddr", "10"),
                                  new NamedConfigItem("AIStatusStartAddr", "11"),
                                  new NamedConfigItem("DIStatusStartAddr", "12"),
                                  new NamedConfigItem("AOStatusStartAddr", "13"),
                                  new NamedConfigItem("DOStatusStartAddr", "14"),
                                  new NamedConfigItem("SysStatusStartAddr", "15"),
                                  new NamedConfigItem("HeartbeatAddr", "16"),
                                  new NamedConfigItem("IRIGAddr", "17"),
                                  new NamedConfigItem("LoopTime", "18")
                              };

            var doc = new ExtensionDoc();
            var rfmConfig = ExtensionDocManager.LoadFromExtensionDoc(source, ref doc);
            if (rfmConfig != null) return;
            rfmConfig = new RFMConfig(source.CellID, source.SourceID);
            foreach (var namedConfigItem in defRfMs.Select(defRFMItem => new NamedConfigItem(defRFMItem.ItemName, defRFMItem.StringValue)))
            {
                rfmConfig.AddNew(namedConfigItem);
            }

            rfmConfig.Save();
        }

        /// <summary>
        ///   Gets the command delivery id.
        /// </summary>
        /// <param name = "environment">The environment.</param>
        /// <returns></returns>
        private static int GetCommandDeliveryId(Environment environment)
        {
            var mnemonics = environment.Mnemonics.SelectAll();
            var cmdDelId = mnemonics.Aggregate(0, (current, m) => Math.Max(m.CommandDeliveryID, current));
            cmdDelId = ((cmdDelId / 10000) * (10000)) + 10000;  // Normalize the CmdDelID in case of bad values in the DB
            return cmdDelId;
        }

        /// <summary>
        ///   Creates the node.
        /// </summary>
        /// <param name = "parentNode">The parent node.</param>
        private static void CreateNode(SystemExplorerNode parentNode)
        {
            var cell = parentNode.Tag as TSCell;
            if (cell == null) return;
            foreach (var source in cell.SourcesList.Where(src => src.Mnemonic == Resources.Mnemonic))
            {
                CreateChildNode(source, parentNode);
            }
        }
    }
}