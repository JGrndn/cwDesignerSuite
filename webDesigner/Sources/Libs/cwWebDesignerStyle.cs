using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Casewise.Services.Entities;
using System.Drawing.Drawing2D;
using System.Web.UI.WebControls;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;

namespace Casewise.webDesigner.Libs
{


    internal class cwWebDesignerStyleJSON
    {
        public string textColor { get; set; }
        public string fillColor { get; set; }
        public string fillPattern { get; set; }
        public string strokeColor { get; set; }
        public string strokePattern { get; set; }
        public int lineWidth { get; set; }
        public bool hasGradient { get; set; }
        public int gradientDir { get; set; }
        public string gradientFromFill { get; set; }
        public string gradientToFill { get; set; }
        public bool hasShadow { get; set; }
        public int gradientSize { get; set; }
        public cwWebDesignerFontJSON font { get; set; }
        public bool hasOpacity { get; set; }
        public int opacityPercentage { get; set; }
    }


    internal class cwWebDesignerStyle
    {
        private bool defaultStyle = false;

        private Color textColor;
        private Color fillColor;
        private Color strokeColor;
        private DashStyle strokePattern;
        private int lineWidth;
        private FillStyle fillPattern;
        private cwWebDesignerFont font = null;
        private int cwLineToLineRatio = 160;

        private LinearGradientMode gradienDir;
        private Color gradientFromFillColor;
        private Color gradientToFillColor;
        private bool hasGradient = false;
        private int gradientSize = 0;
        private int shadowSize = 0;

        private bool hasOpacity = false;

        private int opacityPercentage = 0;

        private const int FLAG_GRADIENT = 4;
        private const int FLAG_OPACITY = 2;

        public cwWebDesignerStyle()
        {
            this.defaultStyle = true;
        }

        // create from DB
        public cwWebDesignerStyle(int _textColorINT, cwWebDesignerFont _font, int _fillColorINT, int _fillPattern, int _strokeColorINT, int _lineWidth, int _strokePattern, int _gradientDir, int _gradientSize, int _gradientFromColorINT, int _gradientToColorINT, int _shadowDirection, int _styleEffect, int _opacityPercentage)
        {
            textColor = cwWebDesignerStyle.intToColor(_textColorINT);
            fillColor = cwWebDesignerStyle.intToColor(_fillColorINT);
            strokeColor = cwWebDesignerStyle.intToColor(_strokeColorINT);
            strokePattern = (DashStyle)_strokePattern;
            lineWidth = _lineWidth;// / cwLineToLineRatio;
            font = _font;
            fillPattern = (FillStyle)_fillPattern;
            gradientFromFillColor = cwWebDesignerStyle.intToColor(_gradientFromColorINT);
            gradientToFillColor = cwWebDesignerStyle.intToColor(_gradientToColorINT);
            gradientSize = _gradientSize;
            opacityPercentage = _opacityPercentage;

            if ((FLAG_GRADIENT & _styleEffect) == FLAG_GRADIENT)
            {
                hasGradient = true;
            }
            gradienDir = (LinearGradientMode)_gradientDir;
            shadowSize = _shadowDirection;

            if ((FLAG_OPACITY & _styleEffect) == FLAG_OPACITY)
            {
                hasOpacity = true;
            }
        }



        /// <summary>
        /// Sets the gradient.
        /// </summary>
        /// <param name="_style">The _style.</param>
        public void setGradient(cwWebDesignerStyle _style)
        {
            hasGradient = true;
            gradientSize = _style.gradientSize;
            gradienDir = _style.gradienDir;
            gradientFromFillColor = _style.gradientFromFillColor;
            gradientToFillColor = _style.gradientToFillColor;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="cwWebDesignerStyle"/> class.
        /// create from the palette entry
        /// </summary>
        /// <param name="_textColor">Color of the _text.</param>
        /// <param name="_font">The _font.</param>
        /// <param name="_fillColor">Color of the _fill.</param>
        /// <param name="_fillPattern">The _fill pattern.</param>
        /// <param name="_strokeColor">Color of the _stroke.</param>
        /// <param name="_lineWidth">Width of the _line.</param>
        /// <param name="_strokePattern">The _stroke pattern.</param>
        public cwWebDesignerStyle(Color _textColor, CWFont _font, Color _fillColor, FillStyle _fillPattern, Color _strokeColor, int _lineWidth, DashStyle _strokePattern)
        {
            textColor = _textColor;
            fillColor = _fillColor;
            strokeColor = _strokeColor;
            lineWidth = _lineWidth / cwLineToLineRatio;
            font = new cwWebDesignerFont(_font);
            fillPattern = _fillPattern;
            strokePattern = _strokePattern;
        }

        private static Color intToColor(int colorInt)
        {
            var bytes = BitConverter.GetBytes(colorInt);
            //Color c = Color.FromArgb(bytes[3], bytes[2], bytes[1], bytes[0]);
            Color c = Color.FromArgb(bytes[3], bytes[0], bytes[1], bytes[2]);
            return c;
        }



        public cwWebDesignerStyleJSON getJSONObject()
        {
            cwWebDesignerStyleJSON json = new cwWebDesignerStyleJSON();
            if (this.defaultStyle)
            {
                json.fillColor = "#FFFF80";
                json.fillPattern = "Transparent";
                json.font = new cwWebDesignerFontJSON();
                json.font.font = " 10pt Arial";
                json.font.size = 10;
            }
            else
            {
                json.textColor = cwWebDesignerJSONTools.colorToJSON(textColor);
                json.fillColor = cwWebDesignerJSONTools.colorToJSON(fillColor);
                json.fillPattern = fillPattern.ToString();
                json.strokeColor = cwWebDesignerJSONTools.colorToJSON(strokeColor);
                json.strokePattern = strokePattern.ToString();
                json.lineWidth = lineWidth;
                if (hasGradient)
                {
                    json.hasGradient = true;
                    json.gradientDir = (int)gradienDir;
                    json.gradientSize = gradientSize;
                    json.gradientFromFill = cwWebDesignerJSONTools.colorToJSON(gradientFromFillColor);
                    json.gradientToFill = cwWebDesignerJSONTools.colorToJSON(gradientToFillColor);
                }
                else
                {
                    json.hasGradient = false;
                }
                json.hasShadow = ((shadowSize > 0) ? true : false);
                json.font = (font != null) ? font.getJSONObject() : null;
                json.hasOpacity = hasOpacity;
                json.opacityPercentage = opacityPercentage;
            }
            return json;
        }

        public string toJSON()
        {
            cwWebDesignerStyleJSON json = getJSONObject();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string output = serializer.Serialize(json);
            return output;
        }
    }
}
