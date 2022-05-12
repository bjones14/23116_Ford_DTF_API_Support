namespace Customer.TestSLATE.Mnemonic.UI
{
    sealed partial class Editor
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            TD.SandDock.DockingRules dockingRules1 = new TD.SandDock.DockingRules();
            this.bindDataSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.bindDataSource2 = new System.Windows.Forms.BindingSource(this.components);
            this.bindDataSource3 = new System.Windows.Forms.BindingSource(this.components);
            this.bindDataSource4 = new System.Windows.Forms.BindingSource(this.components);
            this.bindDataSource5 = new System.Windows.Forms.BindingSource(this.components);
            this.grid = new DevExpress.XtraGrid.GridControl();
            this.view = new DevExpress.XtraGrid.Views.Grid.GridView();
            ((System.ComponentModel.ISupportInitialize)(this.bindDataSource1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindDataSource2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindDataSource3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindDataSource4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindDataSource5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.view)).BeginInit();
            this.SuspendLayout();
            // 
            // grid
            // 
            this.grid.DataSource = this.bindDataSource1;
            this.grid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grid.EmbeddedNavigator.ButtonClick += new DevExpress.XtraEditors.NavigatorButtonClickEventHandler(this.OnNavigatorButtonClick);
            this.grid.Location = new System.Drawing.Point(0, 0);
            this.grid.MainView = this.view;
            this.grid.Name = "grid";
            this.grid.Size = new System.Drawing.Size(787, 519);
            this.grid.TabIndex = 3;
            this.grid.UseEmbeddedNavigator = true;
            this.grid.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.view});
            // 
            // view
            // 
            this.view.GridControl = this.grid;
            this.view.Name = "view";
            // 
            // Editor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.grid);
            dockingRules1.AllowDockBottom = false;
            dockingRules1.AllowDockLeft = false;
            dockingRules1.AllowDockRight = false;
            dockingRules1.AllowDockTop = false;
            dockingRules1.AllowFloat = true;
            dockingRules1.AllowTab = true;
            this.DockingRules = dockingRules1;
            this.Name = "Editor";
            this.Size = new System.Drawing.Size(787, 519);
            this.Text = "";
            this.Load += new System.EventHandler(this.OnLoad);
            this.DockSituationChanged += new System.EventHandler(this.OnDockSituationChanged);
            this.DragOver += new System.Windows.Forms.DragEventHandler(this.OnDragOver);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnKeyDown);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.OnMouseMove);
            ((System.ComponentModel.ISupportInitialize)(this.bindDataSource1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindDataSource2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindDataSource3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindDataSource4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindDataSource5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.view)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.BindingSource bindDataSource1;
        private System.Windows.Forms.BindingSource bindDataSource2;
        private System.Windows.Forms.BindingSource bindDataSource3;
        private System.Windows.Forms.BindingSource bindDataSource4;
        private System.Windows.Forms.BindingSource bindDataSource5;
        private DevExpress.XtraGrid.GridControl grid;
        private DevExpress.XtraGrid.Views.Grid.GridView view;



    }
}