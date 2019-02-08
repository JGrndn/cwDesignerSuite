using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Casewise.GraphAPI.Exceptions;

namespace Casewise.GraphAPI.PSF
{
    /// <summary>
    /// Allow to select a file with a browse button
    /// </summary>
    public class cwPSFPropertyBoxFile : cwPSFPropertyBox
    {       
        private TableLayoutPanel p = null;

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns></returns>
        public string getValue()
        {
            return Text;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return getValue();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="cwPSFPropertyBoxString"/> class.
        /// </summary>
        public cwPSFPropertyBoxFile(String _helpName, String _helpDescription, String _keyName)
            : base(_helpName, _helpDescription, _keyName)
        {
            initPSFTextBoxFolder();
        }

        /// <summary>
        /// Inits the PSF text box folder.
        /// </summary>
        private void initPSFTextBoxFolder()
        {
            p = new TableLayoutPanel();
            p.BorderStyle = System.Windows.Forms.BorderStyle.None;
            p.Height = 60;
            p.Dock = DockStyle.Fill;
            p.ColumnCount = 1;
            p.RowCount = 2;
            Button browse = new Button();
            browse.Text = "Browse";            
            browse.Click += new EventHandler(browse_Click);
            this.Dock = DockStyle.Fill;
            browse.Dock = DockStyle.Fill;
            p.Controls.Add(this, 1, 1);
            p.Controls.Add(browse, 1, 2);
        }

        /// <summary>
        /// Handles the Changed event of the cwPSFPropertyBoxString control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        public void browse_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Text = ofd.FileName;
            } 
        }

        /// <summary>
        /// Gets the control.
        /// </summary>
        /// <returns></returns>
        public override Control getControl()
        {
            return p;
        }

        /// <summary>
        /// Sets the item.
        /// </summary>
        /// <param name="_item">The _item.</param>
        public override void setValue(object _item)
        {
            Text = _item.ToString();
        }

        /// <summary>
        /// Handles the Changed event of the cwPSFPropertyBoxString control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        public void cwPSFPropertyBoxString_Changed(object sender, EventArgs e)
        {
            checkFormat();            
        }

        /// <summary>
        /// Handles the Changed event of the cwPSFPropertyBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        public override void cwPSFProperyBox_Changed(object sender, EventArgs e)
        {
        }

        ///// <summary>
        ///// Loads from configuration file.
        ///// </summary>
        ///// <param name="optionName">Name of the option.</param>
        ///// <param name="config">The config.</param>
        ///// <param name="guiHelper">The GUI helper.</param>
        //public void loadFromConfigurationFile(string optionName, cwConfiguration config, cwGUIHelper guiHelper)
        //{
        //    if (config == null) throw new cwExceptionFatal("Configuration is required while loading a property");
        //    string _value = config.getOptionValue(optionName);
        //    if (_value == null) throw new cwExceptionFatal("The following property should be set [" + optionName+ "] in .exe.config file");
        //    this.Text = _value;
        //}
    }
}
