//-----------------------------------------------------------------------
// <copyright file="DisplayGroupEditor.cs" company="Jacobs Technology, Inc.">
//     Copyright (c) Jacobs Technology, Inc. All rights reserved.
// </copyright>
// <author>Jason D Peek</author>
// <date>10 August 2010</date>
// <summary>Editor for editing Display Groups.</summary>
//-----------------------------------------------------------------------

using System;
using System.Windows.Forms;
using Jacobs.TestSLATE.Cell.DomainLayer.StandardDisplayGroups;
using Jacobs.TestSLATE.Displays.Properties;

namespace Jacobs.TestSLATE.Displays.Editors
{
    using DispGroups = ObjectClass;

    /// <summary>
    /// Editor for editing Display Groups.
    /// </summary>
    public partial class DisplayGroupEditor : Form
    {
        /// <summary>
        /// The display group.
        /// </summary>
        private readonly DispGroups _group;

        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayGroupEditor"/> class.
        /// </summary>
        /// <param name="group">The group.</param>
        public DisplayGroupEditor(DispGroups group)
        {
            InitializeComponent();
            this._group = group;
        }

        /// <summary>
        /// Called when [load].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnLoad(object sender, EventArgs e)
        {
            if (!DesignMode)
            {
                txtGroupName.Text = _group.GroupName;
            }
        }

        /// <summary>
        /// Called when [save click].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnSaveClick(object sender, EventArgs e)
        {
            if (txtGroupName.Text.Length <= 0)
            {
                MessageBox.Show(Resources.NoBlankGroupName);
            }
            else
            {
                _group.GroupName = txtGroupName.Text;
                _group.Save();
                Close();
            }
        }

        /// <summary>
        /// Called when [cancel click].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnCancelClick(object sender, EventArgs e)
        {
            Close();
        }
    }
}