using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Casewise.Data.ICM;
using System.Data;
using System.Windows.Forms;
using Casewise.Services.Entities;
using Casewise.Services.ICM;
using System.Drawing;
using System.Collections;
using Casewise.Collections;
using Casewise.Services.JoinerRouting;
using Casewise.GraphAPI.Exceptions;
using Casewise.Services.COMFacade;
using CasewiseDataTier2004;
using System.IO;
using System.Xml;

namespace Casewise.GraphAPI.API
{
    /// <summary>
    /// Represents a Diagram
    /// </summary>
    public class cwLightDiagram : cwLightObject
    {
        /// <summary>
        /// Equal to 1 grid box (16x16) in Corporate Modeler
        /// </summary>
        public static int one_pixel = 131072;
        /// <summary>
        /// mmRatio
        /// </summary>
        public static int mmRatio = 2165;
        /// <summary>
        /// the title of the diagram
        /// </summary>
        public string title = null;
        /// <summary>
        /// the name of the diagram
        /// </summary>
        public string name = null;
        /// <summary>
        /// the parent object if exists
        /// </summary>
        public cwLightObject parent = null;
        private ICMDiagramLock diagramLock = null;
        private static int diagramOTID = 6854;
        /// <summary>
        /// The diagrams properties to load
        /// </summary>
        public const string diagramSelectionProperties = "ID, TITLE, NAME, TYPE, DIAGRAMMER, TABLENUMBER, OBJECTID, DESCRIPTION, UNIQUEIDENTIFIER, VALIDATED, SYMBOLS";
        /// <summary>
        /// The Diagram Loader
        /// </summary>
        public DiagramLoader diagramLoader = null;
        /// <summary>
        /// The ICM Diagram
        /// </summary>
        public Diagram diagram = null;
        /// <summary>
        /// The UUID of the diagram
        /// </summary>
        public string uuid = null;

        /// <summary>
        /// The most left coord of the diagram
        /// </summary>
        public float mostLeft = 0;
        /// <summary>
        /// The most top coord of the diagram
        /// </summary>
        public float mostTop = 0;

        /// <summary>
        /// AUTOMATICDIAGRAM_PROPERTYSCRIPTNAME
        /// </summary>
        public const string AUTOMATICDIAGRAM_PROPERTYSCRIPTNAME = "CWAUTOMATICDIAGRAM";

        /// <summary>
        /// The symbols XML
        /// </summary>
        internal string symbolsXml = "";

        /// <summary>
        /// Initializes a new instance of the <see cref="cwLightDiagram"/> class.
        /// </summary>
        /// <param name="currentLightModel">The current light model.</param>
        /// <param name="_id">The _id.</param>
        /// <param name="_name">The _name.</param>
        /// <param name="_title">The _title.</param>
        /// <param name="_uuid">The _uuid.</param>
        public cwLightDiagram(cwLightModel currentLightModel, int _id, string _name, string _title, string _uuid)
            : base(currentLightModel, _id, diagramOTID)
        {
            name = _name;
            title = _title;
            properties["TITLE"] = title;
            properties["NAME"] = name;
            properties["ID"] = _id.ToString();
            properties["UNIQUEIDENTIFIER"] = _uuid;
            uuid = _uuid;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="cwLightDiagram"/> class.
        /// </summary>
        /// <param name="createdDiagram">The created diagram.</param>
        public cwLightDiagram(cwLightObject createdDiagram)
            : base(createdDiagram.Model, createdDiagram.ID, diagramOTID)
        {
            name = createdDiagram.ToString();
            title = createdDiagram.ToString();
            properties["TITLE"] = createdDiagram.ToString();
            properties["NAME"] = createdDiagram.ToString();
            uuid = createdDiagram.properties[cwLightObject.UNIQUEIDENTIFIER];

        }


        /// <summary>
        /// Saves to image.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="format">The format (bmp or png).</param>
        public void saveToImage(string path, string format)
        {
            try
            {
                if (format != "png" && format != "bmp")
                    throw new Exception("Chosen format to save diagram as image is not valid. Valid format are 'png' or 'bmp'");
                IcwLogon pLogon = Model.currentConnection.ConnectionList.LogonContext as IcwLogon;
                cwDiagramLoader loader = new cwDiagramLoader();
                IcwDiagram diag = loader.OpenDiagram(pLogon, Model.FileName, ID);

                int width = 0;
                int height = 0;
                diag.CalculateHeightAndWidthFromScale(100, out width, out height);
                diag.SaveAsRevealAllImage("image/" + format, width, height, 100, path, null);
                loader.Dispose();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Saves the image without blank frame.
        /// </summary>
        /// <param name="path">The path.</param>
        public void saveImageWithoutBlankFrame(string path)
        {
            try
            {
                int extIndex = path.LastIndexOf('.');
                File.Delete(path);
                string tmpPath = path.Remove(extIndex) + ".bmp";
                this.saveToImage(tmpPath, "bmp");
                Bitmap bmp = new Bitmap(tmpPath);
                Bitmap result = cwTools.RemoveSurroundingWhitespaceFromImage(bmp);
                result.Save(path, System.Drawing.Imaging.ImageFormat.Png);
                bmp.Dispose();
                result.Dispose();
                File.Delete(tmpPath);
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        /// <summary>
        /// Inserts the shapes.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="diagramID">The diagram ID.</param>
        /// <param name="shapes">The shapes.</param>
        public static void insertShapes(cwLightModel model, int diagramID, List<cwShape> shapes)
        {
            if (0.Equals(shapes.Count))
            {
                return;
            }
            cwLightObjectType OTShape = model.getObjectTypeByScriptName("SHAPE");
            using (IDbTransaction trans = model.getConnection().BeginTransaction())
            {
                for (int i = 0; i < shapes.Count; ++i)
                {
                    cwShape shape = shapes[i];
                    Dictionary<string, string> shapeProperties = new Dictionary<string, string>();
                    shapeProperties["X"] = shape.Position.X.ToString();
                    shapeProperties["Y"] = (-shape.Position.Y).ToString();
                    shapeProperties["WIDTH"] = shape.Size.Width.ToString();
                    shapeProperties["HEIGHT"] = shape.Size.Height.ToString();
                    shapeProperties["SEQUENCE"] = shape.sequence.ToString();
                    shapeProperties["OBJECTID"] = shape.objectID.ToString();
                    shapeProperties["TABLENUMBER"] = shape.objectTypeID.ToString();
                    shapeProperties["DIAGRAMID"] = diagramID.ToString();
                    shapeProperties["STYLEID"] = shape.customStyleID.ToString();
                    shapeProperties["SYMBOLID"] = shape.customSymbol.ToString();
                    shapeProperties["FLAGS1"] = shape.flag1.ToString();
                    shapeProperties["FLAGS2"] = shape.flag2.ToString();
                    shapeProperties["PARENTSEQUENCE"] = shape.parentSequence.ToString();

                    if (shape.nextZSequence == 0 && i < shapes.Count - 1)
                    {
                        shapeProperties["NEXTZSEQUENCE"] = (shape.sequence + 1).ToString();
                    }
                    else
                    {
                        shapeProperties["NEXTZSEQUENCE"] = shape.nextZSequence.ToString();
                    }

                    OTShape.createObject(shapeProperties, false);
                }
                trans.Commit();
            }
        }

        /// <summary>
        /// Loads the most top and left.
        /// </summary>
        public void loadMostTopAndLeft()
        {
            mostLeft = shapeMostLeft();
            mostTop = shapeMostTop();
        }



        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        public void Dispose()
        {
            if (diagramLoader != null)
            {
                diagramLoader.Dispose();
                diagramLoader = null;
            }
            if (diagram != null)
            {
                diagram.Dispose();
                diagram = null;
            }


        }

        /// <summary>
        /// Saves this instance.
        /// </summary>
        /// <returns></returns>
        public bool Save()
        {
            if (diagramLoader != null)
            {
                if (diagram != null)
                {
                    // FAILURE HERE
                    diagramLoader.Save(diagram);
                    return true;
                }
                return false;
            }
            return false;
        }


        /// <summary>
        /// Loads the specified _light model.
        /// </summary>
        /// <param name="_lightModel">The _light model.</param>
        public void Load(cwLightModel _lightModel)
        {
            diagram = new Diagram(_lightModel.currentConnection);
            ModelLoader mL = new ModelLoader(_lightModel.currentConnection);
            diagramLoader = new DiagramLoader(mL, ID);
            diagramLoader.LockDiagram();
            diagramLoader.LoadDiagram(diagram);
            diagramLoader.UnLockDiagram();
        }

        /// <summary>
        /// Loads the specified _light model.
        /// </summary>
        /// <param name="_lightModel">The _light model.</param>
        /// <param name="_d">The _d.</param>
        public void Load(cwLightModel _lightModel, Diagram _d)
        {
            diagram = _d;// new Diagram(_lightModel.currentConnection);
            if (ID == -1) ID = 0;
            ModelLoader mL = new ModelLoader(_lightModel.currentConnection);
            diagramLoader = new DiagramLoader(mL, ID);
            diagramLoader.LoadDiagram(diagram);
        }
        /// <summary>
        /// Locks the diagram.
        /// </summary>
        /// <param name="_lightModel">The _light model.</param>
        public void lockDiagram(cwLightModel _lightModel)
        {
            if (!_lightModel.currentConnection.IsDiagramLocked(ID))
            {
                diagramLock = _lightModel.currentConnection.LockDiagram(ID);
                if (diagramLoader != null)
                {
                    diagramLoader.LockDiagram();
                }
            }
        }

        /// <summary>
        /// Unlocks the diagram.
        /// </summary>
        public void unlockDiagram()
        {
            if (diagramLoader != null)
            {
                diagramLoader.UnLockDiagram();
            }
            if (diagramLock != null)
            {
                diagramLock.Dispose();
                diagramLock = null;
            }
        }

        private List<cwShape> shapes = new List<cwShape>();
        /// <summary>
        /// Gets the shapes.
        /// </summary>
        /// <returns></returns>
        public List<cwShape> getShapes()
        {
            return shapes;
        }


        /// <summary>
        /// Gets the name of the diagram by.
        /// </summary>
        /// <param name="_lightModel">The _light model.</param>
        /// <param name="_name">The _name.</param>
        /// <returns></returns>
        public static cwLightDiagram getDiagramByName(cwLightModel _lightModel, string _name)
        {
            try
            {
                int diagramID = diagramOTID;
                cwLightObjectType OT = _lightModel.getObjectTypeByID(diagramID);

                StringBuilder query = new StringBuilder();
                query.Append("SELECT " + diagramSelectionProperties + " FROM DIAGRAM WHERE NAME = @SEARCHNAME");
                using (ICMCommand command = new ICMCommand(query.ToString(), _lightModel.currentConnection))
                {
                    command.Parameters.Add("@SEARCHNAME", _name);
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cwLightDiagram diagram = createDiagramFromReader(reader, _lightModel);
                            return diagram;
                        }
                    }
                }
                return null;
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        /// <summary>
        /// Shapes the most left.
        /// </summary>
        /// <returns></returns>
        public float shapeMostLeft()
        {
            int i = 0;
            float left_x = 0;// new PointF(0, 0);
            foreach (Shape ds in diagram.GetShapes())
            {
                if (i == 0)
                {
                    left_x = ds.Location.X;// new PointF(ds.Location.X, ds.Location.Y);
                }
                if (ds.Location.X < left_x)
                {
                    left_x = ds.Location.X;// new PointF(ds.Location.X, ds.Location.Y);
                }
                i++;
            }
            return left_x;
        }
        /// <summary>
        /// Shapes the most top.
        /// </summary>
        /// <returns></returns>
        public float shapeMostTop()
        {
            int i = 0;
            float top_y = 0;
            foreach (Shape ds in diagram.GetShapes())
            {
                if (i == 0)
                {
                    top_y = ds.Location.Y;
                }
                if (ds.Location.Y < top_y)
                {
                    top_y = ds.Location.Y;
                }
                i++;
            }
            return top_y;
        }


        /// <summary>
        /// Gets the palette entry.
        /// </summary>
        /// <param name="_lightModel">The _light model.</param>
        /// <param name="_object">The _object.</param>
        /// <returns></returns>
        public PaletteEntry getPaletteEntry(cwLightModel _lightModel, cwLightObject _object)
        {
            cwLightObjectType OT = _lightModel.getObjectTypeByID(_object.OTID);
            PaletteEntry PE = null;
            // TODO
            //if (_object.properties.ContainsKey("TYPE" + cwLookupManager.LOOKUPID_KEY))
            //{
            //    int typeID = int.Parse(_object.properties["TYPE" + cwLookupManager.LOOKUPID_KEY]);
            //    if (typeID == 0)
            //    {
            //        PE = diagram.Palette.FindPaletteEntry(OT.mycwLightObjectType);
            //        return PE;
            //    }

            //    PE = diagram.Palette.FindExactPaletteEntry(OT.mycwLightObjectType, typeID, 0);
            //    if (PE == null)
            //    {
            //        PE = diagram.Palette.FindPaletteEntry(OT.mycwLightObjectType);
            //        return PE;
            //    }
            //}
            //else
            //{
            //    PE = diagram.Palette.FindPaletteEntry(OT.mycwLightObjectType);
            //    return PE;
            //}
            return PE;
        }

        /// <summary>
        /// Deletes the diagram.
        /// </summary>
        /// <param name="_lightModel">The _light model.</param>
        /// <param name="_ID">The _ ID.</param>
        public static void deleteDiagram(cwLightModel _lightModel, int _ID)
        {
            using (ICMCommand deleteCommand = new ICMCommand("DELETE FROM DIAGRAM WHERE ID = @DID", _lightModel.currentConnection))
            {
                deleteCommand.Parameters.Add("@DID", _ID);
                using (IDbTransaction trans = _lightModel.currentConnection.BeginTransaction())
                {
                    try
                    {
                        deleteCommand.ExecuteReader();
                        trans.Commit();
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
            }
        }


        /// <summary>
        /// Creates the diagram from reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="_lightModel">The _light model.</param>
        /// <returns></returns>
        public static cwLightDiagram createDiagramFromReader(IDataReader reader, cwLightModel _lightModel)
        {
            int id = reader.GetInt32(0);
            string title = reader.GetString(1);
            string name = reader.GetString(2);
            int type_id = reader.GetInt32(3);
            int diagrammer_id = reader.GetInt32(4);
            int OT_ID = reader.GetInt32(5);
            int O_ID = reader.GetInt32(6);
            string description = reader.GetString(7);
            string UUID = reader.GetString(8);
            int validated = reader.GetInt32(9);
            //int D_SEQ = reader.GetInt32(7);

            // load the type from the lookups list
            string type_name = "";
            string type_abbr = "";
            if (type_id != 0)
            {
                if (!_lightModel.lookupManager.hasLookupID(type_id))
                {
                    throw new cwExceptionFatal("Meta model has been changed, a new lookup has been added, please reload the application");
                }
                type_name = _lightModel.lookupManager.getLookupNameByID(type_id);
                type_abbr = _lightModel.lookupManager.getLookupAbbreviationByID(type_id);
            }
            cwLightDiagram diagram = new cwLightDiagram(_lightModel, id, name, title, UUID);
            diagram.properties["TYPE"] = type_name;
            diagram.properties["TYPE" + cwLookupManager.LOOKUPID_KEY] = type_id.ToString();
            diagram.properties["TYPE" + cwLookupManager.LOOKUPABBR_KEY] = type_abbr.ToString();
            diagram.properties["DESCRIPTION"] = description;
            diagram.properties["VALIDATED"] = validated.ToString();

            diagram.parent = new cwLightObject(_lightModel, O_ID, OT_ID);
            return diagram;
        }

        /// <summary>
        /// Gets the picture UUID from symbol unique identifier.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <returns></returns>
        public static Dictionary<int, string> GetPictureUuidFromSymbolId(cwLightDiagram d)
        {
            Dictionary<int, string> pictureBySymbol = new Dictionary<int, string>();
            XmlDocument diagramInfo = new XmlDocument();
            diagramInfo.LoadXml(d.symbolsXml);
            XmlNodeList symbols = diagramInfo.SelectNodes("./diagramInfo/symbols/symbol");
            foreach (XmlNode n in symbols)
            {
                int id = Convert.ToInt32(n.Attributes["id"].Value);
                string uuid = n.Attributes["picture"].Value.ToString();
                pictureBySymbol[id] = uuid;
            }
            return pictureBySymbol;
        }

        /// <summary>
        /// Creates the diagram from properties.
        /// </summary>
        /// <param name="_lightModel">The _light model.</param>
        /// <param name="_obj">The _obj.</param>
        /// <returns></returns>
        public static cwLightDiagram CreateDiagramFromLightObject(cwLightModel _lightModel, cwLightObject _obj)
        {
            int id = Convert.ToInt32(_obj.properties["ID"]);
            string title = _obj.properties["TITLE"];
            string name = _obj.properties["NAME"];
            int type_id = Convert.ToInt32(_obj.properties["TYPE" + cwLookupManager.LOOKUPID_KEY]);
            int diagrammer_id = Convert.ToInt32(_obj.properties["DIAGRAMMER" + cwLookupManager.LOOKUPID_KEY]);
            int OT_ID = Convert.ToInt32(_obj.properties["TABLENUMBER"]);
            int O_ID = Convert.ToInt32(_obj.properties["OBJECTID"]);
            string description = _obj.properties["DESCRIPTION"];
            string UUID = _obj.properties["UNIQUEIDENTIFIER"];
            int validated = Convert.ToInt32(_obj.properties["VALIDATED"]);

            // load the type from the lookups list
            string type_name = "";
            string type_abbr = "";
            if (type_id != 0)
            {
                if (!_lightModel.lookupManager.hasLookupID(type_id))
                {
                    throw new cwExceptionFatal("Meta model has been changed, a new lookup has been added, please reload the application");
                }
                type_name = _lightModel.lookupManager.getLookupNameByID(type_id);
                type_abbr = _lightModel.lookupManager.getLookupAbbreviationByID(type_id);
            }
            cwLightDiagram diagram = new cwLightDiagram(_lightModel, id, name, title, UUID);
            diagram.properties["TYPE"] = type_name;
            diagram.properties["TYPE" + cwLookupManager.LOOKUPID_KEY] = type_id.ToString();
            diagram.properties["TYPE" + cwLookupManager.LOOKUPABBR_KEY] = type_abbr.ToString();
            diagram.properties["DESCRIPTION"] = description;
            diagram.properties["VALIDATED"] = validated.Equals(0) ? Boolean.FalseString : Boolean.TrueString;

            // all other properties
            cwLightObjectType ot = _lightModel.getObjectTypeByScriptName("DIAGRAM");
            string[] alreadySaved = { "SYMBOLS", "DIAGRAMMER", "DIAGRAMMER" + cwLookupManager.LOOKUPID_KEY, "DIAGRAMMER" + cwLookupManager.LOOKUPABBR_KEY, "TABLENUMBER", "OBJECTID",
                                    "ID", "TITLE", "NAME", "UNIQUEIDENTIFIER", "DESCRIPTION", "VALIDATED", "TYPE", "TYPE"+cwLookupManager.LOOKUPID_KEY, "TYPE"+cwLookupManager.LOOKUPABBR_KEY};
            foreach (var v in _obj.properties)
            {
                if (!alreadySaved.Contains(v.Key))
                {
                    cwLightProperty prop = ot.propertiesByScriptName[v.Key];
                    if (prop.isBoolean)
                    {
                        diagram.properties[v.Key] = v.Value.Equals("0") ? Boolean.FalseString : Boolean.TrueString;
                    }
                    else if (prop.isLookup)
                    {
                        if (!_lightModel.lookupManager.hasLookupID(type_id))
                        {
                            throw new cwExceptionFatal("Meta model has been changed, a new lookup has been added, please reload the application");
                        }
                        string _abbr = "";
                        string _name = "";
                        if (!v.Value.Equals("0"))
                        {
                            _abbr = _lightModel.lookupManager.getLookupNameByID(Convert.ToInt32(v.Value));
                            _name = _lightModel.lookupManager.getLookupAbbreviationByID(Convert.ToInt32(v.Value));
                        }
                    }
                    else
                    {
                        diagram.properties[v.Key] = v.Value;
                    }
                }
            }

            diagram.parent = new cwLightObject(_lightModel, O_ID, OT_ID);
            diagram.symbolsXml = _obj.properties["SYMBOLS"];
            return diagram;
        }

        /// <summary>
        /// Loads all templates.
        /// </summary>
        /// <param name="_lightModel">The _light model.</param>
        /// <param name="_diagrams">The _diagrams.</param>
        public static void LoadAllTemplates(cwLightModel _lightModel, Dictionary<int, cwLightDiagram> _diagrams)
        {
            StringBuilder query = new StringBuilder();
            try
            {
                query.Append("SELECT " + diagramSelectionProperties + " FROM DIAGRAM WHERE TEMPLATE <> 0");

                using (ICMCommand command = new ICMCommand(query.ToString(), _lightModel.currentConnection))
                {
                    using (ICMDataReader reader = command.ExecuteReader() as ICMDataReader)
                    {
                        while (reader.Read())
                        {
                            cwLightDiagram diagram = createDiagramFromReader(reader, _lightModel);
                            _diagrams.Add(diagram.ID, diagram);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Loads the diagrams.
        /// </summary>
        /// <param name="_lightModel">The _light model.</param>
        /// <param name="_diagrams">The _diagrams.</param>
        /// <param name="_diagramsToLoadIDList">The _diagrams to load ID list.</param>
        /// <param name="propertiesToLoad">The properties to load.</param>
        public static void loadAllDiagrams(cwLightModel _lightModel, Dictionary<int, cwLightDiagram> _diagrams, List<int> _diagramsToLoadIDList, List<string> propertiesToLoad)
        {
            try
            {
                cwLightNodeObjectType node = new cwLightNodeObjectType(_lightModel.getObjectTypeByScriptName("DIAGRAM"));
                List<string> _selectedProperties = cwLightDiagram.getPropertiesToSelect(propertiesToLoad);
                node.addPropertiesToSelect(_selectedProperties.ToArray());
                node.preloadLightObjects();
                Dictionary<int, cwLightObject> _selectedDiagrams = node.usedOTLightObjectsByID;
                foreach (int _id in _diagramsToLoadIDList)
                {
                    if (_selectedDiagrams.ContainsKey(_id))
                    {
                        _diagrams[_id] = CreateDiagramFromLightObject(_lightModel, _selectedDiagrams[_id]);
                    }
                }
                //List<string> idsAsListOfString = new List<string>();
                //if (_diagramsToLoadIDList != null)
                //{
                //    _diagramsToLoadIDList.ForEach(k =>
                //    {
                //        idsAsListOfString.Add(k.ToString());
                //    });
                //}
                //int end = _diagramsToLoadIDList != null ? _diagramsToLoadIDList.Count : 0;
                //int MAX = _lightModel.DataBaseType.Equals(DbManagementSystem.SQL_Server) ? 2100 : 1000; // limitation imposée par la technologie utilisée
                //int iterationMax = int.Parse((end / MAX).ToString());
                //List<string> _selectedProperties = cwLightDiagram.getPropertiesToSelect(propertiesToLoad);
                //int i = 0;
                //while (i < iterationMax)
                //{
                //    string ids = string.Join(",", idsAsListOfString.ToArray(), i * MAX, MAX);
                //    cwLightDiagram.ExecuteLoadAllDiagramQuery(_lightModel, _diagrams, ids, _selectedProperties);
                //    i++;
                //}
                //if ((end % MAX) < MAX)
                //{
                //    string ids = string.Join(",", idsAsListOfString.ToArray(), i * MAX, end % MAX);
                //    cwLightDiagram.ExecuteLoadAllDiagramQuery(_lightModel, _diagrams, ids, _selectedProperties);
                //}
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Executes the load all diagram query.
        /// </summary>
        /// <param name="_lightModel">The _light model.</param>
        /// <param name="_diagrams">The _diagrams.</param>
        /// <param name="_diagramsToLoadIDList">The _diagrams to load ID list.</param>
        /// <param name="propertiesToLoad">The properties to load.</param>
        private static void ExecuteLoadAllDiagramQuery(cwLightModel _lightModel, Dictionary<int, cwLightDiagram> _diagrams, string _diagramsToLoadIDList, List<string> propertiesToLoad)
        {
            StringBuilder query = new StringBuilder();
            try
            {
                query.Append("SELECT " + propertiesToLoad + " FROM DIAGRAM");
                if (!string.IsNullOrEmpty(_diagramsToLoadIDList))
                {
                    query.Append(" WHERE ID IN (" + _diagramsToLoadIDList + ")");
                }

                using (ICMCommand command = new ICMCommand(query.ToString(), _lightModel.currentConnection))
                {
                    using (ICMDataReader reader = command.ExecuteReader() as ICMDataReader)
                    {
                        while (reader.Read())
                        {
                            cwLightDiagram diagram = createDiagramFromReader(reader, _lightModel);
                            _diagrams.Add(diagram.ID, diagram);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Gets the properties to select.
        /// </summary>
        /// <param name="properties">The properties.</param>
        /// <returns></returns>
        private static List<string> getPropertiesToSelect(List<string> properties)
        {
            string[] defaultProperties = diagramSelectionProperties.Split(',');
            List<string> propertiesToSelect = new List<string>();
            foreach (string p in defaultProperties)
            {
                propertiesToSelect.Add(p.Trim());
            }

            if (properties != null)
            {
                foreach (string p in properties)
                {
                    if (!propertiesToSelect.Contains(p))
                    {
                        propertiesToSelect.Add(p);
                    }
                }
            }
            return propertiesToSelect;
        }

        /// <summary>
        /// Loads all parent diagram id.
        /// </summary>
        /// <param name="_lightModel">The _light model.</param>
        /// <param name="_diagrams">The _diagrams.</param>
        /// <param name="_fatherDiagramByDiagramId">The _father diagram by diagram id.</param>
        public static void loadAllParentDiagrams(cwLightModel _lightModel, List<int> _diagrams, Dictionary<int, List<int>> _fatherDiagramByDiagramId)
        {
            try
            {
                List<string> idsAsListOfString = new List<string>();
                _diagrams.ForEach(k =>
                {
                    idsAsListOfString.Add(k.ToString());
                });
                int end = _diagrams.Count;
                int MAX = _lightModel.DataBaseType.Equals(DbManagementSystem.SQL_Server) ? 2000 : 1000; // limitation imposée par la technologie utilisée
                int iterationMax = int.Parse((end / MAX).ToString());

                int i = 0;
                while (i < iterationMax)
                {
                    string ids = string.Join(",", idsAsListOfString.ToArray(), i * MAX, MAX);
                    cwLightDiagram.ExecuteLoadParentDiagramQuery(_lightModel, _fatherDiagramByDiagramId, ids);
                    i++;
                }
                if ((end % MAX) < MAX)
                {
                    string ids = string.Join(",", idsAsListOfString.ToArray(), i * MAX, end % MAX);
                    cwLightDiagram.ExecuteLoadParentDiagramQuery(_lightModel, _fatherDiagramByDiagramId, ids);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Executes the load parent diagram query.
        /// </summary>
        /// <param name="_lightModel">The _light model.</param>
        /// <param name="fatherIds">The father ids.</param>
        /// <param name="childrenDiagramIds">The children diagram ids.</param>
        private static void ExecuteLoadParentDiagramQuery(cwLightModel _lightModel, Dictionary<int, List<int>> fatherIds, string childrenDiagramIds)
        {
            try
            {
                Dictionary<int, KeyValuePair<int, int>> fatherObjectByDiagramId = new Dictionary<int, KeyValuePair<int, int>>();
                StringBuilder query = new StringBuilder();
                query.Append("SELECT ID, TABLENUMBER, OBJECTID FROM DIAGRAM WHERE OBJECTID <> 0 AND TABLENUMBER <> 0");
                if (!string.IsNullOrEmpty(childrenDiagramIds))
                {
                    query.Append(" WHERE ID IN (" + childrenDiagramIds + ")");
                }

                using (ICMCommand command = new ICMCommand(query.ToString(), _lightModel.currentConnection))
                {
                    using (ICMDataReader reader = command.ExecuteReader() as ICMDataReader)
                    {
                        while (reader.Read())
                        {
                            fatherObjectByDiagramId[reader.GetInt32(0)] = new KeyValuePair<int, int>(reader.GetInt32(1), reader.GetInt32(2));
                        }
                    }
                }

                foreach (var v in fatherObjectByDiagramId)
                {
                    int childDiagram = v.Key;
                    query = new StringBuilder();
                    query.Append("SELECT DIAGRAMID FROM SHAPE WHERE TABLENUMBER = @TABLENUMBER AND OBJECTID = @OBJECTID");
                    using (ICMCommand cmd = new ICMCommand(query.ToString(), _lightModel.currentConnection))
                    {
                        cmd.Parameters.Add("@TABLENUMBER", v.Value.Key);
                        cmd.Parameters.Add("@OBJECTID", v.Value.Value);
                        using (ICMDataReader reader = cmd.ExecuteReader() as ICMDataReader)
                        {
                            while (reader.Read())
                            {
                                if (!fatherIds.ContainsKey(childDiagram))
                                {
                                    fatherIds[childDiagram] = new List<int>();
                                }
                                fatherIds[childDiagram].Add(reader.GetInt32(0));
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Associates the shapes.
        /// </summary>
        /// <param name="level1Shape">The level1 shape.</param>
        /// <param name="lastLevel2_shape">The last level2_shape.</param>
        /// <param name="AST_Level1ToLevel2">The AS t_ level1 to level2.</param>
        internal void AssociateShapes(cwShape level1Shape, cwShape lastLevel2_shape, cwLightAssociationType AST_Level1ToLevel2)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Shapes the most down.
        /// </summary>
        /// <param name="diagramShapes">The diagram shapes.</param>
        /// <returns></returns>
        internal cwShape shapeMostDown(List<cwShape> diagramShapes)
        {
            if (diagramShapes.Count().Equals(0)) return null;
            cwShape down_y = diagramShapes[0];
            foreach (cwShape ds in diagramShapes)
            {
                if (ds.Position.Y + ds.Size.Height > down_y.Position.Y + down_y.Size.Height)
                {
                    down_y = ds;
                }
            }
            return down_y;
        }
        /// <summary>
        /// Shapes the most right.
        /// </summary>
        /// <param name="diagramShapes">The diagram shapes.</param>
        /// <returns></returns>
        internal cwShape shapeMostRight(List<cwShape> diagramShapes)
        {
            if (diagramShapes == null) return null;
            if (diagramShapes.Count().Equals(0)) return null;
            cwShape right_x = diagramShapes[0];
            foreach (cwShape ds in diagramShapes)
            {
                if (ds.Position.X + ds.Size.Width > right_x.Position.X + right_x.Size.Width)
                {
                    right_x = ds;
                }
            }
            return right_x;
        }
        /// <summary>
        /// Shapes the most top.
        /// </summary>
        /// <param name="diagramShapes">The diagram shapes.</param>
        /// <returns></returns>
        internal cwShape shapeMostTop(List<cwShape> diagramShapes)
        {
            if (diagramShapes.Count().Equals(0)) return null;
            cwShape top_y = diagramShapes[0];
            foreach (cwShape ds in diagramShapes)
            {
                if (ds.Position.Y < top_y.Position.Y)
                {
                    top_y = ds;
                }
            }
            return top_y;
        }




        /// <summary>
        /// Gets the shapes on bottom.
        /// </summary>
        /// <param name="shape">The shape.</param>
        /// <returns></returns>
        internal List<cwShape> getShapesOnBottom(cwShape shape)
        {
            List<cwShape> result = new List<cwShape>();
            float Y = shape.Position.Y + shape.Size.Height;
            float Xleft = shape.Position.X;
            float Xright = shape.Position.X + shape.Size.Width;
            List<cwShape> myShapes = getShapes();
            foreach (cwShape ds in myShapes)
            {
                float y = ds.Position.Y;
                float x1 = ds.Position.X;
                float x2 = ds.Position.X + ds.Size.Width;
                if (y > Y)
                {
                    if ((x1 <= Xleft && x2 >= Xright) || (x1 <= Xleft && x2 <= Xright && x2 >= Xleft) || (x2 >= Xright && x1 >= Xleft && x1 <= Xright) || (x1 <= Xright && x1 >= Xleft && x2 <= Xright && x2 >= Xleft))
                    {
                        // part of ds is in the range of shape
                        result.Add(ds);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Gets the shapes on right.
        /// </summary>
        /// <param name="shape">The shape.</param>
        /// <returns></returns>
        internal List<cwShape> getShapesOnRight(cwShape shape)
        {
            List<cwShape> result = new List<cwShape>();
            float X = shape.Position.X + shape.Size.Width;
            float Ytop = shape.Position.Y;
            float Ybot = shape.Position.Y + shape.Size.Height;
            List<cwShape> myShapes = getShapes();
            foreach (cwShape ds in myShapes)
            {
                float x = ds.Position.X;
                float y1 = ds.Position.Y;
                float y2 = ds.Position.Y + ds.Size.Height;
                if (x > X)
                {
                    if ((y1 <= Ytop && y2 >= Ybot) || (y1 <= Ytop && y2 <= Ybot && y2 >= Ytop) || (y2 >= Ybot && y1 >= Ytop && y1 <= Ybot) || (y1 <= Ybot && y1 >= Ytop && y2 <= Ybot && y2 >= Ytop))
                    {
                        // part of ds is in the range of shape
                        result.Add(ds);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Gets the shapes on top.
        /// </summary>
        /// <param name="parentShape">The parent shape.</param>
        /// <returns></returns>
        internal List<cwShape> getShapesOnTop(cwShape parentShape)
        {
            return new List<cwShape>();
        }


    }
}
