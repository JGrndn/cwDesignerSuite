using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Casewise.webDesigner.Sources.Libs.Diagram.Palette
{
    public class CwPaletteEntry
    {
        public string ObjectTypeScriptname { get; set; }
        public string OjectTypeCategoryUuid { get; set; }
        public int Symbol { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Style { get; set; }

        public int HorizontalJustification { get; set; }
        public int VerticalJustification { get; set; }
        public int TextRotation { get; set; }
        public int Display { get; set; }

        public List<CwPaletteRegion> Regions { get; set; }
    }
}
