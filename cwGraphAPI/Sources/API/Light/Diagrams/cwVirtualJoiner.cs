using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Casewise.GraphAPI.API
{
    /// <summary>
    /// cwVirtualJoiner
    /// </summary>
    public class cwVirtualJoiner
    {
        /// <summary>
        /// sourceShape
        /// </summary>
        public cwShape sourceShape = null;
        /// <summary>
        /// targetShape
        /// </summary>
        public cwShape targetShape = null;
        /// <summary>
        /// sourcePointSide
        /// </summary>
        public pointSide sourcePointSide = 0;
        /// <summary>
        /// targetPointSide
        /// </summary>
        public pointSide targetPointSide = 0;

        /// <summary>
        /// objectTypeID
        /// </summary>
        public int objectTypeID = 0;
        /// <summary>
        /// objectID
        /// </summary>
        public int objectID = 0;

        /// <summary>
        /// side
        /// </summary>
        public string side = "left";

        /// <summary>
        /// pointSide
        /// </summary>
        public enum pointSide
        {
            /// <summary>
            /// top
            /// </summary>
            top,
            /// <summary>
            /// left
            /// </summary>
            left,
            /// <summary>
            /// bottom
            /// </summary>
            bottom,
            /// <summary>
            /// right
            /// </summary>
            right
        }

        /// <summary>
        /// Gets the joiner object.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public cwLightObject getJoinerObject(cwLightModel model)
        {
            cwLightObjectType OT = model.getObjectTypeByID(objectTypeID);
            if (0.Equals(objectID))
            {
                //OT.createObject(
            }
            return null;
        }


        /// <summary>
        /// Creates the object intersection and joiner.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="joinerSequence">The joiner sequence.</param>
        /// <param name="diagramID">The diagram ID.</param>
        public void createObjectIntersectionAndJoiner(cwLightModel model, int joinerSequence, int diagramID)
        {
            cwLightObjectType OT = model.getObjectTypeByID(objectTypeID);
            int intersectionID = 0;
            if ("CONNECTOR".Equals(OT.ScriptName))
            {
                Dictionary<string, string> connectorProperties = new Dictionary<string, string>();
                connectorProperties[OT.getNamePropertyScriptName()] = "";
                connectorProperties["DIAGRAMID"] = diagramID.ToString();
                intersectionID = OT.createObject(connectorProperties);
            }
            else
            {
                intersectionID = objectID;
            }
            if (intersectionID != 0)
            {
                createJoiner(model, intersectionID, OT.ID, joinerSequence, diagramID);
            }
        }

        /// <summary>
        /// Creates the joiner.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="objectID">The object ID.</param>
        /// <param name="objectTypeID">The object type ID.</param>
        /// <param name="joinerSequence">The joiner sequence.</param>
        /// <param name="diagramID">The diagram ID.</param>
        private void createJoiner(cwLightModel model, int objectID, int objectTypeID, int joinerSequence, int diagramID)
        {
            cwLightObjectType JOINER_OT = model.getObjectTypeByScriptName("JOINER");

            Dictionary<string, string> joinerProperties = new Dictionary<string, string>();
            joinerProperties["SEQUENCE"] = joinerSequence.ToString();
            joinerProperties["DIAGRAMID"] = diagramID.ToString();
            joinerProperties["OBJECTID"] = objectID.ToString();
            joinerProperties["TABLENUMBER"] = objectTypeID.ToString();
            joinerProperties["FLAGS1"] = "0";
            joinerProperties["FLAGS2"] = "-2080374464";
            joinerProperties["NAMEFX"] = "16384";
            joinerProperties["NAMEFY"] = "-16384";
            joinerProperties["NAMETX"] = "-16384";
            joinerProperties["NAMETY"] = "-16384";

            if (side != null)
            {
                switch (side)
                {
                    case "left":
                        joinerProperties["FROMOFFSET"] = (sourceShape.Size.Height / 2).ToString();
                        joinerProperties["TOOFFSET"] = (sourceShape.Size.Height / 2 + sourceShape.Position.Y - targetShape.Position.Y).ToString();
                        break;
                    case "right":
                        joinerProperties["FROMOFFSET"] = (targetShape.Size.Height / 2 + targetShape.Position.Y - sourceShape.Position.Y).ToString();
                        joinerProperties["TOOFFSET"] = (targetShape.Size.Height / 2).ToString();
                        break;
                    case "top":
                        joinerProperties["FROMOFFSET"] = (sourceShape.Size.Width / 2).ToString();
                        joinerProperties["TOOFFSET"] = (sourceShape.Position.X - targetShape.Position.X + sourceShape.Size.Width / 2).ToString();
                        break;
                    case "bottom":
                        joinerProperties["FROMOFFSET"] = (targetShape.Position.X - sourceShape.Position.X + targetShape.Size.Width / 2).ToString();
                        joinerProperties["TOOFFSET"] = (targetShape.Size.Width / 2).ToString();
                        break;
                }
            }
            else
            {
                switch (sourcePointSide)
                {
                    case pointSide.top:
                    case pointSide.bottom:
                        joinerProperties["FROMOFFSET"] = (sourceShape.Size.Width / 2).ToString();
                        break;
                    case pointSide.left:
                    case pointSide.right:
                        joinerProperties["FROMOFFSET"] = (sourceShape.Size.Height / 2).ToString();
                        break;
                }
                switch (targetPointSide)
                {
                    case pointSide.top:
                    case pointSide.bottom:
                        joinerProperties["TOOFFSET"] = (targetShape.Size.Width / 2).ToString();
                        break;
                    case pointSide.left:
                    case pointSide.right:
                        joinerProperties["TOOFFSET"] = (targetShape.Size.Height / 2).ToString();
                        break;
                }
            }

            joinerProperties["FROMSEQUENCE"] = sourceShape.sequence.ToString();
            joinerProperties["TOSEQUENCE"] = targetShape.sequence.ToString();
            int sideFlag = (int)sourcePointSide + (int)targetPointSide * 4;
            joinerProperties["TLBRFLAGS"] = sideFlag.ToString();
            JOINER_OT.createObject(joinerProperties);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="cwVirtualJoiner"/> class.
        /// </summary>
        /// <param name="sourceShape">The source shape.</param>
        /// <param name="targetShape">The target shape.</param>
        /// <param name="sourcePointSide">The source point side.</param>
        /// <param name="targetPointSide">The target point side.</param>
        /// <param name="objectTypeID">The object type ID.</param>
        /// <param name="objectID">The object ID.</param>
        /// <param name="side">The side.</param>
        public cwVirtualJoiner(cwShape sourceShape, cwShape targetShape, pointSide sourcePointSide, pointSide targetPointSide, int objectTypeID, int objectID, string side)
        {
            this.sourceShape = sourceShape;
            this.targetShape = targetShape;
            this.sourcePointSide = sourcePointSide;
            this.targetPointSide = targetPointSide;
            this.objectTypeID = objectTypeID;
            this.objectID = objectID;
            this.side = side;
        }


    }
}
