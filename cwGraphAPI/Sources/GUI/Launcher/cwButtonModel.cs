using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Casewise.GraphAPI.API;
using System.Drawing;
using Casewise.GraphAPI.GUI;
using Casewise.GraphAPI.PSF;

namespace Casewise.GraphAPI.Launcher
{
    class cwButtonModel<GuiType, RootNodeType, CreateItemType> : Panel
        where GuiType : cwEditModeGUI
        where RootNodeType : cwPSFTreeNodeConfigurationNode
        where CreateItemType : cwCreateItemForm
    {
        private cwLightModel _model = null;
        PictureBox pb = new PictureBox();
        cwLauncherGUI<GuiType, RootNodeType, CreateItemType> mainGUI = null;
        public cwButtonModel(cwLauncherGUI<GuiType, RootNodeType, CreateItemType> mainGUI, cwLightModel model)
        {
            this.mainGUI = mainGUI;
            _model = model;
            this.Font = new System.Drawing.Font("Trebuchet MS", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.Name = "buttonModel" + model.FileName;
            this.Size = new System.Drawing.Size(136, 116);

            Label l = new Label();
            l.Text = model.ToString();           
            l.Size = new System.Drawing.Size(136, 40);
            l.Location = new Point(0, 76);
            l.Font = new System.Drawing.Font("Trebuchet MS", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            l.Dock = DockStyle.Bottom;
            l.ForeColor = Color.Black;
            l.AutoSize = false;
            l.TextAlign = ContentAlignment.TopCenter;
            l.MouseEnter += new System.EventHandler(this.cwButtonModel_MouseEnter);
            l.MouseLeave += new System.EventHandler(this.cwButtonModel_MouseLeave);
            l.Click += new System.EventHandler(this.cwButtonModel_Click);

            pb.Image = global::Casewise.GraphAPI.Properties.Resources.image_model_small;
            pb.SizeMode = PictureBoxSizeMode.StretchImage;
            //pb.BackgroundImageLayout = ImageLayout.Stretch;
            pb.Size = new System.Drawing.Size(96, 70);
            pb.Location = new System.Drawing.Point(20, 0);
            //pb.Dock = DockStyle.Top;
            pb.MouseEnter += new System.EventHandler(this.cwButtonModel_MouseEnter);
            pb.MouseLeave += new System.EventHandler(this.cwButtonModel_MouseLeave);
            pb.Click += new System.EventHandler(this.cwButtonModel_Click);

            Controls.Add(pb);
            Controls.Add(l);
        }


        public void loadItems(Bitmap itemIcon)
           
        {
            cwLightObjectType itemOT = _model.getObjectTypeByScriptName(mainGUI.options.applicationObjectTypeScriptName);
            cwLightNodeObjectType itemOTNode = new cwLightNodeObjectType(itemOT);
            itemOTNode.addPropertyToSelect("NAME");
            itemOTNode.preloadLightObjects();

            mainGUI.clearFlowLayoutPanel();
            if (itemOTNode.usedOTLightObjects.Count > 0)
            {
                foreach (cwLightObject item in itemOTNode.usedOTLightObjects)
                {
                    cwButtonItem<GuiType, RootNodeType, CreateItemType> bi = new cwButtonItem<GuiType, RootNodeType, CreateItemType>(mainGUI, item);

                    mainGUI.flowLayoutPanelItems.Controls.Add(bi);
                }
            }
            else
            {
                // no items to load
            }
            mainGUI.imageLoader.disposeLoadingForm();
            
        }

        public cwLauncherGUI<GuiType, RootNodeType, CreateItemType> parentGUI
        {
            get
            {
                return Parent.Parent.Parent.Parent.Parent as cwLauncherGUI<GuiType, RootNodeType, CreateItemType>;
            }
        }

        private void cwButtonModel_MouseLeave(object sender, EventArgs e)
        {
            Cursor = Cursors.Default;
            pb.Image = global::Casewise.GraphAPI.Properties.Resources.image_model_small;
        }

        private void cwButtonModel_MouseEnter(object sender, EventArgs e)
        {
            Cursor = Cursors.Hand;
            pb.Image = global::Casewise.GraphAPI.Properties.Resources.image_model_hover;
        }

        private void cwButtonModel_Click(object sender, EventArgs e)
        {
            cwLauncherGUI<GuiType, RootNodeType, CreateItemType> mainGUI = parentGUI;

            mainGUI.clearFlowLayoutPanel();
            mainGUI.imageLoader.startLoadingImage();

            if (_model.hasObjectTypeByScriptName(mainGUI.options.applicationObjectTypeScriptName))
            {
                _model.loadLightModelContent();
                mainGUI.loadFromModel(_model);
            }
        }
    }
}
