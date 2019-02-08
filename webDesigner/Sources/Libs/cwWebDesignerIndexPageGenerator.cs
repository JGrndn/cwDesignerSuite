using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Casewise.GraphAPI.API;
using System.IO;
using Casewise.webDesigner.GUI;
using Casewise.GraphAPI.PSF;
using System.Web.Script.Serialization;
using Casewise.webDesigner.Nodes;


namespace Casewise.webDesigner.Libs
{
    public class cwWebDesignerIndexPageGenerator
    {
        webDesignerOperation webDesigner = null;
        cwWebDesignerFileManager fileManager = null;

        public cwWebDesignerIndexPageGenerator(webDesignerOperation _webDesigner, cwWebDesignerFileManager _fileManager)
        {
            webDesigner = _webDesigner;
            fileManager = _fileManager;
        }


        /// <summary>
        /// Creates the index layout_ standard list.
        /// </summary>
        /// <param name="_page">The _page.</param>
        public void createIndexLayout(cwWebDesignerTreeNodePage _page)
        {
            string _pageName = _page.getName();
            string oFile = fileManager.addToGeneratedPath(_pageName + "/layouts/" + _pageName + ".js");
            cwWebDesignerWriter layout_file = new cwWebDesignerWriter(oFile);
            layout_file.writeToFileI("function draw_" + _pageName + "(" + _pageName + "){");
            layout_file.writeToFile("document.title = '" + _page.getStringProperty(cwWebDesignerTreeNodePage.CONFIG_PAGE_DISPLAY_NAME) + "';");
            layout_file.writeToFile("var output = [];");
            layout_file.writeInOutputI("<div class='" + _pageName + "-index-area " + _pageName + "'>");

            webDesigner.outputManager.createDrawFunctionForNodeWithTabs(layout_file, _pageName, "_" + _pageName, _pageName, _page.getAllChildren(), _page.getChildrenGeneric<cwWebDesignerTreeNodeTab>(), new List<cwWebDesignerTreeNodeUsedTemplateNode>());
            layout_file.writeInOutputD("</div>");

            layout_file.writeToFile("$('#zone_" + _pageName + "').html(output.join(''));");
            layout_file.writeToFile("cwTabManager.activeTab('.cwTabManager-tab');");

            layout_file.writeToFileD("}");

            if (_page.hasAtLeastOnChildNode<cwWebDesignerTreeNodeTab>())
            {
                foreach (cwWebDesignerTreeNodeTab tab in _page.getChildrenNodes<cwWebDesignerTreeNodeTab>())
                {
                    webDesigner.tabManager.createTabFunction(layout_file, tab, "_" + _pageName);
                }
            }
            else
            {
                foreach (cwWebDesignerTreeNodeObjectNode nodeOG in _page.getChildrenObjectNodes())
                {
                    webDesigner.outputManager.createObjectGroupAssociationFunctionIncludingTemplates(layout_file, nodeOG, "_" + _pageName);
                }
            }

            layout_file.close();
        }

        /// <summary>
        /// Creates the generated JS file.
        /// </summary>
        /// <param name="_Page">The _ page.</param>
        /// <param name="pageName">Name of the page.</param>
        private void createGeneratedJSFile(cwWebDesignerTreeNodePage _Page, string pageName)
        {
            string jsFilePath = fileManager.addToGeneratedPath(pageName + "/" + pageName + ".generated.js");
            //StreamWriter generatedJS_file = cwWebDesignerTools.getStreamWriterErase(jsFilePath);
            cwWebDesignerWriter generatedJS_file = new cwWebDesignerWriter(jsFilePath);
            if ("true".Equals(_Page.propertiesBoxes.getPropertyBox(cwWebDesignerTreeNodePage.CONFIG_ADD_SEARCH_ENGINE).ToString()))
            {
                generatedJS_file.writeToFileI(@"var searchEngine = {");
                generatedJS_file.writeToFile("searchFunction : drawItems_" + pageName + @",");
                generatedJS_file.writeToFile("searchRootID : '" + pageName + @"',");
                generatedJS_file.writeToFile("searchClear : false,");
                generatedJS_file.writeToFile("instantSearch : " + _Page.propertiesBoxes.getPropertyBox(cwWebDesignerTreeNodePage.CONFIG_INSTANT_SEARCH).ToString() + ",");
                generatedJS_file.writeToFile("searchItemsRequirements : [" + cwWebDesignerSearchEngine.createSearchEngineRequirements(_Page, 0, pageName) + @"]");
                generatedJS_file.writeToFileD("};");
            }

            generatedJS_file.writeToFileI("function doWhenLoaded_" + pageName + "(" + pageName + ") {");
            generatedJS_file.writeToFile("document.title = '" + pageName + "';");
            generatedJS_file.writeToFile(@"var doSearch = false;");
            if ("true".Equals(_Page.propertiesBoxes.getPropertyBox(cwWebDesignerTreeNodePage.CONFIG_ADD_SEARCH_ENGINE).ToString()))
            {
                generatedJS_file.writeToFile(@"cwAPI.cwSearchEngine.appendSearchEngineInput($.i18n.prop('index_search') + ' : ', '" + pageName + "', $('#top_of_page'), " + pageName + @", searchEngine, '#zone_" + pageName + @"');");
                generatedJS_file.writeToFile(@"doSearch = cwAPI.cwSearchEngine.doCustomSearch(" + pageName + ");");
            }
            generatedJS_file.writeToFile(@"if(!doSearch){");
            generatedJS_file.writeToFile(@"drawItems_" + pageName + @"(" + pageName + @", false);");
            generatedJS_file.writeToFileD("}");
            generatedJS_file.writeToFile("}");

            generatedJS_file.writeToFile("// don't change this function name");
            generatedJS_file.writeToFile("// used to display all the items");
            generatedJS_file.writeToFile("function drawItems_" + pageName + @"(all_items, searching)");
            generatedJS_file.writeToFileI("{");

            generatedJS_file.writeToFile("draw_" + pageName + "({\"associations\" : all_items}, searching);");

            generatedJS_file.writeToFile("cwAPI.cwSiteActions.doLayoutsSpecialActions();");
            generatedJS_file.writeToFile("cwCustomerSiteActions.doActionsForAll_Custom();");
            generatedJS_file.writeToFile("cwCustomerSiteActions.doActionsForIndex_Custom(searching);");



            foreach (cwWebDesignerTreeNodeObjectNode childOGNode in _Page.getChildrenObjectNodes())
            {
                webDesigner.outputManager.addBehaviours_Rec(generatedJS_file, childOGNode, pageName, "index");
            }

            generatedJS_file.writeToFile("doActionsForIndex_" + pageName + "(all_items, searching);");

            generatedJS_file.writeToFile("cwAPI.setToolTipsOnTitles();");

            generatedJS_file.writeToFileD("}");

            generatedJS_file.writeToFile("function doActionsForIndex_" + pageName + "(all_items){}");
            generatedJS_file.close();
        }



        /// <summary>
        /// Creates the index page.
        /// </summary>
        /// <param name="_page">The _page.</param>
        /// <param name="_globalFiles">The _global files.</param>
        public void createIndexPage(cwWebDesignerTreeNodePage _page, cwWebDesignerGlobalFiles _globalFiles)
        {
            string _pageName = _page.getName();
            if (!fileManager.checkWebStructureForPage(_pageName)) return;


            foreach (cwLightNodeObjectType OT in webDesigner.nodes[_pageName])
            {
                webDesigner.preloadRootNode_Rec(OT);
            }


            createGeneratedJSFile(_page, _pageName);
            createJSONFileIndex(_page);

            createCustomJSFile(_page, _pageName);

            _globalFiles.createFile_Helper(_page);
            _globalFiles.createFile_CSS(_pageName);

            createIndexLayout(_page);

        }


        /// <summary>
        /// Creates the custom JS file.
        /// </summary>
        /// <param name="_Page">The _ page.</param>
        /// <param name="pageName">Name of the page.</param>
        private void createCustomJSFile(cwWebDesignerTreeNodePage _Page, string pageName)
        {
            string jsFilePath = fileManager.addToHandmadePath(pageName + "/" + pageName + ".js");
            if (!File.Exists(jsFilePath))
            {
                cwWebDesignerWriter customJS_file = new cwWebDesignerWriter(jsFilePath);
                //StreamWriter customJS_file = cwWebDesignerTools.getStreamWriterErase(jsFilePath);
                customJS_file.writeToFile(webDesignerOperation.INCLUDE_GLOBAL_IN_JS_FOR_LINT);
                customJS_file.writeToFile("function doActionsForIndex_" + pageName + "(all_items, searching){}");
                customJS_file.close();
            }
        }


        public void createJSONFileIndex(cwWebDesignerTreeNodePage page)
        {
            string pageName = page.getName();
            Dictionary<string, List<cwLightObjectJSON>> jsonIndex = getIndexJSON(page);
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = 2147483647;
            string output = serializer.Serialize(jsonIndex);
            StreamWriter json_file = cwWebDesignerTools.getStreamWriterErase(fileManager.addToGeneratedPath(pageName + "/json/" + pageName + "." + page.getParentSite().getJSONExtention()));
            json_file.Write(output);
            json_file.Close();
        }

        public Dictionary<string, List<cwLightObjectJSON>> getIndexJSON(cwWebDesignerTreeNodePage page)
        {
            Dictionary<string, List<cwLightObjectJSON>> jsonIndex = new Dictionary<string, List<cwLightObjectJSON>>();
            if (page == null)
            {
                jsonIndex["error"] = new List<cwLightObjectJSON>();
                return jsonIndex;
            }

            string pageName = page.getName();

            List<cwLightNodeObjectType> OTs = webDesigner.nodes[pageName];
            foreach (cwLightNodeObjectType OGNode in OTs)
            {
                List<cwLightObjectJSON> jsonObjects = new List<cwLightObjectJSON>();
                foreach (cwLightObject o in OGNode.usedOTLightObjects)
                {
                    cwLightObjectJSON json = new cwLightObjectJSON(o, OGNode);
                    jsonObjects.Add(json);
                }
                jsonIndex[OGNode.ID] = jsonObjects;
            }
            return jsonIndex;
        }


    }
}
