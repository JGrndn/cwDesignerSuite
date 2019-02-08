using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Casewise.GraphAPI.Exceptions;
using Casewise.webDesigner.Libs;
using Casewise.GraphAPI.PSF;
using System.Web.Script.Serialization;
using Casewise.webDesigner.Nodes;
using Casewise.webDesigner.GUI;
using System.Reflection;

namespace Casewise.webDesigner.Libs
{
    /// <summary>
    /// Load the json layouts
    /// </summary>
    public class cwWebDesignerLayoutManager
    {
       // public static string LAYOUT_DIRECTORY_PATH = AppDomain.CurrentDomain.BaseDirectory+ "/webDesigner/layouts";
        public Dictionary<string, Dictionary<string, string>> layoutsProperties = new Dictionary<string, Dictionary<string, string>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="cwWebDesignerLayoutManager"/> class.
        /// </summary>
        /// <param name="layoutsDirectoryPath">The layouts directory path.</param>
        public cwWebDesignerLayoutManager(string layoutsDirectoryPath)
        {
            //string location = Assembly.GetExecutingAssembly().Location;
            if (Directory.Exists(layoutsDirectoryPath))
            {
                loadLayouts(layoutsDirectoryPath);
            }
            
        }

        public cwWebDesignerTreeNodeLayout getLayoutNodeByKeyName(string keyName, cwPSFTreeNodeObjectNode parentNode)
        {
            if (layoutsProperties.ContainsKey(keyName))
            {
                cwWebDesignerTreeNodeLayout layout = new cwWebDesignerTreeNodeLayout(parentNode.getGUI<cwWebDesignerGUI>(), parentNode, layoutsProperties[keyName]);
                return layout;
            }
            return null;
        }

        /// <summary>
        /// Loads the layouts.
        /// </summary>
        /// <param name="layoutsDirectoryPath">The layouts directory path.</param>
        private void loadLayouts(string layoutsDirectoryPath)
        {
            foreach (string filePath in Directory.GetFiles(layoutsDirectoryPath, "*.config.json"))
            {
                loadLayoutFromPath(filePath);
            }
        }
        /// <summary>
        /// Loads the layout from path.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        private void loadLayoutFromPath(string filePath)
        {
            StreamReader sr = new StreamReader(filePath);
            string content = sr.ReadToEnd();
            sr.Close();

            JavaScriptSerializer js = new JavaScriptSerializer();
            Dictionary<string, string> dic = js.Deserialize<Dictionary<string, string>>(content);
            dic["file-path"] = filePath;
            if (!dic.ContainsKey("name"))
            {
                throw new cwExceptionFatal("Layout Configuration should have the name attribute set");
            }
            if (!layoutsProperties.ContainsKey(dic["name"]))
            {
                layoutsProperties[dic["name"]] = dic;

            }
            else
            {
                throw new cwExceptionWarning("Layout Configuration should have the name attribute set");
            }
        }


        public List<string> getLayoutFiles(string extention)
        {
            List<string> layoutJSFiles = new List<string>();
            foreach (Dictionary<string, string> layoutProperties in layoutsProperties.Values)
            {
                layoutJSFiles.Add("cwLayouts/layout." + layoutProperties["name"] + "." + extention);
            }
            return layoutJSFiles;
        }



        
    }
}
