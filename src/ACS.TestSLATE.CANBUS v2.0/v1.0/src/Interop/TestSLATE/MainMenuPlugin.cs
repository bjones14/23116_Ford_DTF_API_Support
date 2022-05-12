// <copyright file="MainMenuPlugin.cs" company="Jacobs Technology, Inc.">
//     Copyright (c) Jacobs Technology, Inc. All rights reserved.
// </copyright>
// <summary>This class is what adds menu items into Test SLATE.</summary>


namespace Customer.TestSLATE.Mnemonic.Interop.TestSLATE
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System;
    using System.Windows.Forms;
    using Jacobs.TestSLATE.PluginCore;
    using Jacobs.TestSLATE.Commands.Commands.DomainLayer;
    using Customer.TestSLATE.Mnemonic.Properties;
    using Jacobs.TestSLATE.UX.Core;

    /// <summary>
    ///   This class is what loads the plugin into Test SLATE.
    /// </summary>
    public sealed class MainMenuPlugin : IPlugin
    {
        /// <summary>
        ///   Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return Resources.PluginName + Resources.MenuPluginAppender; }
        }

        /// <summary>
        /// Gets the description.
        /// </summary>
        public string Description
        {
            get { return Resources.PluginName + Resources.MenuPluginAppender; }
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
            get { return Resources.MainMenuPluginPath; }
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
            CreateMainMenuItems();
            CreatePermissions();
            return true;
        }

        /// <summary>
        /// Add main manu items "Data Analysis", "Plot Builder", "Report Builder"  under the main menu "Data Analysis".
        /// </summary>
        // Valid locations to add to the Main Menu: Test SLATE, Edit, View, Tools, Data Anlysis, Diagnostics, Execute, Calibrate, Help
       private static void CreateMainMenuItems()
        {
// Start of Calibrate Menu Code
           // Add Template to Calibrate Menu if required. 
           // Comment out if not needed by your plugin.
           var menuItemCalib = new TSMenuItem
                               {
                                   Name = Resources.MenuItemTemplate,
                                   Text = Resources.MenuItemTemplate,
                                   Permission = Resources.ExecuteTemplateCalibration,
                                   // ValidOn = SystemCategory.MOC
                                   // ValidWhen = ValidWhen.InTest
                               };

            menuItemCalib.Text = menuItemCalib.Name;
            menuItemCalib.Click += OnMainmenuTemplateCalibrateClick;
            UXMap.MenuManager.AddMenuItem(Jacobs.TestSLATE.UX.Core.Properties.Resources.uxMenuCalibrate, menuItemCalib);
// End of Calibrate Menu code.

// Start of Diagnostics Menu Code
            // Add Template to Diagnostics Menu if required. 
            // Comment out if not needed by your plugin.
           var menuItemDiagnostics = new TSMenuItem
            {
                Text = Resources.MenuItemTemplate,
                Permission = Resources.ExecuteTemplateDiagnostic,
                ValidWhen = ValidWhen.InTest
                // ValidOn = SystemCategory.MOC
            };
           menuItemDiagnostics.Click += OnMainmenuTemplateDiagnosticsClick;
            UXMap.MenuManager.InsertMenuItem(0, Jacobs.TestSLATE.UX.Core.Properties.Resources.uxMenuDiagnostics, menuItemDiagnostics);
// End of Diagnostics Menu Code.

// Start of Execute Menu Code
            // Add Template to Execute Menu if required. 
            // Comment out if not needed by your plugin.
            var menuItemExecute = new TSMenuItem
            {
                Text = Resources.MenuItemTemplate,
                Permission = Resources.ExecuteTemplateExecute,
                ValidWhen = ValidWhen.InTest
                // ValidOn = SystemCategory.MOC
            };
            menuItemExecute.Click += OnMainmenuTemplateExecuteClick;
            UXMap.MenuManager.InsertMenuItem(0, Jacobs.TestSLATE.UX.Core.Properties.Resources.uxMenuExecute, menuItemExecute);
// End of Execute Menu Code.
       
       }


        /// <summary>
        /// Called when [calibrate click].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
       private static void OnMainmenuTemplateCalibrateClick(object sender, EventArgs e)
        {
            MessageBox.Show("Received Template Calibrate Click", Resources.Confirmation,
                            MessageBoxButtons.OK);
            // This event should launch whatever editor or VI you need from the menu.
        }

        /// <summary>
        /// Called when [Execute click].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private static void OnMainmenuTemplateExecuteClick(object sender, EventArgs e)
        {
            MessageBox.Show("Received Template Execute Click", Resources.Confirmation,
                            MessageBoxButtons.OK);
            // This event should launch whatever editor or VI you need from the menu.
        }

        /// <summary>
        /// Called when [Diagnostics template click].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private static void OnMainmenuTemplateDiagnosticsClick(object sender, EventArgs e)
        {
            MessageBox.Show("Received Template Diagnostics Click", Resources.Confirmation,
                            MessageBoxButtons.OK);
            /* Uncomment this code and modify appropriately to start LabView VI to respond to diagnostics.
                        try
                        {
                            var aiEditor = new AiDiagnosticsVi();
                            if (aiEditor.ExecState == VirtualInstrumentExecState.Idle)
                            {
                                aiEditor.Run();
                            }
                            aiEditor.MakeFrontPanelVisible();
                            aiEditor.CenterFrontPanel();
                        }
                        catch (Exception ex)
                        {
                            ErrorHandler(ex, "Error in launching AI Diagnostics");
                        }
             */
        }

        /// <summary>
        ///   Create permissions.
        /// </summary>
        private static void CreatePermissions()
        {
            // Add any required permissions your source might need to the database using this method.
            var addPermission = new AddPermission(Resources.AddSourcePermission);
            addPermission.Execute();
            addPermission = new AddPermission(Resources.ExecuteTemplateCalibration);
            addPermission.Execute();
            addPermission = new AddPermission(Resources.EditTemplate);
            addPermission.Execute();
            addPermission = new AddPermission(Resources.RemoveTemplate);
            addPermission.Execute();
        }
    }
}