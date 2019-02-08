using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Casewise.webDesigner.Libs
{
    public class cwWebDesignerWriter
    {
        private StreamWriter sw = null;
        private string fileTab = "";
        private const string applyTab = "  ";

        public cwWebDesignerWriter(string _path)
        {               
            Encoding myUTF8 = new UTF8Encoding(false);
            sw = new StreamWriter(_path, false, myUTF8);
        }

        /// <summary>
        /// Increases the tab.
        /// </summary>
        public void increaseTab()
        {
            fileTab += applyTab;
        }

        /// <summary>
        /// Adds the CSS to head.
        /// </summary>
        /// <param name="_path">The _path.</param>
        public void addCSSToHead(string _path)
        {
            writeToFile("<link type='text/css' rel='stylesheet' media='all' href='" + _path + "' />");
        }

        /// <summary>
        /// Adds the JS to head.
        /// </summary>
        /// <param name="_path">The _path.</param>
        public void addJSToHead(string _path)
        {
            writeToFile("<script type='text/javascript' src='" + _path + "'></script>");
        }

        /// <summary>
        /// Decreases the tab.
        /// </summary>
        public void decreaseTab()
        {
            if (fileTab.Length >= applyTab.Length)
            {
                fileTab = fileTab.Substring(0, fileTab.Length - applyTab.Length);
            }
            
        }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public void close()
        {
            sw.Close();
        }

        /// <summary>
        /// Writes the in output.
        /// </summary>
        /// <param name="_text">The _text.</param>
        public void writeInOutput(string _text)
        {
            sw.WriteLine(fileTab + "output.push(\"" + _text + "\");");
        }

        /// <summary>
        /// Writes the in output and Increase Tab
        /// </summary>
        /// <param name="_text">The _text.</param>
        public void writeInOutputI(string _text)
        {
            writeInOutput(_text);
            increaseTab();
        }

        /// <summary>
        /// Writes the in output and Decrease Tab
        /// </summary>
        /// <param name="_text">The _text.</param>
        public void writeInOutputD(string _text)
        {
            decreaseTab();
            writeInOutput(_text);            
        }
        
        /// <summary>
        /// Writes to file.
        /// </summary>
        /// <param name="_text">The _text.</param>
        public void writeToFile(string _text)
        {
            sw.WriteLine(fileTab + _text);
        }

        /// <summary>
        /// Writes to file same line.
        /// </summary>
        /// <param name="_text">The _text.</param>
        public void writeToFileSameLine(string _text)
        {
            sw.Write(fileTab + _text);
        }

        /// <summary>
        /// Writes to file and Increase Tab
        /// </summary>
        /// <param name="_text">The _text.</param>
        public void writeToFileI(string _text)
        {
            writeToFile(_text);
            increaseTab();
        }

        /// <summary>
        /// Writes to file and Decrease Tab
        /// </summary>
        /// <param name="_text">The _text.</param>
        public void writeToFileD(string _text)
        {
            decreaseTab();
            writeToFile(_text);            
        }

        

    }
}
