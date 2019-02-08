using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Casewise.GraphAPI.GUI;
using Casewise.GraphAPI.PSF;
using Casewise.GraphAPI.API;

using Microsoft.Win32;
using Casewise.GraphAPI.ProgramManager;
using log4net;
using System.Diagnostics;
using System.Drawing;
using System.IO;

namespace Casewise.GraphAPI.Launcher
{
    /// <summary>
    /// cwPictureBoxButtonOptions
    /// </summary>
    /// <typeparam name="GuiType">The type of the U i_ TYPE.</typeparam>
    /// <typeparam name="RootNodeType">The type of the OO t_ NODE.</typeparam>
    /// <typeparam name="CreateItemType">The type of the REAT e_ ITEM.</typeparam>
    public class cwPictureBoxButtonOptions<GuiType, RootNodeType, CreateItemType> : cwPictureBoxButton<GuiType, RootNodeType, CreateItemType>
        where GuiType : cwEditModeGUI
        where RootNodeType : cwPSFTreeNodeConfigurationNode
        where CreateItemType : cwCreateItemForm
    {

        private static readonly ILog log = LogManager.GetLogger(typeof(cwPictureBoxButtonOptions<GuiType, RootNodeType, CreateItemType>));

        /// <summary>
        /// Initializes a new instance of the <see cref="cwPictureBoxButtonOptions&lt;GuiType, RootNodeType, CreateItemType&gt;"/> class.
        /// </summary>
        /// <param name="mainGUI">The main GUI.</param>
        public cwPictureBoxButtonOptions(cwLauncherGUI<GuiType, RootNodeType, CreateItemType> mainGUI)
            : base(mainGUI)
        {
            BackgroundImage = Properties.Resources.image_option_add_32x32;
            setUpToolTip();
            this.MouseUp += new MouseEventHandler(cwPictureBoxButtonOptions_MouseUp);
            Visible = false;
        }

        /// <summary>
        /// Sets up tool tip.
        /// </summary>
        public void setUpToolTip()
        {
            tooltip.RemoveAll();
            if (mainGUI.choosedModel != null)
            {
                tooltip.SetToolTip(this, mainGUI.options.addItemTooltipMessage);
            }
            else
            {
                tooltip.SetToolTip(this, Properties.Resources.LAUNCHER_OPTIONS_TOOLTIP_ENABLE_MODEL);
            }
        }

        /// <summary>
        /// Handles the MouseUp event of the cwPictureBoxButtonOptions control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        private void cwPictureBoxButtonOptions_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                cwPSFContextMenuStrip menuStrip = new cwPSFContextMenuStrip(this.Font);
                menuStrip.RightToLeft = System.Windows.Forms.RightToLeft.No;
                if (mainGUI.choosedModel == null)
                {
                    mainGUI.notEnabledModels.Sort();
                    for (var i = 0; i < mainGUI.notEnabledModels.Count; ++i)
                    {
                        cwLightModel model = mainGUI.notEnabledModels[i];
                        ToolStripItem modelItem = menuStrip.Items.Add(model.ToString());
                        modelItem.Click += (modelSender, args) => ctx_setupNewAModel_Click(model, args);
                        menuStrip.Items.Add(modelItem);
                    }                    
                }
                else
                {
                    ctx_createItem(mainGUI.choosedModel);
                }
                Point p = new Point(e.X, e.Y);
                menuStrip.Show(this, p);
            }
        }

        /// <summary>
        /// Ctx_creates the item.
        /// </summary>
        /// <param name="model">The model.</param>
        private void ctx_createItem(cwLightModel model)
        {
            try
            {
                mainGUI.helpTooltip.remove();
                cwLightObjectType itemOT = model.getObjectTypeByScriptName(mainGUI.options.applicationObjectTypeScriptName);
                CreateItemType createItemForm = Activator.CreateInstance(typeof(CreateItemType), new object[] { itemOT }) as CreateItemType;
                createItemPopup itemNamePopup = new createItemPopup(createItemForm);
                if (!itemNamePopup.hasBeenCanceled)
                {
                    cwLightObject newItem = createItemForm.createItem();
                    newItem.freeze(1);

                    mainGUI.loadItemIntoEditGUI(newItem);
                }
                mainGUI.checkAddItemTooltip();
            }
            catch (Exception exception)
            {
                log.Error(exception.ToString());
            }
        }
        

        private void ctx_setupNewAModel_Click(cwLightModel model, EventArgs e)
        {
            try
            {
                mainGUI.imageLoader.startLoadingImage();
                mainGUI.clearFlowLayoutPanel();
                mainGUI.notEnabledModels.Clear();

                string localFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
                localFolder += "\\Casewise";
                if (!Directory.Exists(localFolder)) {
                    Directory.CreateDirectory(localFolder);
                }
                localFolder += "\\DiagramDesigner";
                if (!Directory.Exists(localFolder))
                {
                    Directory.CreateDirectory(localFolder);
                }
                string logFileLocation = localFolder + "\\enableModel.log";
                string arg = "-import -toexistingmodel -importall -keepfromimportfile -model:" + model.FileName + " -filename:\"XMLExport/ObjectTypeExport.xml\" -errorfile:\"" + logFileLocation + "\"";
                string casewiseBinFolder = cwTools.casewiseBinFolder;

                string command = casewiseBinFolder + @"\XMLImportExport.exe";
                ProcessStartInfo start = new ProcessStartInfo(command, arg);
                start.UseShellExecute = false;
                start.CreateNoWindow = true; // Important if you want to keep shell window hidden
                Process p = Process.Start(start);
                p.WaitForExit(); //important to add WaitForExit()
                mainGUI.imageLoader.disposeLoadingForm();
                if (0.Equals(p.ExitCode))
                {
                    MessageBox.Show(Properties.Resources.APPLICATION_WILL_RESTART);
                    mainGUI.Close();
                    Application.Restart();
                }
                else
                {
                    MessageBox.Show(String.Format(Properties.Resources.ERROR_XMLIMPORTEXPORT, model.ToString(), model.FileName, logFileLocation), Properties.Resources.TEXT_WARNING);
                    mainGUI.Activate();
                    mainGUI.loadFromConnection();
                }
            }
            catch (Exception exception)
            {
                log.Error(exception.ToString());
            }
        }

      
    }
}
