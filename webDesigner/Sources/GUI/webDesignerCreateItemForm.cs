using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Casewise.GraphAPI.Launcher;
using Casewise.GraphAPI.API;
using Casewise.GraphAPI.Exceptions;
using Casewise.GraphAPI.PSF;
using Casewise.webDesigner.Nodes;
using Casewise.GraphAPI.ProgramManager;

namespace Casewise.webDesigner.GUI
{
    public partial class webDesignerCreateItemForm : cwCreateItemForm
    {

        private cwPSFPropertyBoxString textName = null;
        /// <summary>
        /// Initializes a new instance of the <see cref="diagramDesignerCreateItemForm"/> class.
        /// </summary>
        /// <param name="itemObjectType">Type of the item object.</param>
        public webDesignerCreateItemForm(cwLightObjectType itemObjectType)
            : base(itemObjectType)
        {
            InitializeComponent();

            cwPSFToolTip ttName = new cwPSFToolTip(this.Font);

            ttName.ToolTipTitle = Properties.Resources.DESIGN_SITE_NODE_NAME;
            labelName.Text = Properties.Resources.DESIGN_SITE_NODE_NAME;
            ttName.SetToolTip(labelName, Properties.Resources.DESIGN_SITE_NAME_HELP);


            textName = new cwPSFPropertyBoxItemName(Properties.Resources.DESIGN_SITE_NAME_NAME, Properties.Resources.DESIGN_SITE_NAME_HELP, CONF_TEXTNAME, itemObjectType);
            textName.setValueNotEmpty();
            textName.Dock = DockStyle.Fill;
            panelLayoutName.Controls.Add(textName);

            textName.TextChanged += new EventHandler(contentHasChanged);
            textName.GotFocus += new EventHandler(contentHasChanged);
            textName.MouseMove += new MouseEventHandler(releaseToolTips);     
        }

        private const string CONF_TEXTNAME = "text-name";


        /// <summary>
        /// Checks the form.
        /// </summary>
        /// <returns></returns>
        public override bool checkForm()
        {
            releaseAllTooltips();

            if (!textName.checkFormat())
            {
                setToolTipFromPSFPropertyBoxError(textName);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Items the can be created.
        /// </summary>
        /// <returns></returns>
        public bool itemCanBeCreated()
        {
            try
            {
                return checkForm();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Ons the create.
        /// </summary>
        /// <param name="parentForm">The parent form.</param>
        public override void onCreate(Form parentForm)
        {
            if (itemCanBeCreated())
            {
                properties["NAME"] = textName.Text;
                parentForm.Close();
            }
        }

        /// <summary>
        /// Creates the item.
        /// </summary>
        /// <returns></returns>
        public override cwLightObject createItem()
        {
            cwWebDesignerTreeNodeSite site = new cwWebDesignerTreeNodeSite(new cwWebDesignerGUI(itemObjectType.Model, new cwProgramManagerOptions()));
            properties["DESCRIPTION"] = site.toXML();
            int itemID = itemObjectType.createObject(properties);
            cwLightObject newItem = itemObjectType.getObjectByID(itemID);
            return newItem;
        }

        private void releaseToolTips(object sender, MouseEventArgs e)
        {
            releaseAllTooltips();
        }
        private void checkCreationFormStructure(object sender, EventArgs e)
        {
            checkForm();
        }
        private void contentHasChanged(object sender, EventArgs e)
        {
            releaseAllTooltips();
        }


    }
}
