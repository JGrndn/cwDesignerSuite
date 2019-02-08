using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Casewise.GraphAPI;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Collections;
using System.Configuration;
using Casewise.GraphAPI.Exceptions;
using Casewise.GraphAPI.API;
using Casewise.GraphAPI.GUI;
using Casewise.GraphAPI.PSF;
using log4net;


namespace Casewise.GraphAPI
{
    /// <summary>
    /// 
    /// </summary>
    public class cwOperationLight
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(cwOperationLight));
        private cwLightModel currentLightModel = null;


        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string name { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string description { get; set; }
        /// <summary>
        /// the error level reported by the operation
        /// </summary>
        public int errorLevel = 0;


        /// <summary>
        /// Initializes a new instance of the <see cref="cwOperationLight"/> class.
        /// </summary>
        public cwOperationLight()
        { }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="currentLightModel">The current light model.</param>
        /// <param name="_name">The _name.</param>
        /// <param name="_description">The _description.</param>
        public cwOperationLight(cwLightModel currentLightModel, string _name, string _description)
        {
            name = _name;
            this.currentLightModel = currentLightModel;
            description = _description;
        }

        /// <summary>
        /// Gets the model.
        /// </summary>
        public cwLightModel Model
        {
            get
            {
                return currentLightModel;
            }
            set
            {
                currentLightModel = value;
            }
        }


        /// <summary>
        /// Edits the mode start operation.
        /// </summary>
        /// <typeparam name="GUI">The type of the UI.</typeparam>
        /// <typeparam name="RootNode">The type of the oot node.</typeparam>
        /// <param name="_model">The _model.</param>
        /// <returns></returns>
        public bool editModeStartOperation<GUI, RootNode>(cwLightModel _model)
            where GUI : cwEditModeGUI
            where RootNode : cwPSFTreeNodeConfigurationNode
        {
            try
            {
                Type typeOfGUI = typeof(GUI);
                object createdObject = Activator.CreateInstance(typeOfGUI, new object[] { _model });
                GUI operationGUI = createdObject as GUI;
                //operationGUI.loadTreeView<RootNode>(_model);
            }
            catch (Exception _exception)
            {
                reportError("ERROR;" + _exception.Message.ToString() + _exception.ToString());
            }
            return true;
        }



        /// <summary>
        /// Reports the warning.
        /// </summary>
        /// <param name="message">The message.</param>
        protected void reportWarning(string message)
        {
            log.Warn(message);
        }

        /// <summary>
        /// Reports the error.
        /// </summary>
        /// <param name="message">The message.</param>
        protected void reportError(string message)
        {
            log.Error(message);
        }

        /// <summary>
        /// Reports the info.
        /// </summary>
        /// <param name="message">The message.</param>
        protected void reportInfo(string message)
        {
            log.Info(message);
        }

        /// <summary>
        /// Do the operation code
        /// </summary>
        /// <param name="m">The model used for the operation</param>
        public virtual void Do(cwLightModel m)
        {
            throw new NotImplementedException("Do");
        }

        /// <summary>
        /// Edits the mode start.
        /// </summary>
        public virtual bool editModeStart(cwLightModel _model)
        {
            reportError("Edit mode has not been set for this Operation");
            return false;
        }


    }

}
