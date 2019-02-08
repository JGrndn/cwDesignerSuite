using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Casewise.GraphAPI.API;
using Casewise.GraphAPI.PSF;

namespace Casewise.GraphAPI.Launcher
{
    /// <summary>
    /// cwCreateItemForm
    /// </summary>
    public class cwCreateItemForm : Form
    {
        /// <summary>
        /// itemObjectType
        /// </summary>
        public cwLightObjectType itemObjectType = null;
        /// <summary>
        /// properties
        /// </summary>
        public Dictionary<string, string> properties = new Dictionary<string,string>();
        /// <summary>
        /// panelMain
        /// </summary>
        public Panel panelMain;
        /// <summary>
        /// hasBeenCancel
        /// </summary>
        public bool hasBeenCancel = false;



        /// <summary>
        /// Initializes a new instance of the <see cref="cwCreateItemForm"/> class.
        /// </summary>
        public cwCreateItemForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="cwCreateItemForm"/> class.
        /// </summary>
        /// <param name="itemObjectType">Type of the item object.</param>
        public cwCreateItemForm(cwLightObjectType itemObjectType)
        {
            InitializeComponent();
            this.itemObjectType = itemObjectType;
        }

        /// <summary>
        /// Creates the item.
        /// </summary>
        /// <returns></returns>
        public virtual cwLightObject createItem()
        {
            throw new NotImplementedException("createItem");
        }

        /// <summary>
        /// Ons the create.
        /// </summary>
        /// <param name="parentForm">The parent form.</param>
        public virtual void onCreate(Form parentForm)
        {
        }


        /// <summary>
        /// Checks the form.
        /// </summary>
        /// <returns></returns>
        public virtual bool checkForm()
        {
            return true;
        }

        /// <summary>
        /// tooltips
        /// </summary>
        protected Dictionary<Control, cwPSFToolTip> tooltips = new Dictionary<Control, cwPSFToolTip>();

        /// <summary>
        /// Releases the tooltip.
        /// </summary>
        /// <param name="control">The control.</param>
        public void releaseTooltip(Control control)
        {
            if (tooltips.ContainsKey(control))
            {
                tooltips[control].RemoveAll();
            }
        }

        /// <summary>
        /// Sets the tool tip from PSF property box error.
        /// </summary>
        /// <param name="box">The box.</param>
        public void setToolTipFromPSFPropertyBoxError(cwPSFPropertyBox box)
        {
            string tooltipTitle = box.getTooltipTitle();
            string tooltipText = box.getTooltipText();
            setErrorTooltip(box.getControl(), tooltipTitle, tooltipText);
        }

        /// <summary>
        /// Releases all tooltips.
        /// </summary>
        public void releaseAllTooltips()
        {
           foreach (var tt in tooltips)
            {
                tt.Value.RemoveAll();
                tt.Value.Dispose();
                //tooltips.Remove(tt.Key);
            }
           tooltips.Clear();
        }

        /// <summary>
        /// Sets the error tooltip.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="title">The title.</param>
        /// <param name="error">The error.</param>
        public void setErrorTooltip(Control control, string title, string error)
        {
            releaseAllTooltips();
            cwPSFToolTip tt = new cwPSFToolTip(this.Font);
            tt.ToolTipTitle = title;
            tt.SetToolTip(control, error);
            tt.Show(error, control);
            tooltips[control] = tt;
        }

        /// <summary>
        /// Ons the cancel.
        /// </summary>
        /// <param name="parentForm">The parent form.</param>
        public virtual void onCancel(Form parentForm)
        {
            hasBeenCancel = true;
            parentForm.Close();
        }

        /// <summary>
        /// Initializes the component.
        /// </summary>
        private void InitializeComponent()
        {
            this.panelMain = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // panelMain
            // 
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(0, 0);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(324, 180);
            this.panelMain.TabIndex = 0;
            // 
            // cwCreateItemForm
            // 
            this.ClientSize = new System.Drawing.Size(324, 180);
            this.Controls.Add(this.panelMain);
            this.Font = new System.Drawing.Font("Trebuchet MS", 8.25F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "cwCreateItemForm";
            this.ResumeLayout(false);

        }

      
    }
}
