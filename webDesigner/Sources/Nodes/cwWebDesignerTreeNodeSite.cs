using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Casewise.GraphAPI;
using Casewise.GraphAPI.Operations;
using System.Drawing;
using System.Xml;
using System.IO;
using Casewise.GraphAPI.Exceptions;
using Casewise.webDesigner.GUI;
using Casewise.GraphAPI.Operations.Web;
using Casewise.GraphAPI.PSF;
using System.Xml.Linq;
using Casewise.GraphAPI.API;
using System.Reflection;
using Casewise.Data.ICM;

namespace Casewise.webDesigner.Nodes
{
    /// <summary>
    /// a configuration tree node
    /// </summary>
    public class cwWebDesignerTreeNodeSite : cwPSFTreeNodeConfigurationNode
    {
        public static string CONFIG_SITE_PATH = "output-folder";
        public static string CONFIG_EXPORT_VALIDATED_TEMPLATES = "export-validated-templates";
        public static string CONFIG_EXPORT_DIAGRAM_IMAGES = "export-diagram-images";
        public static string CONFIG_LANGUAGE_CHOOSED = "choosed-language";
        public static string CONFIG_MEDIA_PATH = "media-path";
        public static string CONFIG_SITE_LINK_MIMETYPE = "link-mimetype";
        public static string CONFIG_JSON_EXTENTION = "json-extention";
        public static string CONFIG_WEBDESIGNER_SERVER_URL = "web-designer-server-url";
        public static string CONFIG_JS_MODE = "javascript-mode";
        public static string CONFIG_RUN_IN_PORTAL = "run-in-portal";
        public static string CONFIG_GENERATE_DIAGRAM_HIERARCHY = "generate-diagram-hierarchy";
        public static string CONFIG_UUID_AS_DIAGRAMFILENAME = "uuid-as-diagram-file-name";

        /// <summary>
        /// Initializes a new instance of the <see cref="cwWebDesignerTreeNodeSite"/> class.
        /// </summary>
        /// <param name="_cwWebIndexEditGUI">The _CW web index edit GUI.</param>
        public cwWebDesignerTreeNodeSite(cwWebDesignerGUI _cwWebIndexEditGUI)
            : base(_cwWebIndexEditGUI)
        {
            updateText("Site");
            operationName = "webDesigner";
            addChildNodeLast(new cwWebDesignerTreeNodePagesIndex(getGUI<cwWebDesignerGUI>(), this));
            addChildNodeLast(new cwWebDesignerTreeNodePagesSingle(getGUI<cwWebDesignerGUI>(), this));
            addChildNodeLast(new cwWebDesignerTreeNodeTemplateNodes(getGUI<cwWebDesignerGUI>(), this));
            setIconForNodeUsingIndex(0);
        }

        /// <summary>
        /// Sets the properties boxes.
        /// </summary>
        public override void setPropertiesBoxes()
        {
            this.propertiesBoxes.addPropertyBox(new cwPSFPropertyBoxFolder("Output Folder", "The publication folder path (absolute path : could not use http://, write permission is required on hard drive)", CONFIG_SITE_PATH));
            this.propertiesBoxes.addPropertyBox(new cwPSFPropertyBoxString("Media Path", "The media path from the main, should be ../, or set a absolute path from using http://, should finish with /", CONFIG_MEDIA_PATH));
            this.propertiesBoxes.getPropertyBox(CONFIG_MEDIA_PATH).setValue("../");
            this.propertiesBoxes.addPropertyBox(new cwPSFPropertyBoxString("JSON Path", "The json path from the main, should be ../, or set a absolute path from using http://, should finish with /", CONFIG_MEDIA_PATH));
            this.propertiesBoxes.getPropertyBox(CONFIG_MEDIA_PATH).setValue("../");
            this.propertiesBoxes.addPropertyBox(new cwPSFPropertyBoxString("Link Extension MimeType", "The mimeType used for the links can be html/aspx ...", CONFIG_SITE_LINK_MIMETYPE));
            this.propertiesBoxes.getPropertyBox(CONFIG_SITE_LINK_MIMETYPE).setValue("html");
            this.propertiesBoxes.addPropertyBox(new cwPSFPropertyBoxCheckBox("Export Validated Templates", "Export the validated tempaltes in order to have the json file template[type_abbreviation].json available on the generated/diagram/json/ folder for dynamic diagramming", CONFIG_EXPORT_VALIDATED_TEMPLATES, false));
            this.propertiesBoxes.addPropertyBox(new cwPSFPropertyBoxCheckBox("Export Diagram Images", "Export the images of diagram in order to have folder for diagram images (png files)", CONFIG_EXPORT_DIAGRAM_IMAGES, false));
            this.propertiesBoxes.addPropertyBox(new cwPSFPropertyBoxComboBox("Language", "The language of the website", CONFIG_LANGUAGE_CHOOSED, new string[] { "en", "fr" }));
            this.propertiesBoxes.addPropertyBox(new cwPSFPropertyBoxString("JSON Extension", "The extention for the json file could be json or cwjson for sharepoint users, the mime type should be set to application/json in the IIS server", CONFIG_JSON_EXTENTION));
            this.propertiesBoxes.getPropertyBox(CONFIG_JSON_EXTENTION).setValue("json");
            this.propertiesBoxes.addPropertyBox(new cwPSFPropertyBoxString("WebDesigner Server URL", "The URL where WebDesignerAPI is installed (example : http://localhost/webDesignerAPI/), leave this field empty if generated files should be used instead of having live data fetched from the server", CONFIG_WEBDESIGNER_SERVER_URL));
            this.propertiesBoxes.addPropertyBox(new cwPSFPropertyBoxComboBox("Javascript Mode", "The javascript method to call libs", CONFIG_JS_MODE, new string[] { "min", "pretty", "concat", "debug" }));
            this.propertiesBoxes.getPropertyBox(CONFIG_JS_MODE).setValue("min");
            this.propertiesBoxes.addPropertyBox(new cwPSFPropertyBoxCheckBox("Run In Portal", "Check this option if you are using the webdesigner in the Portal, in order to have Portal links", CONFIG_RUN_IN_PORTAL, false));
            this.propertiesBoxes.addPropertyBox(new cwPSFPropertyBoxCheckBox("Generate Diagram Hierachy ", "Generate the diagram hierachy in a json file", CONFIG_GENERATE_DIAGRAM_HIERARCHY, false));
            this.propertiesBoxes.addPropertyBox(new cwPSFPropertyBoxCheckBox("UUID As Diagram File Name ", "CP generated diagram file name is based on the diagram uuid, otherwise, the name is based on the ID",CONFIG_UUID_AS_DIAGRAMFILENAME , true));
           
            base.setPropertiesBoxes();
        }


        public string JSMode
        {
            get {
                return this.getStringProperty(cwWebDesignerTreeNodeSite.CONFIG_JS_MODE);
            }
        }
        /// <summary>
        /// Gets the is run in portal.
        /// </summary>
        public bool isRunningInPortal
        {
            get
            {
                return this.propertiesBoxes.getPropertyBoxBoolean(CONFIG_RUN_IN_PORTAL).Checked;
            }
        }

        public override void doCustomActionsAfterNodeHasBeenLoaded(cwLightObject rootObject)
        {
        }

        public override void doCustomActionsAfterNodeHasBeenShown()
        { }

        public override bool checkNodeStructureCustom()
        {
            return true;
        }

        /// <summary>
        /// Gets the JSON extention.
        /// </summary>
        /// <returns></returns>
        public string getJSONExtention()
        {
            return getStringProperty(CONFIG_JSON_EXTENTION);
        }

        /// <summary>
        /// Creates the context menu.
        /// </summary>
        /// <param name="treeView">The tree view.</param>
        /// <param name="position">The position.</param>
        public override void createContextMenu(TreeView treeView, Point position)
        {
            menu_strip.Items.Clear();
            base.createContextMenu(treeView, position);

            ToolStripItem generateValidatedTemplates_item = menu_strip.Items.Add("Generate Validated Templates");
            generateValidatedTemplates_item.Click += new System.EventHandler(this.ctx_generateValidatedTemplatesToolStripMenuItem_Click);

            ToolStripItem generateSite_item = menu_strip.Items.Add("Generate Site Now");
            generateSite_item.Click += new System.EventHandler(this.ctx_generateSiteToolStripMenuItem_Click);

            ToolStripItem generateSiteHierarchy_item = menu_strip.Items.Add("Generate Site Hierarchy Now");
            generateSiteHierarchy_item.Click += new System.EventHandler(this.ctx_generateSiteHierarchyToolStripMenuItem_Click);

            ToolStripItem globalFilesHierarchy_item = menu_strip.Items.Add("Generate Global Files Now");
            globalFilesHierarchy_item.Click += new System.EventHandler(this.ctx_generateGlobalFilesToolStripMenuItem_Click);

            ToolStripItem saveBatchFile_item = menu_strip.Items.Add("Copy batch script into clipboard");
            saveBatchFile_item.Click += new System.EventHandler(this.ctx_saveBatchFileToolStripMenuItem_Click);

            menu_strip.Show(treeView, position);
        }

        private void ctx_saveBatchFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                cwLightObjectType OT = operationEditModeGUI.Model.getObjectTypeByScriptName(webDesignerOperation.OBJECTTYPE_NAME_IN_CM);
                ICMConnection c = operationEditModeGUI.Model.getConnection();

                cwLightObject webDesignObject = OT.getObjectsByFilter(new string[] { "DESCRIPTION", cwLightObject.UNIQUEIDENTIFIER }.ToList<string>(), cwLightObject.UNIQUEIDENTIFIER, getStringProperty(CONFIG_NODE_UUID), "=").First();
                string bInstruction = Assembly.GetExecutingAssembly().GetName().Name + ".exe -connection set_your_connection_here -model " + operationEditModeGUI.Model.FileName + " -username " + c.ConnectionList.UserName + " -password set_your_password_here -id " + webDesignObject.ID.ToString();
                Clipboard.SetText(bInstruction);
                MessageBox.Show(bInstruction + "\n\nPaste it anywhere (Ctrl + V).\nIf your password is empty, just use \"\".", "The batch script has been copied into the clipboard");
            }
            catch (Exception exception)
            {
                cwPSFTreeNode.log.Error("Error while generating validated templates");
                cwPSFTreeNode.log.Error(exception.ToString());
            }
        }
        ///// <summary>
        ///// Handles the Click event of the addSiteStructureToolStripMenuItem control.
        ///// </summary>
        ///// <param name="sender">The source of the event.</param>
        ///// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ctx_generateValidatedTemplatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime start = DateTime.Now;
                operationEditModeGUI.appendInfo("Start generating validated templates...");
                webDesignerOperation _webDesigner = new webDesignerOperation(operationEditModeGUI.Model, this);
                _webDesigner.addValidatedTemplatesToDiagramsToLoad();
                //_webDesigner.exportDiagrams();
                operationEditModeGUI.appendInfo("Validated Templates generated in " + DateTime.Now.Subtract(start).ToString() + "s.");
            }
            catch (Exception exception)
            {
                cwPSFTreeNode.log.Error("Error while generating validated templates");
                cwPSFTreeNode.log.Error(exception.ToString());
            }
        }

        /// <summary>
        /// Handles the Click event of the ctx_generateGlobalFilesToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ctx_generateGlobalFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime start = DateTime.Now;
                operationEditModeGUI.appendInfo("Start generating global files...");
                webDesignerOperation _webDesigner = new webDesignerOperation(operationEditModeGUI.Model, this);
                _webDesigner.GlobalFiles.createGlobalFiles();
                operationEditModeGUI.appendInfo("Site global files generated in " + DateTime.Now.Subtract(start).ToString() + "s.");
            }
            catch (Exception exception)
            {
                appendError(exception.Message.ToString());
                cwPSFTreeNode.log.Error("Error while generating global files");
                cwPSFTreeNode.log.Error(exception.ToString());
            }
        }

        /// <summary>
        /// Handles the Click event of the ctx_generateSiteHierarchyToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ctx_generateSiteHierarchyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime start = DateTime.Now;
                operationEditModeGUI.appendInfo("Start generating site hierarchy...");
                webDesignerOperation _webDesigner = new webDesignerOperation(operationEditModeGUI.Model, this);
                _webDesigner.GlobalFiles.createFileJSONPages();
                operationEditModeGUI.appendInfo("Site hierarchy generated in " + DateTime.Now.Subtract(start).ToString() + "s.");
            }
            catch (Exception exception)
            {
                cwPSFTreeNode.log.Error("Error while generating the site hierarchy");
                cwPSFTreeNode.log.Error(exception.ToString());
            }
        }
        ///// <summary>
        ///// Handles the Click event of the addSiteStructureToolStripMenuItem control.
        ///// </summary>
        ///// <param name="sender">The source of the event.</param>
        ///// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ctx_generateSiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime start = DateTime.Now;
                operationEditModeGUI.appendInfo("Start generating the whole site...");
                webDesignerOperation _webDesigner = new webDesignerOperation(operationEditModeGUI.Model, this);
                _webDesigner.Do(operationEditModeGUI.Model);
                operationEditModeGUI.appendInfo("Site generated in " + DateTime.Now.Subtract(start).ToString() + "s.");
            }
            catch (Exception exception)
            {
                operationEditModeGUI.appendError("Unable to perform the requested operation because " + exception.Message.ToString());
                cwPSFTreeNode.log.Error("Error while generating site templates");
                cwPSFTreeNode.log.Error(exception.ToString());
            }
        }
    }
}
