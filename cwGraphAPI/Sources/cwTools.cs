using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Collections;
using Casewise.GraphAPI;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using Casewise.GraphAPI.API;
using Casewise.GraphAPI.ProgramManager;
using Microsoft.Win32;
using log4net;

namespace Casewise.GraphAPI
{
    /// <summary>
    /// Tool methods to generate reports or do special operation on the repository
    /// </summary>
    public class cwTools
    {
       
        /// <summary>
        /// errorColor
        /// </summary>
        public static Color errorColor = Color.LightCoral;

        /// <summary>
        /// log
        /// </summary>
        public static readonly ILog log = LogManager.GetLogger(typeof(cwTools));

        /// <summary>
        /// Gets the casewise bin folder.
        /// </summary>
        public static string casewiseBinFolder
        {
            get
            {
                string casewiseBinFolder = (string)Registry.GetValue(cwProgramManager.CM_REGISTERY_KEY + "Directories", "Progs", @"C:\Program Files\Casewise\CM10\BIN");
                return casewiseBinFolder;
            }
        }

        /// <summary>
        /// Gets the casewise default connection.
        /// </summary>
        public static string casewiseDefaultConnection
        {
            get
            {
                string casewiseBinFolder = (string)Registry.GetValue(cwProgramManager.CM_REGISTERY_KEY + "DatabaseConnections", "Default", @"Stand-alone");
                return casewiseBinFolder;
            }
        }

        /// <summary>
        /// transform a string to a correct filename
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string stringToFile(string s)
        {
            string output = s.Replace("/", "-");
            output = output.Replace("&", "-");
            output = output.Replace("?", "-");
            output = output.Replace(":", "-");
            output = output.Replace("*", "-");
            output = output.Replace("\"", "-");
            output = output.Replace("<", "-");
            output = output.Replace(">", "-");
            output = output.Replace("|", "-");
            return output;
        }

        /// <summary>
        /// Updates the query string with insert properties list.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="_properties">The _properties.</param>
        /// <param name="IDValueName">Name of the ID value.</param>
        public static void updateQueryStringWithInsertPropertiesList(StringBuilder query, string[] _properties, string IDValueName)
        {
            for (int i = 0; i < _properties.Length; ++i)
            {
                if (0.Equals(i))
                {
                    // IDValueName should be "@@IDENTITY" if it's a auto increment value
                    query.Append(IDValueName);
                }
                else
                {
                    query.Append("@" + _properties[i]);
                }
                if (i < _properties.Length - 1)
                {
                    query.Append(",");
                }
            }
        }



        /// <summary>
        /// Strings to string separatedby.
        /// </summary>
        /// <param name="separator">The separator.</param>
        /// <param name="_ids">The _ids.</param>
        /// <param name="putSimpleQuoteAround">if set to <c>true</c> [put simple quote around].</param>
        /// <returns></returns>
        public static string stringToStringSeparatedby(string separator, List<string> _ids, bool putSimpleQuoteAround)
        {
            string result = "";
            for (int i = 0; i < _ids.Count; i++)
            {
                if (putSimpleQuoteAround)
                {
                    result += "'" + _ids[i].ToString() + "'";
                }
                else
                {
                    result += _ids[i].ToString();
                }

                if (i < _ids.Count - 1)
                {
                    result += separator;
                }
            }
            return result;
        }

        /// <summary>
        /// Escapes the chars.
        /// </summary>
        /// <param name="_element">The _element.</param>
        /// <returns></returns>
        public static string escapeChars(string _element)
        {
            string res = _element.Replace("\\", "\\\\");
            //res = res.Replace("'", "\\'");
            res = res.Replace("\"", "\\\"");
            res = res.Replace("”", "\\\"");
            res = res.Replace("\r\n", "<br/>");
            res = res.Replace("\n", "<br/>");
            res = res.Replace(Convert.ToChar(01), ' ');
            res = res.Replace(Convert.ToChar(02), ' ');
            res = res.Replace("Â§", "");
            //res = res.Replace("\t", "");
            return res;
        }

        /// <summary>
        /// Strings to ID.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns></returns>
        public static string stringToID(string s)
        {
            string output = s.ToLower();
            output = output.Replace("/", "");
            output = output.Replace("&", "");
            output = output.Replace("?", "");
            output = output.Replace(":", "");
            output = output.Replace("*", "");
            output = output.Replace("\"", "");
            output = output.Replace("<", "");
            output = output.Replace(">", "");
            output = output.Replace("|", "");
            output = output.Replace("'", "");
            output = output.Replace(" ", "_");
            output = output.Replace("é", "e");
            output = output.Replace("è", "e");
            output = output.Replace("ë", "e");
            output = output.Replace("ê", "e");
            output = output.Replace("ù", "u");
            output = output.Replace("ô", "o");
            output = output.Replace("î", "i");
            output = output.Replace("ç", "c");
            output = output.Replace("à", "a");
            output = output.Replace("-", "_");
            return output;
        }

        /// <summary>
        /// Ids to string separatedby.
        /// </summary>
        /// <param name="separator">The separator.</param>
        /// <param name="_ids">The _ids.</param>
        /// <returns></returns>
        public static string idToStringSeparatedby(string separator, List<int> _ids)
        {
            string result = "";
            for (int i = 0; i < _ids.Count; i++)
            {
                result += _ids[i];
                if (i < _ids.Count - 1)
                {
                    result += separator + " ";
                }
            }
            return result;
        }


        /// <summary>
        /// Crops the specified BMP.
        /// </summary>
        /// <param name="bmp">The BMP.</param>
        /// <returns></returns>
        public static Bitmap RemoveSurroundingWhitespaceFromImage(Bitmap bmp)
        {
            int w = bmp.Width;
            int h = bmp.Height;

            Func<int, bool> allWhiteRow = row =>
            {
                for (int i = 0; i < w; ++i)
                    if (bmp.GetPixel(i, row).R != 255)
                        return false;
                return true;
            };

            Func<int, bool> allWhiteColumn = col =>
            {
                for (int i = 0; i < h; ++i)
                    if (bmp.GetPixel(col, i).R != 255)
                        return false;
                return true;
            };

            int topmost = 0;
            for (int row = 0; row < h; ++row)
            {
                if (allWhiteRow(row))
                    topmost = row;
                else break;
            }

            int bottommost = 0;
            for (int row = h - 1; row >= 0; --row)
            {
                if (allWhiteRow(row))
                    bottommost = row;
                else break;
            }

            int leftmost = 0, rightmost = 0;
            for (int col = 0; col < w; ++col)
            {
                if (allWhiteColumn(col))
                    leftmost = col;
                else
                    break;
            }

            for (int col = w - 1; col >= 0; --col)
            {
                if (allWhiteColumn(col))
                    rightmost = col;
                else
                    break;
            }

            if (rightmost == 0) rightmost = w; // As reached left 
            if (bottommost == 0) bottommost = h; // As reached top. 

            int croppedWidth = rightmost - leftmost;
            int croppedHeight = bottommost - topmost;

            if (croppedWidth == 0) // No border on left or right 
            {
                leftmost = 0;
                croppedWidth = w;
            }

            if (croppedHeight == 0) // No border on top or bottom 
            {
                topmost = 0;
                croppedHeight = h;
            }

            try
            {
                var target = new Bitmap(croppedWidth, croppedHeight);
                using (Graphics g = Graphics.FromImage(target))
                {
                    g.DrawImage(bmp,
                      new RectangleF(0, 0, croppedWidth, croppedHeight),
                      new RectangleF(leftmost, topmost, croppedWidth, croppedHeight),
                      GraphicsUnit.Pixel);
                }
                return target;
            }
            catch (Exception ex)
            {
                throw new Exception(
                  string.Format("Values are topmost={0} btm={1} left={2} right={3} croppedWidth={4} croppedHeight={5}", topmost, bottommost, leftmost, rightmost, croppedWidth, croppedHeight),
                  ex);
            }
        } 


    }
}
