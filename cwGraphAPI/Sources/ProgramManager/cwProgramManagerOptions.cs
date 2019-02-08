using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Drawing;
using Casewise.GraphAPI.Launcher;

namespace Casewise.GraphAPI.ProgramManager
{
    /// <summary>
    /// cwProgramManagerOptions
    /// </summary>
    public class cwProgramManagerOptions
    {
        /// <summary>
        /// executingAssembly
        /// </summary>
        public Assembly executingAssembly = null;
        /// <summary>
        /// warningText
        /// </summary>
        public String warningText;
        /// <summary>
        /// applicationName
        /// </summary>
        public String applicationName;
        /// <summary>
        /// applicationLogo
        /// </summary>
        public Bitmap applicationLogo;
        /// <summary>
        /// applicationObjectTypeScriptName
        /// </summary>
        public String applicationObjectTypeScriptName;
        /// <summary>
        /// startUpGUIText
        /// </summary>
        public String startUpGUIText;
        /// <summary>
        /// itemIcon
        /// </summary>
        public Bitmap itemIcon;
        /// <summary>
        /// itemIconAnimated
        /// </summary>
        public Bitmap itemIconAnimated;

        /// <summary>
        /// helpURL
        /// </summary>
        public string helpURL = "http://www.casewise.com/";

        /// <summary>
        /// addItemTooltipMessage
        /// </summary>
        public string addItemTooltipMessage = Properties.Resources.LAUNCHER_OPTIONS_TOOLTIP_ADD_ITEM;

        /// <summary>
        /// Initializes a new instance of the <see cref="cwProgramManagerOptions"/> class.
        /// </summary>
        public cwProgramManagerOptions()
        { 
        }
    }
}
