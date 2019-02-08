namespace Casewise.GraphAPI.PSF
{
    /// <summary>
    /// 
    /// </summary>
    public partial class cwPSFPropertyBoxDataGrid 
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dataGridViewKeepProperties = new System.Windows.Forms.DataGridView();
            this.Property = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Operation = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Value = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.colorDialog2 = new System.Windows.Forms.ColorDialog();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewKeepProperties)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridViewKeepProperties
            // 
            this.dataGridViewKeepProperties.BackgroundColor = System.Drawing.Color.White;
            this.dataGridViewKeepProperties.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewKeepProperties.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridViewKeepProperties.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewKeepProperties.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Property,
            this.Operation,
            this.Value});
            this.dataGridViewKeepProperties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewKeepProperties.GridColor = System.Drawing.Color.MediumTurquoise;
            this.dataGridViewKeepProperties.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewKeepProperties.Name = "dataGridViewKeepProperties";
            this.dataGridViewKeepProperties.RowHeadersVisible = false;
            this.dataGridViewKeepProperties.RowHeadersWidth = 21;
            this.dataGridViewKeepProperties.Size = new System.Drawing.Size(220, 137);
            this.dataGridViewKeepProperties.TabIndex = 0;
            this.dataGridViewKeepProperties.CellMouseEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewKeepProperties_CellMouseEnter);
            this.dataGridViewKeepProperties.CellMouseLeave += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewKeepProperties_CellMouseLeave);
            this.dataGridViewKeepProperties.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewKeepProperties_CellValueChanged);
            this.dataGridViewKeepProperties.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dataGridViewKeepProperties_DataError);
            this.dataGridViewKeepProperties.MouseClick += new System.Windows.Forms.MouseEventHandler(this.dataGridViewKeepProperties_MouseClick);
            // 
            // Property
            // 
            this.Property.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Property.HeaderText = "Property";
            this.Property.Name = "Property";
            this.Property.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Property.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // Operation
            // 
            this.Operation.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.Operation.HeaderText = "Operation";
            this.Operation.Items.AddRange(new object[] {
            "=",
            "≠",
            "<",
            ">"
            });
            this.Operation.Name = "Operation";
            this.Operation.Width = 59;
            // 
            // Value
            // 
            this.Value.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Value.HeaderText = "Value";
            this.Value.Name = "Value";
            // 
            // cwPSFPropertyBoxDataGrid
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dataGridViewKeepProperties);
            this.Name = "cwPSFPropertyBoxDataGrid";
            this.Size = new System.Drawing.Size(220, 137);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewKeepProperties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.ColorDialog colorDialog2;
        /// <summary>
        /// 
        /// </summary>
        public System.Windows.Forms.DataGridView dataGridViewKeepProperties;
        private System.Windows.Forms.DataGridViewComboBoxColumn Property;
        private System.Windows.Forms.DataGridViewComboBoxColumn Operation;
        private System.Windows.Forms.DataGridViewTextBoxColumn Value;
    }
}
