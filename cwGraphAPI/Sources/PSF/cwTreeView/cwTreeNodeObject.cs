using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using Casewise.GraphAPI.API;

namespace Casewise.GraphAPI.PSF
{
    /// <summary>
    /// Create a node sorter that implements the IComparer interface.
    /// </summary>
    public class NodeSorter : IComparer
    {

        /// <summary>
        /// Compare the length of the strings, or the strings
        /// themselves, if they are the same length.
        /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>
        /// Value
        /// Condition
        /// Less than zero
        /// <paramref name="x"/> is less than <paramref name="y"/>.
        /// Zero
        /// <paramref name="x"/> equals <paramref name="y"/>.
        /// Greater than zero
        /// <paramref name="x"/> is greater than <paramref name="y"/>.
        /// </returns>
        /// <exception cref="T:System.ArgumentException">
        /// Neither <paramref name="x"/> nor <paramref name="y"/> implements the <see cref="T:System.IComparable"/> interface.
        /// -or-
        ///   <paramref name="x"/> and <paramref name="y"/> are of different types and neither one can handle comparisons with the other.
        ///   </exception>
        public int Compare(object x, object y)
        {
            TreeNode tx = x as TreeNode;
            TreeNode ty = y as TreeNode;

            // Compare the length of the strings, returning the difference.
            if (tx.Text.Length != ty.Text.Length)
                return tx.Text.Length - ty.Text.Length;

            // If they are the same length, call Compare.
            return string.Compare(tx.Text, ty.Text);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class cwTreeNodeObject : TreeNode
    {

        /// <summary>
        /// 
        /// </summary>
        public cwLightObject cwLightObject = null;


        /// <summary>
        /// Initializes a new instance of the <see cref="cwTreeNodeObject"/> class.
        /// </summary>
        /// <param name="o">The o.</param>
        public cwTreeNodeObject(cwLightObject o)
            : base(o.ToString())
        {
            cwLightObject = o;
            this.Checked = false;
        }




        /// <summary>
        /// Sorts the by alpha.
        /// </summary>
        public void sortByAlpha()
        {
            List<cwTreeNodeObject> nodes = new List<cwTreeNodeObject>();
            foreach (cwTreeNodeObject tn in this.Nodes)
            {
                nodes.Add(tn);
            }
            nodes.Sort(delegate(cwTreeNodeObject tn1, cwTreeNodeObject tn2) { return tn1.Text.CompareTo(tn2.Text); });
            this.Nodes.Clear();
            nodes.ForEach(tn => this.Nodes.Add(tn));
            nodes.ForEach(tn => tn.sortByAlpha());
        }

      

    }
}
