using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Casewise.webDesigner.GUI;
using Casewise.webDesigner.Nodes;
using Casewise.GraphAPI.Operations.Web;
using log4net;
using System.Net;

namespace Casewise.webDesigner.Libs
{
    public class cwWebDesignerGlobalFiles
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(cwWebDesignerGlobalFiles));

        private webDesignerOperation _webDesigner = null;
        private cwWebDesignerFileManager _fileManager = null;
        private cwWebDesignerTreeNodeSite _siteNode = null;
        private cwClientPackageManager _clientPackageManager = null;
        private cwWebDesignerLayoutManager layoutManager = new cwWebDesignerLayoutManager(AppDomain.CurrentDomain.BaseDirectory + "/webDesigner/layouts");
        /// <summary>
        /// Initializes a new instance of the <see cref="cwWebDesignerGlobalFiles"/> class.
        /// </summary>
        /// <param name="webDesigner">The web designer.</param>
        /// <param name="fileManager">The file manager.</param>
        /// <param name="siteNode">The site node.</param>
        public cwWebDesignerGlobalFiles(webDesignerOperation webDesigner, cwWebDesignerFileManager fileManager, cwWebDesignerTreeNodeSite siteNode)
        {
            _webDesigner = webDesigner;
            _fileManager = fileManager;
            _siteNode = siteNode;
            _clientPackageManager = new cwClientPackageManager(_siteNode.getGUI<cwWebDesignerGUI>());

        }



        private void deployFile(string fileName, string filePathFromExe, string typeFolder)
        {
            string currentDir = AppDomain.CurrentDomain.BaseDirectory + "/";
            string filePath = currentDir + filePathFromExe + fileName;
            if (File.Exists(filePath))
            {
                File.Copy(filePath, _webDesigner.fileManager.getOutputFolder() + typeFolder + "/casewise/" + fileName, true);
            }
            else
            {
                log.Info("Unable to find file " + filePath + " to deploy it");
            }
        }

        /// <summary>
        /// Deploys the casewise file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="typeFolder">The type folder.</param>
        private void deployCasewiseFile(string fileName, string typeFolder)
        {
            string currentDir = AppDomain.CurrentDomain.BaseDirectory + "/";
            string filePath = currentDir + fileName;
            if (File.Exists(filePath))
            {
                File.Copy(filePath, _webDesigner.fileManager.getOutputFolder() + typeFolder + "/casewise/" + fileName, true);
            }
            else
            {
                log.Info("Unable to find file " + filePath + " to deploy it");
            }
        }

        /// <summary>
        /// Deploys the casewise JS and CSS files.
        /// </summary>
        private void deployCasewiseJSAndCSSFiles()
        {
            deployCasewiseFile("casewise-webdesigner-concat.js", "js");
            deployCasewiseFile("casewise-webdesigner-pretty.js", "js");
            deployCasewiseFile("casewise-webdesigner-min.js", "js");
            deployCasewiseFile("casewise-external-concat.js", "js");
            deployCasewiseFile("casewise-webdesigner-concat.css", "css");

            deployFile("cw-portal-plugin.js", "webDesigner/js/", "js");
            string currentDir = AppDomain.CurrentDomain.BaseDirectory + "/";
            copyFileToOutputFolder(currentDir + "webDesigner/", "images/_loading.gif");
            copyFileToOutputFolder(currentDir + "webDesigner/", "images/pictures/picture-1.png");
            copyFileToOutputFolder(currentDir + "webDesigner/", "images/diagramcursor.cur");
            copyFileToOutputFolder(currentDir + "webDesigner/", "images/casewise.ico");
            copyFileToOutputFolder(currentDir + "webDesigner/", "images/treeview/treeview-default-line.gif");
            copyFileToOutputFolder(currentDir + "webDesigner/", "images/treeview/treeview-default.gif");
            copyFileToOutputFolder(currentDir + "webDesigner/", "i18n/translations.txt");
            copyFileToOutputFolder(currentDir + "webDesigner/", "i18n/translations_en.txt");
            copyFileToOutputFolder(currentDir + "webDesigner/", "i18n/translations_fr.txt");
            copyFolderToOutputFolder(currentDir + "webDesigner/", "css/");
        }

        /// <summary>
        /// Copies the folder to output folder.
        /// </summary>
        /// <param name="sourceDir">The source dir.</param>
        /// <param name="name">The name.</param>
        private void copyFolderToOutputFolder(string sourceDir, string name)
        {
            string[] _files = Directory.GetFiles(sourceDir + name, "*.*", System.IO.SearchOption.AllDirectories);
            string[] _directories = Directory.GetDirectories(sourceDir + name, "*", System.IO.SearchOption.AllDirectories);
            string output = _webDesigner.fileManager.getOutputFolder();
            foreach (string dir in _directories)
            {
                string subDir = dir.Substring(sourceDir.Length);
                if (!Directory.Exists(output + subDir))
                {
                    Directory.CreateDirectory(output + subDir);
                }
            }
            foreach (string file_name in _files)
            {
                string _file = file_name.Substring(sourceDir.Length);
                if (!File.Exists(output + _file))
                {
                    File.Copy(file_name, output + _file);
                }
            }
        }

        /// <summary>
        /// Copies the file to output folder.
        /// </summary>
        /// <param name="sourceDir">The source dir.</param>
        /// <param name="name">The name.</param>
        private void copyFileToOutputFolder(string sourceDir, string name)
        {
            File.Copy(sourceDir + name, _webDesigner.fileManager.getOutputFolder() + name, true);
        }

        /// <summary>
        /// Creates the global files.
        /// </summary>
        public void createGlobalFiles()
        {
            createFileMainJS();
            createFileGeneratedMainJS();
            createIndexControlerPage();
            createFileJSONPages();

            _clientPackageManager.packageFiles();

            deployCasewiseJSAndCSSFiles();
        }



        /// <summary>
        /// Creates the JS main file.
        /// </summary>
        public void createFileMainJS()
        {
            string filePath = _fileManager.addToSitePath("js/main.js");
            if (!File.Exists(filePath))
            {
                StreamWriter jsFile = cwWebDesignerTools.getStreamWriterErase(filePath);
                //jsFile.WriteLine("var MAIL_ADDRESS_FOR_DIAGRAM_FEEDBACK = 'pouya.mohtacham@casewise.com';");
                jsFile.WriteLine("/*global cwCustomerSiteActions : true, cwAPI:true, cwConfigs:true */");
                jsFile.WriteLine("cwCustomerSiteActions.doActionsForSingle_Custom = function(rootNode) {}");
                jsFile.WriteLine("cwCustomerSiteActions.doActionsForIndex_Custom = function(searching) {}");
                jsFile.WriteLine("cwCustomerSiteActions.doActionsForAll_Custom = function() {}");
                jsFile.Close();
            }
        }


        /// <summary>
        /// Gets the IP.
        /// </summary>
        /// <returns></returns>
        public string getIP()
        {

            IPAddress[] addr = Dns.GetHostAddresses(System.Environment.MachineName);

            for (int i = 0; i < addr.Length; i++)
            {
                string ip = addr[i].ToString();
                if (ip.StartsWith("192"))
                {
                    return ip;
                }
            }
            return "127.0.0.1";
        }
        /// <summary>
        /// Includes the common headers.
        /// </summary>
        /// <param name="html_file">The html_file.</param>
        /// <param name="fileManager">The file manager.</param>
        public void includeCommonHeaders(cwWebDesignerWriter html_file, cwWebDesignerFileManager fileManager)
        {
            html_file.writeToFile("<link rel='shortcut icon' href='" + fileManager.getMediaPath() + "images/favicon.ico' type='image/x-icon' />");
            html_file.writeToFile("<link type='text/css' rel='stylesheet' media='all' href='" + fileManager.getMediaPath() + "css/theme/jquery-ui.custom.css' />");
            html_file.writeToFile("<link rel='apple-touch-icon' href='../images/apple-touch-icon.png'/>");


            string jsType = fileManager.getJSMode();
            switch (jsType)
            {
                case "debug":
                    //string url = "http://localhost/Libs-Debug/";
                    string url = "http://localhost/WebUtility/Libs-Debug/";
                    _clientPackageManager.appendFilesToOutputStream(html_file, url, _clientPackageManager.getJSExternalFiles(), "js");
                    _clientPackageManager.appendFilesToOutputStream(html_file, url, _clientPackageManager.getJSFiles(), "js");
                    _clientPackageManager.appendFilesToOutputStream(html_file, url, _clientPackageManager.getCSSFiles(), "css");
                    break;
                default:
                    html_file.writeToFile("<script type='text/javascript' src='" + fileManager.getMediaPath() + "js/casewise/casewise-external-concat.js'></script>");
                    html_file.writeToFile("<script type='text/javascript' src='" + fileManager.getMediaPath() + "js/casewise/casewise-webdesigner-" + jsType + ".js'></script>");
                    html_file.writeToFile("<link type='text/css' rel='stylesheet' media='all' href='" + fileManager.getMediaPath() + "css/casewise/casewise-webdesigner-concat.css' />");
                    break;
            }

            // CUSTOM MAIN JS
            html_file.writeToFile("<script type='text/javascript' src='" + fileManager.getMediaPath() + fileManager.addToGeneratedPathRelative("main-generated.js") + "'></script>");
            html_file.writeToFile("<script type='text/javascript' src='" + fileManager.getMediaPath() + "js/main.js'></script>");
        }
        /// <summary>
        /// Creates the file JSON pages.
        /// </summary>
        public void createFileJSONPages()
        {
            cwWebDesignerTreeNodePagesIndex _rootPagesIndex = _siteNode.getFirstChildNode<cwWebDesignerTreeNodePagesIndex>();
            cwWebDesignerTreeNodePagesSingle _rootPagesSingle = _siteNode.getFirstChildNode<cwWebDesignerTreeNodePagesSingle>();
            string pagesFileName = _fileManager.addToGeneratedPath("pages." + _siteNode.getJSONExtention());
            cwWebDesignerWriter pagesFile = new cwWebDesignerWriter(pagesFileName);

            pagesFile.writeToFileSameLine("{");
            pagesFile.writeToFileSameLine("\"index\":");
            if (_rootPagesIndex.hasAtLeastOnChildNode<cwWebDesignerTreeNodePage>())
            {
                pagesFile.writeToFileSameLine("[");
                int i = 0;
                List<cwWebDesignerTreeNodePage> indexPages = _rootPagesIndex.getChildrenNodes<cwWebDesignerTreeNodePage>();
                foreach (cwWebDesignerTreeNodePage page in indexPages)
                {
                    pagesFile.writeToFileSameLine("{\"cwview\":\"" + page.getName() + "\", \"link\":\"" + _webDesigner.jsonTools.getLinkForIndex(page.getName
                        ()) + "\", \"name\":\"" + page.getStringProperty(cwWebDesignerTreeNodePage.CONFIG_PAGE_DISPLAY_NAME) + "\"}");
                    if (i < indexPages.Count() - 1)
                    {
                        pagesFile.writeToFileSameLine(",");
                    }
                    i++;
                }
                pagesFile.writeToFileSameLine("]");
            }
            else
            {
                pagesFile.writeToFileSameLine("[]");
            }
            pagesFile.writeToFileSameLine("}");
            pagesFile.close();
        }

        public void createFileGeneratedMainJS()
        {
            string oFile = _fileManager.addToGeneratedPath("main-generated.js");
            cwWebDesignerWriter generatedMainJS_file = new cwWebDesignerWriter(oFile);

            generatedMainJS_file.writeToFile("cwConfigs = {};");
            generatedMainJS_file.writeToFile("cwConfigs.SITE_MEDIA_PATH = '" + _fileManager.getMediaPath() + "';");
            generatedMainJS_file.writeToFile("cwConfigs.JSON_EXTENTION = '" + _siteNode.getJSONExtention() + "';");
            generatedMainJS_file.writeToFile("cwConfigs.SITE_LINK_EXTENTION = '" + _siteNode.getStringProperty(cwWebDesignerTreeNodeSite.CONFIG_SITE_LINK_MIMETYPE) + "';");
            //generatedMainJS_file.writeToFile("cwConfigs.RUN_IN_PORTAL = " + _siteNode.isRunningInPortal.ToString().ToLower() + ";");
            generatedMainJS_file.writeToFile("cwConfigs.MODEL_FILENAME = '" + _siteNode.Model.FileName.ToString() + "';");

            string serverURL = "";
            try
            {
                //serverURL = _siteNode.getStringProperty(cwWebDesignerTreeNodeSite.CONFIG_WEBDESIGNER_SERVER_URL);
            }
            catch (Exception)
            {
            }

            if (!"".Equals(serverURL))
            {
                //generatedMainJS_file.writeToFile("cwConfigs.WEBDESIGNER_SERVER_URL = '" + serverURL + "';");
            }

            generatedMainJS_file.writeToFileI("jQuery(function(){");
            string language = _siteNode.getStringProperty(cwWebDesignerTreeNodeSite.CONFIG_LANGUAGE_CHOOSED);
            generatedMainJS_file.writeToFile("cwAPI.setupLanguage('" + language + "');");
            generatedMainJS_file.writeToFileD("});");
            generatedMainJS_file.close();
        }

        /// <summary>
        /// Creates the custom CSS file.
        /// </summary>
        /// <param name="_pageName">Name of the _page.</param>
        public void createFile_CSS(string _pageName)
        {
            string cssFilePath = _fileManager.addToHandmadePath(_pageName + "/" + _pageName + ".css");
            if (!File.Exists(cssFilePath))
            {
                StreamWriter customCSS_file = cwWebDesignerTools.getStreamWriterErase(cssFilePath);
                customCSS_file.Close();
            }
            cwWebDesignerFileManager.checkIfFileExistsOrCreateIt(_fileManager.addToHandmadePath(_pageName + "/" + _pageName + ".css"));
            cwWebDesignerFileManager.checkIfFileExistsOrCreateIt(_fileManager.addToHandmadePath(_pageName + "/" + _pageName + ".ie7.css"));
            cwWebDesignerFileManager.checkIfFileExistsOrCreateIt(_fileManager.addToHandmadePath(_pageName + "/" + _pageName + ".ie8.css"));
            cwWebDesignerFileManager.checkIfFileExistsOrCreateIt(_fileManager.addToHandmadePath(_pageName + "/" + _pageName + ".lt-ie9.css"));
        }


        /// <summary>
        /// Creates the index controler page.
        /// </summary>
        public void createIndexControlerPage()
        {
            cwWebDesignerWriter html_file = new cwWebDesignerWriter(_fileManager.addToSitePath("/main/index.html"));
            html_file.writeToFile("<!DOCTYPE html>");
            html_file.writeToFileI("<html lang='en'>");
            html_file.writeToFileI("<head>");
            html_file.writeToFile("<meta http-equiv='Content-Type' content='text/html; charset=UTF-8'/>");
            html_file.writeToFile("<meta name='viewport' content='initial-scale=1.0, maximum-scale=1.0, user-scalable=0.0'/>");
            html_file.writeToFile("<meta name='apple-mobile-web-app-capable' content='yes'/>");
            // COMMON LIBS
            includeCommonHeaders(html_file, _fileManager);
            //layoutManager.appendLayoutHTMLHeaderToOutputStream(html_file, _fileManager);

            html_file.writeToFile("<link type='text/css' rel='stylesheet' media='all' href='" + _fileManager.getMediaPath() + "css/main.css' />");
            html_file.writeToFile("<!--[if IE 7]>  <link type='text/css' rel='stylesheet' media='all' href='" + _fileManager.getMediaPath() + "css/main-ie7.css'/> <![endif]--> ");
            html_file.writeToFile("<!--[if lt IE 9]>  <link type='text/css' rel='stylesheet' media='all' href='" + _fileManager.getMediaPath() + "css/main-ie-lt9.css'/> <![endif]--> ");
            //html_file.writeToFile("<script type='text/javascript' src='" + _fileManager.getLibPath() + "templates/loadUniquePage.js'></script>");
            html_file.writeToFileI("<script type='text/javascript'>");
            html_file.writeToFileI("jQuery(function(){");
            html_file.writeToFile("cwAPI.loadTopMenu();");
            html_file.writeToFile("cwAPI.loadIndexPage();");
            html_file.writeToFileD("});");
            html_file.writeToFileD("</script>");
            html_file.writeToFileD("</head>");
            html_file.writeToFileI("<body>");
            html_file.writeToFile("<div id='top_of_page'></div>");
            html_file.writeToFile("<img class='cwloading' src='../images/_loading.gif' alt='Loading...'>");
            html_file.writeToFileD("</body>");
            html_file.writeToFileD("</html>");
            html_file.close();
        }

        /// <summary>
        /// Creates the helper page.
        /// </summary>
        /// <param name="_page">The _page.</param>
        public void createFile_Helper(cwWebDesignerTreeNodePage _page)
        {
            string _pageName = _page.getName();
            string oFileHelper = _fileManager.addToHandmadePath(_pageName + "/_" + _pageName + ".helper.json");
            StreamWriter json_fileHelper = cwWebDesignerTools.getStreamWriterErase(oFileHelper);
            cwWebDesignerTools.createJSONDocumentation(_page, json_fileHelper, _pageName);
            json_fileHelper.Close();
        }
    }
}
