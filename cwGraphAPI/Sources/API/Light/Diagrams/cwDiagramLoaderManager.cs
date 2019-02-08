using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Casewise.GraphAPI.API.Diagrams
{
    /// <summary>
    /// cwDiagramLoaderManager
    /// </summary>
    public class cwDiagramLoaderManager
    {
        /// <summary>
        /// diagramLoader
        /// </summary>
        protected  cwDiagramLoader diagramLoader = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="cwDiagramLoaderManager"/> class.
        /// </summary>
        /// <param name="diagramLoader">The diagram loader.</param>
        public cwDiagramLoaderManager(cwDiagramLoader diagramLoader)
        {
            this.diagramLoader = diagramLoader;
        }

        /// <summary>
        /// Gets the model.
        /// </summary>
        public cwLightModel Model
        {
            get
            {
                return this.diagramLoader.Model;
            }
        }

        /// <summary>
        /// Cleans the lists.
        /// </summary>
        public virtual void cleanLists()
        { 
            throw new NotImplementedException("cleanLists");
        }
    }
}
