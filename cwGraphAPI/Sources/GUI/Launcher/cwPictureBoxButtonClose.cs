using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Casewise.GraphAPI.GUI;
using Casewise.GraphAPI.PSF;

namespace Casewise.GraphAPI.Launcher
{
    class cwPictureBoxButtonClose<GuiType, RootNodeType, CreateItemType> : cwPictureBoxButton<GuiType, RootNodeType, CreateItemType>
        where GuiType : cwEditModeGUI
        where RootNodeType : cwPSFTreeNodeConfigurationNode
        where CreateItemType : cwCreateItemForm
    {
        public cwPictureBoxButtonClose(cwLauncherGUI<GuiType, RootNodeType, CreateItemType> mainGUI)
            : base(mainGUI)
        {
            BackgroundImage = Properties.Resources.image_option_close_32x32;
            tooltip.SetToolTip(this, Properties.Resources.LAUNCHER_OPTIONS_BUTTONS_CLOSE_TOOLTIP);
            this.Click += new EventHandler(cwPictureBoxClose_Click);
            Visible = false;
        }

        private void cwPictureBoxClose_Click(object sender, EventArgs e)
        {
            if (mainGUI.connection != null)
            {
                mainGUI.connection.Close();
            }
            Application.Exit();
        }
    }
}
