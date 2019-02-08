using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Casewise.Data.ICM;
using System.Data;
using log4net;

namespace Casewise.GraphAPI.API.Diagrams
{
    /// <summary>
    /// cwDiagramLoaderShapesManager
    /// </summary>
    public class cwDiagramLoaderShapesManager : cwDiagramLoaderManager
    {
        /// <summary>
        /// diagramsShapes
        /// </summary>
        public Dictionary<int, Dictionary<int, cwShape>> diagramsShapes = new Dictionary<int, Dictionary<int, cwShape>>();

        /// <summary>
        /// The diagram ids by internal event
        /// </summary>
        public Dictionary<int, List<int>> diagramIdsByInternalEvent = new Dictionary<int, List<int>>();
        /// <summary>
        /// The diagram ids by internal result
        /// </summary>
        public Dictionary<int, List<int>> diagramIdsByInternalResult = new Dictionary<int, List<int>>();

        /// <summary>
        /// orderedShapes
        /// </summary>
        public Dictionary<int, List<cwShape>> orderedShapes = new Dictionary<int, List<cwShape>>();
        private static readonly ILog log = LogManager.GetLogger(typeof(cwDiagramLoaderShapesManager));

        /// <summary>
        /// Initializes a new instance of the <see cref="cwDiagramLoaderShapesManager"/> class.
        /// </summary>
        /// <param name="diagramLoader">The diagram loader.</param>
        public cwDiagramLoaderShapesManager(cwDiagramLoader diagramLoader)
            : base(diagramLoader)
        { }


        /// <summary>
        /// Cleans the lists.
        /// </summary>
        public override void cleanLists()
        {
            orderedShapes.Clear();
            diagramsShapes.Clear();
        }


        /// <summary>
        /// Orders the index of the shapes by Z.
        /// </summary>
        private void orderShapesByZIndex()
        {
            foreach (var diagramVar in this.diagramsShapes)
            {
                List<cwShape> _orderedShapesList = new List<cwShape>();
                Dictionary<int, cwShape> shapesByNextSequence = new Dictionary<int, cwShape>();
                foreach (var shapeVar in diagramVar.Value)
                {
                    if (!shapesByNextSequence.ContainsKey(shapeVar.Value.nextZSequence))
                    {
                        shapesByNextSequence[shapeVar.Value.nextZSequence] = shapeVar.Value;
                    }
                }

                List<int> zIndexes = new List<int>(shapesByNextSequence.Keys);
                List<string> shapesUsed = new List<string>();

                int nextZsequence = 0;
                while (zIndexes.Count > 0)
                {
                    if (!zIndexes.Contains(nextZsequence))
                    {
                        nextZsequence = zIndexes.Max();
                    }
                    while (zIndexes.Contains(nextZsequence))
                    {
                        cwShape shape = shapesByNextSequence[nextZsequence]; // top level
                        zIndexes.Remove(nextZsequence);
                        nextZsequence = shape.sequence;
                        if (!shapesUsed.Contains(shape.ToString()))
                        {
                            _orderedShapesList.Add(shape);
                            shapesUsed.Add(shape.ToString());
                        }
                    }
                }
                orderedShapes[diagramVar.Key] = _orderedShapesList;
            }
        }


        /// <summary>
        /// Loads the shapes and order them.
        /// </summary>
        /// <param name="diagramsIDList">The diagrams ID list.</param>
        public void loadShapesAndOrderThem(string diagramsIDList)
        {
            // select all diagrams shapes              
            DateTime start = DateTime.Now;
            loadAllShapesByDiagram(diagramsIDList);
            log.Info("TIME;SHAPES-LOADED;" + DateTime.Now.Subtract(start).ToString());

            start = DateTime.Now;
            orderShapesByZIndex();
            log.Info("TIME;SHAPES-ORDERED-BY-Z;" + DateTime.Now.Subtract(start).ToString());
        }

        /// <summary>
        /// Gets all shapes by diagram.
        /// </summary>
        /// <param name="_diagramsIDList">The _diagrams ID list.</param>
        private void loadAllShapesByDiagram(string _diagramsIDList)
        {
            try
            {
                StringBuilder query = new StringBuilder();
                query.Append("SELECT DIAGRAMID, TABLENUMBER, OBJECTID, WIDTH, HEIGHT, X, Y, SEQUENCE, FLAGS1, FLAGS2, PARENTSEQUENCE, NEXTZSEQUENCE, STYLEID, SYMBOLID FROM SHAPE");
                if (_diagramsIDList != null && !0.Equals(_diagramsIDList.Count()))
                {
                    query.Append(" WHERE DIAGRAMID IN (" + _diagramsIDList + ")");
                }
                using (ICMCommand command = new ICMCommand(query.ToString(), Model.getConnection()))
                {
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int dID = reader.GetInt32(0);
                            int otID = reader.GetInt32(1);
                            int oID = reader.GetInt32(2);
                            int width = reader.GetInt32(3) / cwLightDiagram.mmRatio;
                            int height = reader.GetInt32(4) / cwLightDiagram.mmRatio;
                            int x = reader.GetInt32(5) / cwLightDiagram.mmRatio;
                            int y = -reader.GetInt32(6) / cwLightDiagram.mmRatio;
                            int seq = reader.GetInt32(7);
                            int flag1 = reader.GetInt32(8);
                            int flag2 = reader.GetInt32(9);
                            int parentSequence = reader.GetInt32(10);
                            int nextZSequence = reader.GetInt32(11);
                            int styleID = reader.GetInt32(12);
                            int symbol = reader.GetInt32(13);

                            // check if the diagram cache containts the diagram
                            if (!diagramsShapes.ContainsKey(dID)) // create the diagram
                            {
                                diagramsShapes[dID] = new Dictionary<int, cwShape>();
                            }
                            diagramLoader.updateMinMax(x, y, width, height, dID);

                            cwShape shape = new cwShape(oID, otID, width, height, x, y, seq);
                            diagramsShapes[dID][seq] = shape;
                            diagramsShapes[dID][seq].lightObject = diagramLoader.getLightObject(Model, otID, oID);
                            diagramsShapes[dID][seq].flag1 = flag1;
                            diagramsShapes[dID][seq].flag2 = flag2;
                            diagramsShapes[dID][seq].parentSequence = parentSequence;
                            diagramsShapes[dID][seq].nextZSequence = nextZSequence;
                            if (symbol != 0)
                            {
                                diagramsShapes[dID][seq].customSymbol = symbol;
                            }
                            if (styleID != 0)
                            {
                                diagramsShapes[dID][seq].customStyleID = styleID;
                                if (!diagramLoader.stylesRequired.Contains(styleID))
                                {
                                    diagramLoader.stylesRequired.Add(styleID);
                                }
                            }

                            // pour chaque internal event/result, on liste les diagrammes affichant les elements
                            // d'un côté les diagrammes avec les internal events
                            // de l'autre les diagrammes avec les internal results
                            if (shape.isInternalEvent())
                            {
                                if (!this.diagramIdsByInternalEvent.ContainsKey(oID))
                                {
                                    this.diagramIdsByInternalEvent[oID] = new List<int>();
                                }
                                this.diagramIdsByInternalEvent[oID].Add(dID);
                            }
                            if (shape.isInternalResult())
                            {
                                if (!this.diagramIdsByInternalResult.ContainsKey(oID))
                                {
                                    this.diagramIdsByInternalResult[oID] = new List<int>();
                                }
                                this.diagramIdsByInternalResult[oID].Add(dID);
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
    }
}
