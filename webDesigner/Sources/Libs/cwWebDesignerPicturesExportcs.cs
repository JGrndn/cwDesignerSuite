using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Casewise.GraphAPI;
using System.Threading;
using Casewise.Data.ICM;
using Casewise.GraphAPI.API;
using System.Data;
using System.IO;
using Casewise.Services.Entities;
using Casewise.webDesigner.Libs;
using Casewise.GraphAPI.Exceptions;
using System.Drawing;
using Casewise.Services.Rendering;
using Casewise.Services.Entities.DashBoard;
using Casewise.Shared.Definitions;
using log4net;
using Casewise.GraphAPI.PSF;
using System.Web.Script.Serialization;
using Casewise.GraphAPI.API.Diagrams;
using Casewise.Services.ICM;
using SvgNet.SvgGdi;
using System.Drawing.Imaging;

namespace Casewise.webDesigner.Libs
{
    /// <summary>
    /// List of pictures in a template
    /// </summary>
    public class cwWebDesignerPicturesExports
    {
        private const int IMAGE_SIZE_FACTOR = 2;

        /// <summary>
        /// list of pictures and coords
        /// </summary>
        public List<cwPicturesCoord> picturesCoords = null;

        float totalWidth = 0;
        float totalHeight = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="cwWebDesignerPicturesExports"/> class.
        /// </summary>
        public cwWebDesignerPicturesExports()
        {
            this.picturesCoords = new List<cwPicturesCoord>();
        }

        /// <summary>
        /// Gets the total width for planche.
        /// </summary>
        /// <returns></returns>
        public float GetTotalWidth()
        {
            return totalWidth;
        }

        /// <summary>
        /// Gets the total height for planche.
        /// </summary>
        /// <returns></returns>
        public float GetTotalHeight()
        {
            return totalHeight;
        }

        /// <summary>
        /// Adds the specified picture ID.
        /// </summary>
        /// <param name="pictureID">The picture ID.</param>
        /// <param name="pictureWidth">Width of the picture.</param>
        /// <param name="pictureHeight">Height of the picture.</param>
        public cwPicturesCoord Add(int pictureID, float pictureWidth, float pictureHeight)
        {
            cwPicturesCoord coord = null;
            float x = 0;
            float y = 0;
            float w = pictureWidth * IMAGE_SIZE_FACTOR;
            float h = pictureHeight * IMAGE_SIZE_FACTOR;
            if (this.picturesCoords.Count != 0)
            {
                x = (tmpCoord.x + tmpCoord.w);
                y = tmpCoord.y;
            }
            coord = new cwPicturesCoord(pictureID, x, y, w, h);
            this.picturesCoords.Add(coord);
            tmpCoord = coord;
            return coord;
        }

        public cwPicturesCoord Get(int pictureID, float width, float height)
        {
            return this.picturesCoords.Find(p => p.ID.Equals(pictureID) && p.w.Equals(width * cwWebDesignerPicturesExports.IMAGE_SIZE_FACTOR) && p.h.Equals(height * cwWebDesignerPicturesExports.IMAGE_SIZE_FACTOR));
        }

        private cwPicturesCoord tmpCoord = new cwPicturesCoord(0, 0, 0, 0, 0);

        /// <summary>
        /// Sets the values.
        /// </summary>
        public void SetValues()
        {
            this.totalWidth = 0;
            this.totalHeight = 0;
            for (int i = 0; i < picturesCoords.Count; i++)
            {
                this.totalWidth += picturesCoords[i].w;
                this.totalHeight = Math.Max(this.totalHeight, picturesCoords[i].h);
            }
        }

    }
}
