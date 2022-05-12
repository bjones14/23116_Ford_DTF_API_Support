// <copyright file="Editor.cs" company="Jacobs Technology, Inc.">
//     Copyright (c) Jacobs Technology, Inc. All rights reserved.
// </copyright>
// <summary>
//  This is the user interface. 
//  It implements the TestSLATE Tabbed Document.
//  It implements a generic IView that is custom to the type of Presenter it is interacting with.
//  In this example, the Drag and Drop events demonstrate moving rows around in the grid. Not dropping data in from external sources;
//  although it functions similarly.
//
//  This example implements:
//  IEditable / ISavable - to provide basic Test SLATE functionality such as Append, Insert, Cut, Copy, Paste and Save
//  IExportable - to provide basic export to file functions
//  In addition, this View provides support for Keyboard shortcuts.
//  All actions taken by the user in this UI are passed off to the Presenter for execution against the data.
//  Once the Presenter has executed the Command, it signals the UI to refresh the data.
//
//--------------------------------------------------------------------------------------------------------------
//  All editors must inherit from TSTabbedDocument and allow to be docked center and floating as options.
//  All editors must implement IEditable and ISavable (Test SLATE required).
//  All editors should handle KeyDown events, Drag and Drop events, IExportable, IReportable and IPrintable.
//
//  Initialization of variables and arrays should occur in the constructors, initialization that depends upon construction should happen in the OnLoad event.
// </summary>

using Customer.TestSLATE.Mnemonic.Models;

namespace Customer.TestSLATE.Mnemonic.UI
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;
    using DevExpress.XtraEditors;
    using DevExpress.XtraEditors.Controls;
    using DevExpress.XtraGrid;
    using DevExpress.XtraGrid.Columns;
    using DevExpress.XtraGrid.Views.Base;
    using DevExpress.XtraGrid.Views.Grid;
    using DevExpress.XtraGrid.Views.Grid.ViewInfo;
    using Customer.TestSLATE.Mnemonic.Presenters;
    using Customer.TestSLATE.Mnemonic.Properties;
    using Jacobs.TestSLATE.UX.Commands.DXGrid;
    using Jacobs.TestSLATE.UX.Core;
    using TSCell = Jacobs.TestSLATE.Cell.DomainLayer.Cells.ObjectClass;
    using TSConfig = Jacobs.TestSLATE.Cell.DomainLayer.Configurations.ObjectClass;
    using TSSource = Jacobs.TestSLATE.Cell.DomainLayer.Sources.ObjectClass;
    using TD.SandDock;

    /// <summary>
    ///   The Balances Editor. This is the Example User Interface.
    /// </summary>
    public sealed partial class Editor : TSEditor, IView, IEditable, IExportable
    {
        /// <summary>
        ///   Tracks where the MouseDown event fires in the GridControl.
        /// </summary>
        private GridHitInfo _downHitInfo;

        /// <summary>
        /// The Cell assoicated with the Editor
        /// </summary>
        private TSCell _cell;

        /// <summary>
        /// The Configuration associated with the Editor
        /// </summary>
        private TSConfig _config;

        /// <summary>
        /// The Source associated with the Editor
        /// </summary>
        private TSSource _source;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "Editor" /> class.
        /// </summary>
        public Editor(TSCell cell, TSConfig config, TSSource source)
        {
            _cell = cell;
            _config = config;
            _source = source;
            InitializeComponent();
            TabText = Resources.TabText;
            bindDataSource1.AllowNew = true;
        }

        /// <summary>
        ///   Appends this instance.
        /// </summary>
        public void Append()
        {
            Presenter.Append();
        }

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
        ///   Deletes this instance.
        /// </summary>
        public void Delete()
        {
            if (MessageBox.Show(Resources.ConfirmationDeleteRows, Resources.Confirmation, MessageBoxButtons.YesNo) !=
                DialogResult.Yes) return;
            var rowsSelected = this.view.GetSelectedRows();
            if (rowsSelected.Length < 1) return;
            Array.Reverse(rowsSelected);
            foreach (var row in rowsSelected)
            {
                this.Presenter.Delete(row);
            }
        }

        /// <summary>
        ///   Inserts this instance.
        /// </summary>
        public void Insert()
        {
            this.Presenter.Insert(this.view.FocusedRowHandle);
        }

        /// <summary>
        ///   Pastes this instance.
        /// </summary>
        public void Paste()
        {
            TSUtilities.ShallowPaste();
        }

        /// <summary>
        ///   Gets a value indicating whether this instance has error.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has error; otherwise, <c>false</c>.
        /// </value>
        public override bool HasError
        {
            get { return this.Presenter.IsValid; }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether this instance is dirty.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is dirty; otherwise, <c>false</c>.
        /// </value>
        public override bool IsDirty
        {
            get { return this.Presenter.IsDirty; }
        }

        /// <summary>
        ///   Saves this instance.
        /// </summary>
        public override void SaveChanges()
        {
            this.Presenter.Save();
        }

        /// <summary>
        ///   Exports this instance to file.
        /// </summary>
        public void Export()
        {
            // Export to xlsx will be the default.
            this.Export("xlsx");
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
                    var exportExcel = new ExportDXGridToXlsx(this.grid, "balances");
                    exportExcel.Execute();
                    break;
                case "pdf":
                    var exportPdf = new ExportDXGridToPdf(this.grid, "balances");
                    exportPdf.Execute();
                    break;
            }
        }

        /// <summary>
        ///   Gets or sets the data source.
        /// </summary>
        /// <value>
        ///   The data source.
        /// </value>
        public object Data
        {
            get { return this.bindDataSource1.DataSource; }
            set { this.bindDataSource1.DataSource = value; }
        }

        /// <summary>
        ///   The presenter that provides this view with data.
        /// </summary>
        public GridPresenter Presenter { get; set; }

        /// <summary>
        ///   Indicates the unsaved changes.
        /// </summary>
        public void ShowUnsavedChanges()
        {
            this.TabText = this.IsDirty ? Resources.TabTextUnsaved : Resources.TabText;
        }

        /// <summary>
        ///   Refreshes the data.
        /// </summary>
        public void RefreshData()
        {
            this.bindDataSource1.ResetBindings(true);
            this.grid.Update();
            this.grid.ResetBindings();
            this.grid.RefreshDataSource();
            this.grid.Refresh();
            this.grid.Invalidate();
        }

        /// <summary>
        ///   Called when [validate row].
        /// </summary>
        /// <param name = "sender">The sender.</param>
        /// <param name = "e">The <see cref = "DevExpress.XtraGrid.Views.Base.ValidateRowEventArgs" /> instance containing the event data.</param>
        private void OnValidateRow(object sender, ValidateRowEventArgs e)
        {
            e.Valid = this.Presenter.Validate(e.RowHandle);
            var columnView = sender as ColumnView;
            if (columnView == null) return;
            foreach (GridColumn column in columnView.Columns)
            {
                columnView.SetColumnError(column, this.Presenter.GetErrorMessage(e.RowHandle, column.FieldName));
            }

            this.RefreshData();
            this.ShowUnsavedChanges();
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
                    this.Copy();
                    break;
                case Keys.V:
                    this.Paste();
                    break;
                case Keys.S:
                    this.Save();
                    break;
                case Keys.Z:
                    this.Presenter.Undo();
                    break;
                case Keys.Y:
                    this.Presenter.Redo();
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
                    this.Append();
                    break;
                case NavigatorButtonType.Remove:
                    e.Handled = true;
                    this.Delete();
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
            this.ShowUnsavedChanges();
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
            this.MoveRow(sourceRow, targetRow);

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
            this._downHitInfo = e.Data.GetData(typeof (GridHitInfo)) as GridHitInfo;
            if (this._downHitInfo == null) return;
            var gridControl = sender as GridControl;
            if (gridControl == null) return;
            var gridView = gridControl.MainView as GridView;
            if (gridView == null) return;
            var hitInfo = gridView.CalcHitInfo(gridControl.PointToClient(new Point(e.X, e.Y)));
            e.Effect = hitInfo.InRow && hitInfo.RowHandle != this._downHitInfo.RowHandle &&
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
            this.Presenter.SwapRows(sourceRow, targetRow);
        }

        /// <summary>
        ///   Called when [load].
        /// </summary>
        /// <param name = "sender">The sender.</param>
        /// <param name = "e">The <see cref = "System.EventArgs" /> instance containing the event data.</param>
        private void OnLoad(object sender, EventArgs e)
        {
            ToolTipText = _cell.CellName + "." + _config.ConfigName;
            if (this.Presenter == null)
                return;
            this.Presenter.ValidateAll();
            // Add code here to show all errors in initial Validation.
        }

        /// <summary>
        ///   Called when [mouse down].
        /// </summary>
        /// <param name = "sender">The sender.</param>
        /// <param name = "e">The <see cref = "System.Windows.Forms.MouseEventArgs" /> instance containing the event data.</param>
        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            var gridView = sender as GridView;
            this._downHitInfo = null;
            if (gridView == null) return;
            var hitInfo = gridView.CalcHitInfo(new Point(e.X, e.Y));
            if (ModifierKeys != Keys.None) return;
            if (e.Button == MouseButtons.Left && hitInfo.InRow && hitInfo.RowHandle != GridControl.NewItemRowHandle)
            {
                this._downHitInfo = hitInfo;
            }
        }

        /// <summary>
        ///   Called when [mouse move].
        /// </summary>
        /// <param name = "sender">The sender.</param>
        /// <param name = "e">The <see cref = "System.Windows.Forms.MouseEventArgs" /> instance containing the event data.</param>
        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            var gridView = sender as GridView;
            if (e.Button != MouseButtons.Left || this._downHitInfo == null) return;
            var dragSize = SystemInformation.DragSize;
            var dragRect =
                new Rectangle(
                    new Point(this._downHitInfo.HitPoint.X - dragSize.Width/2,
                              this._downHitInfo.HitPoint.Y - dragSize.Height/2), dragSize);
            if (dragRect.Contains(new Point(e.X, e.Y))) return;
            if (gridView != null) gridView.GridControl.DoDragDrop(this._downHitInfo, DragDropEffects.All);
            this._downHitInfo = null;
        }

        /// <summary>
        /// Called when [dock situation changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnDockSituationChanged(object sender, EventArgs e)
        {
            switch (this.DockSituation)
            {
                case DockSituation.Docked:
                    this.TabText = Resources.TabText;
                    break;
                case DockSituation.Floating:
                    this.Text = _cell.CellName + "--" + _config.ConfigName + "--" + Resources.TabText;
;
                    break;
            }
        }
    }
}