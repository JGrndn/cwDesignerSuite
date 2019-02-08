using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Casewise.Data.ICM;
using System.Data;
using Casewise.GraphAPI.Exceptions;
using log4net;
using System.Drawing;
using Casewise.Services.Entities;
using Casewise.Services.Entities.DashBoard;

namespace Casewise.GraphAPI.API.Diagrams
{
    /// <summary>
    /// Picture ID and its coords on planche
    /// </summary>
    public class cwPicturesCoord
    {
        /// <summary>
        /// picture id
        /// </summary>
        public int ID;
        /// <summary>
        /// abs (mm)
        /// </summary>
        public float x;
        /// <summary>
        /// ords (mm)
        /// </summary>
        public float y;
        /// <summary>
        /// width (mm)
        /// </summary>
        public float w;
        /// <summary>
        /// height (mm)
        /// </summary>
        public float h;

        /// <summary>
        /// Initializes a new instance of the <see cref="cwPicturesCoord"/> struct.
        /// </summary>
        /// <param name="_id">The _id.</param>
        /// <param name="_x">The _x.</param>
        /// <param name="_y">The _y.</param>
        /// <param name="_w">The _w.</param>
        /// <param name="_h">The _h.</param>
        public cwPicturesCoord(int _id, float _x, float _y, float _w, float _h)
        {
            this.ID = _id;
            this.x = _x;
            this.y = _y;
            this.w = _w;
            this.h = _h;
        }
    }

    /// <summary>
    /// cwDiagramLoader
    /// </summary>
    public class cwDiagramLoader
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(cwDiagramLoader));

        /// <summary>
        /// diagramsMinMax
        /// </summary>
        public Dictionary<int, KeyValuePair<Point, Point>> diagramsMinMax = new Dictionary<int, KeyValuePair<Point, Point>>();
        /// <summary>
        /// objectGroups
        /// </summary>
        public Dictionary<int, cwLightNodeObjectType> objectGroups = new Dictionary<int, cwLightNodeObjectType>();
        /// <summary>
        /// stylesRequired
        /// </summary>
        public List<int> stylesRequired = new List<int>();
        /// <summary>
        /// propertiesToExportByOTIDForPropertyValueRegions
        /// </summary>
        public Dictionary<int, List<string>> propertiesToExportByOTIDForPropertyValueRegions = new Dictionary<int, List<string>>();
        /// <summary>
        /// associationsToExportByOTIDForPropertyValueRegions
        /// </summary>
        public Dictionary<string, List<string>> associationsToExportByOTScriptnameForPropertyValueRegions = new Dictionary<string, List<string>>();
        /// <summary>
        /// allDiagrams
        /// </summary>
        public Dictionary<int, cwLightDiagram> allDiagrams = new Dictionary<int, cwLightDiagram>();
        /// <summary>
        /// parent diagrams id
        /// </summary>
        public Dictionary<int, List<int>> parentDiagramIds = new Dictionary<int, List<int>>();
        /// <summary>
        /// templates
        /// </summary>
        public Dictionary<string, KeyValuePair<cwLightDiagram, Palette>> templates = new Dictionary<string, KeyValuePair<cwLightDiagram, Palette>>();

        /// <summary>
        /// list of template id where planche is already created
        /// </summary>
        public List<int> plancheCreatedTemplateID = new List<int>();

        private cwLightModel _model = null;
        private cwDiagramLoaderShapesManager shapesManager = null;
        private cwDiagramLoaderJoinersManager joinersManager = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="cwDiagramLoader"/> class.
        /// </summary>
        /// <param name="model">The model.</param>
        public cwDiagramLoader(cwLightModel model)
        {
            this._model = model;
            this.shapesManager = new cwDiagramLoaderShapesManager(this);
            this.joinersManager = new cwDiagramLoaderJoinersManager(this);
        }

        /// <summary>
        /// Gets the shapes manager.
        /// </summary>
        /// <returns></returns>
        public cwDiagramLoaderShapesManager getShapesManager()
        {
            return this.shapesManager;
        }

        /// <summary>
        /// Gets the ordered shapes.
        /// </summary>
        public Dictionary<int, List<cwShape>> orderedShapes
        {
            get
            {
                return this.shapesManager.orderedShapes;
            }
        }

        /// <summary>
        /// Gets the diagrams shapes.
        /// </summary>
        public Dictionary<int, Dictionary<int, cwShape>> diagramsShapes
        {
            get
            {
                return this.shapesManager.diagramsShapes;
            }
        }

        /// <summary>
        /// Gets the joiners by diagram.
        /// </summary>
        public Dictionary<int, List<cwJoiner>> joinersByDiagram
        {
            get
            {
                return this.joinersManager.joinersByDiagram;
            }
        }

        /// <summary>
        /// Gets the model.
        /// </summary>
        public cwLightModel Model
        {
            get
            {
                return this._model;
            }
        }

        /// <summary>
        /// Updates the min max.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="dID">The d ID.</param>
        internal void updateMinMax(int x, int y, int width, int height, int dID)
        {
            if (!diagramsMinMax.ContainsKey(dID))
            {
                diagramsMinMax[dID] = new KeyValuePair<Point, Point>(new Point(x, y), new Point(x, y));
            }
            Point minPoint = diagramsMinMax[dID].Key;
            if (x < minPoint.X) minPoint.X = x;
            if (y < minPoint.Y) minPoint.Y = y;
            Point maxPoint = diagramsMinMax[dID].Value;
            if (x + width > maxPoint.X) maxPoint.X = x + width;
            if (y + height > maxPoint.Y) maxPoint.Y = y + height;
            diagramsMinMax[dID] = new KeyValuePair<Point, Point>(minPoint, maxPoint);
        }

        /// <summary>
        /// Does the loads.
        /// </summary>
        /// <param name="_diagramsToLoad">The _diagrams to load.</param>
        /// <param name="_loadTemplates">if set to <c>true</c> [_load templates].</param>
        /// <param name="propertiesToLoad">The properties to load.</param>
        public void doLoads(List<int> _diagramsToLoad, bool _loadTemplates, List<string> propertiesToLoad)
        {
            cleanLists();

            string diagramsIDList = cwTools.idToStringSeparatedby(",", _diagramsToLoad);

            DateTime start = DateTime.Now;

            cwLightDiagram.LoadAllTemplates(Model, allDiagrams);
            cwLightDiagram.loadAllDiagrams(Model, allDiagrams, _diagramsToLoad, propertiesToLoad);
            cwLightDiagram.loadAllParentDiagrams(Model, _diagramsToLoad, parentDiagramIds);
            log.Info("TIME;DIAGRAMS-LOADED;" + DateTime.Now.Subtract(start).ToString());

            if (_loadTemplates)
            {
                start = DateTime.Now;
                loadTemplates(_diagramsToLoad);
                log.Info("TIME;TEMPLATES-LOADED;" + DateTime.Now.Subtract(start).ToString());
            }

            shapesManager.loadShapesAndOrderThem(diagramsIDList);
            joinersManager.loadJoinersAndPoints(diagramsIDList);
        }


        /// <summary>
        /// Cleans the lists.
        /// </summary>
        public void cleanLists()
        {
            shapesManager.cleanLists();
            joinersManager.cleanLists();
            objectGroups.Clear();
            diagramsMinMax.Clear();
            allDiagrams.Clear();
            templates.Clear();
            stylesRequired.Clear();
        }

        /// <summary>
        /// Loads the templates.
        /// </summary>
        /// <param name="_diagramsToLoad">The _diagrams to load.</param>
        private void loadTemplates(List<int> _diagramsToLoad)
        {
            List<int> diagramsToLoadTypes = new List<int>();

            // select the categories by type
            foreach (int dID in _diagramsToLoad)
            {
                cwLightDiagram d = allDiagrams[dID];
                int dTypeID = Convert.ToInt32(d.properties["TYPE" + cwLookupManager.LOOKUPID_KEY]);
                if (!diagramsToLoadTypes.Contains(dTypeID))
                {
                    diagramsToLoadTypes.Add(dTypeID);
                }
            }

            cwLightNodeObjectType _templates = new cwLightNodeObjectType(Model.getObjectTypeByScriptName("DIAGRAM"));
            _templates.addPropertyToSelect("TYPE");
            _templates.addAttributeForFilterAND("TEMPLATE", "1", "=");
            _templates.preloadLightObjects();

            foreach (cwLightObject t in _templates.usedOTLightObjects)
            {
                if (allDiagrams.ContainsKey(t.ID))
                {
                    cwLightDiagram d = allDiagrams[t.ID];
                    int templateTypeID = Convert.ToInt32(d.properties["TYPE" + cwLookupManager.LOOKUPID_KEY]);
                    if (!diagramsToLoadTypes.Contains(templateTypeID)) continue;
                    if (d.properties.ContainsKey("TYPE") && d.properties["TYPE"] != null && !"".Equals(d.properties["TYPE"]))
                    {
                        log.Info("LOADING TEMPLATE : " + d.ToString());
                        d.Load(Model);
                        templates[d.properties["TYPE"]] = new KeyValuePair<cwLightDiagram, Palette>(d, d.diagram.Palette);

                        foreach (PaletteEntry dpe in d.diagram.Palette.Entries)
                        {
                            foreach (PaletteEntry.PaletteShapeRegion paletteShapeRegion in dpe.Regions)
                            {
                                IDashboard dashboard = paletteShapeRegion.DashBoard;
                                if (paletteShapeRegion.ReferenceType.CurrentType.Equals(RegionRepresentationType.Navigation))
                                {
                                    ICMObjectType ot = dpe.ObjectType;
                                    string atScriptname = "ANYOBJECTSHOWNASSHAPEINDIAGRAM";
                                    if (!associationsToExportByOTScriptnameForPropertyValueRegions.ContainsKey(ot.ScriptName))
                                    {
                                        associationsToExportByOTScriptnameForPropertyValueRegions[ot.ScriptName] = new List<string>();
                                    }
                                    if (!associationsToExportByOTScriptnameForPropertyValueRegions[ot.ScriptName].Contains(atScriptname))
                                    {
                                        associationsToExportByOTScriptnameForPropertyValueRegions[ot.ScriptName].Add(atScriptname);
                                    }
                                }
                                //if (dashboard.RegionRepresentation.HasSourcePropertyType && !dashboard.RegionRepresentation.HasOperator)
                                if (dashboard.RegionRepresentation.HasSourcePropertyType)
                                {
                                    ICMPropertyType p = dashboard.SourcePropertyType;
                                    string pScriptName = p.ScriptName;
                                    int OTID = Model.getObjectTypeIDByScriptName(dpe.ObjectType.ScriptName);
                                    if (!propertiesToExportByOTIDForPropertyValueRegions.ContainsKey(OTID))
                                    {
                                        propertiesToExportByOTIDForPropertyValueRegions[OTID] = new List<string>();
                                    }
                                    if (!propertiesToExportByOTIDForPropertyValueRegions[OTID].Contains(pScriptName))
                                    {
                                        propertiesToExportByOTIDForPropertyValueRegions[OTID].Add(pScriptName);
                                    }
                                }
                                if (paletteShapeRegion.RepresentsAssociation)
                                {
                                    ICMObjectType ot = dpe.ObjectType;
                                    IShapeAdditionalData additionalData = paletteShapeRegion.ShapeAdditionalData;
                                    string atScriptname = additionalData.ScriptName;
                                    if (!associationsToExportByOTScriptnameForPropertyValueRegions.ContainsKey(ot.ScriptName))
                                    {
                                        associationsToExportByOTScriptnameForPropertyValueRegions[ot.ScriptName] = new List<string>();
                                    }
                                    if (!associationsToExportByOTScriptnameForPropertyValueRegions[ot.ScriptName].Contains(atScriptname))
                                    {
                                        associationsToExportByOTScriptnameForPropertyValueRegions[ot.ScriptName].Add(atScriptname);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the object group.
        /// </summary>
        /// <param name="_lightModel">The _light model.</param>
        /// <param name="OTID">The OTID.</param>
        /// <returns></returns>
        public cwLightNodeObjectType getObjectGroupNode(cwLightModel _lightModel, int OTID)
        {
            if (!objectGroups.ContainsKey(OTID))
            {
                cwLightObjectType OT = _lightModel.getObjectTypeByID(OTID);

                DateTime start = DateTime.Now;
                cwLightNodeObjectType OG = new cwLightNodeObjectType(OT);
                if ("PICTURE".Equals(OT.ScriptName)) return null;
                if ("RELATIONSHIP".Equals(OT.ScriptName)) return null;
                if ("OBJECTLINK".Equals(OT.ScriptName)) return null;

                // add type
                if (OT.hasProperty(OT.getTypePropertyScriptName()))
                {
                    OG.addPropertyToSelect(OT.getTypePropertyScriptName());
                }

                // add name
                OG.addPropertyToSelect(OT.getNamePropertyScriptName());
                OG.addPropertyToSelect(cwLightObject.UNIQUEIDENTIFIER);
                // load the properties required for property value type regions
                if (propertiesToExportByOTIDForPropertyValueRegions.ContainsKey(OT.ID))
                {
                    foreach (string propertyToLoadScriptName in propertiesToExportByOTIDForPropertyValueRegions[OTID])
                    {
                        if (!OG.selectedPropertiesScriptName.Contains(propertyToLoadScriptName))
                        {
                            OG.addPropertyToSelect(propertyToLoadScriptName);
                        }
                    }
                }
                // add exploded diagram
                cwLightAssociationType ATToDiagram = OG.sourceObjectType.getAssociationTypeByScriptName("ANYOBJECTEXPLODEDASDIAGRAM");
                if (ATToDiagram != null)
                {
                    cwLightNodeAssociationType ATExplodedDiagramNode = new cwLightNodeAssociationType(OG, ATToDiagram.Target, ATToDiagram);
                    ATExplodedDiagramNode.ID = "diagramExploded";
                    ATExplodedDiagramNode.sortOnPropertyScriptName = "ID";
                    ATExplodedDiagramNode.preloadLightObjects();
                }

                OG.preloadLightObjects();
                if (associationsToExportByOTScriptnameForPropertyValueRegions.ContainsKey(OG.sourceObjectType.ScriptName))
                {
                    foreach (string atScriptname in associationsToExportByOTScriptnameForPropertyValueRegions[OG.sourceObjectType.ScriptName])
                    {
                        cwLightAssociationType at = OG.sourceObjectType.getAssociationTypeByScriptName(atScriptname);
                        if (at != null)
                        {
                            cwLightNodeAssociationType ATNode = new cwLightNodeAssociationType(OG, at.Target, at);
                            ATNode.ID = atScriptname;
                            ATNode.preloadLightObjects();
                        }
                    }
                }
                log.Info("LOADED;" + OT.ToString() + ";" + DateTime.Now.Subtract(start).ToString());
                objectGroups[OTID] = OG;
            }
            return objectGroups[OTID];
        }

        /// <summary>
        /// Gets the light object.
        /// </summary>
        /// <param name="_lightModel">The _light model.</param>
        /// <param name="otID">The ot ID.</param>
        /// <param name="oID">The o ID.</param>
        /// <returns></returns>
        public cwLightObject getLightObject(cwLightModel _lightModel, int otID, int oID)
        {
            if (oID == 0) return null;
            cwLightNodeObjectType OG = getObjectGroupNode(_lightModel, otID);
            if (OG == null) return null;
            if (!OG.usedOTLightObjectsByID.ContainsKey(oID))
            {
                throw new cwExceptionFatal("Required Object [" + oID + "] has not been found inside the cwLightObjectType [" + OG.sourceObjectType.ToString() + "]");
            }
            return OG.usedOTLightObjectsByID[oID];
        }

        /// <summary>
        /// Gets the diagrams where event result is shown.
        /// </summary>
        /// <param name="o">The automatic.</param>
        /// <param name="diagramsForEvent">The diagrams for event.</param>
        /// <param name="diagramsForResult">The diagrams for result.</param>
        public void getDiagramsWhereEventResultIsShown(cwLightObject o, List<cwLightObject> diagramsForEvent, List<cwLightObject> diagramsForResult)
        {
            using (ICMCommand cmd = new ICMCommand("SELECT DIAGRAMID, FLAGS2 FROM SHAPE WHERE TABLENUMBER = " + o.OTID + " AND OBJECTID = " + o.ID, o.Model.currentConnection))
            {
                using (ICMDataReader reader = cmd.ExecuteReader() as ICMDataReader)
                {
                    while (reader.Read())
                    {
                        int diagramId = reader.GetInt32(0);
                        int flags = reader.GetInt32(1);
                        if (cwShape.isInternalEvent(flags))
                        {
                            if (!diagramsForEvent.Exists(d => d.ID == diagramId))
                            {
                                cwLightObjectType ot = Model.getObjectTypeByID(6854);
                                cwLightObject d = ot.getObjectByID(diagramId);
                                diagramsForEvent.Add(d);
                            }
                        }
                        if (cwShape.isInternalResult(flags))
                        {
                            if (!diagramsForResult.Exists(d => d.ID == diagramId))
                            {
                                cwLightObjectType ot = Model.getObjectTypeByID(6854);
                                cwLightObject d = ot.getObjectByID(diagramId);
                                diagramsForResult.Add(d);
                            }
                        }
                    }
                }
            }
        }


    }
}
