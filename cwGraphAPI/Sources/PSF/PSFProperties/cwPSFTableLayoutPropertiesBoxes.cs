using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Casewise.GraphAPI.PSF
{
    /// <summary>
    /// 
    /// </summary>
    public class cwPSFTableLayoutPropertiesBoxes : TableLayoutPanel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="cwPSFTableLayoutPropertiesBoxes"/> class.
        /// </summary>
        public cwPSFTableLayoutPropertiesBoxes()
        {
            AutoScroll = true;
            AutoSize = true;
            DoubleBuffered = true;
            ColumnCount = 2;
            RowCount = 1;
            ColumnStyle cS_Name = new ColumnStyle(SizeType.Percent, 30);
            ColumnStyle cS_Content = new ColumnStyle(SizeType.Percent, 70);
            ColumnStyles.Add(cS_Name);
            ColumnStyles.Add(cS_Content);
            BackColor = Color.White;
            CellBorderStyle = TableLayoutPanelCellBorderStyle.Inset;
            Dock = DockStyle.Fill;
        }

        /// <summary>
        /// Adds the details table row.
        /// </summary>
        /// <param name="_properyBox">The _propery box.</param>
        public void addPropertyBox(cwPSFPropertyBox _properyBox)
        {
            int rowNumber = RowCount - 1;
            Label title_label = new Label();
            title_label.Text = _properyBox.helpName;
            title_label.Height = 30;

            ToolTip HelpToolTip = new ToolTip();
            HelpToolTip.IsBalloon = true;
            HelpToolTip.ToolTipIcon = ToolTipIcon.Info;
            HelpToolTip.ToolTipTitle = _properyBox.helpName;
            HelpToolTip.SetToolTip(title_label, _properyBox.helpDescription);

            Control _propertyControl = _properyBox.getControl();

            Controls.Add(title_label, 0, rowNumber);
            Controls.Add(_propertyControl, 1, rowNumber);

            GetControlFromPosition(0, rowNumber).Padding = new Padding(0, 0, 0, 3);
            GetControlFromPosition(1, rowNumber).Padding = new Padding(0, 0, 0, 3);
            RowCount++;


            _propertyControl.Dock = DockStyle.Fill;
            title_label.Dock = DockStyle.Left;
            title_label.Anchor = AnchorStyles.Top;
            title_label.AutoSize = true;

            //tableLayoutPanelDetails.Dock = DockStyle.Fill;
        }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        public void Reset()
        {            
            Controls.Clear();
            RowCount = 1;            
        }
    }
}
