using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Casewise.GraphAPI.PSF
{
    /// <summary>
    /// Act as a tree view
    /// </summary>
    public class cwPSFPropertyBoxTreeView : cwPSFPropertyBoxCollection
    {

        internal cwPSFTreeView treeView = new cwPSFTreeView();

        /// <summary>
        /// Initializes a new instance of the <see cref="cwPSFPropertyBoxTreeView"/> class.
        /// </summary>
        /// <param name="_helpName">Name of the _help.</param>
        /// <param name="_helpDescription">The _help description.</param>
        /// <param name="_keyName">Name of the _key.</param>
        public cwPSFPropertyBoxTreeView(String _helpName, String _helpDescription, String _keyName)
            : base (_helpName, _helpDescription, _keyName)
        { 
            
        }



        /// <summary>
        /// Toes the string collection.
        /// </summary>
        /// <returns></returns>
        public override string ToStringCollection()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Clears the items.
        /// </summary>
        public override void clearItems()
        {
            treeView.Nodes.Clear();
        }
        /// <summary>
        /// Gets the control.
        /// </summary>
        /// <returns></returns>
        public override Control getControl()
        {
            return treeView;
        }

    }
}
