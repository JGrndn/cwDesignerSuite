using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Casewise.GraphAPI.ProgramManager;
using System.Reflection;
using Casewise.webDesigner.GUI;
using Casewise.webDesigner.Nodes;
using Casewise.GraphAPI.Launcher;
using System.Diagnostics;


namespace Casewise.webDesigner
{
    static class Program
    {
        [STAThread]
        static int Main(string[] args)
        {
            //args = new string[] { "-connection", "Autonome", "-model", "GALDERMA", "-username", "ADMIN", "-password", "", "-id", "1" };

            cwProgramManagerOptions options = new cwProgramManagerOptions();
            options.executingAssembly = Assembly.GetExecutingAssembly();
            options.applicationName = "Casewise webUtility Tools Framework";
            options.applicationLogo = Properties.Resources.image_logo_webdesigner;
            options.applicationObjectTypeScriptName = webDesignerOperation.OBJECTTYPE_NAME_IN_CM;
            options.warningText = String.Format("Casewise webUtility Beta v{0} - {1}", getFileVersion(), Casewise.GraphAPI.Properties.Resources.LAUNCHER_WARNING_TEXT);
            options.startUpGUIText = "webUtility Launcher";
            options.itemIcon = Properties.Resources.image_item_webdesigner;
            options.itemIconAnimated = Properties.Resources.image_item_webdesigner;
            options.addItemTooltipMessage = @"Add a website";
            cwProgramManager pm = new cwProgramManager(options);
            return pm.launchApplication<cwWebDesignerGUI, cwWebDesignerTreeNodeSite, webDesignerOperation, webDesignerCreateItemForm>(args);
        }

        /// <summary>
        /// Gets the file version.
        /// </summary>
        /// <returns></returns>
        static string getFileVersion()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            return fvi.ProductVersion;
        }
    }
}
