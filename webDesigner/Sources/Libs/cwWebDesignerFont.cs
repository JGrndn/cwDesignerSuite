using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Casewise.Services.Entities;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;

namespace Casewise.webDesigner.Libs
{

    internal class cwWebDesignerFontJSON
    {
        public int size { get; set; }
        public string font { get; set; }
    }

    internal class cwWebDesignerFont
    {
        int number = 0;
        string face = "Arial";
        int height = 0;
        int width = 0;
        CWFont.Styles fontStyle = CWFont.Styles.GPD_FONT_PLAIN;

        public cwWebDesignerFont(IDataReader reader)
        {
            number = reader.GetInt32(0);
            face = reader.GetString(1);
            height = reader.GetInt32(2);
            width = reader.GetInt32(3);
            Int32 _fontStyleINT = reader.GetInt32(4);
            fontStyle = (CWFont.Styles)_fontStyleINT;
        }

        public cwWebDesignerFont(CWFont _font)
        {
            if (_font != null)
            {
                number = _font.CWFontId;
                face = _font.FamilyName;
                height = Convert.ToInt32(_font.Size);
                fontStyle = _font.CasewiseStyle;
            }
            width = 0;
        }


        public cwWebDesignerFontJSON getJSONObject()
        {
            cwWebDesignerFontJSON json = new cwWebDesignerFontJSON();
            json.size = height;
            string _fontStyle = "";
            if (CWFont.Styles.GPD_FONT_BOLD.Equals(CWFont.Styles.GPD_FONT_BOLD & fontStyle))
            {
                _fontStyle = "bold";
            }
            json.font = _fontStyle + " " + height.ToString() + "pt " + face.ToString();
            return json;
        }

        public string toJSON()
        {
            cwWebDesignerFontJSON json = getJSONObject();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Serialize(json);
        }
    }
}
