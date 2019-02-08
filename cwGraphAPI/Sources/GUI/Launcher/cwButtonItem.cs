using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Casewise.GraphAPI.API;
using System.Drawing;
using Casewise.GraphAPI.GUI;
using System.Reflection;
using Casewise.GraphAPI.PSF;

namespace Casewise.GraphAPI.Launcher
{
    class cwButtonItem<GuiType, RootNodeType, CreateItemType> : Panel
        where GuiType : cwEditModeGUI
        where RootNodeType : cwPSFTreeNodeConfigurationNode
        where CreateItemType : cwCreateItemForm
    {
        cwLauncherGUI<GuiType, RootNodeType, CreateItemType> mainGUI = null;
        private cwLightObject _sourceObject = null;
        public PictureBox pb = new PictureBox();

        public cwButtonItem(cwLauncherGUI<GuiType, RootNodeType, CreateItemType> mainGUI, cwLightObject sourceObject) 
        {
            this.mainGUI = mainGUI;
            _sourceObject = sourceObject;
            this.Font = new System.Drawing.Font("Trebuchet MS", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.Name = "button" + sourceObject.ID.ToString();
            this.Size = new System.Drawing.Size(136, 116);

            this.MouseEnter += new System.EventHandler(this.cwButtonItem_MouseEnter);
            this.MouseLeave += new System.EventHandler(this.cwButtonItem_MouseLeave);
            this.Click += new System.EventHandler(this.cwButtonItem_Click);

            Label l = new Label();
            l.Text = sourceObject.ToString();
            l.Font = new System.Drawing.Font("Trebuchet MS", 7.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            l.Size = new System.Drawing.Size(136, 40);
            l.Location = new System.Drawing.Point(0, 76);
            l.Dock = DockStyle.Bottom;
            l.ForeColor = Color.Black;
            l.TextAlign = ContentAlignment.TopCenter;
            l.MouseEnter += new System.EventHandler(this.cwButtonItem_MouseEnter);
            l.MouseLeave += new System.EventHandler(this.cwButtonItem_MouseLeave);
            l.Click += new System.EventHandler(this.cwButtonItem_Click);

            pb.Image = mainGUI.options.itemIcon;
            pb.SizeMode = PictureBoxSizeMode.StretchImage;
            pb.Size = new System.Drawing.Size(96, 70);
            pb.Location = new System.Drawing.Point(20, 0);  
            //pb.Dock = DockStyle.Top;
            pb.MouseEnter += new System.EventHandler(this.cwButtonItem_MouseEnter);
            pb.MouseLeave += new System.EventHandler(this.cwButtonItem_MouseLeave);
            pb.Click += new System.EventHandler(this.cwButtonItem_Click);

            Controls.Add(pb);
            Controls.Add(l);
        }


        private void cwButtonItem_MouseLeave(object sender, EventArgs e)
        {
            Cursor = Cursors.Default;
            pb.Image = mainGUI.options.itemIcon;
        }

        private void cwButtonItem_MouseEnter(object sender, EventArgs e)
        {
            Cursor = Cursors.Hand;
            pb.Image = mainGUI.options.itemIconAnimated;
        }


        private void cwButtonItem_Click(object sender, EventArgs e)
        {
            try
            {
                mainGUI.loadItemIntoEditGUI(_sourceObject);
            }
            catch (Exception exception)
            {
                cwTools.log.Error(exception.ToString());
            }
        }
    }
}
