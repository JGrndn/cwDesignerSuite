using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Casewise.GraphAPI.GUI;
using Casewise.GraphAPI.PSF;

namespace Casewise.GraphAPI.Launcher
{
    class cwPictureBoxButtonBack<GuiType, RootNodeType, CreateItemType> : cwPictureBoxButton<GuiType, RootNodeType, CreateItemType>
        where GuiType : cwEditModeGUI
        where RootNodeType : cwPSFTreeNodeConfigurationNode
        where CreateItemType : cwCreateItemForm
    {
        public cwPictureBoxButtonBack(cwLauncherGUI<GuiType, RootNodeType, CreateItemType> mainGUI)
            : base(mainGUI)
        {
            BackgroundImage = Properties.Resources.image_option_prev_32x32;
            tooltip.SetToolTip(this, Properties.Resources.LAUNCHER_OPTIONS_BUTTONS_BACK_TOOLTIP);
            this.Click += new EventHandler(cwPictureBoxButtonBack_Click);
            Visible = false;
        }

        private void cwPictureBoxButtonBack_Click(object sender, EventArgs e)
        {
            mainGUI.loadFromConnection();
        }

    }
}
