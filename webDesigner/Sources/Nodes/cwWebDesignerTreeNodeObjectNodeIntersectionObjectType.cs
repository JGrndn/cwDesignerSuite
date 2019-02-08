using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Casewise.GraphAPI.PSF;
using Casewise.GraphAPI.API;
using Casewise.webDesigner.GUI;
using System.Drawing;
using Casewise.GraphAPI.ProgramManager;


namespace Casewise.webDesigner.Nodes
{
    internal class cwWebDesignerTreeNodeObjectNodeIntersectionObjectType : cwWebDesignerTreeNodeObjectNode
    {
        public cwWebDesignerTreeNodeObjectNodeIntersectionObjectType(cwWebDesignerGUI _cwEditModeGUI, cwPSFTreeNode _parent)
            : base(_cwEditModeGUI, _parent)
        {
            updateText("WebDesigner Intersection Object Type Node");
            cwWebDesignerTreeNodeObjectNodeAssociationType AT = _parent as cwWebDesignerTreeNodeObjectNodeAssociationType;
            if (AT != null)
            {
                setIntersectionObjectType(AT.getSelectedAssociationType());
            }
        }

        public cwWebDesignerTreeNodeObjectNodeIntersectionObjectType(cwLightAssociationType associationType)
            : base(new cwWebDesignerGUI(associationType.Source.Model, new cwProgramManagerOptions()), null)
        {
            updateText("WebDesigner Intersection Object Type Node");
            setIntersectionObjectType(associationType);
            
        }

        public void addAndSetObjectType(cwLightObjectType objectType)
        {
            propertiesBoxes.getPropertyBox<cwPSFPropertyBoxComboBoxObjectType>(cwPSFTreeNodeObjectNodeObjectType.CONFIG_OBJECT_TYPE).add(objectType);
            setSelectedObjectType(objectType);
        }

        private void setIntersectionObjectType(cwLightAssociationType associationType)
        {
            if (associationType.hasIntersection && associationType.Intersection != null)
            {
                addAndSetObjectType(associationType.Intersection);
            }
        }

        public override bool checkNodeStructureCustom()
        {
            return true;
        }

        #region override section
        public override void setPropertiesBoxes()
        {
            cwPSFTreeNodeObjectNodeObjectType.setPropertiesBoxes(this);
            base.setPropertiesBoxes();
        }

        /// <summary>
        /// Gets the type of the selected object.
        /// </summary>
        /// <returns></returns>
        public override cwLightObjectType getSelectedObjectType()
        {
            return cwPSFTreeNodeObjectNodeObjectType.getSelectedObjectType(this);
        }

        /// <summary>
        /// Sets the type of the selected object.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        public override void setSelectedObjectType(cwLightObjectType objectType)
        {
            cwPSFTreeNodeObjectNodeObjectType.setSelectedObjectType(this, objectType);
        }

        /// <summary>
        /// Updates the name.
        /// </summary>
        public override void updateName()
        {
            cwPSFTreeNodeObjectNodeObjectType.updateName(this);
        }

        /// <summary>
        /// Gets the name of the box.
        /// </summary>
        /// <returns></returns>
        public override string getBoxName()
        {
            return cwPSFTreeNodeObjectNodeObjectType.getBoxName(this);
        }

        #endregion
    }
}
