using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Collections;

namespace Casewise.GraphAPI.API
{
    /// <summary>
    /// a Joiner Point
    /// </summary>
    public class cwJoinerPoint : IComparable
    {
        private Point p = new Point(0, 0);
        /// <summary>
        /// The joiner sequence
        /// </summary>
        public int sequence = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="cwJoinerPoint"/> class.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="_sequence">The _sequence.</param>
        public cwJoinerPoint(int x, int y, int _sequence) 
        {
            p = new Point(x, y);
            sequence = _sequence;
        }


        /// <summary>
        /// Compares to.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        int IComparable.CompareTo(object obj)
        {
            cwJoinerPoint j2 = obj as cwJoinerPoint;
            return Convert.ToInt32(this.sequence == j2.sequence);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="cwJoinerPoint"/> class.
        /// </summary>
        /// <param name="_p">The _p.</param>
        /// <param name="_sequence">The _sequence.</param>
        public cwJoinerPoint(Point _p, int _sequence)
        {
            p = _p;
            sequence = _sequence;
        }


        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return "[" + X.ToString() + ", " + Y.ToString() + "]";
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
                return p.X;
            }
            set
            {
                p.X = value;
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
                return p.Y;
            }
            set
            {
                p.Y = value;
            }
        }
    }
}
