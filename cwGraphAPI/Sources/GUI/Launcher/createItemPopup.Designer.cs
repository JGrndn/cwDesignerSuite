namespace Casewise.GraphAPI.Launcher
{
    partial class createItemPopup
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(createItemPopup));
            this.panelMain = new System.Windows.Forms.Panel();
            this.panelData = new System.Windows.Forms.Panel();
            this.panelTitle = new System.Windows.Forms.Panel();
            this.labelChooseName = new System.Windows.Forms.Label();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.panelMain.SuspendLayout();
            this.panelTitle.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelMain
            // 
            this.panelMain.BackColor = System.Drawing.Color.White;
            this.panelMain.Controls.Add(this.panelData);
            this.panelMain.Controls.Add(this.panelTitle);
            this.panelMain.Controls.Add(this.buttonCancel);
            this.panelMain.Controls.Add(this.buttonOK);
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(2, 2);
            this.panelMain.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(464, 291);
            this.panelMain.TabIndex = 0;
            // 
            // panelData
            // 
            this.panelData.Location = new System.Drawing.Point(15, 43);
            this.panelData.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panelData.Name = "panelData";
            this.panelData.Size = new System.Drawing.Size(432, 204);
            this.panelData.TabIndex = 5;
            // 
            // panelTitle
            // 
            this.panelTitle.BackColor = System.Drawing.Color.DeepSkyBlue;
            this.panelTitle.Controls.Add(this.labelChooseName);
            this.panelTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTitle.Location = new System.Drawing.Point(0, 0);
            this.panelTitle.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panelTitle.Name = "panelTitle";
            this.panelTitle.Size = new System.Drawing.Size(464, 34);
            this.panelTitle.TabIndex = 4;
            // 
            // labelChooseName
            // 
            this.labelChooseName.BackColor = System.Drawing.Color.White;
            this.labelChooseName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelChooseName.Font = new System.Drawing.Font("Trebuchet MS", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelChooseName.ForeColor = System.Drawing.Color.MediumBlue;
            this.labelChooseName.Location = new System.Drawing.Point(0, 0);
            this.labelChooseName.Name = "labelChooseName";
            this.labelChooseName.Size = new System.Drawing.Size(464, 34);
            this.labelChooseName.TabIndex = 4;
            this.labelChooseName.Text = "designName";
            this.labelChooseName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // buttonCancel
            // 
            this.buttonCancel.BackColor = System.Drawing.Color.DarkRed;
            this.buttonCancel.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonCancel.Font = new System.Drawing.Font("Trebuchet MS", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonCancel.ForeColor = System.Drawing.Color.White;
            this.buttonCancel.Location = new System.Drawing.Point(333, 255);
            this.buttonCancel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(114, 28);
            this.buttonCancel.TabIndex = 1;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = false;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonOK
            // 
            this.buttonOK.BackColor = System.Drawing.Color.OliveDrab;
            this.buttonOK.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonOK.Font = new System.Drawing.Font("Trebuchet MS", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonOK.ForeColor = System.Drawing.Color.White;
            this.buttonOK.Location = new System.Drawing.Point(15, 255);
            this.buttonOK.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(114, 28);
            this.buttonOK.TabIndex = 0;
            this.buttonOK.Text = "Create";
            this.buttonOK.UseVisualStyleBackColor = false;
            this.buttonOK.Click += new System.EventHandler(this.bOK_Click);
            // 
            // createItemPopup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.MediumBlue;
            this.ClientSize = new System.Drawing.Size(468, 295);
            this.Controls.Add(this.panelMain);
            this.Font = new System.Drawing.Font("Trebuchet MS", 8.25F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "createItemPopup";
            this.Padding = new System.Windows.Forms.Padding(2);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "createItemPopup";
            this.Activated += new System.EventHandler(this.createItemPopup_Activated);
            this.Deactivate += new System.EventHandler(this.createItemPopup_Deactivate);
            this.panelMain.ResumeLayout(false);
            this.panelTitle.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Panel panelTitle;
        private System.Windows.Forms.Label labelChooseName;
        private System.Windows.Forms.Panel panelData;
    }
}