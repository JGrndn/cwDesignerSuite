using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Casewise.GraphAPI.GUI;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Drawing;
using Casewise.GraphAPI.API;

namespace Casewise.GraphAPI.PSF
{
    /// <summary>
    /// Root node which is linked to the corporate exchange item
    /// </summary>
    public class cwPSFTreeNodeConfigurationNode : cwPSFTreeNode
    {
        /// <summary>
        /// The name of the operation where the node is linked to
        /// </summary>
        public String operationName = "";
        private cwLightObject _repositoryObject = null;

        /// <summary>
        /// The node UUID Key
        /// </summary>
        public static string CONFIG_NODE_UUID = "node-uuid";

        /// <summary>
        /// Initializes a new instance of the <see cref="cwPSFTreeNodeConfigurationNode"/> class.
        /// </summary>
        /// <param name="_operationEditModeGUI">The _operation edit mode GUI.</param>
        public cwPSFTreeNodeConfigurationNode(cwEditModeGUI _operationEditModeGUI)
            : base(_operationEditModeGUI, null)
        {
        }


        /// <summary>
        /// Gets or sets the repository object.
        /// </summary>
        /// <value>
        /// The repository object.
        /// </value>
        public cwLightObject repositoryObject
        {
            get { return _repositoryObject; }
            set
            {
                _repositoryObject = value;
            }
        }



        /// <summary>
        /// Gets the UUID.
        /// </summary>
        public string UUID
        {
            get
            {
                return propertiesBoxes.getPropertyBox(CONFIG_NODE_UUID).ToString();
            }
        }

        /// <summary>
        /// Does the custom actions after node has been loaded.
        /// </summary>
        public virtual void doCustomActionsAfterNodeHasBeenLoaded(cwLightObject rootObject)
        {
            throw new NotImplementedException("doCustomActionsAfterNodeHasBeenLoaded");
        }

        /// <summary>
        /// Does the custom actions after node has been shown.
        /// </summary>
        public virtual void doCustomActionsAfterNodeHasBeenShown()
        {
            throw new NotImplementedException("doCustomActionsAfterNodeHasBeenShown");
        }

        /// <summary>
        /// Creates the context menu.
        /// </summary>
        /// <param name="treeView">The tree view.</param>
        /// <param name="position">The position.</param>
        public override void createContextMenu(TreeView treeView, Point position)
        {
            if (checkNodeStructureRec())
            {
                ToolStripItem save_item = menu_strip.Items.Add(Properties.Resources.PSF_TN_CTX_SAVE);
                save_item.Image = Properties.Resources.image_tvicon_save;
                save_item.Click += new System.EventHandler(this.ctx_saveConfigurationToolStripMenuItem_Click);
            }
            base.createContextMenu(treeView, position);
        }

        /// <summary>
        /// Sets the properties boxes.
        /// </summary>
        public override void setPropertiesBoxes()
        {
            this.propertiesBoxes.addPropertyBox(new cwPSFPropertyBoxString(Properties.Resources.PSF_CONFIGURATION_NODE_NODEUUID_NAME, Properties.Resources.PSF_CONFIGURATION_NODE_NODEUUID_HELP, CONFIG_NODE_UUID));
            propertiesBoxes.getPropertyBox(CONFIG_NODE_UUID).disable();

        }

        /// <summary>
        /// Saves the item.
        /// </summary>
        public void saveItem()
        {
            try
            {
                if (_repositoryObject != null)
                {
                    cwLightObjectType OT = _repositoryObject.getObjectType();
                    _repositoryObject.freeze(0);
                    _repositoryObject.properties["EXPORTFLAG"] = "1";
                    _repositoryObject.properties["DESCRIPTION"] = this.toXML();
                    _repositoryObject.updatePropertiesInModel();
                    //operationEditModeGUI.appendInfo(Properties.Resources.PSF_CONFIGURATION_NODE_SAVE_OK);
                }
            }
            catch (Casewise.Data.ICM.ICMException _exception)
            {
                if (_exception.Code.Equals(Casewise.Data.ICM.ICMException.Reason.COM_EXCEPTION))
                {
                    appendError(Casewise.GraphAPI.Properties.Resources.PSF_DBLINK_ERROR);
                }
            }
            catch (Exception _exception)
            {
                operationEditModeGUI.appendError(_exception.Message.ToString());
                cwPSFTreeNode.log.Error(_exception.ToString());
            }
        }

        /// <summary>
        /// Handles the Click event of the saveConfigurationToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        public void ctx_saveConfigurationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveItem();
        }


        /// <summary>
        /// Gets the XML.
        /// </summary>
        /// <returns></returns>
        public string toXML()
        {
            StringBuilder b = new StringBuilder();

            XmlWriterSettings xml_settings = new XmlWriterSettings();
            xml_settings.Indent = true;
            Encoding myUTF8 = new UTF8Encoding(false);
            xml_settings.Encoding = myUTF8;
            // create the xml write
            XmlWriter xmlwriter = XmlWriter.Create(b);
            // start the document
            xmlwriter.WriteStartDocument(true);

            this.saveInConfigurationFile(xmlwriter);

            xmlwriter.WriteEndDocument();
            xmlwriter.Flush();
            xmlwriter.Close();
            return b.ToString();
        }

        /// <summary>
        /// Gets the configuration file path.
        /// </summary>
        /// <returns></returns>
        private string getConfigurationFilePath()
        {
            string fileSavePath = getName() + ".xml";
            fileSavePath = operationName + "/" + cwTools.stringToFile(fileSavePath);
            return fileSavePath;
        }


    }
}
