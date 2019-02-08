using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Casewise.GraphAPI.GUI
{
    /// <summary>
    /// PSF Context Strip
    /// </summary>
    public class cwPSFContextMenuStrip : ContextMenuStrip
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="cwPSFContextMenuStrip"/> class.
        /// </summary>
        /// <param name="f">The f.</param>
        public cwPSFContextMenuStrip(Font f)
        {
            this.Font = f;
        }
    }
}
