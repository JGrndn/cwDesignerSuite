namespace Casewise.webDesigner.GUI
{
    /// <summary>
    /// diagramDesignerCreateItemForm
    /// </summary>
    partial class webDesignerCreateItemForm
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
            this.panelLayoutName = new System.Windows.Forms.Panel();
            this.labelName = new System.Windows.Forms.Label();
            this.panelMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelMain
            // 
            this.panelMain.Controls.Add(this.labelName);
            this.panelMain.Controls.Add(this.panelLayoutName);
            // 
            // panelLayoutName
            // 
            this.panelLayoutName.Location = new System.Drawing.Point(12, 28);
            this.panelLayoutName.Name = "panelLayoutName";
            this.panelLayoutName.Size = new System.Drawing.Size(300, 23);
            this.panelLayoutName.TabIndex = 0;
            // 
            // labelName
            // 
            this.labelName.AutoSize = true;
            this.labelName.Location = new System.Drawing.Point(12, 9);
            this.labelName.Name = "labelName";
            this.labelName.Size = new System.Drawing.Size(35, 16);
            this.labelName.TabIndex = 0;
            this.labelName.Text = "Name";
            // 
            // webDesignerCreateItemForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(324, 180);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "webDesignerCreateItemForm";
            this.Text = "createItemForm";
            this.panelMain.ResumeLayout(false);
            this.panelMain.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelLayoutName;
        private System.Windows.Forms.Label labelName;


    }
}