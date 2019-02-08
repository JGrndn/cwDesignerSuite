using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Casewise.GraphAPI.API;
using System.IO;
using log4net;
using Casewise.GraphAPI.PSF;

namespace Casewise.webDesigner.Libs
{
    public class cwWebDesignerJSONTools
    {

        internal static readonly ILog log = LogManager.GetLogger(typeof(cwWebDesignerJSONTools));
        string _linkMIMEType = "html";
        /// <summary>
        /// Initializes a new instance of the <see cref="cwWebDesignerJSONTools"/> class.
        /// </summary>
        public cwWebDesignerJSONTools()
        {
        }

        /// <summary>
        /// Sets the type of the link MIME.
        /// </summary>
        /// <param name="mimeType">Type of the MIME.</param>
        public void setLinkMIMEType(string mimeType)
        {
            _linkMIMEType = mimeType;
        }
        /// <summary>
        /// Gets the link for object.
        /// </summary>
        /// <param name="OTScriptNameLower">The OT script name lower.</param>
        /// <param name="ID">The ID.</param>
        /// <returns></returns>
        public string getLinkForObject(string OTScriptNameLower, string ID)
        {
            return "index." + _linkMIMEType + "?cwtype=single&cwview=" + OTScriptNameLower + "&cwid=" + ID;
        }

        /// <summary>
        /// Gets the index of the link for.
        /// </summary>
        /// <param name="viewName">Name of the view.</param>
        /// <returns></returns>
        public string getLinkForIndex(string viewName)
        {
            return "index." + _linkMIMEType + "?cwtype=index&cwview=" + viewName;
        }

        public static string floatToString(float value)
        {
            return value.ToString().Replace(",", ".");
        }



        private static string intColorToHex(string hexValue)
        {
            if (1.Equals(hexValue.Length))
            {
                return "0" + hexValue;
            }
            return hexValue;
        }

        /// <summary>
        /// Colors to JSON.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns></returns>
        public static string colorToJSON(System.Drawing.Color color)
        {
            return "#" + intColorToHex(color.R.ToString("X")) + intColorToHex(color.G.ToString("X")) + intColorToHex(color.B.ToString("X"));
        }


    
    }
}
