using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Casewise.GraphAPI.Batch;
using Casewise.GraphAPI.API;
using Casewise.GraphAPI.GUI;
using log4net;
using Casewise.GraphAPI.PSF;
using System.Windows.Forms;
using Casewise.GraphAPI.Launcher;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Drawing;

namespace Casewise.GraphAPI.ProgramManager
{
    /// <summary>
    /// cwProgramManager
    /// </summary>
    public class cwProgramManager
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(cwProgramManager));

        /// <summary>
        /// CM_REGISTERY_KEY
        /// </summary>
        public const string CM_REGISTERY_KEY = @"HKEY_LOCAL_MACHINE\SOFTWARE\Casewise\CorporateModeler\10\";

        [DllImport("kernel32.dll")]
        static extern bool AttachConsole(int dwProcessId);
        private const int ATTACH_PARENT_PROCESS = -1;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 

        private int errorCode = 0;

        /// <summary>
        /// options
        /// </summary>
        public cwProgramManagerOptions options = new cwProgramManagerOptions();

        /// <summary>
        /// Initializes a new instance of the <see cref="cwProgramManager"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        public cwProgramManager(cwProgramManagerOptions options)
        {
            this.options = options;
        }

        /// <summary>
        /// Launches the application.
        /// </summary>
        /// <typeparam name="GuiType">The type of the U i_ TYPE.</typeparam>
        /// <typeparam name="RootNodeType">The type of the OO t_ NODE.</typeparam>
        /// <typeparam name="OPERATION_TYPE">The type of the PERATIO n_ TYPE.</typeparam>
        /// <typeparam name="CreateItemType">The type of the REAT e_ ITEM.</typeparam>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        public int launchApplication<GuiType, RootNodeType, OPERATION_TYPE, CreateItemType>(string[] args)
            where GuiType : cwEditModeGUI
            where RootNodeType : cwPSFTreeNodeConfigurationNode
            where OPERATION_TYPE : cwOperationLight
            where CreateItemType : cwCreateItemForm
        {
            try
            {
                AttachConsole(ATTACH_PARENT_PROCESS);
                log4net.Config.XmlConfigurator.Configure();
                log.Debug("Program Starts");
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                log.Debug("mainGUI Created");
                Console.WriteLine("\n!! use -h option for batch usage !!");
                if (0.Equals(args.Count()))
                {
                    loadStandAlone<GuiType, RootNodeType, CreateItemType>(options);
                }
                else
                {
                    loadFromArgumentsInBatchMode<GuiType, OPERATION_TYPE, RootNodeType>(args, options);
                }

                return errorCode;
            }
            catch (Exception e)
            {
                log.Error(e.ToString());
                //MessageBox.Show("ERROR > " + e.Message.ToString(), "Error");
                return 1;
            }
        }



        private void showStarterComponents<GuiType, RootNodeType, CreateItemType>(cwLauncherGUI<GuiType, RootNodeType, CreateItemType> startupGUI)
            where GuiType : cwEditModeGUI
            where RootNodeType : cwPSFTreeNodeConfigurationNode
            where CreateItemType : cwCreateItemForm
        {
            //startupGUI.labelCopyright.Visible = false;
            //startupGUI.pictureBoxLogo.Visible = false;   
            startupGUI.labelCopyright.Text = options.warningText;
            startupGUI.pictureBoxLogo.Image = options.applicationLogo;
            startupGUI.labelCopyright.Visible = true;
            startupGUI.pictureBoxLogo.Visible = true;

            startupGUI.optionClose.Visible = true;
            startupGUI.optionHelp.Visible = true;
            startupGUI.optionOptions.Visible = true;
        }

        /// <summary>
        /// Loads the stand alone.
        /// </summary>
        /// <typeparam name="GuiType">The type of the U i_ TYPE.</typeparam>
        /// <typeparam name="RootNodeType">The type of the OO t_ NODE.</typeparam>
        /// <typeparam name="CreateItemType">The type of the REAT e_ ITEM.</typeparam>
        /// <param name="options">The options.</param>
        public void loadStandAlone<GuiType, RootNodeType, CreateItemType>(cwProgramManagerOptions options)
            where GuiType : cwEditModeGUI
            where RootNodeType : cwPSFTreeNodeConfigurationNode
            where CreateItemType : cwCreateItemForm
        {
            cwLauncherGUI<GuiType, RootNodeType, CreateItemType> startupGUI = new cwLauncherGUI<GuiType, RootNodeType, CreateItemType>(options);
            startupGUI.Text = options.startUpGUIText;
            try
            {
                
                startupGUI.Show();
                showStarterComponents(startupGUI);
                startupGUI.helpTooltip.RemoveAll();
                startupGUI.imageLoader.startLoadingImage();

                ApplicationConfigurationLoader appLoader = new ApplicationConfigurationLoader(options.executingAssembly.Location);
                cwConnection connection = null;
                if (appLoader.hasDefaultCredentials())
                {
                    connection = new cwConnection(appLoader.loginInfo.name, appLoader.loginInfo.password);
                }
                else
                {
                    connection = new cwConnection();
                }
                if (!connection.isConnected())
                {
                    startupGUI.imageLoader.disposeLoadingForm();
                    return;
                }
                startupGUI.connection = connection;
                if (appLoader.hasDefaultModel())
                {
                    connection.loadModels();
                    cwLightModel model = connection.getModel(appLoader.loginInfo.modelFileName);
                    model.loadLightModelContent();
                    log.Debug("Connected to the model " + model.FileName);
                    if (appLoader.itemID != 0)
                    {
                        GuiType editGUI = Activator.CreateInstance(typeof(GuiType), new object[] { model, options }) as GuiType;
                        editGUI.loadTreeView<RootNodeType>(model, options.applicationObjectTypeScriptName, appLoader.itemID);
                        startupGUI.imageLoader.disposeLoadingForm();
                        startupGUI.Close();
                        Application.Run(editGUI);
                    }
                    else
                    {
                        startupGUI.loadFromModel(model);
                        startupGUI.imageLoader.disposeLoadingForm();
                        Application.Run(startupGUI);
                    }

                }
                else
                {
                    //startupGUI.connection = connection;
                    if (startupGUI.hasBeenShown)
                    {
                        startupGUI.loadFromConnection();
                    }
                    Application.Run(startupGUI);
                }
                if (connection.isConnected())
                {
                    connection.Close();
                }
            }

            catch (Exception e)
            {
                log.Error(e.ToString());
                throw e;
            }



        }


        /// <summary>
        /// Loads from arguments in batch mode.
        /// </summary>
        /// <typeparam name="GuiType">The type of the U i_ TYPE.</typeparam>
        /// <typeparam name="OPERATION">The type of the PERATION.</typeparam>
        /// <typeparam name="ROOTNODE">The type of the OOTNODE.</typeparam>
        /// <param name="args">The args.</param>
        /// <param name="options">The options.</param>
        public static void loadFromArgumentsInBatchMode<GuiType, OPERATION, ROOTNODE>(string[] args, cwProgramManagerOptions options)
            where GuiType : cwEditModeGUI
            where OPERATION : cwOperationLight
            where ROOTNODE : cwPSFTreeNodeConfigurationNode
        {
            if (1.Equals(args.Count()))
            {
                if ("-h".Equals(args[0]))
                {
                    Console.WriteLine("\n" + options.executingAssembly.GetName().Name.ToString() + ".exe -connection <connection_to_database> -model <model_filename> -username <username> -password <password> -id <item_id>");
                }
            }
            else
            {
                ApplicationBatchManager batchManager = new ApplicationBatchManager(args);

                Console.WriteLine("Start connection to the repository...");
                cwConnection connection = new cwConnection(batchManager.arguments["username"], batchManager.arguments["password"]);
                connection.loadModels();
                cwLightModel model = connection.getModel(batchManager.arguments["model"]);
                model.loadLightModelContent();
                log.Debug("Connected to the model " + model.FileName);
                Console.WriteLine("Connection done to model [" + model.ToString() + "].");

                GuiType editGUI = Activator.CreateInstance(typeof(GuiType), new object[] { model, options }) as GuiType;
                int id = Convert.ToInt32(batchManager.arguments["id"]);
                ROOTNODE nodeSite = editGUI.loadTreeView<ROOTNODE>(model, options.applicationObjectTypeScriptName, id);//, id);
                OPERATION _operation = Activator.CreateInstance(typeof(OPERATION), new object[] { model, nodeSite }) as OPERATION;
                Console.WriteLine(_operation.name + " loaded.");
                Console.WriteLine("[" + nodeSite.getName() + "] is being generated...");
                DateTime start = DateTime.Now;
                _operation.Do(model);
                Console.WriteLine("[" + nodeSite.getName() + "] has been generated in " + DateTime.Now.Subtract(start).ToString() + "s");
                connection.Close();
            }
        }
    }
}
