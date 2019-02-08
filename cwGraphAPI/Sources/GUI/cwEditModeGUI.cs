using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Casewise.GraphAPI.PSF;
using Casewise.GraphAPI.Exceptions;
using System.Reflection;
using log4net;
using Casewise.GraphAPI.API;
using Casewise.GraphAPI.GUI;
using log4net.Core;
using Casewise.GraphAPI.ProgramManager;
using System.Configuration;

namespace Casewise.GraphAPI.GUI
{
    /// <summary>
    /// 
    /// </summary>
    public partial class cwEditModeGUI : Form
    {

        private static readonly ILog log = LogManager.GetLogger(typeof(cwEditModeGUI));

        private cwLightModel _model = null;
        /// <summary>
        /// rootNode
        /// </summary>
        public cwPSFTreeNodeConfigurationNode rootNode = null;
        private Dictionary<string, List<string>> enableMove = new Dictionary<string, List<string>>();

        /// <summary>
        /// options
        /// </summary>
        public cwProgramManagerOptions options = new cwProgramManagerOptions();

        /// <summary>
        /// Initializes a new instance of the <see cref="cwEditModeGUI"/> class.
        /// </summary>
        /// <param name="_model">The _model.</param>
        /// <param name="options">The options.</param>
        public cwEditModeGUI(cwLightModel _model, cwProgramManagerOptions options)
            : base()
        {
            InitializeComponent();
            this._model = _model;
            this.options = options;
            this.Icon = Properties.Resources.Casewise_Icon;
            toolTip = new cwPSFToolTip(this.Font);
            treeViewConfigurations.ItemDrag += new ItemDragEventHandler(treeView_ItemDrag);
            treeViewConfigurations.DragEnter += new DragEventHandler(treeView_DragEnter);
            treeViewConfigurations.DragDrop += new DragEventHandler(treeView_DragDrop);
            treeViewConfigurations.DragOver += new DragEventHandler(treeView_DragOver);
            treeViewConfigurations.DragLeave += new EventHandler(treeView_DragLeave);
            log.Debug("Edit mode GUI Created");

            groupBoxDesigns.Text = Properties.Resources.GUI_EDITMODE_GROUPBOXDESIGN_TEXT;
            groupBoxDetails.Text = Properties.Resources.GUI_EDITMODE_GROUPBOXOPTIONS_TEXT;

            saveToolStripMenuItem.Text = Properties.Resources.PSF_TN_CTX_SAVE;
            helpToolStripMenuItem.Text = Properties.Resources.LAUNCHER_OPTIONS_BUTTONS_HELP_TOOLTIP;

            try {
                autoSaveToolStripMenuItem.Checked = Properties.Settings.Default.optionAutoSave;
                checkSaveToolStripMenuItem();
            }
            catch (ConfigurationErrorsException e)
            {
                log.Error(e);
            }
            
       
           
          
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="cwEditModeGUI"/> class.
        /// </summary>
        public cwEditModeGUI()
        { }
        /// <summary>
        /// Gets the model.
        /// </summary>
        public cwLightModel Model
        {
            get { return _model; }
        }
        /// <summary>
        /// Adds the drag and drop for.
        /// </summary>
        /// <param name="sourceType">Type of the source.</param>
        /// <param name="acceptedTargetStyle">The accepted target style.</param>
        protected void addDragAndDropFor(string sourceType, string acceptedTargetStyle)
        {
            if (!enableMove.ContainsKey(sourceType))
            {
                enableMove[sourceType] = new List<string>();
            }
            if (!enableMove[sourceType].Contains(acceptedTargetStyle))
            {
                enableMove[sourceType].Add(acceptedTargetStyle);
            }

        }

        /// <summary>
        /// Adds the details component.
        /// </summary>
        /// <param name="component">The component.</param>
        public void addDetailsComponent(Control component)
        {
            if (groupBoxDetails.Controls.Count.Equals(0))
            {
                component.Location = new Point(5, 20);
            }
            groupBoxDetails.Controls.Add(component);
        }

        /// <summary>
        /// Appends the info.
        /// </summary>
        /// <param name="message">The message.</param>
        public void appendInfo(string message)
        {
            richTextBoxDebug.AppendText(message + "\n");
            richTextBoxDebug.ScrollToCaret();
            richTextBoxDebug.Refresh();
        }

        /// <summary>
        /// Appends the info.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="color">The color.</param>
        public void appendInfo(string message, Color color)
        {
            richTextBoxDebug.SelectionStart = richTextBoxDebug.TextLength;
            richTextBoxDebug.SelectionLength = 0;

            richTextBoxDebug.SelectionColor = color;
            appendInfo(message);
            richTextBoxDebug.SelectionColor = richTextBoxDebug.ForeColor;
        }


        /// <summary>
        /// Appends the error.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="onNode">The on node.</param>
        public void appendError(string message, cwPSFTreeNode onNode)
        {
            appendInfo(String.Format(Properties.Resources.GUI_APPEND_ERROR, message, onNode.ToString(), onNode.GetType().Name), Color.Red);
        }


        /// <summary>
        /// Appends the info.
        /// </summary>
        /// <param name="message">The message.</param>
        public void appendError(string message)
        {
            richTextBoxDebug.AppendText(message + "\n");
            richTextBoxDebug.ScrollToCaret();
            log.Error(message);
        }

        /// <summary>
        /// Appends the info.
        /// </summary>
        /// <param name="exception">The exception.</param>
        public void appendError(Exception exception)
        {
            richTextBoxDebug.AppendText(exception.Message + "\n");
            richTextBoxDebug.ScrollToCaret();
            log.Error(exception.ToString());
        }

        /// <summary>
        /// Suspends the layout.
        /// </summary>
        public void suspendLayout()
        {
            groupBoxDetails.SuspendLayout();
            SuspendLayout();
        }

        /// <summary>
        /// Resumes the layout.
        /// </summary>
        public void resumeLayout()
        {
            ResumeLayout();
            groupBoxDetails.ResumeLayout();

        }

        /// <summary>
        /// Creates the root node.
        /// </summary>
        /// <param name="treeView">The tree view.</param>
        /// <param name="position">The position.</param>
        public virtual void createRootNode(TreeView treeView, Point position)
        {
            cwPSFContextMenuStrip menu_strip = new cwPSFContextMenuStrip(this.Font);
            menu_strip.Items.Clear();
            ToolStripItem add_configurationItem = menu_strip.Items.Add("Add a new Configuration");
            add_configurationItem.Click += new System.EventHandler(this.add_configurationItemToolStripMenuItem_Click);
            menu_strip.Show(treeView, position);
        }


        /// <summary>
        /// Handles the Click event of the add_configurationItemToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        public virtual void add_configurationItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException("add_configurationItemToolStripMenuItem_Click should be implemented");
        }




        /// <summary>
        /// Loads the tree view.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="m">The m.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="objectID">The object ID.</param>cwPSFTreeNodeConfigurationNode
        public T loadTreeView<T>(cwLightModel m, string objectType, int objectID) where T : cwPSFTreeNodeConfigurationNode
        {
            try
            {
                cwLightObjectType OTWD = m.getObjectTypeByScriptName(objectType);

                foreach (cwLightObject siteDesign in OTWD.getObjectsByFilter(new string[] { "NAME", "DESCRIPTION", cwLightObject.UNIQUEIDENTIFIER, "TYPE" }.ToList<string>(), "ID", objectID.ToString(), "="))
                {
                    try
                    {
                        Type classType = typeof(T);
                        ConstructorInfo classConstructor = classType.GetConstructor(new Type[] { this.GetType() });
                        T tn = classConstructor.Invoke(new object[] { this }) as T;
                        if (tn == null)
                        {
                            throw new cwExceptionFatal("Unable to create " + classType.Name + " while loading tree view");
                        }
                        DateTime start = DateTime.Now;

                        string descriptionValue = siteDesign.properties["DESCRIPTION"];
                        tn.repositoryObject = siteDesign;
                        if (!0.Equals(descriptionValue.Length))
                        {
                            tn.loadFromXMLContent(descriptionValue);
                        }
                        tn.updateText(siteDesign.properties["NAME"]);
                        tn.propertiesBoxes.getPropertyBox(tn.CONFIG_NODE_NAME).disable();
                        tn.propertiesBoxes.getPropertyBox(cwPSFTreeNodeConfigurationNode.CONFIG_NODE_UUID).setValue(siteDesign.properties[cwLightObject.UNIQUEIDENTIFIER]);

                        tn.doCustomActionsAfterNodeHasBeenLoaded(siteDesign);
                        addRootNode(tn);
                        return tn;
                    }
                    catch (Exception e)
                    {
                        log.Error(e.ToString());
                    }
                }
            }
            catch (Exception e)
            {
                log.Error(e.ToString());
            }
            return null;
        }


        /// <summary>
        /// Adds the root node.
        /// </summary>
        /// <param name="rootTreeNode">The root tree node.</param>
        public void addRootNode(cwPSFTreeNodeConfigurationNode rootTreeNode)
        {
            treeViewConfigurations.Nodes.Add(rootTreeNode);
            rootNode = rootTreeNode;
        }

        /// <summary>
        /// Trees the view mouse up.
        /// </summary>
        /// <param name="treeView">The tree view.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        protected void treeViewMouseUp(TreeView treeView, MouseEventArgs e)
        {
            try
            {
                // Point where the mouse is clicked.
                Point p = new Point(e.X, e.Y);

                TreeNode locatedTreeNode = treeView.GetNodeAt(p);
                cwPSFTreeNode selected_node = locatedTreeNode as cwPSFTreeNode;

                if (selected_node != null)
                {
                    // Show menu only if the right mouse button is clicked.
                    if (e.Button == MouseButtons.Right)
                    {
                        selected_node.createContextMenu(treeView, p);
                    }
                    else
                    {
                        selected_node.showDetailsComponents();
                        selected_node.OnClick();
                    }
                }
                else
                {
                    // do nothing, can't add from here
                }
            }
            catch (Exception exception)
            {
                log.Error(exception.Message.ToString() + exception.ToString());
            }
        }

        /// <summary>
        /// Handles the MouseUp event of the treeViewConfigurations control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        private void treeViewConfigurations_MouseUp(object sender, MouseEventArgs e)
        {
            treeViewMouseUp(treeViewConfigurations, e);
        }




        /// <summary>
        /// Sets the properties box table.
        /// </summary>
        /// <param name="_tableLayoutPanelDetails">The _table layout panel details.</param>
        internal void setPropertiesBoxTable(cwPSFTableLayoutPropertiesBoxes _tableLayoutPanelDetails)
        {
            if (panelOptions.Controls.Count > 0)
            {
                Control oldBoxes = panelOptions.Controls[0];
                oldBoxes.SuspendLayout();
                panelOptions.Controls.Clear();
                oldBoxes.ResumeLayout();
            }
            panelOptions.Controls.Add(_tableLayoutPanelDetails);
        }


        private void treeView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            treeViewConfigurations.DoDragDrop(e.Item, DragDropEffects.Move);
        }

        private void treeView_DragLeave(object sender, EventArgs e)
        {

        }

        private void treeView_DragOver(object sender, DragEventArgs e)
        {
            foreach (var enableMoveVar in enableMove)
            {
                if (canMove(sender, e, enableMoveVar.Key, enableMoveVar.Value))
                {
                    return;
                }
            }
        }

        private void treeView_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }


        private bool canMove(object sender, DragEventArgs e, string sourceType, List<string> acceptedParentTypes)
        {

            if (e.Data.GetDataPresent(sourceType, false))
            {
                //guiHelper.add_info("SOURCE OK", sourceType);
                Point pt = ((TreeView)sender).PointToClient(new Point(e.X, e.Y));
                TreeNode DestinationNode = ((TreeView)sender).GetNodeAt(pt);

                TreeNode sourceNode = ((TreeNode)e.Data.GetData(sourceType));
                if (sourceNode == DestinationNode)
                {
                    //guiHelper.add_info("NODE IS SAME");
                    return false;
                }

                Type destinationType = DestinationNode.GetType();


                if (!acceptedParentTypes.Contains(destinationType.Name))
                {
                    //guiHelper.add_info("BAD DESTINATION", destinationType.ToString());
                    e.Effect = DragDropEffects.None;
                }
                else
                {
                    //guiHelper.add_info("GOOD DESTINATION", destinationType.ToString());
                    e.Effect = DragDropEffects.Move;
                    return true;
                }
            }
            else
            {
                //guiHelper.add_info("WRONG SOURCE", sourceType);
                e.Effect = DragDropEffects.None;
            }
            return false;
        }

        private void moveIfPossible(object sender, DragEventArgs e, string sourceType, List<string> acceptedParentTypes)
        {
            if (e.Data.GetDataPresent(sourceType, false))
            {
                Point pt = ((TreeView)sender).PointToClient(new Point(e.X, e.Y));
                cwPSFTreeNode DestinationNode = ((TreeView)sender).GetNodeAt(pt) as cwPSFTreeNode;
                Type destinationType = DestinationNode.GetType();
                if (!acceptedParentTypes.Contains(destinationType.Name))
                {
                    return;
                }
                DestinationNode.BackColor = Color.Aqua;
                cwPSFTreeNode NewNode = (cwPSFTreeNode)e.Data.GetData(sourceType);
                cwPSFTreeNode parentNode = NewNode.getParent();
                parentNode.removeChild(NewNode);

                //NewNode.Remove();
                DestinationNode.addChildNodeLast(NewNode);
                //DestinationNode.Nodes.Add(NewNode);
                DestinationNode.Expand();
                DestinationNode.BackColor = Color.White;
            }
        }

        private void treeView_DragDrop(object sender, DragEventArgs e)
        {
            foreach (var enableMoveVar in enableMove)
            {
                moveIfPossible(sender, e, enableMoveVar.Key, enableMoveVar.Value);
            }
        }


        private cwPSFToolTip toolTip = null;
        private void treeViewConfigurations_MouseMove(object sender, MouseEventArgs e)
        {
            TreeNode theNode = treeViewConfigurations.GetNodeAt(e.X, e.Y);
            // Définir une info-bulle uniquement si le pointeur de la souris est en fait placé sur le noeud.
            if (theNode != null)
            {
                // Vérifier que la propriété Tag n'est pas "null".
                if (theNode.Tag != null)
                {
                    // Modifier l'info-bulle uniquement si le pointeur a été déplacé vers un nouveau noeud.
                    string toolTipString = toolTip.GetToolTip(treeViewConfigurations);
                    string nodeName = theNode.Tag.ToString();
                    if (!nodeName.Equals(toolTipString))
                    {
                        //toolTip.RemoveAll();
                        toolTip.showTooltip(theNode.Tag.ToString(), theNode.Bounds.Location, new Size(theNode.Bounds.Width, theNode.Bounds.Height), treeViewConfigurations);
                    }
                }
                else
                {
                    toolTip.hideTooltip(treeViewConfigurations);
                }
            }
            else
            {
                toolTip.hideTooltip(treeViewConfigurations);
            }
        }

        private void cwEditModeGUI_Shown(object sender, EventArgs e)
        {
            if (treeViewConfigurations.Nodes.Count > 0)
            {
                cwPSFTreeNodeConfigurationNode rootNode = treeViewConfigurations.Nodes[0] as cwPSFTreeNodeConfigurationNode;
                rootNode.doCustomActionsAfterNodeHasBeenShown();
            }
        }

        /// <summary>
        /// Handles the Click event of the autoSaveToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void autoSaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            checkSaveToolStripMenuItem();
        }

        /// <summary>
        /// Checks the save tool strip menu item.
        /// </summary>
        public void checkSaveToolStripMenuItem()
        {
            Properties.Settings.Default.optionAutoSave = autoSaveToolStripMenuItem.Checked;
            Properties.Settings.Default.Save();
            if (Properties.Settings.Default.optionAutoSave)
            {
                saveToolStripMenuItem.Visible = false;
            }
            else
            {
                saveToolStripMenuItem.Visible = true;
            }
        }

        /// <summary>
        /// rootNodeHashCode
        /// </summary>
        protected int rootNodeHashCode = 0;


        /// <summary>
        /// Autoes the save.
        /// </summary>
        public void autoSaveIfRequired()
        {
            if (Casewise.GraphAPI.Properties.Settings.Default.optionAutoSave)
            {
                saveIfRequired();
                saveToolStripMenuItem.Enabled = false;
            }
            else
            {
                if (isSaveRequired())
                {
                    saveToolStripMenuItem.Enabled = true;
                }
            }
        }


        /// <summary>
        /// Determines whether [is save required].
        /// </summary>
        /// <returns>
        ///   <c>true</c> if [is save required]; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool isSaveRequired()
        {
            if (rootNode == null) return false;
            int newHashCode = rootNode.GetHashCode();
            return !newHashCode.Equals(rootNodeHashCode);
        }
        /// <summary>
        /// Saves this instance.
        /// </summary>
        public void saveIfRequired()
        {
            saveToolStripMenuItem.Enabled = true;
            if (isSaveRequired())
            {
                int newHashCode = rootNode.GetHashCode();
                rootNode.saveItem();
                rootNodeHashCode = newHashCode;
            }
            saveToolStripMenuItem.Enabled = false;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveIfRequired();
        }

        private void cwEditModeGUI_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (isSaveRequired())
            {
                DialogResult answer = MessageBox.Show(Properties.Resources.TEXT_SAVEREQUIRED_ON_CLOSE, Properties.Resources.TEXT_WARNING, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if ("Yes".Equals(answer.ToString()))
                {
                    saveIfRequired();
                }
            }
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(options.helpURL);            
        }
    }
}
