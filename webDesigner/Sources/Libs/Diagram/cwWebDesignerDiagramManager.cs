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
using Casewise.webDesigner.Sources.Libs.Diagram;

namespace Casewise.webDesigner.Libs.Diagram
{
    internal class cwWebDesignerDiagramManager
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(cwWebDesignerExportDiagrams));
        private const float MillimeterToPixel = 3.779f;

        private Dictionary<int, CwStyle> _stylesById = new Dictionary<int, CwStyle>();
        private Dictionary<int, CwFont> _fontsById = new Dictionary<int, CwFont>();
        private Dictionary<int, cwLightObject> _pictureByIds = new Dictionary<int, cwLightObject>();
        private Dictionary<int, cwLightDiagram> _diagramByIds = new Dictionary<int, cwLightDiagram>();

        private cwLightModel _currentLightModel = null;
        private cwDiagramLoader _diagramLoader = null;


        public cwWebDesignerDiagramManager(cwLightModel model)
        {
            this._currentLightModel = model;
            this._diagramLoader = new cwDiagramLoader(model);
        }

        public void Clean()
        {
            this._stylesById.Clear();
            this._fontsById.Clear();
            this._pictureByIds.Clear();
            this._diagramByIds.Clear();
        }

        public void DoLoad(List<int> diagramIds, List<string> propToLoad)
        {
            this.Clean();
            this.LoadMetaData();
            this.LoadDiagrams(diagramIds, propToLoad);
        }

        #region MetaData
        public void LoadMetaData()
        {
            // load fonts & pictures & styles
            this.LoadFonts();
            this.LoadStyles();
            this.LoadPictures();
        }

        private void LoadFonts()
        {
            try
            {
                StringBuilder query = new StringBuilder();
                query.Append("SELECT " + CwFont.PROP_TO_LOAD + " FROM FONT");
                using (ICMCommand command = new ICMCommand(query.ToString(), this._currentLightModel.getConnection()))
                {
                    using (ICMDataReader reader = command.ExecuteReader() as ICMDataReader)
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32(0);
                            if (!this._fontsById.ContainsKey(id))
                            {
                                this._fontsById[id] = new CwFont(reader);
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

        private void LoadStyles()
        {
            try
            {
                StringBuilder query = new StringBuilder();
                query.Append("SELECT " + CwStyle.PROP_TO_LOAD + " FROM STYLE");
                using (ICMCommand command = new ICMCommand(query.ToString(), this._currentLightModel.getConnection()))
                {
                    using (ICMDataReader reader = command.ExecuteReader() as ICMDataReader)
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32(0);
                            if (!this._stylesById.ContainsKey(id))
                            {
                                this._stylesById[id] = new CwStyle(reader, this._fontsById);
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

        private void LoadPictures()
        {
            cwLightNodeObjectType picturesNode = new cwLightNodeObjectType(this._currentLightModel.getObjectTypeByScriptName("PICTURE"));
            picturesNode.addPropertyToSelect(cwLightObject.UNIQUEIDENTIFIER);
            picturesNode.addPropertyToSelect("ID");
            picturesNode.preloadLightObjects();
            this._pictureByIds = picturesNode.usedOTLightObjectsByID;
        }
        #endregion

        public void LoadDiagrams(List<int> diagramIds, List<string> propToLoad)
        {
            // load diagram objects
            cwLightDiagram.loadAllDiagrams(this._currentLightModel, this._diagramByIds, diagramIds, propToLoad);
            // load shapes & joiners
            // dont forget to load objects and associations

            // load meta data of diagram (palette and used pictures)

            // load palettes & regions
        }


    }
}
