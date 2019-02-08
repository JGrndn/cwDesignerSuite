using Casewise.Data.ICM;
using Casewise.Services.Entities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;

namespace Casewise.webDesigner.Sources.Libs.Diagram
{
    class CwStyle
    {
        private static int FLAG_GRADIENT = 4;
        private static int FLAG_OPACITY = 2;
        private static Color DEFAULT_FILL_COLOR = Color.Yellow;
        private static Color DEFAULT_TEXT_COLOR = Color.Black;
        private static int DEFAULT_LINE_WIDTH = 1;
        private static FillStyle DEFAULT_FILL_STYLE = FillStyle.Solid;
        private static DashStyle DEFAULT_LINE_STYLE = DashStyle.Solid;
        private static LinearGradientMode DEFAULT_GRADIENT = LinearGradientMode.Horizontal;

        internal static string PROP_TO_LOAD = @"ID, TEXTCOLOUR, FONTNR, FILLCOLOUR, LINECOLOUR,
LINEWIDTH, FILLSTYLE, LINESTYLE, NAMELINESTYLE, SPECIALEFFECTS, NAMEFILLPATTERN, NAMEFILLCOLOUR,
SHADOWDIRECTION, NAMESHADOWSIZE, NAMESHADOWCOLOUR";

        internal int id { get; set; }
        public string textColor { get; set; }
        public CwFont font { get; set; }
        public string fillColor { get; set; }
        public FillStyle fillPattern { get; set; }
        public string strokeColor { get; set; }
        public DashStyle strokePattern { get; set; }
        public int lineWidth { get; set; }
        public bool hasGradient { get; set; }
        public LinearGradientMode gradientDir { get; set; }
        public string gradientFromFillColor { get; set; }
        public string gradientToFillColor { get; set; }
        public int gradientSize { get; set; }
        public int shadowSize { get; set; }
        public bool hasOpacity { get; set; }
        public int opacityPercentage { get; set; }


        public CwStyle(ICMDataReader reader, Dictionary<int, CwFont> fontsById)
        {
            this.id = reader.GetInt32(0);
            this.textColor = CwStyle.GetColor(reader.GetInt32(1));
            int fontId = reader.GetInt32(2);
            if (fontsById != null && fontsById.ContainsKey(fontId))
            {
                this.font = fontsById[fontId];
            }
            else
            {
                this.font = new CwFont();
            }
            this.fillColor = CwStyle.GetColor(reader.GetInt32(3));
            this.strokeColor = CwStyle.GetColor(reader.GetInt32(4));
            this.lineWidth = reader.GetInt32(5);
            this.fillPattern = (FillStyle)reader.GetInt32(6);
            this.strokePattern = (DashStyle)reader.GetInt32(7);
            this.gradientSize = reader.GetInt32(8);
            this.gradientFromFillColor = CwStyle.GetColor(reader.GetInt32(10));
            this.gradientToFillColor = CwStyle.GetColor(reader.GetInt32(11));
            this.shadowSize = reader.GetInt32(12);
            this.gradientDir = (LinearGradientMode)reader.GetInt32(13);
            this.opacityPercentage = reader.GetInt32(14) / 5;

            int styleEffects = reader.GetInt32(9);
            this.hasGradient = CwStyle.FLAG_GRADIENT.Equals(CwStyle.FLAG_GRADIENT & styleEffects);
            this.hasOpacity = CwStyle.FLAG_OPACITY.Equals(CwStyle.FLAG_OPACITY & styleEffects);
        }

        public CwStyle()
        {
            this.id = 0;
            this.textColor = CwStyle.GetColor(CwStyle.DEFAULT_TEXT_COLOR);
            this.font = new CwFont();
            this.fillColor = CwStyle.GetColor(CwStyle.DEFAULT_FILL_COLOR);
            this.strokeColor = CwStyle.GetColor(CwStyle.DEFAULT_TEXT_COLOR);
            this.lineWidth = CwStyle.DEFAULT_LINE_WIDTH;
            this.fillPattern = CwStyle.DEFAULT_FILL_STYLE;
            this.strokePattern = CwStyle.DEFAULT_LINE_STYLE;
            this.gradientSize = 0;
            this.gradientFromFillColor = CwStyle.GetColor(CwStyle.DEFAULT_FILL_COLOR);
            this.gradientToFillColor = CwStyle.GetColor(CwStyle.DEFAULT_FILL_COLOR);
            this.shadowSize = 0;
            this.gradientDir = CwStyle.DEFAULT_GRADIENT;
            this.opacityPercentage = 0;

            this.hasGradient = false;
            this.hasOpacity = false;
        }

        private static string GetColor(int cInt)
        {
            Color c = ColorTranslator.FromOle(cInt);
            return CwStyle.GetColor(c);
        }

        private static string GetColor(Color c)
        {
            return ColorTranslator.ToHtml(c);
        }

    }
}
