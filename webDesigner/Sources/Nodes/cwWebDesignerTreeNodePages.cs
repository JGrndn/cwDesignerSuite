using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Casewise.GraphAPI.PSF;
using Casewise.webDesigner.GUI;
using System.Xml;
using Casewise.webDesigner.Libs;
using Casewise.GraphAPI.API;

namespace Casewise.webDesigner.Nodes
{
    /// <summary>
    /// 
    /// </summary>
    public class cwWebDesignerTreeNodePages : cwPSFTreeNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="cwWebDesignerTreeNodePages"/> class.
        /// </summary>
        /// <param name="_cwWebIndexGUI">The _CW web index GUI.</param>
        /// <param name="_parent">The _parent.</param>
        public cwWebDesignerTreeNodePages(cwWebDesignerGUI _cwWebIndexGUI, cwPSFTreeNode _parent)
            : base(_cwWebIndexGUI, _parent)
        {
            updateText("Pages");
      
        }

        /// <summary>
        /// Sets the properties boxes.
        /// </summary>
        public override void setPropertiesBoxes()
        {
        }




        /// <summary>
        /// Adds the association types to context menu.
        /// </summary>
        public void addObjectTypesToContextMenu(ContextMenuStrip menuStrip, cwPSFTreeNode toNode)
        {

            //if (getSelectedObjectType() == null)
            //{
            //    return;
            //}
            ////cwLightObjectType selectedOT = getSelectedObjectType();
            //ToolStripMenuItem c = new ToolStripMenuItem("Add Object Type");

            //foreach (var targetOTVar in selectedOT.getAssociationTypesByTargetObjectType())
            //{
            //    ToolStripMenuItem menuOTTarget = new ToolStripMenuItem(targetOTVar.Key.ToString());
            //    for (var i = 0; i < targetOTVar.Value.Count(); ++i)
            //    {
            //        cwLightAssociationType AT = targetOTVar.Value[i];
            //        ToolStripItem item = menuOTTarget.DropDownItems.Add(AT.ToString());
            //        item.Click += (sender, args) => ctx_addChildAssociationTypeNode(toNode, menuStrip, AT, args);
            //    }
            //    c.DropDownItems.Add(menuOTTarget);
            //}
            //menuStrip.Items.Add(c);
        }

    }
}
