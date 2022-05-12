#region References

using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using Jacobs.TestSLATE.Cell.DomainLayer.Sources;
using Jacobs.TestSLATE.DB;
using Customer.TestSLATE.Mnemonic.Models;
using Customer.TestSLATE.Mnemonic.Presenters;
using Customer.TestSLATE.Mnemonic.Properties;
using Jacobs.TestSLATE.UX.Commands.DXGrid;
using Jacobs.TestSLATE.UX.Core;
using TD.SandDock;

#endregion

namespace Customer.TestSLATE.Mnemonic.UI
{
    #region References

    using TSSource = ObjectClass;
    using TSCell = Jacobs.TestSLATE.Cell.DomainLayer.Cells.ObjectClass;

    #endregion

    /// <summary>
    ///   The Balances Editor. This is the Example User Interface.
    /// </summary>
    public sealed partial class SubsystemEditor : TSEditor, ISubsystemView, IEditable, IExportable
    {
        private readonly TSCell _cell;
        private readonly TextBox[] _rfmTextBoxes;
        private readonly TSSource _source;
        private GridHitInfo _downHitInfo;
        private string _previousRFMValue;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "Editor" /> class.
        /// </summary>
        public SubsystemEditor(TSSource source)
        {
            InitializeComponent();
            TabText = Resources.TabText;
            bindDataSource1.AllowNew = true;
            bindDataSource2.AllowNew = true;
            bindDataSource3.AllowNew = true;
            bindDataSource4.AllowNew = true;
            bindDataSource5.AllowNew = true;

            // Delete the references if you are not using EditBoxPresenter.
            _rfmTextBoxes = new TextBox[18];
            _rfmTextBoxes[0] = AIStartAddrTxt;
            _rfmTextBoxes[1] = AIEndAddrTxt;
            _rfmTextBoxes[2] = AOStartAddrTxt;
            _rfmTextBoxes[3] = AOEndAddrTxt;
            _rfmTextBoxes[4] = DIStartAddrTxt;
            _rfmTextBoxes[5] = DIEndAddrTxt;
            _rfmTextBoxes[6] = DOStartAddrTxt;
            _rfmTextBoxes[7] = DOEndAddrTxt;
            _rfmTextBoxes[8] = BalStartAddrTxt;
            _rfmTextBoxes[9] = BalEndAddrTxt;
            _rfmTextBoxes[10] = AIStatusStartAddrTxt;
            _rfmTextBoxes[11] = AOStatusStartAddrTxt;
            _rfmTextBoxes[12] = DIStatusStartAddrTxt;
            _rfmTextBoxes[13] = DOStatusStartAddrTxt;
            _rfmTextBoxes[14] = SystemStatusStartAddrTxt;
            _rfmTextBoxes[15] = HeartbeatAddrTxt;
            _rfmTextBoxes[16] = IRIGAddrTxt;
            _rfmTextBoxes[17] = BridgeLoopTimeText;

            _source = source;
            _cell = EnvironmentFactory.CellEnvironment().Cells.SelectByKeyItems(source.CellID);
        }

        /// <summary>
        ///   Gets a value indicating whether this instance has error.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has error; otherwise, <c>false</c>.
        /// </value>
        public override bool HasError
        {
            get { return ChannelPresenter.IsValid | /* RFMConfigPresenter.IsValid |*/ (CircularBufferConfigPresenter == null) ? false : CircularBufferConfigPresenter.IsValid; }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether this instance is dirty.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is dirty; otherwise, <c>false</c>.
        /// </value>
        public override bool IsDirty
        {
            get { return ChannelPresenter.IsDirty | /* RFMConfigPresenter.IsDirty |*/  (CircularBufferConfigPresenter == null) ? false : CircularBufferConfigPresenter.IsDirty; }
        }

        /// <summary>
        ///   Gets or sets the data source.
        /// </summary>
        /// <value>
        ///   The data source.
        /// </value>
        public object Data
        {
            get { return bindDataSource1.DataSource; }
            set { bindDataSource1.DataSource = value; }
        }

        #region IEditable Members

        /// <summary>
        ///   Copies this instance.
        /// </summary>
        public void Copy()
        {
            TSUtilities.ShallowCopy();
        }

        /// <summary>
        ///   Cuts this instance.
        /// </summary>
        /// <exception cref = "NotImplementedException"></exception>
        public void Cut()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///   Pastes this instance.
        /// </summary>
        public void Paste()
        {
            TSUtilities.ShallowPaste();
        }

        #endregion

        #region IExportable Members

        /// <summary>
        ///   Exports this instance to file.
        /// </summary>
        public void Export()
        {
            // Export to xlsx will be the default.
            Export("xlsx");
        }

        /// <summary>
        ///   Exports this instance to file using the specified format specifier.
        /// </summary>
        /// <param name = "formatSpecifier">The format specifier.</param>
        public void Export(string formatSpecifier)
        {
            switch (formatSpecifier)
            {
                case "xlsx":
                    var exportExcel = new ExportDXGridToXlsx(AIGrid, "balances");
                    exportExcel.Execute();
                    break;
                case "pdf":
                    var exportPdf = new ExportDXGridToPdf(AIGrid, "balances");
                    exportPdf.Execute();
                    break;
            }
        }

        #endregion

        #region ISubsystemView Members

        /// <summary>
        ///   Gets or sets the AI data source.
        /// </summary>
        /// <value>
        ///   The AI data source.
        /// </value>
        public object AIData
        {
            get { return bindDataSource1.DataSource; }
            set { bindDataSource1.DataSource = value; }
        }

        /// <summary>
        ///   Gets or sets the AO data source.
        /// </summary>
        /// <value>
        ///   The AO data source.
        /// </value>
        public object AOData
        {
            get { return bindDataSource2.DataSource; }
            set { bindDataSource2.DataSource = value; }
        }

        /// <summary>
        ///   Gets or sets the DI data source.
        /// </summary>
        /// <value>
        ///   The DI data source.
        /// </value>
        public object DIData
        {
            get { return bindDataSource3.DataSource; }
            set { bindDataSource3.DataSource = value; }
        }

        /// <summary>
        ///   Gets or sets the DO data source.
        /// </summary>
        /// <value>
        ///   The DO data source.
        /// </value>
        public object DOData
        {
            get { return bindDataSource4.DataSource; }
            set { bindDataSource4.DataSource = value; }
        }

        /// <summary>
        ///   Gets or sets the RFM Config data source.
        /// </summary>
        /// <value>
        ///   The RFM Config data source.
        /// </value>
        public object RFMData
        {
            get { return bindDataSource5.DataSource; }
            set { bindDataSource5.DataSource = value; }
        }

        /// <summary>
        ///   The presenter that provides this view with Circular Buffer configuration data.
        /// </summary>
        public EditBoxPresenter CircularBufferConfigPresenter { get; set; }

        /// <summary>
        ///   The presenter that provides this view with RFM Config data.
        /// </summary>
        public EditBoxPresenter RFMConfigPresenter { get; set; }

        /// <summary>
        ///   The presenter that provides this view with data.
        /// </summary>
        public ChannelPresenter ChannelPresenter { get; set; }

        /// <summary>
        ///   Indicates the unsaved changes.
        /// </summary>
        public void ShowUnsavedChanges()
        {
            TabText = IsDirty ? Resources.TabTextUnsaved : Resources.TabText;
        }

        /// <summary>
        ///   Refreshes the data.
        /// </summary>
        public void RefreshData()
        {
            bindDataSource1.ResetBindings(true);
            AIGrid.Update();
            AIGrid.ResetBindings();
            AIGrid.RefreshDataSource();
            AIGrid.Refresh();
            AIGrid.Invalidate();

            AOGrid.Update();
            AOGrid.ResetBindings();
            AOGrid.RefreshDataSource();
            AOGrid.Refresh();
            AOGrid.Invalidate();

            DIGrid.Update();
            DIGrid.ResetBindings();
            DIGrid.RefreshDataSource();
            DIGrid.Refresh();
            DIGrid.Invalidate();

            DOGrid.Update();
            DOGrid.ResetBindings();
            DOGrid.RefreshDataSource();
            DOGrid.Refresh();
            DOGrid.Invalidate();

            if (RFMConfigPresenter != null)
            {
                for (var i = 0; i < 18; i++)
                {
                    var namedConfigItem = RFMConfigPresenter.Model.GetItemAt(i);
                    _rfmTextBoxes[i].Text = namedConfigItem.StringValue;
                }
            }

        }

        #endregion

        /// <summary>
        ///   Appends this instance.
        /// </summary>
        public void Append()
        {
            ChannelPresenter.Append();
        }

        /// <summary>
        ///   Deletes this instance.
        /// </summary>
        public void Delete()
        {
            var rowsSelected = view.GetSelectedRows();
            if (rowsSelected.Length < 1) return;
            var defaultChannels = new DefaultChannels();
            if ((from row in rowsSelected
                 select ChannelPresenter.ChnlModels[ChannelPresenter.CurrentViewIndex].GetNameAt(row)
                 into channelName where defaultChannels.GetCount() > 0 from chnl in defaultChannels where channelName == chnl.ChannelName select channelName).Any())
            {
                MessageBox.Show(@"Standard Channels cannot be deleted.", Resources.Confirmation,
                                MessageBoxButtons.OK);
                return;
            }
            if (MessageBox.Show(Resources.ConfirmationDeleteRows, Resources.Confirmation, MessageBoxButtons.YesNo) !=
                DialogResult.Yes) return;
            Array.Reverse(rowsSelected);
            foreach (var row in rowsSelected)
            {
                ChannelPresenter.Delete(row);
            }
        }

        /// <summary>
        ///   Inserts this instance.
        /// </summary>
        public void Insert()
        {
            ChannelPresenter.Insert(view.FocusedRowHandle);
        }

        /// <summary>
        ///   Saves this instance.
        /// </summary>
        public override void SaveChanges()
        {
            ChannelPresenter.Save();
            RFMConfigPresenter.Save();
        }

        /// <summary>
        ///   Called when [validate row].
        /// </summary>
        /// <param name = "sender">The sender.</param>
        /// <param name = "e">The <see cref = "DevExpress.XtraGrid.Views.Base.ValidateRowEventArgs" /> instance containing the event data.</param>
        private void OnValidateRow(object sender, ValidateRowEventArgs e)
        {
            e.Valid = ChannelPresenter.Validate(e.RowHandle);
            var columnView = sender as ColumnView;
            if (columnView == null) return;
            foreach (GridColumn column in columnView.Columns)
            {
                columnView.SetColumnError(column, ChannelPresenter.GetErrorMessage(e.RowHandle, column.FieldName));
            }

            RefreshData();
            ShowUnsavedChanges();
        }

        /// <summary>
        ///   Called when [key down].
        /// </summary>
        /// <param name = "sender">The sender.</param>
        /// <param name = "e">The <see cref = "System.Windows.Forms.KeyEventArgs" /> instance containing the event data.</param>
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (!e.Control) return;
            switch (e.KeyCode)
            {
                case Keys.C:
                    Copy();
                    break;
                case Keys.V:
                    Paste();
                    break;
                case Keys.S:
                    Save();
                    break;
                case Keys.Z:
                    ChannelPresenter.Undo();
                    break;
                case Keys.Y:
                    ChannelPresenter.Redo();
                    break;
                case Keys.F1:
                    // Show some kind of help.
                    break;
            }
        }

        /// <summary>
        ///   Called when [navigator button click].
        /// </summary>
        /// <param name = "sender">The sender.</param>
        /// <param name = "e">The <see cref = "DevExpress.XtraEditors.NavigatorButtonClickEventArgs" /> instance containing the event data.</param>
        private void OnNavigatorButtonClick(object sender, NavigatorButtonClickEventArgs e)
        {
            switch (e.Button.ButtonType)
            {
                case NavigatorButtonType.Custom:
                    break;
                case NavigatorButtonType.First:
                    break;
                case NavigatorButtonType.PrevPage:
                    break;
                case NavigatorButtonType.Prev:
                    break;
                case NavigatorButtonType.Next:
                    break;
                case NavigatorButtonType.NextPage:
                    break;
                case NavigatorButtonType.Last:
                    break;
                case NavigatorButtonType.Append:
                    e.Handled = true;
                    Append();
                    break;
                case NavigatorButtonType.Remove:
                    e.Handled = true;
                    Delete();
                    break;
                case NavigatorButtonType.Edit:
                    break;
                case NavigatorButtonType.EndEdit:
                    break;
                case NavigatorButtonType.CancelEdit:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        ///   Called when [invalid row exception].
        /// </summary>
        /// <param name = "sender">The sender.</param>
        /// <param name = "e">The <see cref = "DevExpress.XtraGrid.Views.Base.InvalidRowExceptionEventArgs" /> instance containing the event data.</param>
        private void OnInvalidRowException(object sender, InvalidRowExceptionEventArgs e)
        {
            e.ExceptionMode = ExceptionMode.NoAction;
        }

        /// <summary>
        ///   Called when [invalid value exception].
        /// </summary>
        /// <param name = "sender">The sender.</param>
        /// <param name = "e">The <see cref = "DevExpress.XtraEditors.Controls.InvalidValueExceptionEventArgs" /> instance containing the event data.</param>
        private void OnInvalidValueException(object sender, InvalidValueExceptionEventArgs e)
        {
            e.ExceptionMode = ExceptionMode.NoAction;
        }

        /// <summary>
        ///   Called when [cell value changed].
        /// </summary>
        /// <param name = "sender">The sender.</param>
        /// <param name = "e">The <see cref = "DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs" /> instance containing the event data.</param>
        private void OnCellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            ShowUnsavedChanges();
        }

        /// <summary>
        ///   Called when [drag drop].
        /// </summary>
        /// <param name = "sender">The sender.</param>
        /// <param name = "e">The <see cref = "System.Windows.Forms.DragEventArgs" /> instance containing the event data.</param>
        /// <exception cref = "NotImplementedException"></exception>
        private void OnDragDrop(object sender, DragEventArgs e)
        {
            var gridControl = sender as GridControl;
            if (gridControl == null) return;
            var gridView = gridControl.MainView as GridView;
            var srcHitInfo = e.Data.GetData(typeof (GridHitInfo)) as GridHitInfo;
            if (gridView == null) return;
            var hitInfo = gridView.CalcHitInfo(gridControl.PointToClient(new Point(e.X, e.Y)));
            if (srcHitInfo == null) return;
            var sourceRow = srcHitInfo.RowHandle;
            var targetRow = hitInfo.RowHandle;
            MoveRow(sourceRow, targetRow);

            // To handle drops from external sources (Word, WordPad, etc), cast the e.Data.GetData() to be the type of data you want to expect.
            // Typically in the case of MS Word, that will be typeof(string). Checking the type of the object being dropped will let 
            // you handle both objects being dropped from external programs like MS Word, or internal (like rows inside this GridControl).
        }

        /// <summary>
        ///   Called when [drag over].
        /// </summary>
        /// <param name = "sender">The sender.</param>
        /// <param name = "e">The <see cref = "System.Windows.Forms.DragEventArgs" /> instance containing the event data.</param>
        /// <exception cref = "NotImplementedException"></exception>
        private void OnDragOver(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof (GridHitInfo))) return;
            _downHitInfo = e.Data.GetData(typeof (GridHitInfo)) as GridHitInfo;
            if (_downHitInfo == null) return;
            var gridControl = sender as GridControl;
            if (gridControl == null) return;
            var gridView = gridControl.MainView as GridView;
            if (gridView == null) return;
            var hitInfo = gridView.CalcHitInfo(gridControl.PointToClient(new Point(e.X, e.Y)));
            e.Effect = hitInfo.InRow && hitInfo.RowHandle != _downHitInfo.RowHandle &&
                       hitInfo.RowHandle != GridControl.NewItemRowHandle
                           ? DragDropEffects.Move
                           : DragDropEffects.None;
        }

        /// <summary>
        ///   Moves the row.
        /// </summary>
        /// <param name = "sourceRow">The source row.</param>
        /// <param name = "targetRow">The target row.</param>
        private void MoveRow(int sourceRow, int targetRow)
        {
            // Tell the presenter to swap the rows.
            // Add a method to the presenter to move items around in the collection.
            ChannelPresenter.SwapRows(sourceRow, targetRow);
        }

        /// <summary>
        ///   Called when [load].
        /// </summary>
        /// <param name = "sender">The sender.</param>
        /// <param name = "e">The <see cref = "System.EventArgs" /> instance containing the event data.</param>
        private void OnLoad(object sender, EventArgs e)
        {
            ToolTipText = String.Format("{0}.{1}", _cell.CellName, _source.SourceName);
            if (ChannelPresenter == null) return;
            ChannelPresenter.ValidateAll();
            if (RFMConfigPresenter == null) return;
            RFMConfigPresenter.ValidateAll();
            tabPage5.Visible = false;
            // Add code here to show all errors in initial Validation.
        }

        /// <summary>
        ///   Called when [mouse down].
        /// </summary>
        /// <param name = "sender">The sender.</param>
        /// <param name = "e">The <see cref = "System.Windows.Forms.MouseEventArgs" /> instance containing the event data.</param>
        private void OnMouseDown(object sender, MouseEventArgs e)
        {
        }

        /// <summary>
        ///   Called when [mouse move].
        /// </summary>
        /// <param name = "sender">The sender.</param>
        /// <param name = "e">The <see cref = "System.Windows.Forms.MouseEventArgs" /> instance containing the event data.</param>
        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            var gridView = sender as GridView;
            _downHitInfo = null;
            if (gridView == null) return;
            var hitInfo = gridView.CalcHitInfo(new Point(e.X, e.Y));
            if (ModifierKeys != Keys.None) return;
            if (e.Button == MouseButtons.Left && hitInfo.InRow && hitInfo.RowHandle != GridControl.NewItemRowHandle)
            {
                _downHitInfo = hitInfo;
            }
        }

        /// <summary>
        ///   Called when [dock situation changed].
        /// </summary>
        /// <param name = "sender">The sender.</param>
        /// <param name = "e">The <see cref = "System.EventArgs" /> instance containing the event data.</param>
        private void OnDockSituationChanged(object sender, EventArgs e)
        {
            switch (DockSituation)
            {
                case DockSituation.Docked:
                    TabText = Resources.TabText;
                    break;
                case DockSituation.Floating:
                    Text = String.Format("{0}--{1}--{2}", _cell.CellName, _source.SourceName, Resources.TabText);
                    break;
            }
        }

        /// <summary>
        /// Called when [tab control index change].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnTabControlIndexChange(object sender, EventArgs e)
        {
            ChannelPresenter.CurrentViewIndex = TabControl.SelectedIndex;
        }

        /// <summary>
        /// Gets the current grid.
        /// </summary>
        /// <returns></returns>
        private GridView GetCurrentGrid()
        {
            switch (TabControl.SelectedIndex)
            {
                case 0:
                    return view;
                case 1:
                    return gridView1;
                case 2:
                    return gridView2;
                case 3:
                    return gridView3;
                default:
                    return view;
            }
        }

        /// <summary>
        /// Called when [edit box lost focus].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnEditBoxLostFocus(object sender, EventArgs e)
        {
            for (var i = 0; i < 18; i++)
            {
                if (sender != _rfmTextBoxes[i]) continue;
                try
                {
                    var intvalue = Convert.ToInt32(_rfmTextBoxes[i].Text);
                    if (intvalue < 0)
                    {
                        MessageBox.Show(@"Input value must be positive. Reverting to previous value",
                                        @"Template RFM Editor", MessageBoxButtons.OK);
                        _rfmTextBoxes[i].Text = _previousRFMValue;
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show(
                        @"Input value is not valid. Input must be an integer. Reverting to previous value",
                        @"Template RFM Editor", MessageBoxButtons.OK);
                    _rfmTextBoxes[i].Text = _previousRFMValue;
                }

                if (_rfmTextBoxes[i].Text != _previousRFMValue)
                {
                    RFMConfigPresenter.DataChanged(i, _rfmTextBoxes[i].Text, 0, 0.0F);
                }
            }
        }

        /// <summary>
        /// Called when [edit box got focus].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnEditBoxGotFocus(object sender, EventArgs e)
        {
            for (var i = 0; i < 18; i++)
            {
                if (sender == _rfmTextBoxes[i])
                {
                    _previousRFMValue = _rfmTextBoxes[i].Text;
                }
            }
        }

        /// <summary>
        /// Called when [text control changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnTextControlChanged(object sender, EventArgs e)
        {
            RFMConfigPresenter.IsDirty = true;
        }
    }
}