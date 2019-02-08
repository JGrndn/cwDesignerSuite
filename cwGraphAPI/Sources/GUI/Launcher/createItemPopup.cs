using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Casewise.GraphAPI.API;
using Casewise.GraphAPI.Exceptions;
using Casewise.GraphAPI.PSF;

namespace Casewise.GraphAPI.Launcher
{
    /// <summary>
    /// createItemPopup
    /// </summary>
    public partial class createItemPopup : Form
    {
        cwCreateItemForm createItemForm = null;


        /// <summary>
        /// Initializes a new instance of the <see cref="createItemPopup"/> class.
        /// </summary>
        /// <param name="createItemForm">The create item form.</param>
        public createItemPopup(cwCreateItemForm createItemForm)
        {
            this.createItemForm = createItemForm;
            InitializeComponent();
            panelData.Controls.Add(this.createItemForm.panelMain);
            this.Icon = Properties.Resources.Casewise_Icon;
            buttonOK.Text = Properties.Resources.FORM_ADDITEM_BUTTON_CREATE;
            buttonCancel.Text = Properties.Resources.FORM_ADDITEM_BUTTON_CANCEL;
            labelChooseName.Text = Properties.Resources.FORM_ADDITEM_LABEL_SET_ITEM_NAME;
            Shown += new EventHandler(createItemPopup_Shown);
            ShowDialog();
        }

        /// <summary>
        /// Handles the Shown event of the createItemPopup control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        public void createItemPopup_Shown(object sender, EventArgs e)
        {
            this.createItemForm.checkForm();
        }

        /// <summary>
        /// Gets a value indicating whether this instance has been canceled.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has been canceled; otherwise, <c>false</c>.
        /// </value>
        public bool hasBeenCanceled
        {
            get
            {
                return createItemForm.hasBeenCancel;
            }
        }

        /// <summary>
        /// Gets the properties.
        /// </summary>
        public Dictionary<string, string> properties
        {
            get {
                return createItemForm.properties;
            }
        }

        /// <summary>
        /// Handles the Click event of the bOK control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void bOK_Click(object sender, EventArgs e)
        {
            createItemForm.onCreate(this);
        }

        /// <summary>
        /// Handles the Click event of the buttonCancel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            createItemForm.onCancel(this);
        }

        private void createItemPopup_Activated(object sender, EventArgs e)
        {
            this.createItemForm.checkForm();
        }

        private void createItemPopup_Deactivate(object sender, EventArgs e)
        {
            createItemForm.releaseAllTooltips();
        }

    }
}
