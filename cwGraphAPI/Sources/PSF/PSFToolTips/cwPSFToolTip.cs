using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using log4net;

namespace Casewise.GraphAPI.PSF
{
    /// <summary>
    /// cwPSFToolTip
    /// </summary>
    public class cwPSFToolTip : ToolTip
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(cwPSFToolTip));
        Font font = null;
        /// <summary>
        /// Initializes a new instance of the <see cref="cwPSFToolTip"/> class.
        /// </summary>
        /// <param name="font">The font.</param>
        public cwPSFToolTip(Font font)
        {
            this.font = font;
            initTooltip();
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="cwPSFToolTip"/> class.
        /// </summary>
        /// <param name="Cont">The cont.</param>
        public cwPSFToolTip(System.ComponentModel.IContainer Cont)
        {
            this.OwnerDraw = true;
            this.IsBalloon = true;
            this.Draw += new DrawToolTipEventHandler(OnDraw);
        }


        /// <summary>
        /// Inits the tooltip.
        /// </summary>
        private void initTooltip()
        {
            this.OwnerDraw = true;
            this.IsBalloon = true;
            this.ToolTipTitle = Properties.Resources.PSF_VALIDATION_RULE_TOOLTIP_TITLE_VALIDATION_ERROR;
            this.ToolTipIcon = ToolTipIcon.Info;
            this.UseAnimation = false;
            this.ForeColor = Color.White;
            this.InitialDelay = 250;
            this.ShowAlways = true;
            this.BackColor = Color.Red;
            this.Draw += new DrawToolTipEventHandler(OnDraw);
        }


        /// <summary>
        /// Shows the tooltip.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="control">The control.</param>
        public void showTooltip(string text, Control control)
        {
            showTooltip(text, new Point(0, 0), control.Size, control);
        }

        /// <summary>
        /// Hides the tooltip.
        /// </summary>
        /// <param name="control">The control.</param>
        public void hideTooltip(Control control)
        {
            try
            {
                if (control == null) return;
                Hide(control);
                remove();
                this.Dispose();
            }
            catch (Exception e)
            {
                log.Error(e);
            }
        }

        /// <summary>
        /// Removes this instance.
        /// </summary>
        public void remove()
        {
            RemoveAll();
        }

        /// <summary>
        /// Shows the tooltip.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="initialOffset">The initial offset.</param>
        /// <param name="zoneSize">Size of the zone.</param>
        /// <param name="control">The control.</param>
        public void showTooltip(string text, Point initialOffset, Size zoneSize, Control control)
        {
            Point location = initialOffset;
            //location.Offset(control.Bounds.Location);
            location.Offset(zoneSize.Width / 2, zoneSize.Height);
            SetToolTip(control, text);
            control.Tag = text;
            Show(string.Empty, control, location);
            Show(text, control, location);
        }

        private void OnDraw(object sender, DrawToolTipEventArgs e)
        {
            DrawToolTipEventArgs newArgs = new DrawToolTipEventArgs(e.Graphics, e.AssociatedWindow, e.AssociatedControl, e.Bounds, e.ToolTipText, this.BackColor, this.ForeColor, this.font);
            newArgs.DrawBackground();
            newArgs.DrawBorder();
            newArgs.DrawText(TextFormatFlags.TextBoxControl);
        }
    }
}
