namespace Casewise.GraphAPI.GUI
{
    public partial class cwEditModeGUI
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
            this.richTextBoxDebug = new System.Windows.Forms.RichTextBox();
            this.splitContainerMain = new System.Windows.Forms.SplitContainer();
            this.splitContainerTop = new System.Windows.Forms.SplitContainer();
            this.groupBoxDesigns = new System.Windows.Forms.GroupBox();
            this.treeViewConfigurations = new System.Windows.Forms.TreeView();
            this.groupBoxDetails = new System.Windows.Forms.GroupBox();
            this.panelOptions = new System.Windows.Forms.Panel();
            this.menuStripTop = new System.Windows.Forms.MenuStrip();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.autoSaveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.backgroundWorker2 = new System.ComponentModel.BackgroundWorker();
            this.backgroundWorker3 = new System.ComponentModel.BackgroundWorker();
            this.backgroundWorker4 = new System.ComponentModel.BackgroundWorker();
            this.backgroundWorker5 = new System.ComponentModel.BackgroundWorker();
            this.splitContainerMain.Panel1.SuspendLayout();
            this.splitContainerMain.Panel2.SuspendLayout();
            this.splitContainerMain.SuspendLayout();
            this.splitContainerTop.Panel1.SuspendLayout();
            this.splitContainerTop.Panel2.SuspendLayout();
            this.splitContainerTop.SuspendLayout();
            this.groupBoxDesigns.SuspendLayout();
            this.groupBoxDetails.SuspendLayout();
            this.menuStripTop.SuspendLayout();
            this.SuspendLayout();
            // 
            // richTextBoxDebug
            // 
            this.richTextBoxDebug.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBoxDebug.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBoxDebug.Location = new System.Drawing.Point(0, 0);
            this.richTextBoxDebug.Name = "richTextBoxDebug";
            this.richTextBoxDebug.Size = new System.Drawing.Size(846, 71);
            this.richTextBoxDebug.TabIndex = 3;
            this.richTextBoxDebug.Text = "";
            // 
            // splitContainerMain
            // 
            this.splitContainerMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerMain.Location = new System.Drawing.Point(0, 24);
            this.splitContainerMain.Margin = new System.Windows.Forms.Padding(10, 10, 10, 10);
            this.splitContainerMain.Name = "splitContainerMain";
            this.splitContainerMain.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerMain.Panel1
            // 
            this.splitContainerMain.Panel1.Controls.Add(this.splitContainerTop);
            this.splitContainerMain.Panel1.Padding = new System.Windows.Forms.Padding(8, 8, 8, 8);
            // 
            // splitContainerMain.Panel2
            // 
            this.splitContainerMain.Panel2.Controls.Add(this.richTextBoxDebug);
            this.splitContainerMain.Size = new System.Drawing.Size(846, 488);
            this.splitContainerMain.SplitterDistance = 413;
            this.splitContainerMain.TabIndex = 4;
            // 
            // splitContainerTop
            // 
            this.splitContainerTop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerTop.Location = new System.Drawing.Point(8, 8);
            this.splitContainerTop.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.splitContainerTop.Name = "splitContainerTop";
            // 
            // splitContainerTop.Panel1
            // 
            this.splitContainerTop.Panel1.Controls.Add(this.groupBoxDesigns);
            // 
            // splitContainerTop.Panel2
            // 
            this.splitContainerTop.Panel2.Controls.Add(this.groupBoxDetails);
            this.splitContainerTop.Size = new System.Drawing.Size(830, 397);
            this.splitContainerTop.SplitterDistance = 272;
            this.splitContainerTop.TabIndex = 3;
            // 
            // groupBoxDesigns
            // 
            this.groupBoxDesigns.BackColor = System.Drawing.Color.White;
            this.groupBoxDesigns.Controls.Add(this.treeViewConfigurations);
            this.groupBoxDesigns.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxDesigns.Location = new System.Drawing.Point(0, 0);
            this.groupBoxDesigns.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBoxDesigns.Name = "groupBoxDesigns";
            this.groupBoxDesigns.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBoxDesigns.Size = new System.Drawing.Size(272, 397);
            this.groupBoxDesigns.TabIndex = 0;
            this.groupBoxDesigns.TabStop = false;
            this.groupBoxDesigns.Text = "Configurations";
            // 
            // treeViewConfigurations
            // 
            this.treeViewConfigurations.AllowDrop = true;
            this.treeViewConfigurations.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeViewConfigurations.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewConfigurations.HideSelection = false;
            this.treeViewConfigurations.Location = new System.Drawing.Point(4, 17);
            this.treeViewConfigurations.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.treeViewConfigurations.Name = "treeViewConfigurations";
            this.treeViewConfigurations.Size = new System.Drawing.Size(264, 376);
            this.treeViewConfigurations.TabIndex = 0;
            this.treeViewConfigurations.MouseMove += new System.Windows.Forms.MouseEventHandler(this.treeViewConfigurations_MouseMove);
            this.treeViewConfigurations.MouseUp += new System.Windows.Forms.MouseEventHandler(this.treeViewConfigurations_MouseUp);
            // 
            // groupBoxDetails
            // 
            this.groupBoxDetails.AutoSize = true;
            this.groupBoxDetails.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBoxDetails.BackColor = System.Drawing.Color.White;
            this.groupBoxDetails.Controls.Add(this.panelOptions);
            this.groupBoxDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxDetails.Location = new System.Drawing.Point(0, 0);
            this.groupBoxDetails.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBoxDetails.Name = "groupBoxDetails";
            this.groupBoxDetails.Padding = new System.Windows.Forms.Padding(10, 12, 10, 12);
            this.groupBoxDetails.Size = new System.Drawing.Size(554, 397);
            this.groupBoxDetails.TabIndex = 1;
            this.groupBoxDetails.TabStop = false;
            this.groupBoxDetails.Text = "Options";
            // 
            // panelOptions
            // 
            this.panelOptions.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panelOptions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelOptions.Location = new System.Drawing.Point(10, 25);
            this.panelOptions.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panelOptions.Name = "panelOptions";
            this.panelOptions.Size = new System.Drawing.Size(534, 360);
            this.panelOptions.TabIndex = 2;
            // 
            // menuStripTop
            // 
            this.menuStripTop.Font = new System.Drawing.Font("Trebuchet MS", 8.25F);
            this.menuStripTop.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToolStripMenuItem,
            this.optionsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStripTop.Location = new System.Drawing.Point(0, 0);
            this.menuStripTop.Name = "menuStripTop";
            this.menuStripTop.Size = new System.Drawing.Size(846, 24);
            this.menuStripTop.TabIndex = 5;
            this.menuStripTop.Text = "menuStrip1";
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Enabled = false;
            this.saveToolStripMenuItem.Image = global::Casewise.GraphAPI.Properties.Resources.image_tvicon_save;
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(59, 20);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.autoSaveToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(59, 20);
            this.optionsToolStripMenuItem.Text = "Options";
            // 
            // autoSaveToolStripMenuItem
            // 
            this.autoSaveToolStripMenuItem.CheckOnClick = true;
            this.autoSaveToolStripMenuItem.Name = "autoSaveToolStripMenuItem";
            this.autoSaveToolStripMenuItem.Size = new System.Drawing.Size(126, 22);
            this.autoSaveToolStripMenuItem.Text = "Auto-Save";
            this.autoSaveToolStripMenuItem.Click += new System.EventHandler(this.autoSaveToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.Image = global::Casewise.GraphAPI.Properties.Resources.image_option_help_32x32;
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(57, 20);
            this.helpToolStripMenuItem.Text = "Help";
            this.helpToolStripMenuItem.Click += new System.EventHandler(this.helpToolStripMenuItem_Click);
            // 
            // cwEditModeGUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(846, 512);
            this.Controls.Add(this.splitContainerMain);
            this.Controls.Add(this.menuStripTop);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Trebuchet MS", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MainMenuStrip = this.menuStripTop;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "cwEditModeGUI";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "cwEditModeGUI";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.cwEditModeGUI_FormClosing);
            this.Shown += new System.EventHandler(this.cwEditModeGUI_Shown);
            this.splitContainerMain.Panel1.ResumeLayout(false);
            this.splitContainerMain.Panel2.ResumeLayout(false);
            this.splitContainerMain.ResumeLayout(false);
            this.splitContainerTop.Panel1.ResumeLayout(false);
            this.splitContainerTop.Panel2.ResumeLayout(false);
            this.splitContainerTop.Panel2.PerformLayout();
            this.splitContainerTop.ResumeLayout(false);
            this.groupBoxDesigns.ResumeLayout(false);
            this.groupBoxDetails.ResumeLayout(false);
            this.menuStripTop.ResumeLayout(false);
            this.menuStripTop.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        /// <summary>
        /// Provide the debug/info information
        /// </summary>
        private System.Windows.Forms.RichTextBox richTextBoxDebug;
        private System.Windows.Forms.SplitContainer splitContainerMain;
        private System.Windows.Forms.GroupBox groupBoxDetails;
        private System.Windows.Forms.Panel panelOptions;
        /// <summary>
        /// 
        /// </summary>
        protected System.Windows.Forms.GroupBox groupBoxDesigns;
        /// <summary>
        /// treeViewConfigurations
        /// </summary>
        public System.Windows.Forms.TreeView treeViewConfigurations;
        /// <summary>
        /// splitContainerTop
        /// </summary>
        protected System.Windows.Forms.SplitContainer splitContainerTop;
        private System.Windows.Forms.MenuStrip menuStripTop;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        /// <summary>
        /// save menu
        /// </summary>
        public System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem autoSaveToolStripMenuItem;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.ComponentModel.BackgroundWorker backgroundWorker2;
        private System.ComponentModel.BackgroundWorker backgroundWorker3;
        private System.ComponentModel.BackgroundWorker backgroundWorker4;
        private System.ComponentModel.BackgroundWorker backgroundWorker5;
    }
}