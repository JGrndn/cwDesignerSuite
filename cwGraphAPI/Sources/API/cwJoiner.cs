using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Casewise.GraphAPI.API
{

    /// <summary>
    /// Manage the joiner Text
    /// </summary>
    public class cwJoinerTextZone
    {
        /// <summary>
        /// text
        /// </summary>
        public string text = null;
        /// <summary>
        /// x coord
        /// </summary>
        public int x;
        /// <summary>
        /// y coord
        /// </summary>
        public int y;
        /// <summary>
        /// weight
        /// </summary>
        public int w;
        /// <summary>
        /// height
        /// </summary>
        public int h;


        /// <summary>
        /// Initializes a new instance of the <see cref="cwJoinerTextZone"/> class.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="w">The w.</param>
        /// <param name="h">The h.</param>
        public cwJoinerTextZone(int x, int y, int w, int h)
        {
            this.x = x;
            this.y = y;
            this.w = w;
            this.h = h;
        }


        /// <summary>
        /// Toes the JSON.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
       
        public string toJSON(string key)
        {
            return "\"" + key + "\" : { \"text\":\"" + cwTools.escapeChars(text) + "\",\"x\" : " + x + ", \"y\" : " + y + ", \"w\" : " + w + ", \"h\" : " + h + " }";
        }

        /// <summary>
        /// Adds to position.
        /// </summary>
        /// <param name="p">The p.</param>
        public void addToPosition(Point p)
        {
            this.x += p.X;
            this.y += p.Y;
        }

        /// <summary>
        /// Determines whether this instance is valid.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance is valid; otherwise, <c>false</c>.
        /// </returns>
        public bool isValid()
        {
            return this.w != 0 && this.h != 0 && !"".Equals(text);
        }
    }

    /// <summary>
    /// a joiner links two shapes
    /// </summary>
    public class cwJoiner
    {
        private int diagramID = 0;
        private int joinerID = 0;
        /// <summary>
        /// object ID of the joiner if exists
        /// </summary>
        public int objectID = 0;
        /// <summary>
        /// The type of the object if exists
        /// </summary>
        public int cwLightObjectTypeID = 0;
        private List<Point> points = new List<Point>();
        /// <summary>
        /// The object if exists
        /// </summary>
        public cwLightObject lightObject = null;

        /// <summary>
        /// The source point of the joiner
        /// </summary>
        public Point fromPoint = new Point(0, 0);
        /// <summary>
        /// The end point of the joiner
        /// </summary>
        public Point toPoint = new Point(0, 0);

        /// <summary>
        /// The joiner Label
        /// </summary>
        public cwJoinerTextZone nameZone;

        /// <summary>
        /// Initializes a new instance of the <see cref="cwJoiner"/> class.
        /// </summary>
        /// <param name="_diagramID">The _diagram ID.</param>
        /// <param name="_joinerID">The _joiner ID.</param>
        /// <param name="_objectID">The _object ID.</param>
        /// <param name="_cwLightObjectTypeID">The _object type ID.</param>
        public cwJoiner(int _diagramID, int _joinerID, int _objectID, int _cwLightObjectTypeID)
        {
            diagramID = _diagramID;
            joinerID = _joinerID;
            objectID = _objectID;
            cwLightObjectTypeID = _cwLightObjectTypeID;
        }
        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return joinerID.ToString();
        }

        /// <summary>
        /// Toes the unique string.
        /// </summary>
        /// <returns></returns>
        public string toUniqueString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(fromPoint.ToString());
            sb.Append(toPoint.ToString());
            foreach (Point p in getPoints())
            {
                sb.Append(p.ToString());
            }
            //sb.Append(objectID.ToString());
            sb.Append(cwLightObjectTypeID.ToString());
            return sb.ToString();
        }



        /// <summary>
        /// Adds from point.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        public void setStartPoint(int x, int y)
        {
            fromPoint = new Point(x, y);
        }
        /// <summary>
        /// Sets the end point.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        public void setEndPoint(int x, int y)
        {
            toPoint = new Point(x, y);
        }

        /// <summary>
        /// Adds the point.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        public void addPoint(int x, int y)
        {
            points.Add(new Point(x, y));
        }

        /// <summary>
        /// Gets the points.
        /// </summary>
        public List<Point> getPoints()
        {
            List<Point> _points = new List<Point>();
            //_points.Add(fromPoint);
            _points.AddRange(points);
            //_points.Add(toPoint);
            return _points;
        }


        /// <summary>
        /// Adds the point.
        /// </summary>
        /// <param name="p">The p.</param>
        public void addPoint(Point p)
        {
            points.Add(p);
        }

    }
}
