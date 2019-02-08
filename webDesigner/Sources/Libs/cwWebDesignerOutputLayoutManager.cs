using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Casewise.GraphAPI.PSF;
using Casewise.GraphAPI.API;
using Casewise.GraphAPI.Exceptions;
using Casewise.webDesigner.Nodes;
using Casewise.GraphAPI;


namespace Casewise.webDesigner.Libs
{
    class cwWebDesignerOutputLayoutManager
    {

        private webDesignerOperation _webDesigner = null;
        public cwWebDesignerOutputLayoutManager(webDesignerOperation webDesigner)
        {
            _webDesigner = webDesigner;
        }


        /// <summary>
        /// Adds the layout.
        /// </summary>
        /// <param name="js_file">The js_file.</param>
        /// <param name="nodeOG">The node OG.</param>
        /// <param name="_parentOGID">The _parent OGID.</param>
        /// <param name="nodeID">The node ID.</param>
        /// <param name="nodeCSS">The node CSS.</param>
        public void addLayout(cwWebDesignerWriter js_file, cwWebDesignerTreeNodeObjectNode nodeOG, string _parentOGID, string nodeID, string nodeCSS)
        {
            int childrenCount = nodeOG.getAllChildren().Count();
            cwWebDesignerTreeNodeInterfaceLayout ILayout = nodeOG.getLayout();

            cwWebDesignerTreeNodeLayout TNLayout = ILayout as cwWebDesignerTreeNodeLayout;
          
            if (TNLayout != null)
            {
                bool setLink = TNLayout.isSetLinkEqualsToTrue();

                cwLightObjectType OT = nodeOG.getSelectedObjectType();
                string OGName = cwTools.escapeChars(OT.ToString());

                string targetViewName = TNLayout.getLinkViewName();
                if (string.IsNullOrEmpty(targetViewName))
                {
                    targetViewName = nodeOG.getViewName();
                }
                js_file.writeToFile("layout = new " + TNLayout.getStringProperty(cwWebDesignerTreeNodeLayout.CONFIG_LAYOUT_JS_CLASS_NAME) + "(\"" + nodeCSS + "\", \"" + nodeOG.getSelectedObjectType().ToString() + "\", " + setLink.ToString().ToLower() + ", \"" + _parentOGID + "\", \""+ targetViewName+"\");");

                foreach (cwWebDesignerTreeNodeUsedTemplateNode templateNode in TNLayout.getChildrenGeneric<cwWebDesignerTreeNodeUsedTemplateNode>())
                {
                    cwWebDesignerTreeNodeLayout templateLayout = templateNode.getFirstLayout(_webDesigner) as cwWebDesignerTreeNodeLayout;
                    if (templateLayout != null)
                    {
                        js_file.writeToFile("layout.drawOneMethod = " + templateLayout.getStringProperty(cwWebDesignerTreeNodeLayout.CONFIG_LAYOUT_JS_CLASS_NAME) + ".drawOne;");
                    }
                }
                string boxName = cwTools.escapeChars(nodeOG.getBoxName());
                js_file.writeToFile("layout.drawAssociations(output, \"" + boxName + "\", " + _parentOGID + "_parent, '" + nodeID + "'" + ((childrenCount > 0) ? ", drawChilds.bind(layout)" : "") + ");");
            }


        }


        public void addBehaviours_Rec(cwWebDesignerWriter generatedJS_file, cwWebDesignerTreeNodeObjectNode nodeOG, string pageName, string pageType)
        {
            cwWebDesignerTreeNodeInterfaceLayout layout = nodeOG.getLayout();
            //nodeOG.getFirstChildNode<cwWebDesignerTreeNodeLayout>();
            if (layout.hasAtLeastOnChildNode<cwWebDesignerTreeNodeBehaviour>())
            {
                foreach (cwWebDesignerTreeNodeBehaviour b in layout.getChildrenNodes<cwWebDesignerTreeNodeBehaviour>())
                {
                    generatedJS_file.writeToFile(b.ToString(layout.getParentNode().ID, pageName, pageType));
                }
            }

            foreach (cwWebDesignerTreeNodeObjectNode childOGNode in nodeOG.getAllChildrenNodes())
            {
                addBehaviours_Rec(generatedJS_file, childOGNode, pageName, pageType);
            }
        }


        private string createDiagramDesignLayoutsFunction(cwWebDesignerWriter layout_file, cwWebDesignerTreeNodeObjectNode nodeOG, int level)
        {
            List<string> childrenNames = new List<string>();
            foreach (cwWebDesignerTreeNodeObjectNode childNode in nodeOG.getChildrenObjectNodes())
            {
                childrenNames.Add(createDiagramDesignLayoutsFunction(layout_file, childNode, level + 1));
            }
            cwWebDesignerTreeNodeInterfaceLayout ILayout = nodeOG.getLayout();
            cwDiagramDesignerLayoutNode DLayout = ILayout as cwDiagramDesignerLayoutNode;
            if (DLayout != null)
            {
                DLayout.addLayoutNode(layout_file, nodeOG.ID, level, childrenNames);
            }
            else
            {
                throw new cwExceptionNodeValidation("The layout should be a diagram desing layout", nodeOG);
            }
            return nodeOG.ID;
        }

        private List<string> getNodeIDNames(cwPSFTreeNodeObjectNode nodeOG)
        {
            List<string> childrenNames = new List<string>();
            childrenNames.Add(nodeOG.ID);
            foreach (cwPSFTreeNodeObjectNode childNode in nodeOG.getChildrenObjectNodes())
            {
                childrenNames.AddRange(getNodeIDNames(childNode));
            }
            return childrenNames;
        }


        private void createDiagramDesignFunction(cwWebDesignerWriter layout_file, cwWebDesignerTreeNodeObjectNode nodeOG, string nodeID, string nodePath)
        {
            cwDiagramDesignerDesignNode designNode = nodeOG.getFirstChildNode<cwDiagramDesignerDesignNode>();
            cwLightDiagram template = designNode.propertiesBoxes.getPropertyBox<cwPSFPropertyBoxComboBoxTemplate>(cwDiagramDesignerDesignNode.CONFIG_TEMPLATE).getTemplateDiagram();
            if (template == null)
            {
                throw new cwExceptionNodeValidation("Please select a valid template", designNode);
            }
            string diagramName = designNode.getStringProperty(cwDiagramDesignerDesignNode.CONFIG_DIAGRAM_NAME).ToString();
            diagramName = cwTools.escapeChars(diagramName);

            layout_file.writeToFileI("function draw" + nodePath + "_diagram_design(items){");

            List<string> nodeIDsName = getNodeIDNames(nodeOG);
            layout_file.writeToFile("var " + cwTools.stringToStringSeparatedby(",", nodeIDsName, false) + ";");
            string level0NodeID = createDiagramDesignLayoutsFunction(layout_file, nodeOG, 0);
            layout_file.writeToFileI("new DiagramDesigner(items, " + level0NodeID + ", 'diagram-" + level0NodeID + "', '" + template.properties["TYPE" + cwLookupManager.LOOKUPABBR_KEY] + "', '" + DIAGRAM_DESIGNER_CONTAINER_PREFIX + nodePath + "', \"" + diagramName + "\", '600px', function(err, diagram){");
            layout_file.writeToFileI("if (!diagram.hasShapes){");
            layout_file.writeToFile("jQuery('#" + DIAGRAM_DESIGNER_CONTAINER_PREFIX + nodePath + "').remove();");
            layout_file.writeToFileD("}");
            layout_file.writeToFileD("});");

            layout_file.writeToFileD("}");
        }

        public const string DIAGRAM_DESIGNER_CONTAINER_PREFIX = "cw-diagram-design-container";

        /// <summary>
        /// Creates the object group associations functions.
        /// </summary>
        /// <param name="layout_file">The layout_file.</param>
        /// <param name="nodeOG">The node OG.</param>
        /// <param name="_parentPath">The _parent path.</param>
        /// <param name="nodeID">The node ID.</param>
        /// <param name="nodeCSS">The node CSS.</param>
        public void createObjectGroupAssociationsFunctions(cwWebDesignerWriter layout_file, cwWebDesignerTreeNodeObjectNode nodeOG, string _parentPath, string nodeID, string nodeCSS)
        {
            string nodePath = _parentPath + "_" + nodeID;
            if (nodeOG.hasAtLeastOnChildNode<cwDiagramDesignerDesignNode>())
            {
                createDiagramDesignFunction(layout_file, nodeOG, nodeID, nodePath);
            }



            layout_file.writeToFileI("function draw" + nodePath + "(output, " + nodeID + "_parent, parent){");
            if (nodeOG.hasAtLeastOnChildNode<cwDiagramDesignerDesignNode>())
            {
                cwDiagramDesignerDesignNode designNode = nodeOG.getFirstChildNode<cwDiagramDesignerDesignNode>();
                layout_file.writeToFile("draw" + nodePath + "_diagram_design(" + nodeID + "_parent);");
                //layout_file.writeToFileI("if (hasShapes) {");
                layout_file.writeInOutputI("<div id='" + DIAGRAM_DESIGNER_CONTAINER_PREFIX + nodePath + "' class='" + DIAGRAM_DESIGNER_CONTAINER_PREFIX + "'>");
                layout_file.writeInOutput("<div class='ui-widget-header'>" + designNode.getStringProperty(cwDiagramDesignerDesignNode.CONFIG_DIAGRAM_NAME) + "</div>");
                layout_file.writeInOutputD("</div>");
                //layout_file.writeToFileD("}");
            }
            cwWebDesignerTreeNodeInterfaceLayout ILayout = nodeOG.getLayout();
            cwWebDesignerTreeNodeLayout TNLayout = ILayout as cwWebDesignerTreeNodeLayout;
            if (TNLayout != null)
            {
                int childrenCount = nodeOG.getAllChildren().Count();
                if (childrenCount > 0)
                {
                    layout_file.writeToFile("var drawChilds, layout;");
                    layout_file.writeToFileI("drawChilds = function(output, child, parent){");
                    layout_file.writeInOutputI("<div class='cw-children children-\", this.css, \"'>");
                    createDrawFunctionForNodeWithTabs(layout_file, nodeID, nodePath, "child", nodeOG.getAllChildren(), nodeOG.getChildrenGeneric<cwWebDesignerTreeNodeTab>(), nodeOG.getChildrenGeneric<cwWebDesignerTreeNodeUsedTemplateNode>());
                    layout_file.writeInOutputD("</div>");
                    layout_file.writeToFileD("};");
                }
                addLayout(layout_file, nodeOG, nodeID, nodeID, nodeCSS);
            }
            else
            {

            }


            layout_file.writeToFileD("}");
            foreach (cwWebDesignerTreeNodeObjectNode childNode in nodeOG.getChildrenObjectNodes())
            {
                createObjectGroupAssociationFunctionIncludingTemplates(layout_file, childNode, _parentPath + "_" + nodeID);
            }
            if (nodeOG.hasAtLeastOnChildNode<cwWebDesignerTreeNodeTab>())
            {
                foreach (cwWebDesignerTreeNodeTab tab in nodeOG.getChildrenNodes<cwWebDesignerTreeNodeTab>())
                {
                    _webDesigner.tabManager.createTabFunction(layout_file, tab, _parentPath + "_" + nodeID);
                }
            }

        }

        public void createObjectGroupAssociationFunctionIncludingTemplates(cwWebDesignerWriter layout_file, cwWebDesignerTreeNodeObjectNode nodeOG, string parentPath)
        {
            List<cwWebDesignerTreeNodeUsedTemplateNode> usedTemplateNodes = nodeOG.getAllChildrenGeneric<cwWebDesignerTreeNodeUsedTemplateNode>();
            if (usedTemplateNodes.Count > 0)
            {
                cwWebDesignerTreeNodeUsedTemplateNode templateNode = usedTemplateNodes.First();
                cwWebDesignerTreeNodeObjectNodeObjectType templateObjectTypeNode = _webDesigner.nodeRootObjectTypesByTemplateName[templateNode.getTemplateName()];
                string templateCSS = templateObjectTypeNode.ID;

                _webDesigner.outputManager.createObjectGroupAssociationsFunctions(layout_file, templateObjectTypeNode, parentPath, nodeOG.ID, templateObjectTypeNode.ID);
            }
            else
            {
                _webDesigner.outputManager.createObjectGroupAssociationsFunctions(layout_file, nodeOG, parentPath, nodeOG.ID, nodeOG.ID);
            }
        }

        /// <summary>
        /// Creates the draw function for node.
        /// </summary>
        /// <param name="layout_file">The layout_file.</param>
        /// <param name="parentNodeID">The parent node ID.</param>
        /// <param name="childOGLevel2">The child OG level2.</param>
        /// <param name="parentPath">The parent path.</param>
        /// <param name="nodeVariableName">Name of the node variable.</param>
        public void createDrawFunctionForNode(cwWebDesignerWriter layout_file, string parentNodeID, cwPSFTreeNode childOGLevel2, string parentPath, string nodeVariableName)
        {
            string typeName = childOGLevel2.GetType().Name;
            switch (typeName)
            {
                case "cwWebDesignerTreeNodeObjectNodeAssociationType":
                case "cwWebDesignerTreeNodeObjectNodeObjectType":
                    cwPSFTreeNodeObjectNode childNodeLevel2 = childOGLevel2 as cwPSFTreeNodeObjectNode;
                    layout_file.writeToFile("draw" + parentPath + "_" + childNodeLevel2.ID + "(output, " + nodeVariableName + ", parent);");
                    break;
                case "cwWebDesignerTreeNodePropertiesGroup":
                    cwWebDesignerTreeNodePropertiesGroup childPropertiesGroupLevel2 = childOGLevel2 as cwWebDesignerTreeNodePropertiesGroup;
                    layout_file.writeToFile("cwAPI.cwPropertiesGroups.displayPropertiesGroupFromKey(output, " + nodeVariableName + ", \"" + childPropertiesGroupLevel2.ID + "\");");
                    break;
                default:
                    break;
            }
        }


        /// <summary>
        /// Creates the draw function for node with tabs.
        /// </summary>
        /// <param name="layout_file">The layout_file.</param>
        /// <param name="nodeID">The node ID.</param>
        /// <param name="nodePath">The node path.</param>
        /// <param name="nodeChildVarName">Name of the node child var.</param>
        /// <param name="allChildren">All children.</param>
        /// <param name="tabs">The tabs.</param>
        /// <param name="usedTemplateNodes">The used template nodes.</param>
        public void createDrawFunctionForNodeWithTabs(cwWebDesignerWriter layout_file, string nodeID, string nodePath, string nodeChildVarName, List<cwPSFTreeNode> allChildren, List<cwWebDesignerTreeNodeTab> tabs, List<cwWebDesignerTreeNodeUsedTemplateNode> usedTemplateNodes)
        {
            if (tabs.Count > 0)
            {
                string tabPath = "tab" + nodePath;
                layout_file.writeInOutputI("<div id='" + tabPath + "' class='cwTabManager-tab'>");
                _webDesigner.tabManager.createTabHeader(layout_file, tabs, tabPath);
                _webDesigner.tabManager.createTabContent(layout_file, tabs, nodePath, nodeChildVarName, tabPath);
                layout_file.writeInOutputD("</div>");

            }
            else
            {
                if (usedTemplateNodes.Count > 0)
                {
                    cwWebDesignerTreeNodeUsedTemplateNode templateNode = usedTemplateNodes.First();
                    cwWebDesignerTreeNodeObjectNodeObjectType templateObjectTypeNode = _webDesigner.nodeRootObjectTypesByTemplateName[templateNode.getTemplateName()];
                    foreach (cwWebDesignerTreeNodeObjectNodeAssociationType childOGLevel2 in templateObjectTypeNode.getChildrenObjectNodes())
                    {
                        createDrawFunctionForNode(layout_file, templateObjectTypeNode.ID, childOGLevel2, nodePath, nodeChildVarName);
                    }


                }
                else
                {
                    foreach (cwPSFTreeNode childOGLevel2 in allChildren)
                    {
                        createDrawFunctionForNode(layout_file, nodeID, childOGLevel2, nodePath, nodeChildVarName);
                    }
                }
            }
        }
    }
}
