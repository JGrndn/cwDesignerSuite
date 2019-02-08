using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Casewise.GraphAPI.PSF;
using Casewise.GraphAPI.GUI;
using System.Windows.Forms;
using System.Drawing;
using Casewise.GraphAPI.Exceptions;
using Casewise.GraphAPI.API;
using Casewise.webDesigner.GUI;
using Casewise.GraphAPI;

namespace Casewise.webDesigner.Nodes
{
    /// <summary>
    /// 
    /// </summary>
    public class cwWebDesignerTreeNodeObjectNodeOnDiagram : cwWebDesignerTreeNodeObjectNodeAssociationType
    {

        internal static string CONFIG_OBJECTTYPE_DIAGRAM_ON = "object-type-on-diagram";


        /// <summary>
        /// Initializes a new instance of the <see cref="cwPSFTreeNodeObjectNodeOnDiagram"/> class.
        /// </summary>
        /// <param name="_webEditModeGUI">The _web edit mode GUI.</param>
        /// <param name="_parent">The _parent.</param>
        public cwWebDesignerTreeNodeObjectNodeOnDiagram(cwWebDesignerGUI _webEditModeGUI, cwPSFTreeNode _parent)
            : base(_webEditModeGUI, _parent)
        {
        }

        /// <summary>
        /// Handles the Click event of the ctx_addChildrenPages_StripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ctx_addChildrenPages_StripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                //cwWebDesignerTreeNodeObjectGroups OGsParent = getParent() as cwWebDesignerTreeNodeObjectGroups;
                //initObjectGroup();
                //cwLightObjectType OT = objectGroup.localcwLightObjectType;
                //if (objectGroup.getAssociationType() != null)
                //{
                //    OT = objectGroup.getAssociationType().Target;
                //}
                //cwWebDesignerTreeNodePages.createMissingAssociations(OT, OGsParent, getGUI<cwWebDesignerGUI>());
            }
            catch (Exception _exception)
            {
                cwPSFTreeNode.log.Error(_exception.ToString());
            }
        }

        /// <summary>
        /// Gets the type of the selected object.
        /// </summary>
        /// <returns></returns>
        public override cwLightObjectType getSelectedObjectType()
        {
            cwPSFPropertyBoxComboBoxObjectType OTNode = propertiesBoxes.getPropertyBox<cwPSFPropertyBoxComboBoxObjectType>(CONFIG_OBJECTTYPE_DIAGRAM_ON);
            return OTNode.getSelectedObjectType();
            //cwLightObjectType DiagramOnOT = getOnDiagramcwLightObjectType();
            //if (DiagramOnOT == null)
            //{
            //    return base.getSelectedObjectType();
            //}
            //return DiagramOnOT;
        }

        private void removeFromSelectedcwLightObjectTypeDependents(string _removeThisKey)
        {
            //int i = selectedObjectTypeDependantsPropertiesBoxes.FindIndex(p => p.keyName.Equals(_removeThisKey));
            //if ((-1).Equals(i))
            //{
            //    throw new cwExceptionFatal("required propery box [" + _removeThisKey + "] is missing in selected object type dependants list");
            //}
            //selectedObjectTypeDependantsPropertiesBoxes.RemoveAt(i);
        }


        //private cwLightObjectType getOnDiagramcwLightObjectType()
        //{
        //    cwPSFPropertyBoxComboBoxObjectType OTNode = propertiesBoxes.getPropertyBox<cwPSFPropertyBoxComboBoxObjectType>(CONFIG_cwLightObjectType_DIAGRAM_ON);
        //    return OTNode.getSelectedObjectType();
        
        //}

        /// <summary>
        /// Updates the name.
        /// </summary>
        public override void updateName()
        {
            cwLightObjectType OT = getSelectedObjectType();
            if (OT == null)
            {
                base.updateName();
            }
            else
            {
                updateText(OT.ToString() + " [on Diagram]");
                ID = cwTools.stringToID(OT.ScriptName);
                ForeColor = Color.Green;
            }
        }


        /// <summary>
        /// Handles the NameValueChanged event of the CBObjectTypes control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        public void CBObjectTypes_NameValueChanged(object sender, EventArgs e)
        {
            updateName();
            //if (!"".Equals(getStringProperty(CONFIG_OG_ID))) return;
            //base.CBcwLightObjectTypes_NameValueChanged(sender, e);
            //cwLightObjectType OT = getOnDiagramcwLightObjectType();
            //if (OT != null)
            //{
            //    string _id = cwTools.stringToID(OT.ToString().ToLower());
            //    propertiesBoxes.getPropertyBox(CONFIG_OG_ID).Text = _id;
            //    propertiesBoxes.getPropertyBox(CONFIG_OG_CSS).Text = _id; 
            //}            
        }




        /// <summary>
        /// Sets the properties boxes.
        /// </summary>
        public override void setPropertiesBoxes()
        {

            cwPSFPropertyBoxComboBoxObjectType OTOnDiagram = new cwPSFPropertyBoxComboBoxObjectType("On Diagram Object Type", "Select which object should be on the diagram", CONFIG_OBJECTTYPE_DIAGRAM_ON, this);
            //List<cwLightObjectType> OTs = mainForm.m.ObjectsType.ToList<cwLightObjectType>();
            OTOnDiagram.SelectedIndexChanged(new EventHandler(CBObjectTypes_NameValueChanged));
            OTOnDiagram.loadNodes(operationEditModeGUI.Model);
            propertiesBoxes.addPropertyBox(OTOnDiagram);
            base.setPropertiesBoxes();
            //removeFromSelectedcwLightObjectTypeDependents(CONFIG_ASSOCIATION_TYPE);

            cwPSFPropertyBoxComboBoxAssociationType AT = propertiesBoxes.getPropertyBox<cwPSFPropertyBoxComboBoxAssociationType>(cwPSFTreeNodeObjectNodeAssociationType.CONFIG_ASSOCIATION_TYPE);
            //cwLightObjectType OTDiagram = operationEditModeGUI.Model["DIAGRAM"];
            //AT.loadNodes(OTDiagram);
            AT.setValue("ANYOBJECTSHOWNASSHAPEINDIAGRAM");
            AT.disable();



        }

    }
}
