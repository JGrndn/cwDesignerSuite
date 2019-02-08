using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.Drawing;
using System.Data;
using Casewise.Data.ICM;

namespace Casewise.GraphAPI.API.Diagrams
{
    class cwDiagramLoaderJoinersManager : cwDiagramLoaderManager
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(cwDiagramLoaderManager));
        public Dictionary<int, Dictionary<int, SortedDictionary<int, cwJoinerPoint>>> pointsByDiagram = new Dictionary<int, Dictionary<int, SortedDictionary<int, cwJoinerPoint>>>();
        public Dictionary<int, List<cwJoiner>> joinersByDiagram = new Dictionary<int, List<cwJoiner>>();


        public cwDiagramLoaderJoinersManager(cwDiagramLoader diagramLoader)
            : base(diagramLoader)
        { }

        public override void cleanLists()
        {
            pointsByDiagram.Clear();
            joinersByDiagram.Clear();
        }

        public void loadJoinersAndPoints(string diagramsIDList)
        {
            DateTime start = DateTime.Now;
            loadAllJoinerPoints(diagramsIDList);
            log.Info("TIME;POINTS-LOADED;" + DateTime.Now.Subtract(start).ToString());

            start = DateTime.Now;
            loadAllJoinersByDiagram(diagramsIDList);
            log.Info("TIME;JOINERS-LOADED;" + DateTime.Now.Subtract(start).ToString());
        }

        /// <summary>
        /// Loads all joiner points.
        /// </summary>
        /// <param name="_diagramsIDList">The _diagrams ID list.</param>
        private void loadAllJoinerPoints(string _diagramsIDList)
        {
            try
            {
                StringBuilder query = new StringBuilder();
                query.Append("SELECT DIAGRAMID, JOINERSEQUENCE, SEQUENCE, X, Y FROM POINT");
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
                            int djSeq = reader.GetInt32(1);
                            int seq = reader.GetInt32(2);
                            int x = (reader.GetInt32(3) / cwLightDiagram.mmRatio);
                            int y = -(reader.GetInt32(4) / cwLightDiagram.mmRatio);
                            if (!pointsByDiagram.ContainsKey(dID))
                            {
                                pointsByDiagram[dID] = new Dictionary<int, SortedDictionary<int, cwJoinerPoint>>();
                            }
                            if (!pointsByDiagram[dID].ContainsKey(djSeq))
                            {
                                pointsByDiagram[dID][djSeq] = new SortedDictionary<int, cwJoinerPoint>();
                            }
                            cwJoinerPoint p = new cwJoinerPoint(x, y, seq);
                            diagramLoader.updateMinMax(x, y, 1, 1, dID);

                            pointsByDiagram[dID][djSeq][seq] = p;
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
        /// Loads all joiners by diagram.
        /// </summary>
        /// <param name="_diagramsIDList">The _diagrams ID list.</param>
        private void loadAllJoinersByDiagram(string _diagramsIDList)
        {
            List<int> joinersIds = new List<int>();
            try
            {
                StringBuilder query = new StringBuilder();
                query.Append("SELECT DIAGRAMID, TABLENUMBER, OBJECTID, SEQUENCE, NAMEFX, NAMEFY, FROMNAMEWIDTH, FROMNAMEHEIGHT, TLBRFLAGS, FROMOFFSET, TOOFFSET, FROMSEQUENCE, TOSEQUENCE FROM JOINER");
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
                            int djSID = reader.GetInt32(3);

                            int x = reader.GetInt32(4) / cwLightDiagram.mmRatio;
                            int y = reader.GetInt32(5) / cwLightDiagram.mmRatio;
                            int w = reader.GetInt32(6) / cwLightDiagram.mmRatio;
                            int h = reader.GetInt32(7) / cwLightDiagram.mmRatio;
                            cwJoinerTextZone textZone = new cwJoinerTextZone(x, -y, w, -h);

                            int points_flag = reader.GetInt32(8);
                            int fromOffset = reader.GetInt32(9) / cwLightDiagram.mmRatio;
                            int toOffset = reader.GetInt32(10) / cwLightDiagram.mmRatio;
                            int fromSeq = reader.GetInt32(11);
                            int toSeq = reader.GetInt32(12);
                            cwShape fromShape = diagramLoader.diagramsShapes[dID][fromSeq];
                            cwShape toShape = diagramLoader.diagramsShapes[dID][toSeq];
                            Point fromPoint = new Point(0, 0);
                            Point toPoint = new Point(0, 0);

                            switch (points_flag % 4)//source
                            {
                                case 0: // top
                                    fromPoint.X = fromShape.Position.X + fromOffset;
                                    fromPoint.Y = fromShape.Position.Y;
                                    break;
                                case 1: // left
                                    fromPoint.X = fromShape.Position.X;
                                    fromPoint.Y = fromShape.Position.Y + fromOffset;
                                    break;
                                case 2: // bottom
                                    fromPoint.X = fromShape.Position.X + fromOffset;
                                    fromPoint.Y = fromShape.Position.Y + fromShape.Size.Height;
                                    break;
                                case 3: // right
                                    fromPoint.X = fromShape.Position.X + fromShape.Size.Width;
                                    fromPoint.Y = fromShape.Position.Y + fromOffset;
                                    break;
                                default:
                                    break;
                            }

                            int div = points_flag / 4;
                            switch (div)
                            {
                                case 0: // top
                                    toPoint.X = toShape.Position.X + toOffset;
                                    toPoint.Y = toShape.Position.Y;
                                    break;
                                case 1: // left
                                    toPoint.X = toShape.Position.X;
                                    toPoint.Y = toShape.Position.Y + toOffset;
                                    break;
                                case 2: // bottom
                                    toPoint.X = toShape.Position.X + toOffset;
                                    toPoint.Y = toShape.Position.Y + toShape.Size.Height;
                                    break;
                                case 3: // right
                                    toPoint.X = toShape.Position.X + toShape.Size.Width;
                                    toPoint.Y = toShape.Position.Y + toOffset;
                                    break;
                                default:
                                    break;
                            }
                            cwJoiner joiner = new cwJoiner(dID, djSID, oID, otID);

                            cwLightObject o = diagramLoader.getLightObject(Model, otID, oID);
                            joiner.lightObject = o;

                            joiner.setStartPoint(fromPoint.X, fromPoint.Y);
                            diagramLoader.updateMinMax(fromPoint.X, fromPoint.Y, 1, 1, dID);

                            joiner.setEndPoint(toPoint.X, toPoint.Y);
                            diagramLoader.updateMinMax(toPoint.X, toPoint.Y, 1, 1, dID);

                            // update textZone offset point to absolute point
                            joiner.nameZone = textZone;
                            joiner.nameZone.x += fromPoint.X;
                            joiner.nameZone.y += fromPoint.Y;
                            joiner.nameZone.text = (o != null) ? o.ToString(): "";

                            if (pointsByDiagram.ContainsKey(dID) && pointsByDiagram[dID].ContainsKey(djSID))
                            {
                                foreach (var jointP in pointsByDiagram[dID][djSID])
                                {
                                    joiner.addPoint(jointP.Value.X, jointP.Value.Y);
                                }
                            }
                            if (!joinersByDiagram.ContainsKey(dID))
                            {
                                joinersByDiagram[dID] = new List<cwJoiner>();
                            }
                            joinersByDiagram[dID].Add(joiner);
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
