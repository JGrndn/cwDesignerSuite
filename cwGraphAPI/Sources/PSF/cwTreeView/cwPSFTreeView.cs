using System;
using System.Collections.Generic;

using System.Text;
using System.Windows.Forms;


namespace Casewise.GraphAPI.PSF
{
    /// <summary>
    /// Create a treeview 
    /// </summary>
    public class cwPSFTreeView : TreeView
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="cwPSFTreeView"/> class.
        /// </summary>
        public cwPSFTreeView()
        {
            //tv = _tv;
            AfterCheck += new TreeViewEventHandler(tv_AfterCheck);
        }

        /// <summary>
        /// after a case has been checked
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.TreeViewEventArgs"/> instance containing the event data.</param>
        public void tv_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Action != TreeViewAction.Unknown)
            {
                if (e.Node.Nodes.Count > 0)
                {
                    /* Calls the CheckAllChildNodes method, passing in the current 
                    Checked value of the TreeNode whose checked state changed. */
                    BeginUpdate();
                    cwPSFTreeView.CheckAllChildNodes(e.Node, e.Node.Checked);
                    EndUpdate();
                }
                if (e.Node.Parent != null)
                {
                    e.Node.Parent.Checked = true;
                }
            }
        }

        /// <summary>
        /// Checks for checked children handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.TreeViewCancelEventArgs"/> instance containing the event data.</param>
        public static void CheckForCheckedChildrenHandler(object sender, TreeViewCancelEventArgs e)
        {
            if (!HasCheckedChildNodes(e.Node)) e.Cancel = true;
        }


        /// <summary>
        /// Determines whether [has checked child nodes] [the specified node].
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>
        ///   <c>true</c> if [has checked child nodes] [the specified node]; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasCheckedChildNodes(TreeNode node)
        {
            if (node.Nodes.Count == 0) return false;
            foreach (TreeNode childNode in node.Nodes)
            {
                if (childNode.Checked) return true;
                // Recursively check the children of the current child node.
                if (HasCheckedChildNodes(childNode)) return true;
            }
            return false;
        }


        /// <summary>
        /// Checks all child nodes.
        /// </summary>
        /// <param name="treeNode">The tree node.</param>
        /// <param name="nodeChecked">if set to <c>true</c> [node checked].</param>
        public static void CheckAllChildNodes(TreeNode treeNode, bool nodeChecked)
        {
            foreach (TreeNode node in treeNode.Nodes)
            {
                node.Checked = nodeChecked;
                if (node.Nodes.Count > 0)
                {
                    // If the current node has child nodes, call the CheckAllChildsNodes method recursively.
                    CheckAllChildNodes(node, nodeChecked);
                }
            }
        }
    }
}
