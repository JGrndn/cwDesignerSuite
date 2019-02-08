using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Casewise.GraphAPI.GUI;
using Casewise.GraphAPI.PSF;

namespace Casewise.GraphAPI.Launcher
{
    /// <summary>
    /// cwPictureBoxLogo
    /// </summary>
    public class cwPictureBoxLogo : PictureBox
    {
        /// <summary>
        /// WM_NCLBUTTONDOWN
        /// </summary>
        public const int WM_NCLBUTTONDOWN = 0xA1;
        /// <summary>
        /// HT_CAPTION
        /// </summary>
        public const int HT_CAPTION = 0x2;
        Form startGUI = null;



        /// <summary>
        /// Initializes a new instance of the <see cref="cwPictureBoxLogo"/> class.
        /// </summary>
        /// <param name="startGUI">The start GUI.</param>
        /// <param name="mainSplitContainer">The main split container.</param>
        public cwPictureBoxLogo(Form startGUI, SplitContainer mainSplitContainer)
        {
            BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            Location = new System.Drawing.Point(198, 3);
            Name = "pictureBoxLogo";
            Size = new System.Drawing.Size(338, 100);
            TabIndex = 5;
            TabStop = false;
            Visible = false;
            //WaitOnLoad = true;

            this.startGUI = startGUI;
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBoxLogo_MouseDown);
            mainSplitContainer.Panel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBoxLogo_MouseDown);

            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBoxLogo_MouseUp);
            mainSplitContainer.Panel1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBoxLogo_MouseUp);

        }

        /// <summary>
        /// Sends the message.
        /// </summary>
        /// <param name="hWnd">The h WND.</param>
        /// <param name="Msg">The MSG.</param>
        /// <param name="wParam">The w param.</param>
        /// <param name="lParam">The l param.</param>
        /// <returns></returns>
        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        /// <summary>
        /// Releases the capture.
        /// </summary>
        /// <returns></returns>
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();
        /// <summary>
        /// Handles the MouseDown event of the pictureBoxLogo control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        private void pictureBoxLogo_MouseDown(object sender, MouseEventArgs e)
        {
            Cursor.Current = Cursors.NoMove2D;
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(startGUI.Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void pictureBoxLogo_MouseUp(object sender, MouseEventArgs e)
        {
            Cursor.Current = Cursors.Default;
        }

    }
}
