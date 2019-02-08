using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Casewise.GraphAPI.API;
using System.IO;
using Casewise.GraphAPI.Exceptions;
using Casewise.GraphAPI.PSF;
using Casewise.webDesigner.GUI;
using System.Web.Script.Serialization;

using Casewise.webDesigner.Nodes;


namespace Casewise.webDesigner.Libs
{
    internal class cwWebDesignerSinglePageGenerator
    {

        public const string cwWebDesignerFolder = "webdesigner";

        private webDesignerOperation webDesigner = null;
        private cwWebDesignerFileManager fileManager = null;

        public cwWebDesignerSinglePageGenerator(webDesignerOperation _webDesigner, cwWebDesignerFileManager _fileManager)
        {
            webDesigner = _webDesigner;
            fileManager = _fileManager;
        }

        /// <summary>
        /// Creates the custom JS file single.
        /// </summary>
        /// <param name="_Page">The _ page.</param>
        public void createCustomJSFileSingle(cwWebDesignerTreeNodePage _Page)
        {
            string _pageName = _Page.getName();
            string jsFilePath = fileManager.addToHandmadePath(_pageName + "/" + _pageName + ".js");
            if (!File.Exists(jsFilePath))
            {
                cwWebDesignerWriter customJS_file = new cwWebDesignerWriter(jsFilePath);
                customJS_file.writeToFile(webDesignerOperation.INCLUDE_GLOBAL_IN_JS_FOR_LINT);
                customJS_file.writeToFile("function doActionsForSingle_" + _pageName + "(" + _pageName + ") {}");
                customJS_file.close();
            }
        }


        /// <summary>
        /// Creates the single layout_ standard list.
        /// </summary>
        /// <param name="_page">The _page.</param>
        public void createSingleLayout(cwWebDesignerTreeNodePage _page)
        {
            string _pageName = _page.ToString();
            string layoutFilePath = fileManager.addToGeneratedPath(_pageName + "/layouts/" + _pageName + ".js");
            cwWebDesignerWriter layout_file = new cwWebDesignerWriter(layoutFilePath);

            layout_file.writeToFile("/*global cwTabManager:true, cwAPI:true */");

            layout_file.writeToFileI(@"function drawProperties_" + _pageName + @"(output, " + _pageName + @"){");
            //foreach (cwPSFTreeNodeObjectNode nodeOG in _page.getChildrenObjectNodes())
            //{
            //    addObjectGroupPropertiesToFile(layout_file, nodeOG, _pageName);
            //}

            layout_file.writeToFileD("}");
            layout_file.writeToFileI(@"function drawItems_" + _pageName + @"(" + _pageName + @"){");

            layout_file.writeToFile("var output;");
            layout_file.writeToFile("document.title = " + _pageName + @".name;");
            layout_file.writeToFile("output = [];");

            foreach (cwPSFTreeNodeObjectNode nodeOG in _page.getChildrenObjectNodes())
            {
                layout_file.writeToFile("draw_" + nodeOG.ID + "(output, {\"associations\" : {\"" + nodeOG.ID + "\" : [" + _pageName + "]}});");
                //layout_file.writeInOutputI("<ul class='properties-zone-area properties-" + nodeOG.ID + "'>");
                //layout_file.writeToFile("drawProperties_" + _pageName + @"(output, " + _pageName + @");");
                //layout_file.writeInOutputD("</ul>");
            }

            layout_file.writeToFile("$('#top_of_page').next().html(output.join(''));");
            layout_file.writeToFile("cwAPI.cwSiteActions.doActionsForSingle();");
            layout_file.writeToFile("cwCustomerSiteActions.doActionsForAll_Custom();");
            layout_file.writeToFile("cwCustomerSiteActions.doActionsForSingle_Custom(" + _pageName + ");");
            layout_file.writeToFile("doActionsForSingle_" + _pageName + "(" + _pageName + ");");
            layout_file.writeToFile("cwAPI.setToolTipsOnTitles();");
            layout_file.writeToFile("cwTabManager.activeTab(\".cwTabManager-tab\");");


            foreach (cwWebDesignerTreeNodeObjectNode childOGNode in _page.getChildrenObjectNodes())
            {
                webDesigner.outputManager.addBehaviours_Rec(layout_file, childOGNode, _pageName, "single");
            }
            layout_file.writeToFileD("}");
            foreach (cwWebDesignerTreeNodeObjectNode nodeOG in _page.getChildrenObjectNodes())
            {
                webDesigner.outputManager.createObjectGroupAssociationsFunctions(layout_file, nodeOG, "", nodeOG.ID, nodeOG.ID);
            }
            layout_file.close();
        }



        /// <summary>
        /// Adds the page custom CSS.
        /// </summary>
        /// <param name="html_file">The html_file.</param>
        /// <param name="pageName">Name of the page.</param>
        private void addPageCustomCSS(cwWebDesignerWriter html_file, string pageName)
        {
            html_file.addCSSToHead("../" + fileManager.addToHandmadePathRelative(pageName + "/" + pageName + ".css"));
            html_file.writeToFile("<!--[if IE 7]><link type='text/css' rel='stylesheet' media='all' href='" + fileManager.getMediaPath() + fileManager.addToHandmadePathRelative(pageName + "/" + pageName + ".ie7.css") + "'/><![endif]-->");
            html_file.writeToFile("<!--[if IE 8]><link type='text/css' rel='stylesheet' media='all' href='" + fileManager.getMediaPath() + fileManager.addToHandmadePathRelative(pageName + "/" + pageName + ".ie8.css") + "'/><![endif]-->");
            html_file.writeToFile("<!--[if lt IE 9]><link type='text/css' rel='stylesheet' media='all' href='" + fileManager.getMediaPath() + fileManager.addToHandmadePathRelative(pageName + "/" + pageName + ".lt-ie9.css") + "'/><![endif]-->");
        }


        /// <summary>
        /// Creates the JSON file single.
        /// </summary>
        /// <param name="rootObject">The root object.</param>
        /// <param name="_Page">The _ page.</param>
        /// <param name="OID">The OID.</param>
        /// <param name="pageName">Name of the page.</param>
        public void createJSONFileSingle(cwLightObject rootObject, cwWebDesignerTreeNodePage _Page, string OID, string pageName)
        {
            cwLightNodeObjectType OT = webDesigner.nodes[pageName].First();
            cwLightObjectJSON json = new cwLightObjectJSON(rootObject, OT, true);
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string output = serializer.Serialize(json);
            StreamWriter json_file = cwWebDesignerTools.getStreamWriterErase(fileManager.addToGeneratedPath(pageName + "/json/" + OID + "." + _Page.getParentSite().getJSONExtention()));
            json_file.Write(output);
            json_file.Close();
        }


        //private static void addPropertiesToFile(cwWebDesignerWriter layout_file, List<cwLightProperty> _properties, string specialClass, string _pageName)
        //{
        //    foreach (cwLightProperty p in _properties)
        //    {

        //        var pScriptNameLower = p.ScriptName.ToLower();
        //        var pText = cwWebDesignerTools.escapeChars(p.ToString());
        //        string _value = "(" + _pageName + "." + pScriptNameLower + " != '') ? " + _pageName + "." + pScriptNameLower + ": '&nbsp;'";
        //        layout_file.writeToFile("addPropertyBox(output, '" + pScriptNameLower + "', '" + pText + "', " + _pageName + ", '" + specialClass + "');");
        //    }
        //}

        //public static void addObjectGroupPropertiesToFile(cwWebDesignerWriter layout_file, cwPSFTreeNodeObjectNode nodeOG, string _pageName)
        //{
        //    List<cwLightProperty> normalProperties = new List<cwLightProperty>();
        //    List<cwLightProperty> memoProperties = new List<cwLightProperty>();
        //    foreach (cwLightProperty p in nodeOG.selectedCheckedProperties)
        //    {
        //        if ("NAME".Equals(p.ScriptName))
        //        {
        //            layout_file.writeInOutput("<li class='property-name'><span class='ogs-name'>" + nodeOG.getSelectedObjectType().ToString() + " : </span>\", " + _pageName + ".name,\"</li>");
        //            continue;
        //        }
        //        if ("Memo".Equals(p.DataType))
        //        {
        //            memoProperties.Add(p);
        //            continue;
        //        }
        //        normalProperties.Add(p);
        //    }

        //    addPropertiesToFile(layout_file, normalProperties, "property-box-normal", _pageName);
        //    addPropertiesToFile(layout_file, memoProperties, "property-box-memo", _pageName);
        //}
    }


}
