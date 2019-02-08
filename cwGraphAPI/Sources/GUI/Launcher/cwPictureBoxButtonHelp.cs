using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Casewise.GraphAPI.GUI;
using Casewise.GraphAPI.PSF;

namespace Casewise.GraphAPI.Launcher
{
    class cwPictureBoxButtonHelp<GuiType, RootNodeType, CreateItemType> : cwPictureBoxButton<GuiType, RootNodeType, CreateItemType>
        where GuiType : cwEditModeGUI
        where RootNodeType : cwPSFTreeNodeConfigurationNode
        where CreateItemType : cwCreateItemForm
    {
        public cwPictureBoxButtonHelp(cwLauncherGUI<GuiType, RootNodeType, CreateItemType> mainGUI)
            : base(mainGUI)
        {
            BackgroundImage = Properties.Resources.image_option_help_32x32;
            tooltip.SetToolTip(this, Properties.Resources.LAUNCHER_OPTIONS_BUTTONS_HELP_TOOLTIP);
            this.Click += new EventHandler(cwPictureBoxHelp_Click);
            Visible = false;
        }

        private void cwPictureBoxHelp_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(mainGUI.options.helpURL);            
        }
    }
}
