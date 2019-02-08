using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Casewise.GraphAPI;
using System.Xml;
using Casewise.GraphAPI.API;
using Casewise.GraphAPI.GUI;
using log4net;
using Casewise.GraphAPI.Exceptions;

namespace Casewise.GraphAPI.PSF
{
    public partial class cwPSFPropertyBoxDataGrid : UserControl
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(cwEditModeGUI));

        private cwLightObjectType OT = null;
        /// <summary>
        /// Separator between values
        /// </summary>
        public string[] separatorValues = new string[] { "|||" };
        /// <summary>
        /// Separator between Attributes
        /// </summary>
        public string[] separatorAttributes = new string[] { "~~~" };

        private cwPSFPropertyBoxFilterProperties parentPropertyBox;

        /// <summary>
        /// Initializes a new instance of the <see cref="cwPSFPropertyBoxDataGrid"/> class.
        /// </summary>
        public cwPSFPropertyBoxDataGrid(cwPSFPropertyBoxFilterProperties parentPropertyBox)
        {
            InitializeComponent();
            this.parentPropertyBox = parentPropertyBox;
            ContextMenuStrip = new cwPSFContextMenuStrip(this.Font);
            this.Enabled = false;
            Property.HeaderText = Properties.Resources.PSF_FILTER_HEADER_PROPERTY;
            Operation.HeaderText = Properties.Resources.PSF_FILTER_HEADER_OPERATION;
            Value.HeaderText = Properties.Resources.PSF_FILTER_HEADER_VALUE;
        }


        /// <summary>
        /// Gets the attributes filtered.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, List<KeyValuePair<string, string>>> getAttributesFiltered()
        {
            Dictionary<string, List<KeyValuePair<string, string>>> attribute_filters_keep = new Dictionary<string, List<KeyValuePair<string,string>>>();
            foreach (DataGridViewRow row in dataGridViewKeepProperties.Rows)
            {
                DataGridViewComboBoxCell propertyCell = row.Cells[0] as DataGridViewComboBoxCell;
                DataGridViewComboBoxCell operationCell = row.Cells[1] as DataGridViewComboBoxCell;
                DataGridViewTextBoxCell valueCell = row.Cells[2] as DataGridViewTextBoxCell;

                string _value = null;
                if (propertyCell == null) continue;
                if (propertyCell.Value == null) continue;
                if (operationCell == null) continue;
                if (operationCell.Value == null) continue;
                if (valueCell == null)
                {
                    DataGridViewComboBoxCell valueLookupCell = row.Cells[2] as DataGridViewComboBoxCell;
                    if (valueLookupCell == null) continue;
                    if (valueLookupCell.Value == null) continue;
                    _value = valueLookupCell.Value.ToString();
                }
                else
                {
                    if (valueCell.Value == null) continue;
                    _value = valueCell.Value.ToString();
                }
                string propertyName = propertyCell.Value.ToString();
                cwLightProperty property = getPropertyByName(propertyName);
                string _scriptname = property.ScriptName;
                _value = property.getSaveValueFromDisplay(_value);
                if (!attribute_filters_keep.ContainsKey(_scriptname))
                {
                    attribute_filters_keep.Add(_scriptname, new List<KeyValuePair<string, string>>());
                }
                attribute_filters_keep[_scriptname].Add(new KeyValuePair<string, string>(operationCell.Value.ToString(),_value));
            }
            return attribute_filters_keep;
        }

        /// <summary>
        /// Clears the grid.
        /// </summary>
        public void clearGrid()
        {
            dataGridViewKeepProperties.Rows.Clear();
            //attribute_filters_keep.Clear();
            Property.Items.Clear();
        }

        /// <summary>
        /// Loads the type of the properties from object.
        /// </summary>
        /// <param name="_OT">The _ OT.</param>
        public void loadPropertiesFromObjectType(cwLightObjectType _OT)
        {
            if (_OT != null)
            {
                OT = _OT;

                foreach (cwLightProperty p in OT.getSortedProperties())
                {
                    Property.Items.Add(p.ToString());
                }
                this.Enabled = true;
            }
        }


        /// <summary>
        /// Updates the object group using data grid info.
        /// </summary>
        public void updateObjectGroupUsingDataGridInfo()
        {

        }


        /// <summary>
        /// Gets the name of the property by.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public cwLightProperty getPropertyByName(string name)
        {
            cwLightProperty foundProperty = OT.getProperties().Find(p => name.Equals(p.ToString()));
            if (foundProperty == null)
            {
                throw new cwExceptionFatal(String.Format("Unable to find the property {0} in the object type {1}", name, OT.ToString()));
            }
            return foundProperty;
        }

        /// <summary>
        /// Loads the data grid.
        /// </summary>
        /// <param name="_mode">The _mode.</param>
        /// <param name="_scriptname">The _scriptname.</param>
        /// <param name="_value">The _value.</param>
        public void addDataGridRow(string _mode, string _scriptname, string _value)
        {
            int new_row = this.dataGridViewKeepProperties.Rows.Add(1);
            cwLightProperty p = OT.getProperty(_scriptname);
            _value = p.getDisplayValueFromSaveValue(_value);
            this.dataGridViewKeepProperties.Rows[new_row].Cells[0].Value = p.ToString();
            this.dataGridViewKeepProperties.Rows[new_row].Cells[1].Value = _mode;
            this.dataGridViewKeepProperties.Rows[new_row].Cells[2].Value = _value;
            parentPropertyBox.checkFormat();
        }


        /// <summary>
        /// Saves to XML.
        /// </summary>
        /// <returns></returns>
        public string toStringCollection()
        {
            string output = "";
            foreach (DataGridViewRow row in dataGridViewKeepProperties.Rows)
            {
                DataGridViewComboBoxCell PropertyCell = row.Cells[0] as DataGridViewComboBoxCell;
                DataGridViewComboBoxCell OperationCell = row.Cells[1] as DataGridViewComboBoxCell;
                DataGridViewTextBoxCell valueCell = row.Cells[2] as DataGridViewTextBoxCell;
                if (PropertyCell != null && PropertyCell.Value != null)
                {
                    cwLightProperty p = getPropertyByName(PropertyCell.Value.ToString());
                    if (OperationCell != null && OperationCell.Value != null)
                    {
                        output += OperationCell.Value.ToString();
                    }
                    output += separatorValues[0] + p.ScriptName;
                    string _value = "";
                    //string _
                    if (valueCell != null)
                    {
                        if (valueCell.Value != null)
                        {
                            _value = valueCell.Value.ToString();
                        }
                    }
                    else
                    {
                        DataGridViewComboBoxCell valuesCBCell = row.Cells[2] as DataGridViewComboBoxCell;
                        if (valuesCBCell != null)
                        {
                            if (valuesCBCell.Value != null)
                            {

                                _value = p.getSaveValueFromDisplay(valuesCBCell.Value.ToString());
                            }
                        }
                    }
                    output += separatorValues[0] + _value;
                    output += separatorAttributes[0];
                }
            }
            return output;
        }

        /// <summary>
        /// Handles the DataError event of the dataGridViewKeepProperties control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="anError">The <see cref="System.Windows.Forms.DataGridViewDataErrorEventArgs"/> instance containing the event data.</param>
        private void dataGridViewKeepProperties_DataError(object sender, DataGridViewDataErrorEventArgs anError)
        {
            MessageBox.Show("Error happened " + anError.Context.ToString());
            if (anError.Context == DataGridViewDataErrorContexts.Commit)
            {
                MessageBox.Show("Commit error");
            }
            if (anError.Context == DataGridViewDataErrorContexts.CurrentCellChange)
            {
                MessageBox.Show("Cell change");
            }
            if (anError.Context == DataGridViewDataErrorContexts.Parsing)
            {
                MessageBox.Show("parsing error");
            }
            if (anError.Context == DataGridViewDataErrorContexts.LeaveControl)
            {
                MessageBox.Show("leave control error");
            }
            if ((anError.Exception) is ConstraintException)
            {
                DataGridView view = (DataGridView)sender;
                view.Rows[anError.RowIndex].ErrorText = "an error";
                view.Rows[anError.RowIndex].Cells[anError.ColumnIndex].ErrorText = "an error";
                anError.ThrowException = false;
            }
        }



        /// <summary>
        /// Handles the CellValueChanged event of the dataGridViewKeepProperties control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.DataGridViewCellEventArgs"/> instance containing the event data.</param>
        private void dataGridViewKeepProperties_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {

            DataGridViewRow row = null;
            if (dataGridViewKeepProperties != null)
            {
                if (2.Equals(e.ColumnIndex) && parentPropertyBox != null)
                {
                    parentPropertyBox.checkFormat();
                }
                if (!0.Equals(e.ColumnIndex))
                {
                    return;
                }
                if (e.RowIndex == -1 || e.ColumnIndex == -1) return;
                DataGridViewCell cell = dataGridViewKeepProperties[e.ColumnIndex, e.RowIndex];
                row = dataGridViewKeepProperties.Rows[e.RowIndex];
                // if the first cell value is set
                if (row.Cells[0] != null && row.Cells[0].Value != null)
                {
                    DataGridViewComboBoxCell propertyCell = row.Cells[0] as DataGridViewComboBoxCell;
                    if (propertyCell == null) return;

                    cwLightProperty p = getPropertyByName(propertyCell.Value.ToString());
                    //= propertyCell.Value as cwLightProperty;// OT.getProperty(propertyCell.Value.ToString());
                    //cwLightProperty p = propertyCell.property;
                    //OT.getProperties().Find(p_inline => p_inline.ToString().Equals(propertyCell.Value));
                    // if the cell is a property
                    if (p != null)
                    {
                        // if has cell 2
                        if (row.Cells[2] != null)//&& row.Cells[2].Value != null && "".Equals(row.Cells[2].Value.ToString()))
                        {
                            // if the cell is a lookup type
                            if (p.isLookup)
                            {

                                DataGridViewComboBoxCell content = new DataGridViewComboBoxCell();
                                List<cwLookup> lookupOrderdContent = p.lookupContent;
                                lookupOrderdContent.Sort();
                                content.Items.Add(Properties.Resources.TEXT_CATEGORY_UNDEFINED);
                                lookupOrderdContent.ForEach(value => content.Items.Add(value.ToString()));
                                row.Cells[2] = content;
                            }
                            else if (p.isBoolean)
                            {
                                DataGridViewComboBoxCell content = new DataGridViewComboBoxCell();
                                content.Items.Add(Properties.Resources.TEXT_TRUE);
                                content.Items.Add(Properties.Resources.TEXT_FALSE);
                                row.Cells[2] = content;
                            }
                            else
                            {
                                row.Cells[2] = new DataGridViewTextBoxCell();
                            }
                        }
                    }
                }

            }


        }
        int currentRowMouseOver = -1;
        private void dataGridViewKeepProperties_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            currentRowMouseOver = e.RowIndex;
        }

        private void dataGridViewKeepProperties_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            currentRowMouseOver = -1;
        }

        private void dataGridViewKeepProperties_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && currentRowMouseOver != -1)
            {
                if (dataGridViewKeepProperties.Rows[currentRowMouseOver].IsNewRow)
                {
                    return;
                }
                ContextMenuStrip m = new ContextMenuStrip();
                ToolStripMenuItem deleteItem = new ToolStripMenuItem(Properties.Resources.PSF_TN_CTX_DELETE);
                deleteItem.Image = Properties.Resources.image_tvicon_delete;
                deleteItem.Click += new System.EventHandler(this.ctx_deleteRow_Click);
                m.Items.Add(deleteItem);
                m.Show(this, new Point(e.X, e.Y));
            }
        }

        private void ctx_deleteRow_Click(object sender, EventArgs e)
        {
            try
            {
                if (currentRowMouseOver != -1)
                {
                    if (dataGridViewKeepProperties != null)
                    {
                        if (!dataGridViewKeepProperties.Rows[currentRowMouseOver].IsNewRow)
                        {
                            dataGridViewKeepProperties.Rows.RemoveAt(currentRowMouseOver);
                            if (parentPropertyBox != null) parentPropertyBox.checkFormat();
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                cwPSFTreeNode parentTN = this.parentPropertyBox.ParentTreeNode;
                if (parentTN != null)
                {
                    parentTN.appendError(exception.Message);
                    log.Error(exception.ToString());
                }
            }
        }
    }
}

