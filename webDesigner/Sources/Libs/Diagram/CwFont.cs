using Casewise.Data.ICM;
using Casewise.Services.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Casewise.webDesigner.Sources.Libs.Diagram
{
    public class CwFont
    {
        private static string BOLD = " bold ";
        private static string ITALIC = " italic ";
        private static int DEFAULT_SIZE = 12;
        private static string DEFAULT_FONT = "Arial";

        internal static string PROP_TO_LOAD = "FONTNR, FONTFACE, FONTHEIGHT, FONTWIDTH, FONTSTYLE, FONTFLAGS";

        internal int id { get; set; }

        public string font { get; set; }
        public int h { get; set; }
        public int w { get; set; }
        public string style { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CwFont"/> class.
        /// </summary>
        /// <param name="reader">The reader.</param>
        public CwFont(ICMDataReader reader)
        {
            this.id = reader.GetInt32(0);
            this.font = reader.GetString(1);
            this.h = reader.GetInt32(2);
            this.w = reader.GetInt32(3);
            CWFont.Styles tmpStyle = (CWFont.Styles)reader.GetInt32(4);
            this.style = CwFont.GetStyle(tmpStyle);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CwFont"/> class.
        /// </summary>
        /// <param name="f">The configuration.</param>
        public CwFont(CWFont f= null)
        {
            if (f != null)
            {
                this.id = f.CWFontId;
                this.font = f.FamilyName;
                this.h = Convert.ToInt32(f.Size);
                this.style = CwFont.GetStyle(f.CasewiseStyle);
            }
            else
            {
                this.id = 0;
                this.font = CwFont.DEFAULT_FONT;
                this.h = CwFont.DEFAULT_SIZE;
                this.style = CwFont.GetStyle(CWFont.Styles.GPD_FONT_PLAIN);
            }
            this.w = 0;
        }

        private static string GetStyle(CWFont.Styles s)
        {
            string style = string.Empty;
            if (CWFont.Styles.GPD_FONT_BOLD.Equals(s & CWFont.Styles.GPD_FONT_BOLD))
            {
                style += CwFont.BOLD;
            }
            if (CWFont.Styles.GPD_FONT_ITALIC.Equals(s & CWFont.Styles.GPD_FONT_ITALIC))
            {
                style += CwFont.ITALIC;
            }
            return style.Trim();
        }

    }
}
