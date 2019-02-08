using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Casewise.Services.Entities;
using System.Drawing;

namespace Casewise.GraphAPI.API
{

    /// <summary>
    /// cwBoundingBox
    /// </summary>
    public class cwBoundingBox
    {
        /// <summary>
        /// size
        /// </summary>
        public Size size = new Size(0, 0);
        /// <summary>
        /// position
        /// </summary>
        public Point position = new Point(0, 0);

        /// <summary>
        /// Initializes a new instance of the <see cref="cwBoundingBox"/> class.
        /// </summary>
        public cwBoundingBox()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="cwBoundingBox"/> class.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="size">The size.</param>
        public cwBoundingBox(Point position, Size size)
        {
            this.size = size;
            this.position = position;
        }
    }

    /// <summary>
    /// Represents a shape
    /// </summary>
    public class cwShape
    {
        /// <summary>
        /// the id of the object behind the shape
        /// </summary>
        public int objectID = 0;
        /// <summary>
        /// the ID of the object type of the shape
        /// </summary>
        public int objectTypeID = 0;
        /// <summary>
        /// the sequence of the shape
        /// </summary>
        public int sequence = 0;
        /// <summary>
        /// the object behind the shape
        /// </summary>
        public cwLightObject lightObject = null;
        /// <summary>
        /// database flag1
        /// </summary>
        public int flag1 = 0;
        /// <summary>
        /// database flag2
        /// </summary>
        public int flag2 = 0;
        /// <summary>
        /// the parent sequence
        /// </summary>
        public int parentSequence = 0;
        /// <summary>
        /// the next z sequence
        /// </summary>
        public int nextZSequence = 0;
        /// <summary>
        /// the custom symbol
        /// </summary>
        public int customSymbol = 0;
        /// <summary>
        /// the custom style id
        /// </summary>
        public int customStyleID = 0;

        /// <summary>
        /// boundingBox
        /// </summary>
        public cwBoundingBox boundingBox = null;


        /// <summary>
        /// The interna l_ event
        /// </summary>
        private static int INTERNAL_EVENT = 0x20000;
        /// <summary>
        /// The interna l_ result
        /// </summary>
        private static int INTERNAL_RESULT = 0xA0000;

        /// <summary>
        /// Initializes a new instance of the <see cref="cwShape"/> class.
        /// </summary>
        /// <param name="objectID">The object ID.</param>
        /// <param name="cwLightObjectTypeID">The cw light object type ID.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="_sequence">The _sequence.</param>
        public cwShape(int objectID, int cwLightObjectTypeID, int width, int height, int x, int y, int _sequence)
        {
            this.objectID = objectID;
            this.objectTypeID = cwLightObjectTypeID;
            this._position.X = x;
            this._position.Y = y;
            this._size.Width = width;
            this._size.Height = height;
            this.sequence = _sequence;
            this.boundingBox = new cwBoundingBox(_position, _size);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return this.sequence.ToString() + " - " + objectName.ToString() + " / " + objectTypeID + "(" + _position.X + "," + _position.Y + "), (" + _size.Width + ", " + _size.Height + ")";
        }

        //        public AnyObject anyObject = null;
        /// <summary>
        /// the position of the shape
        /// </summary>
        private Point _position = new Point(0, 0);
        /// <summary>
        /// the size of the shape
        /// </summary>
        private Size _size = new Size(0, 0);

        /// <summary>
        /// Determines whether [is internal event].
        /// </summary>
        /// <returns></returns>
        public bool isInternalEvent()
        {
            return cwShape.isInternalEvent(this.flag2);
        }

        /// <summary>
        /// Determines whether [is internal event] [the specified flags].
        /// </summary>
        /// <param name="flags">The flags.</param>
        /// <returns></returns>
        public static bool isInternalEvent(int flags)
        {
            if ((cwShape.INTERNAL_EVENT & flags) == cwShape.INTERNAL_EVENT)
            {
                if (cwShape.isInternalResult(flags))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else return false;
        }

        /// <summary>
        /// Determines whether [is internal result].
        /// </summary>
        /// <returns></returns>
        public bool isInternalResult()
        {
            return cwShape.isInternalResult(this.flag2);
        }

        /// <summary>
        /// Determines whether [is internal result] [the specified flags].
        /// </summary>
        /// <param name="flags">The flags.</param>
        /// <returns></returns>
        public static bool isInternalResult(int flags)
        {
            if ((cwShape.INTERNAL_RESULT & flags) == cwShape.INTERNAL_RESULT)
            {
                return true;
            }
            else return false;
        }

        /// <summary>
        /// Gets the name of the object.
        /// </summary>
        /// <value>
        /// The name of the object.
        /// </value>
        public string objectName
        {
            get
            {
                if (lightObject != null)
                {
                    return lightObject.ToString();
                }
                return "NO_OBJECT";
                //return anyObject.Name;
            }
        }
        /// <summary>
        /// Sets the position.
        /// </summary>
        /// <param name="W">The W.</param>
        /// <param name="H">The H.</param>
        public void setSize(int W, int H)
        {
            _size.Width = W;
            _size.Height = H;
        }

        /// <summary>
        /// Sets the position.
        /// </summary>
        /// <param name="X">The X.</param>
        /// <param name="Y">The Y.</param>
        public void setPosition(int X, int Y)
        {
            _position.X = X;
            _position.Y = Y;
        }


        /// <summary>
        /// Gets the width of the X plus.
        /// </summary>
        /// <value>
        /// The width of the X plus.
        /// </value>
        public int XPlusWidth
        {
            get
            {
                return _position.X + _size.Width;
            }
        }

        /// <summary>
        /// Gets the height of the Y plus.
        /// </summary>
        /// <value>
        /// The height of the Y plus.
        /// </value>
        public int YPlusHeight
        {
            get
            {
                return _position.Y + _size.Height;
            }
        }

        /// <summary>
        /// Gets or sets the Y.
        /// </summary>
        /// <value>
        /// The Y.
        /// </value>
        public int Y
        {
            get
            {
                return _position.Y;
            }
            set
            {
                _position.Y = value;
            }
        }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>
        /// The height.
        /// </value>
        public int Height
        {
            get
            {
                return _size.Height;
            }
            set
            {
                _size.Height = value;
            }
        }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>
        /// The width.
        /// </value>
        public int Width
        {
            get
            {
                return _size.Width;
            }
            set
            {
                _size.Width = value;
            }
        }

        /// <summary>
        /// Gets or sets the X.
        /// </summary>
        /// <value>
        /// The X.
        /// </value>
        public int X
        {
            get
            {
                return _position.X;
            }
            set
            {
                _position.X = value;
            }
        }

        /// <summary>
        /// Gets the size.
        /// </summary>
        public Size Size
        {
            get
            {
                return _size;
            }
            set
            {
                _size = value;
            }
        }

        /// <summary>
        /// Gets the position.
        /// </summary>
        public Point Position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
            }
        }


        /// <summary>
        /// Determines whether the specified father is included.
        /// </summary>
        /// <param name="father">The father.</param>
        /// <returns>
        ///   <c>true</c> if the specified father is included; otherwise, <c>false</c>.
        /// </returns>
        internal bool isIncluded(cwShape father)
        {
            if (this._position.X + this._size.Width > father._position.X + father._size.Width)
            {
                return false;
            }
            if (this._position.X < father._position.X)
            {
                return false;
            }
            if (this._position.Y > father._position.Y)
            {
                return false;
            }
            if (this._position.Y + this._size.Height > father._position.Y + father._size.Height)
            {
                return false;
            }
            return true;
        }
    }
}
