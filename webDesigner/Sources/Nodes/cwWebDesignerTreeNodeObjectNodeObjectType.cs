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
    internal class cwWebDesignerTreeNodeObjectNodeObjectType : cwWebDesignerTreeNodeObjectNode
    {
        public cwWebDesignerTreeNodeObjectNodeObjectType(cwWebDesignerGUI _cwEditModeGUI, cwPSFTreeNode _parent)
            : base(_cwEditModeGUI, _parent)
        {
            string s = this.getName();
            updateText("WebDesigner Object Type Node");
        }

        public cwWebDesignerTreeNodeObjectNodeObjectType(cwLightObjectType objectType)
            : base(new cwWebDesignerGUI(objectType.Model, new cwProgramManagerOptions()), null)
        {
            updateText("WebDesigner Object Type Node");
            setSelectedObjectType(objectType);
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
            // permet de personnaliser le nom des nodes dans la publication
            return getName();
            //return cwPSFTreeNodeObjectNodeObjectType.getBoxName(this);
        }

        #endregion
    }
}
