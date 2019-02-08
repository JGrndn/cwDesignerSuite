using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Casewise.GraphAPI.PSF;
using Casewise.webDesigner.Nodes;
using Casewise.GraphAPI;


namespace Casewise.webDesigner.Libs
{
    internal class cwWebDesignerTabManager
    {


        private webDesignerOperation _webDesigner = null;
        public cwWebDesignerTabManager(webDesignerOperation webDesigner)
        {
            _webDesigner = webDesigner;
        }


        /// <summary>
        /// Creates the content of the tab.
        /// </summary>
        /// <param name="layout_file">The layout_file.</param>
        /// <param name="tabs">The tabs.</param>
        /// <param name="_parentOGID">The _parent OGID.</param>
        /// <param name="parentOGVariable">The parent OG variable.</param>
        /// <param name="tabPath">The tab path.</param>
        public void createTabContent(cwWebDesignerWriter layout_file, List<cwWebDesignerTreeNodeTab> tabs, string _parentOGID, string parentOGVariable, string tabPath)
        {

            foreach (cwWebDesignerTreeNodeTab tab in tabs)
            {
                string nodeName = cwTools.escapeChars(tab.getName());
                string nodeID = cwTools.stringToID(nodeName).ToLower();
                layout_file.writeToFile("cwTabManager.createTextTabContent(output, \"" + tabPath + "-" + nodeID + "\", draw" + _parentOGID + "_" + nodeID + ".bind(" + parentOGVariable + ")"+", " + tab.propertiesBoxes.getPropertyBox(cwWebDesignerTreeNodeTab.CONFIG_TAB_HIDE) +");");
            }
        }

        /// <summary>
        /// Creates the tab header.
        /// </summary>
        /// <param name="layout_file">The layout_file.</param>
        /// <param name="tabs">The tabs.</param>
        /// <param name="tabPath">The tab path.</param>
        public void createTabHeader(cwWebDesignerWriter layout_file, List<cwWebDesignerTreeNodeTab> tabs, string tabPath)
        {
            layout_file.writeInOutputI("<ul>");
            foreach (cwWebDesignerTreeNodeTab tab in tabs)
            {
                // create header
                string nodeName = cwTools.escapeChars(tab.getName());
                layout_file.writeToFile("cwTabManager.createTextTab(output, \"" + tabPath + "-" + cwTools.stringToID(nodeName) + "\", \"" + nodeName + "\", '" + tab.getStringProperty(cwWebDesignerTreeNodeTab.CONFIG_TAB_ICON).ToString() + "', '" + tabPath + "', " + tab.propertiesBoxes.getPropertyBox(cwWebDesignerTreeNodeTab.CONFIG_TAB_HIDE) + ");");
            }
            layout_file.writeInOutputD("</ul>");
        }

        /// <summary>
        /// Creates the tab function.
        /// </summary>
        /// <param name="layout_file">The layout_file.</param>
        /// <param name="tab">The tab.</param>
        /// <param name="parentID">The parent ID.</param>
        public void createTabFunction(cwWebDesignerWriter layout_file, cwWebDesignerTreeNodeTab tab, string parentID)
        {
            string nodeName = cwTools.escapeChars(tab.getName());
            string nodeID = parentID + "_" + cwTools.stringToID(nodeName);
            layout_file.writeToFileI("function draw" + nodeID + "(output){");
            foreach (cwPSFTreeNode childOGLevel2 in tab.Nodes)
            {
                _webDesigner.outputManager.createDrawFunctionForNode(layout_file, parentID, childOGLevel2, nodeID, "this");
            }
            layout_file.writeToFileD("}");
            foreach (cwWebDesignerTreeNodeObjectNode childNodeOG in tab.getChildrenObjectNodes())
            {
                _webDesigner.outputManager.createObjectGroupAssociationFunctionIncludingTemplates(layout_file, childNodeOG, nodeID);                    
            }
        }
    }
}
