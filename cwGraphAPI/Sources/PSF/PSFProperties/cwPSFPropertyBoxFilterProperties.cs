using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using Casewise.GraphAPI.API;

namespace Casewise.GraphAPI.PSF
{
    /// <summary>
    /// All to have the list of properties in a combox box in a data grid
    /// </summary>
    public class cwPSFPropertyBoxFilterProperties : cwPSFPropertyBoxCollection
    {

        private cwPSFPropertyBoxDataGrid datagrid = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="cwPSFPropertyBoxFilterProperties"/> class.
        /// </summary>
        /// <param name="_helpName">Name of the _help.</param>
        /// <param name="_helpDescription">The _help description.</param>
        /// <param name="_keyName">Name of the _key.</param>
        public cwPSFPropertyBoxFilterProperties(String _helpName, String _helpDescription, String _keyName)
            : base(_helpName, _helpDescription, _keyName)
        {
            datagrid = new cwPSFPropertyBoxDataGrid(this);
        }

        /// <summary>
        /// Checks the format.
        /// </summary>
        /// <returns></returns>
        public override bool checkFormat()
        {
            if (ParentTreeNode != null)
            {
                if (isDataGridValid())
                {
                    ParentTreeNode.checkNodeStructureRec();
                    return true;
                }
                else
                {
                    ParentTreeNode.structureRecIsUnValid();
                }
            }
            return false;
        }

        private bool isDataGridValid()
        {
            if (datagrid == null) return false;
            foreach (DataGridViewRow row in datagrid.dataGridViewKeepProperties.Rows)
            {
                if (row.IsNewRow) continue;
                for (int i = 0; i < 3; i++)
                {
                    if (row.Cells[i].Value == null) return false;
                }

            }

            return true;
        }

        /// <summary>
        /// Updates the control check node events.
        /// </summary>
        protected override void updateControlCheckNodeEvents()
        {
        }
        /// <summary>
        /// Gets the data grid.
        /// </summary>
        /// <returns></returns>
        public cwPSFPropertyBoxDataGrid getDataGrid()
        {
            return datagrid;
        }

        /// <summary>
        /// Sets the item.
        /// </summary>
        /// <param name="_item">The _item.</param>
        public override void setValue(object _item)
        {
            String str = _item as String;
            if (str == null) return;
            if ("".Equals(str)) return;
            string[] attributes = str.Split(datagrid.separatorAttributes, StringSplitOptions.None);
            foreach (string attr in attributes)
            {
                string[] values = attr.Split(datagrid.separatorValues, StringSplitOptions.None);
                if (!3.Equals(values.Length))
                {
                    continue;
                }
                datagrid.addDataGridRow(values[0], values[1], values[2]);
            }
        }
        /// <summary>
        /// Loads the nodes.
        /// </summary>
        /// <param name="_sourcecwLightObjectType"></param>
        public override void loadNodes(object _sourcecwLightObjectType)
        {
            clearItems();
            cwLightObjectType OT = _sourcecwLightObjectType as cwLightObjectType;
            datagrid.loadPropertiesFromObjectType(OT);

        }

        /// <summary>
        /// Clears the items.
        /// </summary>
        public override void clearItems()
        {
            datagrid.clearGrid();
        }

        /// <summary>
        /// Toes the string collection.
        /// </summary>
        /// <returns></returns>
        public override string ToStringCollection()
        {
            if (datagrid == null) return "NO_SET";
            return datagrid.toStringCollection();
        }

        /// <summary>
        /// Gets the control.
        /// </summary>
        /// <returns></returns>
        public override Control getControl()
        {
            return datagrid;
        }
    }
}
