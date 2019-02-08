using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Casewise.webDesigner.GUI;
using Casewise.GraphAPI.PSF;
using Casewise.GraphAPI.GUI;


namespace Casewise.webDesigner.Nodes
{

    /// <summary>
    /// Allow the creation of include shapes in other shapes type of design rule..
    /// </summary>
    internal class cwDiagramDesignerLayoutIncludeType : cwDiagramDesignerLayoutNode
    {
        public const string CONFIG_PADDING_LEFT = "padding_left";
        public const string CONFIG_PADDING_RIGHT = "padding_right";
        public const string CONFIG_PADDING_TOP = "padding_top";
        public const string CONFIG_PADDING_BOTTOM = "padding_bottom";
        public const string CONFIG_SPACE_X = "space_between_child_x";
        public const string CONFIG_SPACE_Y = "space_between_child_y";
        public const string CONFIG_MAX_COLUMNS = "max_columns";
        public const string CONFIG_DRAW_EMPTY_CHILDEN = "draw_empty_children";

        public cwDiagramDesignerLayoutIncludeType(cwWebDesignerGUI GUI, cwPSFTreeNode _parent)
            : base(GUI, _parent)
        {
            updateText("Layout Diagram Include Type");
        }

        public override bool checkNodeStructureCustom()
        {
            return true;
        }

        public override void setPropertiesBoxes()
        {
            propertiesBoxes.addPropertyBox(new cwPSFPropertyBoxFloat("Left Padding", "The cwBox number of inside left padding from the parent's shape", CONFIG_PADDING_LEFT));
            propertiesBoxes.addPropertyBox(new cwPSFPropertyBoxFloat("Right Padding", "The cwBox number of inside right and bottom padding from the parent's shape", CONFIG_PADDING_RIGHT));
            propertiesBoxes.addPropertyBox(new cwPSFPropertyBoxFloat("Top Padding", "The cwBox number of inside top padding from the parent's shape", CONFIG_PADDING_TOP));
            propertiesBoxes.addPropertyBox(new cwPSFPropertyBoxFloat("Bottom Padding", "The cwBox number of inside bottom padding from the parent's shape", CONFIG_PADDING_BOTTOM));
            propertiesBoxes.addPropertyBox(new cwPSFPropertyBoxFloat("Space Between Child X", "Horizontal space between two child", CONFIG_SPACE_X));
            propertiesBoxes.addPropertyBox(new cwPSFPropertyBoxFloat("Space Between Child Y", "Vertical space between two child", CONFIG_SPACE_Y));
            propertiesBoxes.addPropertyBox(new cwPSFPropertyBoxInt("Number of Columns", "The number of columns required to draw objects", CONFIG_MAX_COLUMNS, 1));
            propertiesBoxes.addPropertyBox(new cwPSFPropertyBoxCheckBox("Draw Empty Children", "Do you want to draw parents with empty children ?", CONFIG_DRAW_EMPTY_CHILDEN, true));
        }

    }


}

