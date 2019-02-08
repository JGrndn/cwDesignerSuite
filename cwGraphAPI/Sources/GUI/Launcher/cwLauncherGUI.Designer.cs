
namespace Casewise.GraphAPI.Launcher
{
    /// <summary>
    /// cwLauncherGUI
    /// </summary>
    /// <typeparam name="GuiType">The type of the U i_ TYPE.</typeparam>
    /// <typeparam name="RootNodeType">The type of the OO t_ NODE.</typeparam>
    /// <typeparam name="CreateItemType">The type of the REAT e_ ITEM.</typeparam>
    partial class cwLauncherGUI<GuiType, RootNodeType, CreateItemType>
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
            this.flowLayoutPanelItems = new System.Windows.Forms.FlowLayoutPanel();
            this.panelMain = new System.Windows.Forms.Panel();
            this.splitContainerMain = new System.Windows.Forms.SplitContainer();
            this.flowLayoutPanelOptions = new System.Windows.Forms.FlowLayoutPanel();
            this.labelCopyright = new System.Windows.Forms.Label();
            this.panelMain.SuspendLayout();
            this.splitContainerMain.Panel1.SuspendLayout();
            this.splitContainerMain.Panel2.SuspendLayout();
            this.splitContainerMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanelItems
            // 
            this.flowLayoutPanelItems.AutoScroll = true;
            this.flowLayoutPanelItems.BackColor = System.Drawing.Color.White;
            this.flowLayoutPanelItems.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.flowLayoutPanelItems.Cursor = System.Windows.Forms.Cursors.Default;
            this.flowLayoutPanelItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanelItems.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanelItems.Margin = new System.Windows.Forms.Padding(20);
            this.flowLayoutPanelItems.Name = "flowLayoutPanelItems";
            this.flowLayoutPanelItems.Padding = new System.Windows.Forms.Padding(20, 10, 20, 20);
            this.flowLayoutPanelItems.Size = new System.Drawing.Size(689, 246);
            this.flowLayoutPanelItems.TabIndex = 0;
            // 
            // panelMain
            // 
            this.panelMain.BackColor = System.Drawing.Color.White;
            this.panelMain.Controls.Add(this.splitContainerMain);
            this.panelMain.Controls.Add(this.labelCopyright);
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(5, 5);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(689, 379);
            this.panelMain.TabIndex = 2;
            // 
            // splitContainerMain
            // 
            this.splitContainerMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerMain.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainerMain.IsSplitterFixed = true;
            this.splitContainerMain.Location = new System.Drawing.Point(0, 0);
            this.splitContainerMain.Name = "splitContainerMain";
            this.splitContainerMain.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerMain.Panel1
            // 
            this.splitContainerMain.Panel1.Controls.Add(this.flowLayoutPanelOptions);
            this.splitContainerMain.Panel1MinSize = 60;
            // 
            // splitContainerMain.Panel2
            // 
            this.splitContainerMain.Panel2.Controls.Add(this.flowLayoutPanelItems);
            this.splitContainerMain.Size = new System.Drawing.Size(689, 360);
            this.splitContainerMain.SplitterDistance = 113;
            this.splitContainerMain.SplitterWidth = 1;
            this.splitContainerMain.TabIndex = 5;
            // 
            // flowLayoutPanelOptions
            // 
            this.flowLayoutPanelOptions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanelOptions.BackColor = System.Drawing.Color.Transparent;
            this.flowLayoutPanelOptions.Location = new System.Drawing.Point(613, 0);
            this.flowLayoutPanelOptions.Name = "flowLayoutPanelOptions";
            this.flowLayoutPanelOptions.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.flowLayoutPanelOptions.Size = new System.Drawing.Size(76, 28);
            this.flowLayoutPanelOptions.TabIndex = 4;
            // 
            // labelCopyright
            // 
            this.labelCopyright.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.labelCopyright.Font = new System.Drawing.Font("Trebuchet MS", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelCopyright.Location = new System.Drawing.Point(0, 360);
            this.labelCopyright.Name = "labelCopyright";
            this.labelCopyright.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.labelCopyright.Size = new System.Drawing.Size(689, 19);
            this.labelCopyright.TabIndex = 3;
            this.labelCopyright.Text = "Copyright Casewise 2012 - Internal Use Only";
            this.labelCopyright.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelCopyright.Visible = false;
            // 
            // cwLauncherGUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.RoyalBlue;
            this.ClientSize = new System.Drawing.Size(699, 389);
            this.Controls.Add(this.panelMain);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Trebuchet MS", 8.25F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "cwLauncherGUI";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.Text = "Casewise Launcher";
            this.Activated += new System.EventHandler(this.cwLauncherGUI_Activated);
            this.Deactivate += new System.EventHandler(this.cwLauncherGUI_Deactivate);
            this.Shown += new System.EventHandler(this.cwLauncherGUI_Shown);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.cwLauncherGUI_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.cwLauncherGUI_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.cwLauncherGUI_MouseUp);
            this.Move += new System.EventHandler(this.cwLauncherGUI_Move);
            this.Resize += new System.EventHandler(this.cwLauncherGUI_Resize);
            this.panelMain.ResumeLayout(false);
            this.splitContainerMain.Panel1.ResumeLayout(false);
            this.splitContainerMain.Panel2.ResumeLayout(false);
            this.splitContainerMain.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.FlowLayoutPanel flowLayoutPanelItems;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelOptions;
        /// <summary>
        /// labelCopyright
        /// </summary>
        public System.Windows.Forms.Label labelCopyright;
        internal System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.SplitContainer splitContainerMain;
        
    }
}