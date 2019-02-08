using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Xml;
using System.ComponentModel;
using Casewise.GraphAPI.GUI;
using Casewise.GraphAPI.Exceptions;
using System.Reflection;
using Casewise.GraphAPI.PSF;
using log4net;
using log4net.Core;
using Casewise.GraphAPI.API;
using System.Runtime.Serialization;

namespace Casewise.GraphAPI.PSF
{
    /// <summary>
    /// The virtual cwDesigner TreeNode
    /// </summary>
    public class cwPSFTreeNode : TreeNode, ICloneable, ISerializable
    {

        /// <summary>
        /// The logger
        /// </summary>
        public static readonly ILog log = LogManager.GetLogger(typeof(cwPSFTreeNode));
        internal Dictionary<string, List<cwPSFTreeNode>> childrenNodes = new Dictionary<string, List<cwPSFTreeNode>>();
        /// <summary>
        /// The properties container
        /// </summary>
        public cwPSFPropertiesBoxes propertiesBoxes = null;
        /// <summary>
        /// init the nodeType to notSet value
        /// </summary>
        public ContextMenuStrip menu_strip = null;
        /// <summary>
        /// 
        /// </summary>
        public cwEditModeGUI operationEditModeGUI = null;

        /// <summary>
        /// The name of a node
        /// </summary>
        public string CONFIG_NODE_NAME = "node_name";

        /// <summary>
        /// the name of a layout
        /// </summary>
        public string layoutName = "";


        /// <summary>
        /// tableLayoutPanelDetails
        /// </summary>
        public cwPSFTableLayoutPropertiesBoxes tableLayoutPanelDetails = new cwPSFTableLayoutPropertiesBoxes();
        private cwPSFTreeNode parent = null;

        /// <summary>
        /// backgroundColor
        /// </summary>
        protected Color backgroundColor = Color.White;

        /// <summary>
        /// Initializes a new instance of the <see cref="cwPSFTreeNode"/> class.
        /// </summary>
        /// <param name="_operationEditModeGUI">The _operation edit mode GUI.</param>
        /// <param name="_parent">The _parent.</param>
        public cwPSFTreeNode(cwEditModeGUI _operationEditModeGUI, cwPSFTreeNode _parent)
            : base()
        {
            parent = _parent;
            ToolTipText = "PSF";
            propertiesBoxes = new cwPSFPropertiesBoxes(this);
            operationEditModeGUI = _operationEditModeGUI;
            menu_strip = new cwPSFContextMenuStrip(operationEditModeGUI.Font);

            cwPSFPropertyBox TB_NodeName = new cwPSFPropertyBoxString(Properties.Resources.PSF_TN_NODE_NAME_NAME, Properties.Resources.PSF_TN_NODE_NAME_HELP, CONFIG_NODE_NAME);
            TB_NodeName.TextChanged += new EventHandler(TB_NodeName_TextChanged);

            propertiesBoxes.addPropertyBox(TB_NodeName);
            updateText(Text);
            setPropertiesBoxes();
            propertiesBoxes.addPropertiesToTreeNodeDrawTableBox(tableLayoutPanelDetails);


        }

        /// <summary>
        /// Sets the tooltip error.
        /// </summary>
        /// <param name="message">The message.</param>
        public void setTooltipError(string message)
        {
            BackColor = Color.Red;
            ForeColor = Color.White;
            base.ToolTipText = message.ToString();
            expandParent_Rec();
            log.Debug(message);
        }



        /// <summary>
        /// Checks the property format.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected void checkPropertyFormat(string propertyName)
        {
            string error = String.Format(Casewise.GraphAPI.Properties.Resources.PSF_TN_FORMAT_ERROR_ON_PROPERTY, propertiesBoxes.getPropertyBox(propertyName).helpName);
            if (!propertiesBoxes.getPropertyBox(propertyName).checkFormat()) throw new cwExceptionNodeValidation(error, this);
        }

        /// <summary>
        /// Sets the index of the icon for node using.
        /// </summary>
        /// <param name="index">The index.</param>
        public void setIconForNodeUsingIndex(int index)
        {
            this.ImageIndex = index;
            this.SelectedImageIndex = index;
            this.StateImageIndex = index;
        }

        /// <summary>
        /// Appends the error.
        /// </summary>
        /// <param name="message">The message.</param>
        public void appendError(string message)
        {
            operationEditModeGUI.appendError(message, this);
        }

        /// <summary>
        /// Sets the color of the background.
        /// </summary>
        /// <param name="bgColor">Color of the bg.</param>
        protected void setBackgroundColor(Color bgColor)
        {
            BackColor = bgColor;
            backgroundColor = bgColor;
        }

        /// <summary>
        /// Appends the info.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="color">The color.</param>
        public void appendInfo(string message, Color color)
        {
            operationEditModeGUI.appendInfo(message, color);
        }

        /// <summary>
        /// Appends the info.
        /// </summary>
        /// <param name="message">The message.</param>
        public void appendInfo(string message)
        {
            operationEditModeGUI.appendInfo(message);
        }


        /// <summary>
        /// Expands the parent_ rec.
        /// </summary>
        public void expandParent_Rec()
        {
            if (getParent() != null)
            {
                getParent().Expand();
                getParent().expandParent_Rec();
            }
        }

        /// <summary>
        /// Gets the model.
        /// </summary>
        public cwLightModel Model
        {
            get
            {
                return operationEditModeGUI.Model;
            }
        }

        /// <summary>
        /// Disables the name.
        /// </summary>
        public void disableName()
        {
            propertiesBoxes.getPropertyBox(CONFIG_NODE_NAME).disable();
        }


        /// <summary>
        /// Add_s the add child menu item to context menu.
        /// </summary>
        /// <typeparam name="treeNode">The type of the ree node.</typeparam>
        /// <typeparam name="editGUI">The type of the dit GUI.</typeparam>
        /// <param name="text">The text.</param>
        /// <param name="icon">The icon.</param>
        protected void add_AddChildMenuItemToContextMenuFirst<treeNode, editGUI>(string text, Bitmap icon)
            where treeNode : cwPSFTreeNode
            where editGUI : cwEditModeGUI
        {
            ToolStripItem item = menu_strip.Items.Add(text);
            item.Image = icon;
            item.Click += new System.EventHandler(this.ctx_addChildNodeFirst<treeNode, editGUI>);
        }

        /// <summary>
        /// Add_s the add child menu item to context menu first.
        /// </summary>
        /// <typeparam name="treeNode">The type of the ree node.</typeparam>
        /// <typeparam name="editGUI">The type of the dit GUI.</typeparam>
        /// <param name="toolStrip">The tool strip.</param>
        /// <param name="text">The text.</param>
        /// <param name="icon">The icon.</param>
        protected void add_AddChildMenuItemToContextMenuFirst<treeNode, editGUI>(ToolStripMenuItem toolStrip, string text, Bitmap icon)
            where treeNode : cwPSFTreeNode
            where editGUI : cwEditModeGUI
        {
            ToolStripItem item = toolStrip.DropDownItems.Add(text);
            item.Image = icon;
            item.Click += new System.EventHandler(this.ctx_addChildNodeFirst<treeNode, editGUI>);
        }

        /// <summary>
        /// Add_s the add child menu item to context menu last.
        /// </summary>
        /// <typeparam name="treeNode">The type of the ree node.</typeparam>
        /// <typeparam name="editGUI">The type of the dit GUI.</typeparam>
        /// <param name="text">The text.</param>
        /// <param name="icon">The icon.</param>
        protected void add_AddChildMenuItemToContextMenuLast<treeNode, editGUI>(string text, Bitmap icon)
            where treeNode : cwPSFTreeNode
            where editGUI : cwEditModeGUI
        {
            ToolStripItem item = menu_strip.Items.Add(text);
            item.Image = icon;
            item.Click += new System.EventHandler(this.ctx_addChildNodeLast<treeNode, editGUI>);
        }

        /// <summary>
        /// Add_s the add child menu item to context menu last.
        /// </summary>
        /// <typeparam name="treeNode">The type of the ree node.</typeparam>
        /// <typeparam name="editGUI">The type of the dit GUI.</typeparam>
        /// <param name="toolStrip">The tool strip.</param>
        /// <param name="text">The text.</param>
        /// <param name="icon">The icon.</param>
        protected void add_AddChildMenuItemToContextMenuLast<treeNode, editGUI>(ToolStripMenuItem toolStrip, string text, Bitmap icon)
            where treeNode : cwPSFTreeNode
            where editGUI : cwEditModeGUI
        {
            ToolStripItem item = toolStrip.DropDownItems.Add(text);
            item.Image = icon;
            item.Click += new System.EventHandler(this.ctx_addChildNodeLast<treeNode, editGUI>);
        }


        /// <summary>
        /// Gets the parent.
        /// </summary>
        /// <returns></returns>
        public cwPSFTreeNode getParent()
        {
            return parent;
        }


        /// <summary>
        /// Handles the Click event of the ctx_addDesignTypeToolStripMenuItem control.
        /// </summary>
        /// <typeparam name="designType">The type of the esign type.</typeparam>
        /// <typeparam name="GUI">The type of the UI.</typeparam>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void ctx_addChildNodeFirst<designType, GUI>(object sender, EventArgs e)
            where designType : cwPSFTreeNode
            where GUI : cwEditModeGUI
        {
            Type typeOfDesignType = typeof(designType);
            object createdObject = Activator.CreateInstance(typeOfDesignType, new object[] { getGUI<GUI>(), this });
            designType requiredDesignType = createdObject as designType;
            addChildNodeFirst(requiredDesignType);
            checkNodeStructureRec();
        }

        /// <summary>
        /// Handles the addChildNodeLast event of the ctx control.
        /// </summary>
        /// <typeparam name="designType">The type of the esign type.</typeparam>
        /// <typeparam name="GUI">The type of the UI.</typeparam>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void ctx_addChildNodeLast<designType, GUI>(object sender, EventArgs e)
            where designType : cwPSFTreeNode
            where GUI : cwEditModeGUI
        {
            try
            {
                Type typeOfDesignType = typeof(designType);
                object createdObject = Activator.CreateInstance(typeOfDesignType, new object[] { getGUI<GUI>(), this });
                designType requiredDesignType = createdObject as designType;
                addChildNodeLast(requiredDesignType);
                checkNodeStructureRec();
            }
            catch (Exception exception)
            {
                log.Error(exception.ToString());
            }
        }

        /// <summary>
        /// Allows the drag and drop.
        /// </summary>
        protected virtual void allowDragAndDrop()
        {
        }

        /// <summary>
        /// Determines whether [has at least on child node] [the specified _child node name].
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>
        ///   <c>true</c> if [has at least on child node] [the specified _child node name]; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool hasAtLeastOnChildNode<T>() where T : cwPSFTreeNode
        {
            Type typeOfNode = typeof(T);
            string _childNodeName = typeOfNode.Name;
            if (childrenNodes.ContainsKey(_childNodeName)) return true;
            foreach (var childrenNodeVar in childrenNodes)
            {
                foreach (cwPSFTreeNode childrenNode in childrenNodeVar.Value)
                {
                    Type childType = childrenNode.GetType();
                    if (typeEqualsTo(childType, typeOfNode)) return true;
                    //childrenNode.
                    //if (_childNodeName.Equals(childType.Name)) return true;
                    //if (typeOfNode(_childNodeName, childType.BaseType.Name)) return true;
                    //if (childType.BaseType != null && _childNodeName.Equals(childType.BaseType.Name)) return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Gets the string property.
        /// </summary>
        /// <param name="propertyKey">The property key.</param>
        /// <returns></returns>
        public string getStringProperty(string propertyKey)
        {
            return propertiesBoxes.getPropertyBox(propertyKey).ToString();
        }

        /// <summary>
        /// Handles the Click event of the addObjectGroupToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected virtual void deleteMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult answer = MessageBox.Show(String.Format(Properties.Resources.PSF_TN_POPUP_DELETE_NODE_TITLE, Text), Properties.Resources.PSF_TN_POPUP_DELETE_NODE_TEXT, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if ("Yes".Equals(answer.ToString()))
                {
                    TreeNode _parent = this.Parent;
                    cwPSFTreeNode _parentNode = _parent as cwPSFTreeNode;
                    //    throw new cwExceptionNodeValidation(Properties.Resources.PSF_VN_CANT_DELETE_FROM_GUI, this);                 
                    if (_parentNode == null && this is cwPSFTreeNodeConfigurationNode)
                    {
                        cwPSFTreeNodeConfigurationNode rootNode = this as cwPSFTreeNodeConfigurationNode;
                        if (rootNode.repositoryObject != null)
                        {
                            rootNode.repositoryObject.freeze(0);
                            rootNode.repositoryObject.delete();
                            operationEditModeGUI.Close();
                        }
                    }
                    else
                    {
                        _parentNode.removeChild(this);
                        _parentNode.checkNodeStructureRec();
                    }
                }
            }
            catch (Exception exception)
            {
                appendError(exception.Message);
            }


        }

        /// <summary>
        /// Structures the rec is un valid.
        /// </summary>
        public virtual void structureRecIsUnValid()
        {

        }

        /// <summary>
        /// Structures the rec is valid.
        /// </summary>
        public virtual void structureRecIsValid()
        {

        }


        /// <summary>
        /// Checks the node structure rec.
        /// </summary>
        /// <returns></returns>
        public bool checkNodeStructureRec()
        {
            try
            {
                checkNodeStructure_Rec();
            }
            catch (cwExceptionNodeValidation e)
            {
                setTooltipError(e.Message);
                structureRecIsUnValid();
                return false;
            }
            structureRecIsValid();
            return true;
        }


        /// <summary>
        /// Checks the node structure_ rec.
        /// </summary>
        private void checkNodeStructure_Rec()
        {
            checkNodeStructure();
            foreach (cwPSFTreeNode node in getAllChildren())
            {
                node.checkNodeStructure_Rec();
            }
        }

        /// <summary>
        /// Resets the node validation.
        /// </summary>
        private void resetNodeValidation()
        {
            setBackgroundColor(backgroundColor);
            ForeColor = Color.Black;
            Tag = null;
        }

        /// <summary>
        /// Checks the node structure.
        /// </summary>
        public void checkNodeStructure()
        {
            if (checkNodeStructureCustom())
            {
                resetNodeValidation();
            }
        }

        /// <summary>
        /// Checks the node structure custom.
        /// </summary>
        /// <returns></returns>
        public virtual bool checkNodeStructureCustom()
        {
            throw new NotImplementedException(String.Format("checkNodeStructureCustom is not implemented for Type {0}", GetType().ToString()));
        }


        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            StringBuilder sb = new StringBuilder();
            foreach (cwPSFPropertyBox pb in propertiesBoxes.Boxes)
            {
                sb.Append(pb.GetHashCode().ToString());
            }
            foreach (cwPSFTreeNode tn in getAllChildren())
            {
                sb.Append(tn.GetHashCode().ToString());
            }
            return sb.ToString().GetHashCode();
        }


        /// <summary>
        /// Removes the child.
        /// </summary>
        /// <param name="_childNode">The _child node.</param>
        public void removeChild(cwPSFTreeNode _childNode)
        {
            string childType = _childNode.TypeName;
            if (childrenNodes.ContainsKey(childType))
            {
                childrenNodes[childType].Remove(_childNode);
                Nodes.Remove(_childNode);
                if (0.Equals(childrenNodes[childType].Count))
                {
                    childrenNodes.Remove(childType);
                }
            }

        }

        /// <summary>
        /// Updates the name of the node.
        /// </summary>
        private void updateNodeName()
        {
            updateText(propertiesBoxes.getPropertyBox(CONFIG_NODE_NAME).ToString());
        }
        /// <summary>
        /// Handles the TextChanged event of the TB_NodeName control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void TB_NodeName_TextChanged(object sender, EventArgs e)
        {
            updateNodeName();
        }

        /// <summary>
        /// Updates the text.
        /// </summary>
        /// <param name="newText">The new text.</param>
        public void updateText(String newText)
        {
            propertiesBoxes.getPropertyBox(CONFIG_NODE_NAME).Text = newText;
            Text = newText;
        }


        /// <summary>
        /// Gets the name of the type.
        /// </summary>
        /// <value>
        /// The name of the type.
        /// </value>
        public String TypeName
        {
            get
            {
                return this.GetType().Name.ToString();
            }
        }

        /// <summary>
        /// Sets the properties boxes.
        /// </summary>
        public virtual void setPropertiesBoxes()
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Gets the tree view configurations.
        /// </summary>
        public TreeView treeViewConfigurations
        {
            get
            {
                return treeViewConfigurations;
            }
        }

        /// <summary>
        /// Loads the root rule.
        /// </summary>
        /// <param name="xmlContent">Content of the XML.</param>
        public void loadFromXMLContent(string xmlContent)
        {
            clearChildrenNodes();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlContent);
            XmlNode _rootNode = xmlDoc.SelectSingleNode("/" + this.TypeName);
            this.loadFromXMLNode(_rootNode);
        }



        /// <summary>
        /// Gets the first child node.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T getFirstChildNode<T>() where T : cwPSFTreeNode
        {
            List<T> nodes = getChildrenNodes<T>();
            return nodes.First();
        }

        /// <summary>
        /// Types the equals to.
        /// </summary>
        /// <param name="t1">The t1.</param>
        /// <param name="t2">The t2.</param>
        /// <returns></returns>
        public static bool typeEqualsTo(Type t1, Type t2)
        {
            if (t1 == null) return false;
            if (t2 == null) return false;
            Type tempT = t1;
            while (tempT != null)
            {
                if (typeof(cwPSFTreeNode).Name.Equals(tempT.Name)) return false;
                if (tempT.Name.Equals(t2.Name))
                {
                    return true;
                }
                tempT = tempT.BaseType;
            }
            return false;
        }


        /// <summary>
        /// Gets all children.
        /// </summary>
        /// <returns></returns>
        public List<cwPSFTreeNode> getAllChildren()
        {
            List<cwPSFTreeNode> children = new List<cwPSFTreeNode>();

            foreach (var childVar in Nodes)
            {
                children.Add(childVar as cwPSFTreeNode);
            }
            return children;
        }

        /// <summary>
        /// Gets the children nodes.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual List<T> getChildrenNodes<T>() where T : cwPSFTreeNode
        {
            Type t = typeof(T);
            String childNodeType = t.Name;
            List<T> nodes = new List<T>();
            if (!childrenNodes.ContainsKey(childNodeType))
            {
                foreach (var childVar in childrenNodes)
                {
                    foreach (cwPSFTreeNode node in childVar.Value)
                    {
                        Type childType = node.GetType();
                        if (typeEqualsTo(childType, t))
                        {
                            T _node = node as T;
                            nodes.Add(_node);
                        }

                    }
                }
                if (0.Equals(nodes.Count))
                {
                    throw new cwExceptionWarning(String.Format("The requested child node [{0}] is missing", childNodeType));
                }
            }
            else
            {
                foreach (cwPSFTreeNode childNode in childrenNodes[childNodeType])
                {
                    T node = childNode as T;
                    if (node != null)
                    {
                        nodes.Add(node);
                    }
                }
            }
            return nodes;
        }



        /// <summary>
        /// Clears the children nodes.
        /// </summary>
        internal void clearChildrenNodes()
        {
            childrenNodes.Clear();
            Nodes.Clear();
        }


        /// <summary>
        /// Creates the type of the PSF node from.
        /// </summary>
        /// <param name="childNodeName">Name of the child node.</param>
        /// <param name="operationEditModeGUI">The operation edit mode GUI.</param>
        /// <param name="parentNode">The parent node.</param>
        /// <returns></returns>
        public static cwPSFTreeNode createPSFNodeFromType(string childNodeName, cwEditModeGUI operationEditModeGUI, cwPSFTreeNode parentNode)
        {
            Type parentType = parentNode.GetType();
            Type childType = parentType.Assembly.GetType(parentType.Namespace + "." + childNodeName);
            if (childType == null)
            {
                //throw new cwExceptionWarning("The loaded node [" + childNodeName + "] is missing in " + parentType.Namespace);
                childType = parentType.Assembly.GetType("Casewise.GraphAPI.PSF." + childNodeName);
                if (childType == null)
                {
                    childType = parentType.Assembly.GetType("Casewise.GraphAPI.Operations.Web." + childNodeName);
                    if (childType == null)
                    {
                        throw new cwExceptionWarning("The loaded node [" + childNodeName + "] is missing in Casewise.GraphAPI.PSF");
                    }
                }
            }
            object createdObject = Activator.CreateInstance(childType, new object[] { operationEditModeGUI, parentNode });
            cwPSFTreeNode createdNode = createdObject as cwPSFTreeNode;
            return createdNode;
        }


        /// <summary>
        /// Loads from XML node.
        /// </summary>
        /// <param name="thisElementNode">The this element node.</param>
        private void loadFromXMLNode(XmlNode thisElementNode)
        {
            //DateTime start = DateTime.Now;
            clearChildrenNodes();

            //DateTime start_loadPB = DateTime.Now;
            propertiesBoxes.loadProperties(thisElementNode);
            //guiHelper.addDebugGUI("LOAD", "PROPERTIES BOXES", DateTime.Now.Subtract(start_loadPB).ToString());

            //DateTime start_loadChildren = DateTime.Now;
            foreach (XmlNode childNode in thisElementNode.ChildNodes)
            {
                try
                {
                    String childNodeName = childNode.Name;
                    String nodeText = (childNode.Attributes[CONFIG_NODE_NAME] != null) ? childNode.Attributes[CONFIG_NODE_NAME].Value : childNodeName;
                    cwPSFTreeNode createdNode = cwPSFTreeNode.createPSFNodeFromType(childNodeName, operationEditModeGUI, this);
                    if (createdNode == null)
                    {
                        throw new cwExceptionWarning("The loaded node [" + childNodeName + "] do not implement cwPSFTreeNode");
                    }
                    createdNode.loadFromXMLNode(childNode);
                    createdNode.updateText(nodeText);
                    addChildNodeLast(createdNode);
                    createdNode = null;
                }
                catch (cwExceptionWarning e)
                {
                    cwPSFTreeNode.log.Warn(e.ToString());
                }
            }
        }


        /// <summary>
        /// Adds the delete option to context menu.
        /// </summary>
        protected void addDeleteOptionToContextMenu()
        {
            ToolStripItem delete_item = menu_strip.Items.Add(Properties.Resources.PSF_TN_CTX_DELETE);
            delete_item.Image = Properties.Resources.image_tvicon_delete;
            delete_item.Click += new System.EventHandler(this.deleteMenuItem_Click);
        }

        /// <summary>
        /// Gets the children object nodes.
        /// </summary>
        /// <returns></returns>
        public List<T> getChildrenGeneric<T>() where T : cwPSFTreeNode
        {
            List<T> _groups = new List<T>();
            if (hasAtLeastOnChildNode<T>())
            {
                return getChildrenNodes<T>();
            }
            return _groups;
        }

        /// <summary>
        /// Inserts the child node.
        /// </summary>
        /// <param name="_node">The _node.</param>
        private void insertChildNode(cwPSFTreeNode _node)
        {
            String childNodeName = _node.GetType().Name;
            if (!childrenNodes.ContainsKey(childNodeName))
            {
                childrenNodes.Add(childNodeName, new List<cwPSFTreeNode>());
            }
            childrenNodes[childNodeName].Add(_node);
            _node.parent = this;
        }

        /// <summary>
        /// Adds the child node first.
        /// </summary>
        /// <param name="_node">The _node.</param>
        public void addChildNodeFirst(cwPSFTreeNode _node)
        {
            insertChildNode(_node);
            Nodes.Insert(0, _node);
        }

        /// <summary>
        /// Adds the child node.
        /// </summary>
        /// <param name="_node">The _node.</param>
        public void addChildNodeLast(cwPSFTreeNode _node)
        {
            insertChildNode(_node);
            Nodes.Add(_node);
        }


        /// <summary>
        /// Gets the GUI.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T getGUI<T>() where T : class
        {
            T _GUI = operationEditModeGUI as T;
            return _GUI;
        }


        /// <summary>
        /// Saves the custom elements in configuration file.
        /// </summary>
        /// <param name="xmlwriter">The xmlwriter.</param>
        public virtual void saveCustomElementsInConfigurationFile(XmlWriter xmlwriter)
        {
            //throw new NotImplementedException("saveCustomElementsInConfigurationFile");
        }

        /// <summary>
        /// Saves the in configuration file.
        /// </summary>
        /// <param name="xmlwriter">The xmlwriter.</param>
        public virtual void saveInConfigurationFile(XmlWriter xmlwriter)
        {
            xmlwriter.WriteStartElement(TypeName);
            propertiesBoxes.savePropertiesBox(xmlwriter);
            saveCustomElementsInConfigurationFile(xmlwriter);

            foreach (cwPSFTreeNode designerTreeNode in Nodes)
            {
                designerTreeNode.saveInConfigurationFile(xmlwriter);
            }
            xmlwriter.WriteEndElement();
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <returns></returns>
        public virtual string getName()
        {
            return Text;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override String ToString()
        {
            return propertiesBoxes.getPropertyBox(CONFIG_NODE_NAME).ToString();
        }


        /// <summary>
        /// Shows the details components.
        /// </summary>
        public void showDetailsComponents()
        {
            operationEditModeGUI.suspendLayout();
            operationEditModeGUI.setPropertiesBoxTable(tableLayoutPanelDetails);

            for (int i = 0; i < propertiesBoxes.Boxes.Count; ++i)
            {
                cwPSFPropertyBox box = propertiesBoxes.Boxes[i];
                box.unsetErrorInfo();
            }
            operationEditModeGUI.resumeLayout();
        }


        /// <summary>
        /// Creates the context menu.
        /// </summary>
        /// <param name="treeView">The tree view.</param>
        /// <param name="position">The position.</param>
        public virtual void createContextMenu(TreeView treeView, Point position)
        {
            addExpandCollapseOptionToContextMenu();
        }

        /// <summary>
        /// Adds the expand collapse option to context menu.
        /// </summary>
        protected void addExpandCollapseOptionToContextMenu()
        {
            ToolStripItem objectGroup_item = menu_strip.Items.Add(Properties.Resources.PSF_TN_CTX_EXPAND_ALL);
            objectGroup_item.Image = Properties.Resources.image_tvicon_plus;
            objectGroup_item.Click += new System.EventHandler(this.expandAllToolStripMenuItem_Click);

            ToolStripItem objectGroup_itemCollapse = menu_strip.Items.Add(Properties.Resources.PSF_TN_CTX_COLLAPSE);
            objectGroup_itemCollapse.Image = Properties.Resources.image_tvicon_minus;
            objectGroup_itemCollapse.Click += new System.EventHandler(this.collapseAllToolStripMenuItem_Click);
        }

        /// <summary>
        /// Handles the Click event of the collapseAllToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void collapseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Collapse();
        }
        /// <summary>
        /// Handles the Click event of the expandAllToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void expandAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ExpandAll();
        }

        /// <summary>
        /// Called when [click].
        /// </summary>
        public virtual void OnClick()
        { }

        /// <summary>
        /// Copies the tree node and the entire subtree rooted at this tree node.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Object"/> that represents the cloned <see cref="T:System.Windows.Forms.TreeNode"/>.
        /// </returns>
        public override object Clone()
        {
            return this.MemberwiseClone();
        }


    }
}
