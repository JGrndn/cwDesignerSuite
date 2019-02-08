using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Casewise.GraphAPI;
using System.Threading;
using System.IO;
using Casewise.GraphAPI.Exceptions;
using System.Xml;
using Casewise.webDesigner.GUI;
using Casewise.GraphAPI.API;
using Casewise.Data.ICM;
using System.Data;
using Casewise.GraphAPI.PSF;
using Casewise.webDesigner.Libs;
using Casewise.GraphAPI.GUI;
using log4net;
using Casewise.webDesigner.Nodes;
using Casewise.GraphAPI.ProgramManager;
using Casewise.webDesigner.Sources.Libs;


namespace Casewise.webDesigner
{
    /// <summary>
    /// webDesignerOperation
    /// </summary>
    public class webDesignerOperation : cwOperationLight
    {
        public const string OBJECTTYPE_NAME_IN_CM = "WEBDESIGNER";
        public const string INCLUDE_GLOBAL_IN_JS_FOR_LINT = "/*global cwAPI:true*/";
        private static readonly ILog log = LogManager.GetLogger(typeof(webDesignerOperation));

        public Dictionary<int, Dictionary<int, List<cwLightDiagram>>> objectsOnDiagrams = new Dictionary<int, Dictionary<int, List<cwLightDiagram>>>();
        public cwWebDesignerJSONTools jsonTools = null;
        private List<int> diagramsToLoad = new List<int>();
        public cwWebDesignerFileManager fileManager = null;
        private cwWebDesignerGlobalFiles _globalFiles = null;
        public cwWebDesignerTreeNodeSite rootSiteNode = null;

        internal cwWebDesignerOutputLayoutManager outputManager = null;
        internal cwWebDesignerTabManager tabManager = null;

        internal Dictionary<string, cwWebDesignerTreeNodePage> pagesCache = new Dictionary<string, cwWebDesignerTreeNodePage>();
        internal Dictionary<string, cwWebDesignerTreeNodeObjectNodeObjectType> nodeRootObjectTypesByTemplateName = new Dictionary<string, cwWebDesignerTreeNodeObjectNodeObjectType>();
        //public cwWebDesignerLayoutManager layoutManager = new cwWebDesignerLayoutManager(cwWebDesignerLayoutManager.LAYOUT_DIRECTORY_PATH);

        public Dictionary<string, List<cwLightNodeObjectType>> nodes = new Dictionary<string, List<cwLightNodeObjectType>>();
        public Dictionary<string, cwWebDesignerTreeNodeObjectNode> templateTreeNodes = new Dictionary<string, cwWebDesignerTreeNodeObjectNode>();

        /// <summary>
        /// Initializes a new instance of the <see cref="webDesignerOperation"/> class.
        /// </summary>
        /// <param name="lightModel">The light model.</param>
        /// <param name="rootSiteNode">The root site node.</param>
        public webDesignerOperation(cwLightModel lightModel, cwWebDesignerTreeNodeSite rootSiteNode)
            : base(lightModel, "Web Designer", "Web Designer v2 beta")
        {
            initWebDesigner(lightModel, rootSiteNode);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="webDesignerOperation"/> class.
        /// </summary>
        /// <param name="lightModel">The light model.</param>
        /// <param name="webDesignerItemID">The web designer item ID.</param>
        public webDesignerOperation(cwLightModel lightModel, int webDesignerItemID)
        {
            cwLightNodeObjectType OT = new cwLightNodeObjectType(lightModel[webDesignerOperation.OBJECTTYPE_NAME_IN_CM]);
            OT.addAttributeForFilterAND("ID", webDesignerItemID.ToString(), "=");
            OT.addPropertyToSelect("DESCRIPTION");
            OT.preloadLightObjects();

            if (1.Equals(OT.usedOTLightObjectsByID.Count))
            {
                cwWebDesignerTreeNodeSite siteNode = new cwWebDesignerTreeNodeSite(new cwWebDesignerGUI(lightModel, new cwProgramManagerOptions()));
                siteNode.loadFromXMLContent(OT.usedOTLightObjectsByID[webDesignerItemID].properties["DESCRIPTION"]);
                initWebDesigner(lightModel, siteNode);
            }
        }



        /// <summary>
        /// Inits the web designer.
        /// </summary>
        /// <param name="lightModel">The light model.</param>
        /// <param name="rootSiteNode">The root site node.</param>
        private void initWebDesigner(cwLightModel lightModel, cwWebDesignerTreeNodeSite rootSiteNode)
        {
            Model = lightModel;
            outputManager = new cwWebDesignerOutputLayoutManager(this);
            tabManager = new cwWebDesignerTabManager(this);
            jsonTools = new cwWebDesignerJSONTools();
            this.rootSiteNode = rootSiteNode;
            fileManager = new cwWebDesignerFileManager(rootSiteNode);
            _globalFiles = new cwWebDesignerGlobalFiles(this, fileManager, rootSiteNode);
            loadWebDesignerFromSiteNode();
            pagesCache = getPages();
            loadNodeHierarchy(this.rootSiteNode);
        }



        /// <summary>
        /// Preloads the root node_ rec.
        /// </summary>
        /// <param name="node">The node.</param>
        public void preloadRootNode_Rec(cwLightNode node)
        {
            node.preloadLightObjects();
            foreach (cwLightNode childNode in node.childrenNodes)
            {
                preloadRootNode_Rec(childNode);
            }
            addDiagramsToDiagramsToLoad(node);
        }

        /// <summary>
        /// Finds the page.
        /// </summary>
        /// <param name="pageName">Name of the page.</param>
        /// <returns></returns>
        public cwWebDesignerTreeNodePage findPage(string pageName)
        {
            if (pagesCache.ContainsKey(pageName))
            {
                return pagesCache[pageName];
            }
            return null;
        }

        /// <summary>
        /// Gets the global files.
        /// </summary>
        public cwWebDesignerGlobalFiles GlobalFiles
        {
            get
            {
                return _globalFiles;
            }
        }

        /// <summary>
        /// Edits the mode start.
        /// </summary>
        /// <param name="_model"></param>
        /// <returns></returns>
        public override bool editModeStart(cwLightModel _model)
        {
            return editModeStartOperation<cwWebDesignerGUI, cwWebDesignerTreeNodeSite>(_model);
        }



        /// <summary>
        /// Adds the diagrams to diagrams to load.
        /// </summary>
        /// <param name="nodeOG">The node OG.</param>
        private void addDiagramsToDiagramsToLoad(cwLightNode nodeOG)
        {
            cwLightNodeObjectType OTNode = nodeOG as cwLightNodeObjectType;
            if (OTNode != null)
            {
                cwLightObjectType OT = OTNode.sourceObjectType;
                if ("DIAGRAM".Equals(OT.ScriptName))
                {
                    foreach (cwLightObject diagram in OTNode.usedOTLightObjects)
                    {
                        if (!diagramsToLoad.Contains(diagram.ID))
                        {
                            diagramsToLoad.Add(diagram.ID);
                        }
                    }
                }
            }

            cwLightNodeAssociationType ATNode = nodeOG as cwLightNodeAssociationType;
            if (ATNode != null)
            {
                cwLightAssociationType AT = ATNode.AssociationType;
                if ("ANYOBJECTEXPLODEDASDIAGRAM".Equals(AT.ScriptName))
                {
                    foreach (var sourceVar in ATNode.targetLightObjectBySourceKey)
                    {
                        foreach (var diagramVar in sourceVar.Value)
                        {
                            if (!diagramsToLoad.Contains(diagramVar.Value.ID))
                            {
                                diagramsToLoad.Add(diagramVar.Value.ID);
                            }
                        }
                    }
                }
                if ("ANYOBJECTSHOWNASSHAPEINDIAGRAM".Equals(AT.ScriptName))
                {
                    foreach (var sourceVar in ATNode.targetLightObjectBySourceKey)
                    {
                        foreach (var diagramVar in sourceVar.Value)
                        {
                            if (!diagramsToLoad.Contains(diagramVar.Value.ID))
                            {
                                diagramsToLoad.Add(diagramVar.Value.ID);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Loads the node hierarchy.
        /// </summary>
        /// <param name="siteNode">The site node.</param>
        public void loadNodeHierarchy(cwWebDesignerTreeNodeSite siteNode)
        {
            nodes = new Dictionary<string, List<cwLightNodeObjectType>>();
            nodeRootObjectTypesByTemplateName = new Dictionary<string, cwWebDesignerTreeNodeObjectNodeObjectType>();

            cwWebDesignerTreeNodeTemplateNodes templateNodes = siteNode.getChildrenGeneric<cwWebDesignerTreeNodeTemplateNodes>().First();
            List<cwWebDesignerTreeNodeTemplateNode> templateNodesItems = templateNodes.getChildrenGeneric<cwWebDesignerTreeNodeTemplateNode>();

            foreach (cwWebDesignerTreeNodeTemplateNode templateNode in templateNodesItems)
            {
                List<cwWebDesignerTreeNodeObjectNodeObjectType> rootObjectTypes = templateNode.getChildrenGeneric<cwWebDesignerTreeNodeObjectNodeObjectType>();

                nodeRootObjectTypesByTemplateName[templateNode.getName()] = rootObjectTypes.First();
                foreach (cwWebDesignerTreeNodeObjectNodeObjectType templateNodeOG in rootObjectTypes)
                {
                    cwLightNodeObjectType nodeOT = new cwLightNodeObjectType(templateNodeOG);
                    foreach (cwWebDesignerTreeNodeObjectNodeAssociationType childNode in templateNodeOG.getAllChildrenNodes())
                    {
                        loadNodeHierarchy_Rec(nodeOT, childNode, false);
                    }
                    if (!nodes.ContainsKey(templateNode.getName()))
                    {
                        nodes[templateNode.getName()] = new List<cwLightNodeObjectType>();
                    }
                    nodes[templateNode.getName()].Add(nodeOT);
                    templateTreeNodes[templateNode.getName()] = templateNodeOG;
                }
            }

            // create names controller
            List<cwLightObjectType> OTs = Model.getPSFEnabledObjectTypes(false);
            foreach (cwLightObjectType OT in OTs)
            {
                string viewKey = OT.ScriptName.ToLower() + "AllNames";
                cwWebDesignerTreeNodePage allNamesPage = new cwWebDesignerTreeNodePage(siteNode.getGUI<cwWebDesignerGUI>(), null);
                allNamesPage.Name = viewKey;
                cwWebDesignerTreeNodeObjectNodeObjectType allNamesMainOTNode = new cwWebDesignerTreeNodeObjectNodeObjectType(OT);
                cwWebDesignerTreeNodeLayout l = new cwWebDesignerTreeNodeLayout(siteNode.getGUI<cwWebDesignerGUI>(), null);
                allNamesMainOTNode.ID = OT.ScriptName.ToLower();
                allNamesMainOTNode.addChildNodeFirst(l);
                allNamesPage.updateText(viewKey);
                allNamesPage.addChildNodeFirst(allNamesMainOTNode);
                pagesCache[viewKey] = allNamesPage;

            }

            foreach (var pageCache in pagesCache)
            {
                nodes[pageCache.Key] = new List<cwLightNodeObjectType>();
                foreach (cwWebDesignerTreeNodeObjectNodeObjectType nodeOG in pageCache.Value.getChildrenObjectNodes())
                {
                    cwLightNodeObjectType nodeOT = new cwLightNodeObjectType(nodeOG);
                    loadPropertiesGroup(nodeOT, nodeOG);
                    foreach (cwWebDesignerTreeNodeObjectNodeAssociationType childNode in nodeOG.getAllChildrenNodes())
                    {
                        loadNodeHierarchy_Rec(nodeOT, childNode, true);
                    }
                    loadTemplateNodesIfRequired(nodeOG, nodeOT);
                    nodes[pageCache.Key].Add(nodeOT);
                }
            }
        }


        /// <summary>
        /// Loads the template nodes if required.
        /// </summary>
        /// <param name="nodeOG">The node OG.</param>
        /// <param name="nodeOT">The node OT.</param>
        private void loadTemplateNodesIfRequired(cwWebDesignerTreeNodeObjectNode nodeOG, cwLightNode nodeOT)
        {
            // create light nodes for the template
            foreach (cwWebDesignerTreeNodeUsedTemplateNode templateNode in nodeOG.getLayout().getChildrenGeneric<cwWebDesignerTreeNodeUsedTemplateNode>())
            {
                string templateName = templateNode.getStringProperty(cwWebDesignerTreeNodeUsedTemplateNode.CONFIG_TEMPLATE_NAME).ToString();
                cwLightNodeObjectType templateOT = nodes[templateName].First();
                nodeOT.selectedPropertiesScriptName = templateOT.selectedPropertiesScriptName;
                nodeOT.selectedProperties = templateOT.selectedProperties;
                foreach (cwLightNodeAssociationType ATNode in templateOT.childrenNodes)
                {
                    cwLightNodeAssociationType node = new cwLightNodeAssociationType(nodeOT, ATNode.AssociationType.Target, ATNode.AssociationType);
                    node.copyNodeProperties(ATNode);
                    node.ID = ATNode.ID;
                }
            }
        }


        /// <summary>
        /// Loads the properties group.
        /// </summary>
        /// <param name="rootNode">The root node.</param>
        /// <param name="nodeTN">The node TN.</param>
        public void loadPropertiesGroup(cwLightNode rootNode, cwWebDesignerTreeNodeObjectNode nodeTN)
        {
            foreach (cwWebDesignerTreeNodePropertiesGroup propertiesGroup in nodeTN.getAllChildrenGeneric<cwWebDesignerTreeNodePropertiesGroup>())
            {
                Dictionary<string, object> propertiesGroupAttributes = new Dictionary<string, object>();
                propertiesGroupAttributes["id"] = cwTools.stringToID(propertiesGroup.getName());
                propertiesGroupAttributes["name"] = propertiesGroup.getName();
                propertiesGroupAttributes["layout"] = propertiesGroup.getStringProperty(cwWebDesignerTreeNodePropertiesGroup.CONFIG_LAYOUT_DESIGN);
                List<Dictionary<string, object>> propertiesList = new List<Dictionary<string, object>>();
                foreach (cwLightProperty property in propertiesGroup.getCheckedPropertiesList())
                {
                    rootNode.addPropertyToSelect(property.ScriptName);
                    Dictionary<string, object> propertyNameAndType = new Dictionary<string, object>();
                    propertyNameAndType["name"] = property.ToString();
                    propertyNameAndType["propertyScriptName"] = property.ScriptName.ToLower();
                    propertyNameAndType["propertyType"] = property.DataType.ToLower();
                    if (property.isLookup)
                    {
                        propertyNameAndType["LookupValues"] = property.lookupContent;
                    }
                    propertiesList.Add(propertyNameAndType);
                }
                propertiesGroupAttributes["properties"] = propertiesList;
                string ID = propertiesGroupAttributes["id"].ToString();
                rootNode.propertiesGroups[ID] = propertiesGroupAttributes;
            }
        }


        /// <summary>
        /// Loads the node hierarchy_ rec.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="nodeOG">The node OG.</param>
        /// <param name="loadTemplates">if set to <c>true</c> [load templates].</param>
        /// <returns></returns>
        private cwLightNode loadNodeHierarchy_Rec(cwLightNode parent, cwWebDesignerTreeNodeObjectNodeAssociationType nodeOG, bool loadTemplates)
        {
            cwLightNodeAssociationType node = new cwLightNodeAssociationType(parent, nodeOG, nodeOG.getSelectedAssociationType());
            if (nodeOG.hasAtLeastOnChildNode<cwWebDesignerTreeNodeObjectNodeIntersectionObjectType>())
            {
                cwWebDesignerTreeNodeObjectNodeIntersectionObjectType intersectionOG = nodeOG.getFirstChildNode<cwWebDesignerTreeNodeObjectNodeIntersectionObjectType>();
                node.intersectionPropertiesToLoad = intersectionOG.selectedCheckedPropertiesScriptNames;
            }
            loadPropertiesGroup(node, nodeOG);
            if (loadTemplates && nodeOG.getLayout().getChildrenGeneric<cwWebDesignerTreeNodeUsedTemplateNode>().Count > 0)
            {
                loadTemplateNodesIfRequired(nodeOG, node);
            }
            foreach (cwPSFTreeNodeObjectNode genericNode in nodeOG.getChildrenObjectNodes())
            {
                if (genericNode is cwWebDesignerTreeNodeObjectNodeAssociationType)
                {
                    cwWebDesignerTreeNodeObjectNodeAssociationType nodeOGChild = genericNode as cwWebDesignerTreeNodeObjectNodeAssociationType;

                    loadNodeHierarchy_Rec(node, nodeOGChild, loadTemplates);
                }
            }
            return node;
        }

        /// <summary>
        /// Loads the web designer from site node.
        /// </summary>
        private void loadWebDesignerFromSiteNode()
        {
            jsonTools.setLinkMIMEType(rootSiteNode.getStringProperty(cwWebDesignerTreeNodeSite.CONFIG_SITE_LINK_MIMETYPE));

            if (!fileManager.checkWebStructure())
            {
                log.Error("Unable to create or load the site structure");
                return;
            }

            diagramsToLoad.Clear();
        }




        /// <summary>
        /// Creates the index page.
        /// </summary>
        /// <param name="_page">The _page.</param>
        internal void createIndexPage(cwWebDesignerTreeNodePage _page)
        {
            cwWebDesignerIndexPageGenerator _indexGenerator = new cwWebDesignerIndexPageGenerator(this, fileManager);
            string _pageName = _page.getName();
            try
            {
                DateTime start = DateTime.Now;
                _indexGenerator.createIndexPage(_page, _globalFiles);
                reportInfo("DONE" + _page.propertiesBoxes.getPropertyBox(cwWebDesignerTreeNodePage.CONFIG_PAGE_TYPE).ToString() + ":" + _pageName + DateTime.Now.Subtract(start).ToString());
            }
            catch (Exception e)
            {
                log.Error("Error while creating page [" + _pageName + "]");
                log.Error(e.ToString());
                _page.appendError(e.Message.ToString());
                throw e;
            }

        }

        /// <summary>
        /// Creates the single page.
        /// </summary>
        /// <param name="_page">The _page.</param>
        internal void createSinglePage(cwWebDesignerTreeNodePage _page)
        {
            //string pageLayout = _page.getStringProperty(cwWebDesignerTreeNodePage.CONFIG_PAGE_LAYOUT);
            string _pageName = _page.getName();
            DateTime start = DateTime.Now;
            if (!fileManager.checkWebStructureForPage(_page.getName()))
            {
                reportError("Unable to create the page");
                return;
            }
            cwWebDesignerSinglePageGenerator singleGenerator = new cwWebDesignerSinglePageGenerator(this, fileManager);

            _globalFiles.createFile_Helper(_page);

            //singleGenerator.createHTMLUniqueSingleFile(_pageName);

            cwLightNodeObjectType OT = nodes[_pageName].First();
            preloadRootNode_Rec(OT);
            if (!OT.sourceObjectType.ScriptName.Equals("DIAGRAM"))
            {
                foreach (cwLightObject o in OT.usedOTLightObjects)
                {
                    singleGenerator.createJSONFileSingle(o, _page, _pageName + o.ID, _pageName);
                }
            }
            foreach (cwPSFTreeNodeObjectNode nodeOG in _page.getChildrenObjectNodes())
            {

                _globalFiles.createFile_CSS(_pageName);
                singleGenerator.createCustomJSFileSingle(_page);
                singleGenerator.createSingleLayout(_page);
            }
            reportInfo("DONE" + _page.propertiesBoxes.getPropertyBox(cwWebDesignerTreeNodePage.CONFIG_PAGE_TYPE).ToString() + ":" + _pageName + DateTime.Now.Subtract(start).ToString());
        }


        /// <summary>
        /// Adds the validated templates to diagrams to load.
        /// </summary>
        public void addValidatedTemplatesToDiagramsToLoad()
        {

            cwLightNodeObjectType _templates = new cwLightNodeObjectType(Model.getObjectTypeByScriptName("DIAGRAM"));
            _templates.addPropertyToSelect("TYPE");
            _templates.addAttributeForFilterAND("TEMPLATE", "1", "=");
            _templates.addAttributeForFilterAND("VALIDATED", "1", "=");
            _templates.preloadLightObjects();

            foreach (cwLightObject t in _templates.usedOTLightObjects)
            {
                if (!diagramsToLoad.Contains(t.ID))
                {
                    diagramsToLoad.Add(t.ID);
                }
            }
        }

        /// <summary>
        /// Exports the diagrams.
        /// </summary>
        /// <param name="propertiesToSelectForDiagramObject">The properties to select for diagram object.</param>
        public void exportDiagrams(List<string> propertiesToSelectForDiagramObject)
        {
            cwWebDesignerExportDiagrams exportDiagrams = new cwWebDesignerExportDiagrams(jsonTools, Model);
            bool exportImages = false;
            if (this.rootSiteNode.propertiesBoxes.getPropertyBoxBoolean(cwWebDesignerTreeNodeSite.CONFIG_EXPORT_DIAGRAM_IMAGES).Checked)
            {
                exportImages = true;
            }
            exportDiagrams.exportDiagrams(Model, fileManager, diagramsToLoad, exportImages, propertiesToSelectForDiagramObject);
        }

        /// <summary>
        /// Gets the pages.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, cwWebDesignerTreeNodePage> getPages()
        {
            Dictionary<string, cwWebDesignerTreeNodePage> pages = new Dictionary<string, cwWebDesignerTreeNodePage>();

            cwWebDesignerTreeNodePagesIndex _rootPagesIndex = this.rootSiteNode.getFirstChildNode<cwWebDesignerTreeNodePagesIndex>();
            cwWebDesignerTreeNodePagesSingle _rootPagesSingle = this.rootSiteNode.getFirstChildNode<cwWebDesignerTreeNodePagesSingle>();

            if (_rootPagesIndex.hasAtLeastOnChildNode<cwWebDesignerTreeNodePage>())
            {
                foreach (cwWebDesignerTreeNodePage page in _rootPagesIndex.getChildrenNodes<cwWebDesignerTreeNodePage>())
                {
                    pages[page.getName()] = page;
                }
            }
            if (_rootPagesSingle.hasAtLeastOnChildNode<cwWebDesignerTreeNodePage>())
            {
                foreach (cwWebDesignerTreeNodePage page in _rootPagesSingle.getChildrenNodes<cwWebDesignerTreeNodePage>())
                {
                    pages[page.getName()] = page;
                }
            }
            return pages;
        }


        /// <summary>
        /// Does the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        public override void Do(cwLightModel model)
        {
            try
            {
                this.rootSiteNode.operationEditModeGUI.Cursor = System.Windows.Forms.Cursors.WaitCursor;

                _globalFiles.createGlobalFiles();

                List<string> propertiesToSelectForDiagramObject = null;
                Dictionary<string, cwWebDesignerTreeNodePage> allPages = getPages();
                int countError = 0;
                foreach (var v in allPages)
                {
                    try
                    {
                        cwWebDesignerTreeNodePage page = v.Value;
                        DateTime start = DateTime.Now;
                        reportInfo("Start Generating " + page.ToString() + "...");
                        switch (page.getStringProperty(cwWebDesignerTreeNodePage.CONFIG_PAGE_TYPE))
                        {
                            case "index":
                                createIndexPage(page);
                                break;
                            case "single":
                                createSinglePage(page);
                                if (this.nodes[page.getName()].First().sourceObjectType.ScriptName.Equals("DIAGRAM"))
                                {
                                    propertiesToSelectForDiagramObject = this.nodes[page.getName()].First().selectedPropertiesScriptName;
                                    exportDiagrams(propertiesToSelectForDiagramObject);
                                }
                                break;
                            default:
                                break;
                        }
                        page.operationEditModeGUI.appendInfo("Generated " + page.ToString() + " on " + DateTime.Now.Subtract(start).ToString() + "s");
                    }
                    catch (Exception ex)
                    {
                        countError++;
                        log.Error("Unable to create page [" + v.Key + "].", ex);
                        if (countError.Equals(3))
                        {
                            throw ex;
                        }
                    }
                }

                if (this.rootSiteNode.propertiesBoxes.getPropertyBoxBoolean(cwWebDesignerTreeNodeSite.CONFIG_EXPORT_VALIDATED_TEMPLATES).Checked)
                {
                    addValidatedTemplatesToDiagramsToLoad();
                }

                if (this.rootSiteNode.propertiesBoxes.getPropertyBoxBoolean(cwWebDesignerTreeNodeSite.CONFIG_GENERATE_DIAGRAM_HIERARCHY).Checked)
                {
                    bool uuidAsDiagramFileName = this.rootSiteNode.propertiesBoxes.getPropertyBoxBoolean(cwWebDesignerTreeNodeSite.CONFIG_UUID_AS_DIAGRAMFILENAME).Checked;
                    string outputPath = fileManager.getOutputFolder();
                    string json_extention = fileManager.getJSONExtention();
                    cwWebDesignerGenerateDiagramHierarchy di_hir = new cwWebDesignerGenerateDiagramHierarchy(model, outputPath, json_extention, uuidAsDiagramFileName);
                    di_hir.GenerateHierarchy();
                }
                this.rootSiteNode.operationEditModeGUI.Cursor = System.Windows.Forms.Cursors.Default;
            }
            catch (Exception e)
            {
                this.rootSiteNode.operationEditModeGUI.Cursor = System.Windows.Forms.Cursors.Default;
                throw e;
            }
        }
    }
}
