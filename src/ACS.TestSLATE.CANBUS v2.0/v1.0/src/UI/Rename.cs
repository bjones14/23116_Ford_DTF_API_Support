using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Customer.TestSLATE.Mnemonic.Properties;
using TSSource = Jacobs.TestSLATE.Cell.DomainLayer.Sources.ObjectClass;


namespace Customer.TestSLATE.Mnemonic.UI
{
    ///<summary>
    ///</summary>
    public partial class Rename : Form
    {

        private TSSource _source;
        ///<summary>
        ///</summary>
        public Rename(TSSource source)
        {
            InitializeComponent();
            _source = source;
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
                txtGroupName.Text = _source.SourceName;
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
                MessageBox.Show(Resources.NoBlankSourceName);
            }
            else
            {
                _source.SourceName = txtGroupName.Text;
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
