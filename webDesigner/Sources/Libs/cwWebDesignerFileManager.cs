using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using log4net;
using Casewise.webDesigner.Nodes;

namespace Casewise.webDesigner.Libs
{
    public class cwWebDesignerFileManager
    {

        private static readonly ILog log = LogManager.GetLogger(typeof(cwWebDesignerFileManager));

        public const string cwWebDesignerFolder = "webdesigner";
        private cwWebDesignerTreeNodeSite _siteNode = null;


        public cwWebDesignerFileManager(cwWebDesignerTreeNodeSite siteNode)
        {
            _siteNode = siteNode;
        }


        public string getJSMode()
        {
            return _siteNode.getStringProperty(cwWebDesignerTreeNodeSite.CONFIG_JS_MODE);
        }

        /// <summary>
        /// Gets the site path from main.
        /// </summary>
        /// <returns></returns>
        public string getMediaPath()
        {
            return _siteNode.getStringProperty(cwWebDesignerTreeNodeSite.CONFIG_MEDIA_PATH);
        }

        public string getOutputFolder()
        {
            return _siteNode.getStringProperty(cwWebDesignerTreeNodeSite.CONFIG_SITE_PATH);
        }

        public string getJSONExtention()
        {
            return _siteNode.getStringProperty(cwWebDesignerTreeNodeSite.CONFIG_JSON_EXTENTION);
        }

        /// <summary>
        /// Checks if file exists or create it.
        /// </summary>
        /// <param name="outputFolder">The output folder.</param>
        /// <param name="filePath">The file path.</param>
        public static void checkIfFileExistsOrCreateIt(string outputFolder, string filePath)
        {
            if (!File.Exists(outputFolder + filePath)) File.Create(outputFolder + filePath);
        }

        public static void checkIfFileExistsOrCreateIt(string filePath)
        {
            if (!File.Exists(filePath)) File.Create(filePath);
        }

        /// <summary>
        /// Gets the site path.
        /// </summary>
        /// <returns></returns>
        public string addToSitePath(string _path)
        {
            return getOutputFolder() + "/" + _path;
        }

        /// <summary>
        /// Adds to web designer path.
        /// </summary>
        /// <param name="_path">The _path.</param>
        /// <returns></returns>
        public string addToWebDesignerPath(string _path)
        {
            return addToSitePath(cwWebDesignerFolder + "/" + _path);
        }



        /// <summary>
        /// Adds to handmade path relative.
        /// </summary>
        /// <param name="_path">The _path.</param>
        /// <returns></returns>
        public string addToHandmadePathRelative(string _path)
        {
            return cwWebDesignerFolder + "/handmade/" + _path;
        }

        /// <summary>
        /// Adds to generated path relative.
        /// </summary>
        /// <param name="_path">The _path.</param>
        /// <returns></returns>
        public string addToGeneratedPathRelative(string _path)
        {
            return cwWebDesignerFolder + "/generated/" + _path;
        }

        /// <summary>
        /// Gets the handmade path.
        /// </summary>
        /// <param name="_path">The _path.</param>
        /// <returns></returns>
        public string addToHandmadePath(string _path)
        {
            return addToWebDesignerPath("/handmade/" + _path);
        }

        /// <summary>
        /// Gets the path generated.
        /// </summary>
        /// <param name="_path">The _path.</param>
        /// <returns></returns>
        public string addToGeneratedPath(string _path)
        {
            return addToWebDesignerPath("/generated/" + _path);
        }

        /// <summary>
        /// Adds to layout path.
        /// </summary>
        /// <param name="_pageName">Name of the _page.</param>
        /// <param name="_layoutName">Name of the _layout.</param>
        /// <returns></returns>
        public string addToLayoutPath(string _pageName, string _layoutName)
        {
            return addToGeneratedPath(_pageName + "/layouts/" + _pageName + "_" + _layoutName + ".js");
        }

        /// <summary>
        /// Checks the web structure.
        /// </summary>
        /// <returns></returns>
        public bool checkWebStructure()
        {
            string wePath = getOutputFolder() + "/" + cwWebDesignerFolder;
            try
            {
                checkIfDirectoryExistsOrCreateIt(wePath, "");
                checkIfDirectoryExistsOrCreateIt(wePath, "/generated");
                checkIfDirectoryExistsOrCreateIt(wePath, "/handmade");
                checkIfDirectoryExistsOrCreateIt(getOutputFolder(), "/main");
                checkIfDirectoryExistsOrCreateIt(getOutputFolder(), "/i18n");
                checkIfDirectoryExistsOrCreateIt(getOutputFolder(), "/js");
                checkIfDirectoryExistsOrCreateIt(getOutputFolder(), "/js/casewise");
                checkIfDirectoryExistsOrCreateIt(getOutputFolder(), "/css");
                checkIfDirectoryExistsOrCreateIt(getOutputFolder(), "/css/casewise");
                checkIfDirectoryExistsOrCreateIt(getOutputFolder(), "/images");
                checkIfDirectoryExistsOrCreateIt(getOutputFolder(), "/images/print");
                checkIfDirectoryExistsOrCreateIt(getOutputFolder(), "/images/pictures");
                checkIfDirectoryExistsOrCreateIt(getOutputFolder(), "/images/treeview");
                checkIfFileExistsOrCreateIt(getOutputFolder(), "/css/main.css");
                checkIfFileExistsOrCreateIt(getOutputFolder(), "/css/main-ie-lt9.css");
                checkIfFileExistsOrCreateIt(getOutputFolder(), "/css/main-ie7.css");
            }
            catch (Exception e)
            {
                log.Error("Error while checking the structure of outputfolder;" + wePath);
                log.Error(e.Message.ToString() + ";" + e.ToString());
                return false;
            }
            return true;
        }
        /// <summary>
        /// Checks the web structure.
        /// </summary>
        /// <param name="_pageName">Name of the _page.</param>
        /// <returns></returns>
        public bool checkWebStructureForPage(string _pageName)
        {
            string wePath = getOutputFolder() + "/" + cwWebDesignerFolder;           
            checkIfDirectoryExistsOrCreateIt(wePath, "/generated/" + _pageName);
            checkIfDirectoryExistsOrCreateIt(wePath, "/handmade/" + _pageName);
            checkIfDirectoryExistsOrCreateIt(wePath, "/generated/" + _pageName + "/json");
            checkIfDirectoryExistsOrCreateIt(wePath, "/generated/" + _pageName + "/layouts");
            return true;
        }

        /// <summary>
        /// Checks if directory exists or create it.
        /// </summary>
        /// <param name="outputFolder">The output folder.</param>
        /// <param name="DirectoryPath">The directory path.</param>
        public static void checkIfDirectoryExistsOrCreateIt(string outputFolder, string DirectoryPath)
        {
            if (!Directory.Exists(outputFolder + DirectoryPath)) Directory.CreateDirectory(outputFolder + DirectoryPath);
        }
    }
}
