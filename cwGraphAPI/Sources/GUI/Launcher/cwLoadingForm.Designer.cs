namespace Casewise.GraphAPI.Launcher
{
    partial class cwLoadingForm
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
            this.pictureBoxLoading = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLoading)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBoxLoading
            // 
            this.pictureBoxLoading.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBoxLoading.Image = global::Casewise.GraphAPI.Properties.Resources.image_loading;
            this.pictureBoxLoading.Location = new System.Drawing.Point(0, 0);
            this.pictureBoxLoading.Name = "pictureBoxLoading";
            this.pictureBoxLoading.Size = new System.Drawing.Size(67, 69);
            this.pictureBoxLoading.TabIndex = 1;
            this.pictureBoxLoading.TabStop = false;
            // 
            // cwLoadingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DarkRed;
            this.ClientSize = new System.Drawing.Size(67, 69);
            this.Controls.Add(this.pictureBoxLoading);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "cwLoadingForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "LoadingForm";
            this.TransparencyKey = System.Drawing.Color.DarkRed;
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLoading)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoxLoading;

    }
}