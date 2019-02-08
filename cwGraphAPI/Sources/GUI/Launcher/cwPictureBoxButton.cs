using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Casewise.GraphAPI.GUI;
using Casewise.GraphAPI.PSF;

namespace Casewise.GraphAPI.Launcher
{



    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="GuiType">The type of the UI type.</typeparam>
    /// <typeparam name="RootNodeType">The type of the oot node type.</typeparam>
    /// <typeparam name="CreateItemType">The type of the reate item type.</typeparam>
    public class cwPictureBoxButton<GuiType, RootNodeType, CreateItemType> : PictureBox
        where GuiType : cwEditModeGUI
        where RootNodeType : cwPSFTreeNodeConfigurationNode
        where CreateItemType : cwCreateItemForm
    {
        /// <summary>
        /// tooltip
        /// </summary>
        protected ToolTip tooltip = new ToolTip();
        /// <summary>
        /// mainGUI
        /// </summary>
        protected cwLauncherGUI<GuiType, RootNodeType, CreateItemType> mainGUI = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="cwPictureBoxButton&lt;GuiType, RootNodeType, CreateItemType&gt;"/> class.
        /// </summary>
        /// <param name="mainGUI">The main GUI.</param>
        public cwPictureBoxButton(cwLauncherGUI<GuiType, RootNodeType, CreateItemType> mainGUI)
        {
            this.mainGUI = mainGUI;
            this.MouseEnter += new System.EventHandler(this.cwPictureBoxButton_MouseEnter);
            this.MouseLeave += new System.EventHandler(this.cwPictureBoxButton_MouseLeave);
            Size = new System.Drawing.Size(22, 22);
            BackgroundImageLayout = ImageLayout.Stretch;
        }

        /// <summary>
        /// Handles the MouseEnter event of the cwPictureBoxButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void cwPictureBoxButton_MouseEnter(object sender, EventArgs e)
        {
            Cursor = Cursors.Hand;
        }

        /// <summary>
        /// Forces the control to invalidate its client area and immediately redraw itself and any child controls.
        /// </summary>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/>
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   </PermissionSet>
        public override void Refresh()
        {
            tooltip.RemoveAll();
            base.Refresh();
        }

        /// <summary>
        /// Handles the MouseLeave event of the cwPictureBoxButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void cwPictureBoxButton_MouseLeave(object sender, EventArgs e)
        {
            Cursor = Cursors.Default;
        }

    }
}
