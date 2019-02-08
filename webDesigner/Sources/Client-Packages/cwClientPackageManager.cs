using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using Casewise.webDesigner.GUI;
using Casewise.webDesigner.Libs;
using Casewise.webDesigner.Nodes;

namespace Casewise.GraphAPI.Operations.Web
{
    class cwClientPackageManager
    {
        public StringBuilder ConcatFiles(string sourceFolder, List<string> files)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string file in files)
            {
                sb.Append("\n\n/************ " + file + " ************/\n\n");
                string content = File.ReadAllText(sourceFolder + file);
                sb.Append(content);
            }
            return sb;
        }

        private void packageCSSFiles(string sourceFolder)
        {
            string outputFile = AppDomain.CurrentDomain.BaseDirectory +"/casewise-webdesigner";

            List<string> cssFiles = getCSSFiles();
            StringBuilder sb = ConcatFiles(sourceFolder, cssFiles);

            File.WriteAllText(outputFile + "-concat.css", sb.ToString(), Encoding.UTF8);
        }

        public List<string> getCSSFiles()
        {
            List<string> cssFiles = new List<string>();
            //string currentDir = AppDomain.CurrentDomain.BaseDirectory + "/webDesigner/libs/";
            //string[] files = Directory.GetFiles(currentDir, "*.*", System.IO.SearchOption.AllDirectories);
            //foreach (string file in files)
            //{
            //    FileInfo info = new FileInfo(file);
            //    string extension = info.Extension.Replace(".", "");
            //    if (extension.Equals("css"))
            //    {
            //        cssFiles.Add(file.Substring(currentDir.Length));
            //    }
            //}

            cssFiles.Add("cwAPI/cwSearchEngine/cwSearchEngine.css");
            cssFiles.Add("cwAPI/cwPropertiesGroups.css");
            cssFiles.Add("cwAPI/cwDiagramManager.css");
            cssFiles.Add("cwAPI/cwTooltipManager.css");
            cssFiles.Add("cwAPI/cwEditProperties.css");
            cssFiles.Add("cwBehaviours/cwAccordion.css");
            cssFiles.Add("cwBehaviours/cwWorldMap.css");
            cssFiles.Add("cwBehaviours/cwTreeView.css");
            cssFiles.Add("cwAPI/cwPortalPlugin.css");

            cssFiles.AddRange(_layoutManager.getLayoutFiles("css"));
            return cssFiles;
        }

        public List<string> getJSExternalFiles()
        {
            List<string> jsFiles = new List<string>();
            jsFiles.Add("external/jquery/jquery.js");
            //jsFiles.Add("external/jquery/jquery.mobile.js");
            jsFiles.Add("external/jquery/jquery-ui.custom.js");
            jsFiles.Add("external/jquery/jquery.cookie.js");
            jsFiles.Add("external/jquery/jquery.i18n.properties-min.js");
            jsFiles.Add("external/jquery/jquery.mousewheel.js");
            jsFiles.Add("external/jquery/jquery.tooltip.min.js");
            jsFiles.Add("external/jquery/jquery.treeview.js");
            jsFiles.Add("external/kendoui.js");
            jsFiles.Add("external/json-ie7.js");
            jsFiles.Add("external/date.js");
            jsFiles.Add("external/date.format.js");
            jsFiles.Add("external/underscore.js");
            jsFiles.Add("external/excanvas.compiled.js");
            return jsFiles;
        }

        public List<string> getJSFiles()
        {
            List<string> jsFiles = new List<string>();
            jsFiles.Add("cwAPI/cwJSAPI.js");
            jsFiles.Add("cwAPI/cwCustomerSiteActions.js");
            jsFiles.Add("cwAPI/cwSiteActions.js");
            jsFiles.Add("cwAPI/cwLoadUniquePage.js");
            jsFiles.Add("cwAPI/cwTabManager.js");
            jsFiles.Add("cwAPI/cwPropertiesGroups.js");
            jsFiles.Add("cwAPI/cwDiagramManager.js");
            jsFiles.Add("cwAPI/cwTooltipManager.js");
            jsFiles.Add("cwAPI/cwEditProperties.js");
            jsFiles.Add("cwAPI/jquery.cwDiagramManager.js");
            jsFiles.Add("cwAPI/cwSearchEngine/cwSearchEngine.js");
            jsFiles.Add("cwAPI/cwPortalPlugin.js");

            jsFiles.Add("cwDiagram/CanvasCamera/CanvasCamera.js");
            jsFiles.Add("cwDiagram/CanvasCamera/CanvasCameraPoint.js");
            jsFiles.Add("cwDiagram/DiagramCanvas/DiagramCanvas.js");
            jsFiles.Add("cwDiagram/DiagramCanvas/DiagramCanvasNavigationBar.js");
            jsFiles.Add("cwDiagram/DiagramCanvas/DiagramCanvasNavigationExploded.js");
            jsFiles.Add("cwDiagram/DiagramCanvas/DiagramCanvasTooltip.js");
            jsFiles.Add("cwDiagram/DiagramContext.js");
            jsFiles.Add("cwDiagram/DiagramDesigner/DiagramDesigner.js");
            jsFiles.Add("cwDiagram/DiagramDesigner/DiagramDesignerAPI.js");
            jsFiles.Add("cwDiagram/DiagramDesigner/DiagramDesignerIncludeType.js");
            jsFiles.Add("cwDiagram/DiagramDesigner/DiagramDesignerVerticalType.js");
            jsFiles.Add("cwDiagram/DiagramTextZone.js");
            jsFiles.Add("cwDiagram/DrawEngine.js");
            jsFiles.Add("cwDiagram/DiagramJoiner.js");
            jsFiles.Add("cwDiagram/DiagramShape.js");
            
            jsFiles.Add("cwBehaviours/cwAccordion.js");
            jsFiles.Add("cwBehaviours/cwTreeView.js");
            jsFiles.Add("cwBehaviours/cwWorldMap.js");
            jsFiles.Add("cwBehaviours/cwWorldMap.JomCom.js");

            jsFiles.AddRange(_layoutManager.getLayoutFiles("js"));

            return jsFiles;
        }

        private void packageExternalJavascriptFiles(string outputFileName, string sourceFolder)
        {
            List<string> jsFiles = getJSExternalFiles();
            StringBuilder sb = ConcatFiles(sourceFolder, jsFiles);
            string path = AppDomain.CurrentDomain.BaseDirectory + "/" + outputFileName + "-concat.js";
            File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
        }

        private void packageJavaScriptFiles(cwWebDesignerGUI GUI, string sourceFolder)
        {
            //if (!"debug".Equals(this.mode))
            {
                string outputFile = AppDomain.CurrentDomain.BaseDirectory + "/casewise-webdesigner";

                List<string> jsFiles = getJSFiles();
                StringBuilder sb = ConcatFiles(sourceFolder, jsFiles);

                File.WriteAllText(outputFile + "-concat.js", sb.ToString(), Encoding.UTF8);
                try
                {
                    if ("min".Equals(this.mode))
                    {
                        GoogleClosure gc = new GoogleClosure();
                        string scriptPretty = gc.Compress(sb, false);
                        File.WriteAllText(outputFile + "-pretty.js", scriptPretty, Encoding.UTF8);

                        string scriptMin = gc.Compress(sb, true);
                        File.WriteAllText(outputFile + "-min.js", scriptMin, Encoding.UTF8);
                    }
                }
                catch (Exception e)
                {
                    GUI.appendInfo(e.ToString());
                }
            }
        }

        private cwWebDesignerLayoutManager _layoutManager = new cwWebDesignerLayoutManager(AppDomain.CurrentDomain.BaseDirectory + "/webDesigner/layouts");
        private cwWebDesignerGUI GUI = null;
        private string mode;
        public cwClientPackageManager(cwWebDesignerGUI GUI)
        {
            this.GUI = GUI;
            cwWebDesignerTreeNodeSite siteNode = this.GUI.rootNode as cwWebDesignerTreeNodeSite;
            this.mode = siteNode.JSMode;
        }

        public void packageFiles()
        {
            string applicationLocation = AppDomain.CurrentDomain.BaseDirectory;
            string sourceFolder = applicationLocation + "/webDesigner/libs/";
            packageJavaScriptFiles(GUI, sourceFolder);
            packageExternalJavascriptFiles("casewise-external", sourceFolder);
            packageCSSFiles(sourceFolder);
        }

        public void appendFilesToOutputStream(cwWebDesignerWriter html_file, string sourceDir, List<string> files, string type)
        {
            foreach (string file in files)
            {
                if ("js".Equals(type))
                {
                    html_file.writeToFile("<script type='text/javascript' src='" + sourceDir + file + "'></script>");
                }
                else if ("css".Equals(type))
                {
                    html_file.writeToFile("<link type='text/css' rel='stylesheet' media='all' href='" + sourceDir + file + "' />");
                }
            }

        }
    }
}
