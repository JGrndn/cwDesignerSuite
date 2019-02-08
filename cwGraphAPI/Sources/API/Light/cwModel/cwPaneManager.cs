using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Casewise.Data.ICM;
using System.Data;
using log4net;

namespace Casewise.GraphAPI.API
{
    /// <summary>
    /// Manage the Panes
    /// </summary>
    internal class cwPaneManager
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(cwPaneManager));

        //Dictionary<int, SortedDictionary<int, cwLightPane>> panesByObjectTypeID = new Dictionary<int, SortedDictionary<int, cwLightPane>>();

        private cwLightModel _model;

        /// <summary>
        /// Initializes a new instance of the <see cref="cwPaneManager"/> class.
        /// </summary>
        /// <param name="model">The model.</param>
        public cwPaneManager(cwLightModel model)
        {
            _model = model;
            loadPanes();
        }

        /// <summary>
        /// Loads the lookups.
        /// </summary>
        private void loadPanes()
        {
            try
            {
                //panesByObjectTypeID = new Dictionary<int, SortedDictionary<int, cwLightPane>>();
                StringBuilder query = new StringBuilder();

                query.Append("SELECT ID, NAME, SCRIPTNAME, OBJECTTYPEID, SEQUENCE, DESCRIPTION, HELP FROM PANE");
                using (ICMCommand command = new ICMCommand(query.ToString(), _model.getConnection()))
                {
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32(0);
                            string name = reader.GetString(1);
                            string scriptname = reader.GetString(2);
                            int objectTypeID = reader.GetInt32(3);
                            int sequence = reader.GetInt32(4);

                            //if (!panesByObjectTypeID.ContainsKey(objectTypeID))
                            //{
                            //    panesByObjectTypeID[objectTypeID] = new SortedDictionary<int, cwLightPane>();
                            //}
                            cwLightObjectType objectType = _model.getObjectTypeByID(objectTypeID);

                            cwLightPane pane = new cwLightPane(id, name, sequence, scriptname, objectType);
                            //panesByObjectTypeID[objectTypeID][id] = pane;
                            objectType.panesBySequence[sequence] = pane;
                            objectType.panesByID[id] = pane;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
