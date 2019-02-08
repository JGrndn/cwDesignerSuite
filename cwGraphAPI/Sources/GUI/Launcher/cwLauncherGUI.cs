using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Casewise.GraphAPI.API;
using Casewise.GraphAPI.GUI;
using System.Runtime.InteropServices;
using System.Threading;
using System.Diagnostics;
using Casewise.GraphAPI.PSF;
using Casewise.GraphAPI.ProgramManager;
using System.Globalization;
using log4net;


namespace Casewise.GraphAPI.Launcher
{
    public partial class cwLauncherGUI<GuiType, RootNodeType, CreateItemType> : Form
        where GuiType : cwEditModeGUI
        where RootNodeType : cwPSFTreeNodeConfigurationNode
        where CreateItemType : cwCreateItemForm
    {

        /// <summary>
        /// hasBeenShown
        /// </summary>
        public bool hasBeenShown = false;

        /// <summary>
        /// helpTooltip
        /// </summary>
        public cwPSFToolTip helpTooltip = null;


        internal cwPictureBoxButtonClose<GuiType, RootNodeType, CreateItemType> optionClose = null;
        internal cwPictureBoxButtonOptions<GuiType, RootNodeType, CreateItemType> optionOptions = null;
        internal cwPictureBoxButtonBack<GuiType, RootNodeType, CreateItemType> optionBack = null;
        internal cwPictureBoxButtonHelp<GuiType, RootNodeType, CreateItemType> optionHelp = null;
        /// <summary>
        /// connection
        /// </summary>
        public cwConnection connection = null;
        /// <summary>
        /// choosedModel
        /// </summary>
        public cwLightModel choosedModel = null;
        /// <summary>
        /// notWebDesignerModels
        /// </summary>
        public List<cwLightModel> notEnabledModels = new List<cwLightModel>();
        /// <summary>
        /// imageLoader
        /// </summary>
        public cwLauncherLoadingImage<GuiType, RootNodeType, CreateItemType> imageLoader = null;
        /// <summary>
        /// pictureBoxLogo
        /// </summary>
        public cwPictureBoxLogo pictureBoxLogo = null;

        /// <summary>
        /// options
        /// </summary>
        public cwProgramManagerOptions options = null;
        /// <summary>
        /// Initializes a new instance of the <see cref="cwLauncherGUI&lt;GuiType, RootNodeType, CreateItemType&gt;"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        public cwLauncherGUI(cwProgramManagerOptions options)
        {
            InitializeComponent();

            this.Icon = Properties.Resources.Casewise_Icon;
            this.options = options;
            imageLoader = new cwLauncherLoadingImage<GuiType, RootNodeType, CreateItemType>(this);
            optionClose = new cwPictureBoxButtonClose<GuiType, RootNodeType, CreateItemType>(this);
            optionOptions = new cwPictureBoxButtonOptions<GuiType, RootNodeType, CreateItemType>(this);
            optionBack = new cwPictureBoxButtonBack<GuiType, RootNodeType, CreateItemType>(this);
            optionHelp = new cwPictureBoxButtonHelp<GuiType, RootNodeType, CreateItemType>(this);
            DoubleBuffered = true;
            optionOptions.Visible = false;
            optionBack.Visible = false;

            flowLayoutPanelOptions.Controls.Add(optionClose);
            flowLayoutPanelOptions.Controls.Add(optionHelp);
            flowLayoutPanelOptions.Controls.Add(optionOptions);
            flowLayoutPanelOptions.Controls.Add(optionBack);
            flowLayoutPanelOptions.AutoSize = true;
            flowLayoutPanelItems.Refresh();

            pictureBoxLogo = new cwPictureBoxLogo(this, this.splitContainerMain);
            this.splitContainerMain.Panel1.Controls.Add(this.pictureBoxLogo);
            pictureBoxLogo.Image = Properties.Resources.image_logo_graphapi;
            pictureBoxLogo.WaitOnLoad = true;

            helpTooltip = new cwPSFToolTip(this.Font);
        }

        /// <summary>
        /// Checks the add item tooltip.
        /// </summary>
        public void checkAddItemTooltip()
        {
            if (helpTooltip != null)
            {
                helpTooltip.remove();
            }
            if (0.Equals(flowLayoutPanelItems.Controls.Count))
            {
                helpTooltip = new cwPSFToolTip(this.Font);
                if (choosedModel != null)
                {
                    helpTooltip.showTooltip(options.addItemTooltipMessage, optionOptions);
                }
                else
                {
                    helpTooltip.showTooltip(Properties.Resources.LAUNCHER_OPTIONS_TOOLTIP_ENABLE_MODEL, optionOptions);
                }                
            }
            else
            {
                helpTooltip.remove();
            }
        }


        /// <summary>
        /// Clears the flow layout panel.
        /// </summary>
        public void clearFlowLayoutPanel()
        {
            flowLayoutPanelItems.SuspendLayout();
            flowLayoutPanelItems.Controls.Clear();
            flowLayoutPanelItems.VerticalScroll.Value = 0;
            flowLayoutPanelItems.HorizontalScroll.Value = 0;
            flowLayoutPanelItems.Refresh();
            flowLayoutPanelItems.ResumeLayout();
        }

        /// <summary>
        /// Loads from connection.
        /// </summary>
        public void loadFromConnection()
        {
            helpTooltip.RemoveAll();
            clearFlowLayoutPanel();
            optionBack.Visible = false;
            imageLoader.startLoadingImage();
            choosedModel = null;
            connection.loadModels();
            List<cwLightModel> enabledModels = new List<cwLightModel>();
            notEnabledModels.Clear();

            connection.getModels().ForEach(m =>
            {
                m.loadLightObjectTypes();
                if (!m.hasObjectTypeByScriptName(options.applicationObjectTypeScriptName))
                {
                    notEnabledModels.Add(m);
                }
                else
                {
                    enabledModels.Add(m);
                }
            });

            if (notEnabledModels.Count > 0)
            {
                optionOptions.Visible = true;
            }

            if (0.Equals(enabledModels.Count))
            {
            }
            else
            {
                enabledModels.Sort();
                for (int i = 0; i < enabledModels.Count; ++i)
                {
                    cwButtonModel<GuiType, RootNodeType, CreateItemType> modelButton = new cwButtonModel<GuiType, RootNodeType, CreateItemType>(this, enabledModels[i]);
                    flowLayoutPanelItems.Controls.Add(modelButton);
                }
            }
            imageLoader.disposeLoadingForm();
            optionOptions.setUpToolTip();
        }

        /// <summary>
        /// Loads the item into edit GUI.
        /// </summary>
        /// <param name="sourceObject">The source object.</param>
        public void loadItemIntoEditGUI(cwLightObject sourceObject)
        {
            flowLayoutPanelItems.Visible = false;
            imageLoader.startLoadingImage();
            GuiType editGUI = Activator.CreateInstance(typeof(GuiType), new object[] { sourceObject.Model, options }) as GuiType;
            editGUI.loadTreeView<RootNodeType>(sourceObject.Model, options.applicationObjectTypeScriptName, sourceObject.ID);
            imageLoader.disposeLoadingForm();
            editGUI.ShowDialog();
            flowLayoutPanelItems.Visible = true;
            loadFromModel(sourceObject.Model);
        }

        /// <summary>
        /// Loads from model.
        /// </summary>
        /// <param name="model">The model.</param>
        public void loadFromModel(cwLightModel model)
        {
            cwButtonModel<GuiType, RootNodeType, CreateItemType> bm = new cwButtonModel<GuiType, RootNodeType, CreateItemType>(this, model);
            bm.loadItems(options.itemIcon);
            choosedModel = model;
            optionBack.Visible = true;
            optionOptions.Visible = true;
            checkAddItemTooltip();
            optionOptions.setUpToolTip();
        }

        private void cwLauncherGUI_Shown(object sender, EventArgs e)
        {

            hasBeenShown = true;
            if (connection != null && choosedModel == null)
            {
                imageLoader.startLoadingImage();
                loadFromConnection();
            }
        }

        #region resize
        private Point initialResizePoint = new Point();
        private bool resize = true;

        private const int formMinimumSizeWidth = 608;
        private const int formMinimumSizeHeight = 368;

        private void resizeForm(MouseEventArgs e)
        {
            if (0.Equals(initialResizePoint.X) && 0.Equals(initialResizePoint.Y))
            {
                return;
            }

            SuspendLayout();
            helpTooltip.RemoveAll();
            flowLayoutPanelItems.SuspendLayout();
            flowLayoutPanelOptions.SuspendLayout();
            Point currentPoint = new Point(e.X, e.Y);
            currentPoint.Offset(initialResizePoint);

            int w = Size.Width + currentPoint.X;
            int h = Size.Height + currentPoint.Y;
            if (w < formMinimumSizeWidth)
            {
                w = formMinimumSizeWidth;
            }
            if (h < formMinimumSizeHeight)
            {
                h = formMinimumSizeHeight;
            }
            this.Size = new Size(w, h);
            setInitialResizePoint(e);

            pictureBoxLogo.Location = new Point(Size.Width / 2 - pictureBoxLogo.Width / 2, pictureBoxLogo.Location.Y);
            checkAddItemTooltip();
            optionOptions.Refresh();
            optionOptions.setUpToolTip();
            optionBack.Refresh();
            optionClose.Refresh();
            flowLayoutPanelOptions.Refresh();
            flowLayoutPanelOptions.ResumeLayout();
            flowLayoutPanelItems.Refresh();
            flowLayoutPanelItems.ResumeLayout();
            Refresh();
            ResumeLayout();
        }

        private void cwLauncherGUI_MouseMove(object sender, MouseEventArgs e)
        {
            if (resize && e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                resizeForm(e);
            }
        }

        private void setInitialResizePoint(MouseEventArgs e)
        {
            initialResizePoint = new Point(-e.X, -e.Y);
            resize = true;

        }

        private void cwLauncherGUI_MouseUp(object sender, MouseEventArgs e)
        {
            Cursor.Current = Cursors.Default;
            resize = false;
        }

        private void cwLauncherGUI_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                bool left = e.X < Padding.Left;
                bool right = e.X > Size.Width - Padding.Right;
                bool top = e.Y < Padding.Top;
                bool bottom = e.Y > Size.Height - Padding.Bottom;

                bool topLeft = top && left;
                bool bottomRight = bottom && right;
                bool topRight = top && right;
                bool bottomLeft = bottom && left;

                if (bottom || right)
                {
                    Cursor.Current = Cursors.SizeAll;
                    setInitialResizePoint(e);
                }

            }
        }

        #endregion resize

        private void cwLauncherGUI_Move(object sender, EventArgs e)
        {
            checkAddItemTooltip();
        }

        private void cwLauncherGUI_Resize(object sender, EventArgs e)
        {
            checkAddItemTooltip();
        }

        private void cwLauncherGUI_Deactivate(object sender, EventArgs e)
        {
            helpTooltip.remove();
        }

        private void cwLauncherGUI_Activated(object sender, EventArgs e)
        {
            checkAddItemTooltip();
        }
    }
}
