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
    /// cwWebDesignerExportDiagrams
    /// </summary>
    internal class cwWebDesignerExportDiagrams
    {

        private static readonly ILog log = LogManager.GetLogger(typeof(cwWebDesignerExportDiagrams));

        public cwDiagramLoader diagramLoader = null;

        cwLightModel currentLightModel = null;
        private Dictionary<int, cwWebDesignerStyle> stylesByID = new Dictionary<int, cwWebDesignerStyle>();
        private Dictionary<int, cwWebDesignerFont> fontsByID = new Dictionary<int, cwWebDesignerFont>();


        private cwLightNodeObjectType picturesNode = null;

        private cwWebDesignerJSONTools _jsonTools = null;

        private const int TITLE_OT_ID = 7;


        /// <summary>
        /// pictures id by template id
        /// </summary>
        public Dictionary<int, cwWebDesignerPicturesExports> picturesCoordsByTemplateID = new Dictionary<int, cwWebDesignerPicturesExports>();

        /// <summary>
        /// Initializes a new instance of the <see cref="cwWebDesignerExportDiagrams"/> class.
        /// </summary>
        /// <param name="jsonTools">The json tools.</param>
        /// <param name="currentLightModel">The current light model.</param>
        public cwWebDesignerExportDiagrams(cwWebDesignerJSONTools jsonTools, cwLightModel currentLightModel)
        {
            _jsonTools = jsonTools;
            this.currentLightModel = currentLightModel;
            diagramLoader = new cwDiagramLoader(currentLightModel);
            this.picturesCoordsByTemplateID = new Dictionary<int, cwWebDesignerPicturesExports>();

        }

        /// <summary>
        /// Loads the fonts.
        /// </summary>
        private void loadFonts()
        {
            try
            {
                StringBuilder query = new StringBuilder();
                query.Append("SELECT FONTNR, FONTFACE, FONTHEIGHT, FONTWIDTH, FONTSTYLE, FONTFLAGS FROM FONT");
                using (ICMCommand command = new ICMCommand(query.ToString(), currentLightModel.getConnection()))
                {
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32(0);
                            if (!fontsByID.ContainsKey(id))
                            {
                                fontsByID[id] = new cwWebDesignerFont(reader);
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

        /// <summary>
        /// Loads the styles.
        /// </summary>
        private void loadStyles()
        {
            try
            {
                StringBuilder query = new StringBuilder();
                query.Append("SELECT ID, TEXTCOLOUR, FONTNR, FILLCOLOUR, LINECOLOUR, LINEWIDTH, FILLSTYLE, LINESTYLE, NAMELINESTYLE, SPECIALEFFECTS, NAMEFILLPATTERN, NAMEFILLCOLOUR, SHADOWDIRECTION, NAMESHADOWSIZE, NAMESHADOWCOLOUR  FROM STYLE");
                //query.Append(" WHERE ID IN ( " + cwTools.idToStringSeparatedby(",", stylesRequired) + " )");
                using (ICMCommand command = new ICMCommand(query.ToString(), currentLightModel.getConnection()))
                {
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Int32 ID = reader.GetInt32(0);
                            Int32 textColorInt = reader.GetInt32(1);
                            int fontNumber = reader.GetInt32(2);
                            int fillColor = reader.GetInt32(3);
                            int strokeColor = reader.GetInt32(4);
                            int lineWidth = reader.GetInt32(5);
                            int fillPattern = reader.GetInt32(6);
                            int linePattern = reader.GetInt32(7);
                            int gradientSize = reader.GetInt32(8);
                            int styleEffect = reader.GetInt32(9);
                            int gradientFromColor = reader.GetInt32(10);
                            int gradientToColor = reader.GetInt32(11);
                            int shadowDirection = reader.GetInt32(12);
                            int gradientDir = reader.GetInt32(13);
                            int opacityPercentage = reader.GetInt32(14) / 5;

                            if (!stylesByID.ContainsKey(ID))
                            {
                                cwWebDesignerFont font = null;
                                if (!0.Equals(fontNumber))
                                {
                                    font = fontsByID[fontNumber];
                                }
                                stylesByID[ID] = new cwWebDesignerStyle(textColorInt, font, fillColor, fillPattern, strokeColor, lineWidth, linePattern, gradientDir, gradientSize, gradientFromColor, gradientToColor, shadowDirection, styleEffect, opacityPercentage);
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

        private List<KeyValuePair<float, string>> getRegionBound(PaletteEntry.PaletteShapeRegion paletteShapeRegion)
        {
            List<KeyValuePair<float, string>> bounds = new List<KeyValuePair<float, string>>();
            RectangleF bound = paletteShapeRegion.Region;


            RegionDefs.Docking docking = (RegionDefs.Docking)paletteShapeRegion.Docking;

            string leftType = "%";
            string topType = "%";
            string widthType = "mm";
            string heightType = "mm";
            if (!RegionDefs.Docking.None.Equals(docking & RegionDefs.Docking.Left))
            {
                leftType = "mm";
            }
            if (!RegionDefs.Docking.None.Equals(docking & RegionDefs.Docking.Top))
            {
                topType = "mm";
            }
            if (!RegionDefs.Docking.None.Equals(docking & RegionDefs.Docking.Bottom))
            {
                heightType = "%";
            }
            if (!RegionDefs.Docking.None.Equals(docking & RegionDefs.Docking.Right))
            {
                widthType = "%";
            }
            if (!RegionDefs.Docking.None.Equals(docking & RegionDefs.Docking.FillBottom))
            {
                heightType = "fill";
            }
            if (!RegionDefs.Docking.None.Equals(docking & RegionDefs.Docking.FillRight))
            {
                widthType = "fill";
            }

            bounds.Add(new KeyValuePair<float, string>(bound.X, leftType));
            bounds.Add(new KeyValuePair<float, string>(bound.Y, topType));
            bounds.Add(new KeyValuePair<float, string>(bound.Width, widthType));
            bounds.Add(new KeyValuePair<float, string>(bound.Height, heightType));
            return bounds;
        }

        private string getHorizontalJustificationString(RegionDefs.HorizontalJustification _HorizontalJustification)
        {
            switch (_HorizontalJustification)
            {
                case RegionDefs.HorizontalJustification.TopJustify:
                    return "top";
                case RegionDefs.HorizontalJustification.CentreJustify:
                    return "middle";
                case RegionDefs.HorizontalJustification.BottomJustify:
                    return "bottom";
            }
            return "top";
        }

        private string getVerticalJustificationString(RegionDefs.VerticalJustification _VerticalJustification)
        {
            switch (_VerticalJustification)
            {
                case RegionDefs.VerticalJustification.LeftJustify:
                    return "left";
                case RegionDefs.VerticalJustification.CentreJustify:
                    return "center";
                case RegionDefs.VerticalJustification.RightJustify:
                    return "right";
            }
            return "left";
        }


        public class PaletteEntryRegionJSON
        {
            public cwWebDesignerStyleJSON style { get; set; }
            public int symbol { get; set; }
            public List<int> picturesId { get; set; }
            public int pictureID { get; set; }
            public Dictionary<string, int> picturesIdByPropertyValue { get; set; }
            public string propertyScriptName { get; set; }
            public string horizontalAlignment { get; set; }
            public string verticalAlignment { get; set; }
            public string type { get; set; }
            public string associationType { get; set; }
            public string leftType { get; set; }
            public string topType { get; set; }
            public string widthType { get; set; }
            public string heightType { get; set; }
            public float x { get; set; }
            public float y { get; set; }
            public float w { get; set; }
            public float h { get; set; }
            public string labelValue { get; set; }
        }

        private void exportPaletteEntryRegions(StreamWriter json_file, PaletteEntry pe, int templateID)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            if (0.Equals(pe.Regions.Count))
            {
                json_file.Write(",\"regions\" : []");
                return;
            }

            List<PaletteEntry.PaletteShapeRegion> activeRegions = new List<PaletteEntry.PaletteShapeRegion>();
            foreach (PaletteEntry.PaletteShapeRegion paletteShapeRegion in pe.Regions)
            {
                int display = (int)paletteShapeRegion.Display;
                if (((RegionDefs.Display)display) == RegionDefs.Display.No) continue;
                activeRegions.Add(paletteShapeRegion);
            }

            List<PaletteEntryRegionJSON> activeRegionsJSONs = new List<PaletteEntryRegionJSON>();


            foreach (PaletteEntry.PaletteShapeRegion paletteShapeRegion in activeRegions)
            {
                IDashboard dashboard = paletteShapeRegion.DashBoard;
                ICMPropertyType p = dashboard.SourcePropertyType;
                List<KeyValuePair<float, string>> regionBounds = getRegionBound(paletteShapeRegion);
                string type = "none";

                PaletteEntryRegionJSON peRegionJSON = new PaletteEntryRegionJSON();
                RegionRepresentationType regionType = dashboard.RegionRepresentation.CurrentType;


                peRegionJSON.picturesIdByPropertyValue = new Dictionary<string, int>();
                peRegionJSON.picturesId = new List<int>();
                //json_file.Write("{");
                foreach (var br in dashboard.BandingRows)
                {
                    if (br.Value.Style.Style != null)
                    {
                        cwWebDesignerStyle style = stylesByID[br.Value.Style.Style.Id];
                        peRegionJSON.style = style.getJSONObject();
                        int symbol = (int)br.Value.SymbolNumber;
                        peRegionJSON.symbol = symbol;
                        //json_file.Write("\"style\":" + style.toJSON() + ",");
                        //json_file.Write("\"symbol\":" + symbol + ",");
                        if (pictureIDBySymbolIDAndTemplateID[templateID].ContainsKey(symbol))
                        {
                            peRegionJSON.pictureID = pictureIDBySymbolIDAndTemplateID[templateID][symbol];
                            peRegionJSON.picturesId.Add(peRegionJSON.pictureID);
                            if (br.Value.HighValue != null)
                            {
                                int tmpValue = 0;
                                if (int.TryParse(br.Value.HighValue.ToString(), out tmpValue))
                                {
                                    peRegionJSON.picturesIdByPropertyValue[tmpValue.ToString()] = peRegionJSON.pictureID;
                                }
                            }
                            //peRegionJSON.picturesIdByPropertyValue[br.Value.HighValue] = pictureIDBySymbolIDAndTemplateID[templateID][symbol];

                            //cwPicturesCoord coord = GetImagesForTemplate(templateID, symbol, paletteShapeRegion);
                            //if (coord != null)
                            //{
                            //peRegionJSON.pictureCoords[peRegionJSON.pictureID] = new cwPicturesCoord(coord.ID, 0, 0, 0, 0);
                            //peRegionJSON.pictureCoords[peRegionJSON.pictureID].x = Convert.ToInt32(coord.x * cwWebDesignerExportDiagrams.MillimeterToPixel);
                            //peRegionJSON.pictureCoords[peRegionJSON.pictureID].y = Convert.ToInt32(coord.y * cwWebDesignerExportDiagrams.MillimeterToPixel);
                            //peRegionJSON.pictureCoords[peRegionJSON.pictureID].w = Convert.ToInt32(coord.w * cwWebDesignerExportDiagrams.MillimeterToPixel);
                            //peRegionJSON.pictureCoords[peRegionJSON.pictureID].h = Convert.ToInt32(coord.h * cwWebDesignerExportDiagrams.MillimeterToPixel);
                            //}
                        }
                    }
                }
                if (regionType.Equals(RegionRepresentationType.Explosion) || regionType.Equals(RegionRepresentationType.ExplosionWithRuleAndPaletteValue) ||
                    regionType.Equals(RegionRepresentationType.ExplosionWithRuleAndReferenceProperty) || regionType.Equals(RegionRepresentationType.ExplosionWithRuleOnly))
                {
                    type = "explosion";
                }
                if (regionType.Equals(RegionRepresentationType.LocalPropertyActualValue))
                {
                    string pScriptName = p.ScriptName;
                    peRegionJSON.propertyScriptName = pScriptName.ToLower();
                    //json_file.Write("\"propertyScriptName\":\"" + pScriptName.ToLower() + "\",");
                    type = "property_value";
                }
                if (regionType.Equals(RegionRepresentationType.Label))
                {
                    type = "label";
                    peRegionJSON.labelValue = dashboard.Text;
                }
                if (dashboard.RegionRepresentation.CurrentType.Equals(RegionRepresentationType.VisualizationUsingReferenceProperty))
                {
                    type = "conditionalDisplay";
                    string pScriptName = p.ScriptName;
                    peRegionJSON.propertyScriptName = pScriptName.ToLower();
                }
                if (regionType.Equals(RegionRepresentationType.Association) || regionType.Equals(RegionRepresentationType.AssociationTargetObjectType)
                 || regionType.Equals(RegionRepresentationType.AssociationIntersectionObjectType))
                {
                    type = "association";
                    peRegionJSON.associationType = paletteShapeRegion.ShapeAdditionalData.ScriptName;
                }


                RegionDefs.HorizontalJustification horizontalJustification = (RegionDefs.HorizontalJustification)paletteShapeRegion.HorizontalJustification;
                RegionDefs.VerticalJustification verticalJustification = (RegionDefs.VerticalJustification)paletteShapeRegion.VerticalJustification;

                peRegionJSON.horizontalAlignment = getVerticalJustificationString(verticalJustification);
                peRegionJSON.verticalAlignment = getHorizontalJustificationString(horizontalJustification);
                peRegionJSON.type = type;
                peRegionJSON.x = regionBounds[0].Key;
                peRegionJSON.leftType = regionBounds[0].Value;
                peRegionJSON.y = regionBounds[1].Key;
                peRegionJSON.topType = regionBounds[1].Value;
                peRegionJSON.w = regionBounds[2].Key;
                peRegionJSON.widthType = regionBounds[2].Value;
                peRegionJSON.h = regionBounds[3].Key;
                peRegionJSON.heightType = regionBounds[3].Value;



                activeRegionsJSONs.Add(peRegionJSON);
            }
            //regionsDico["regions"] = activeRegionsJSONs;
            json_file.Write(", \"regions\":" + serializer.Serialize(activeRegionsJSONs));
        }

        /// <summary>
        /// Writes the JSON palette entry.
        /// </summary>
        /// <param name="json_file">The json_file.</param>
        /// <param name="pe">The pe.</param>
        private void exportPaletteEntry(StreamWriter json_file, PaletteEntry pe, int templateID)
        {
            int symbol = pe.Symbol;

            string cwLightObjectTypeScriptName = pe.ObjectType.ScriptName;

            string _categoryID = pe.Category.ToString();
            //int lineWidth = Convert.ToInt32(pe.LineWidth / cwLineToLineRatio);
            if (cwLightObjectType.EVENTRESULT_SCRIPTNAME.Equals(cwLightObjectTypeScriptName))
            {
                _categoryID = pe.EventResultCategory.ToString();
            }
            json_file.Write("\"" + cwLightObjectTypeScriptName + "|" + _categoryID + "\":{");

            if (pictureIDBySymbolIDAndTemplateID[templateID].ContainsKey(symbol))
            {

                json_file.Write("\"pictureID\":" + pictureIDBySymbolIDAndTemplateID[templateID][symbol] + ",");
                cwPicturesCoord coord = GetImagesForTemplate(templateID, symbol, pe);
                if (coord != null)
                {
                    string x = (coord.x * cwWebDesignerExportDiagrams.MillimeterToPixel).ToString().Replace(',', '.');
                    string y = (coord.y * cwWebDesignerExportDiagrams.MillimeterToPixel).ToString().Replace(',', '.');
                    string w = (coord.w * cwWebDesignerExportDiagrams.MillimeterToPixel).ToString().Replace(',', '.');
                    string h = (coord.h * cwWebDesignerExportDiagrams.MillimeterToPixel).ToString().Replace(',', '.');
                    // write json
                    json_file.Write("\"pictureCoords\":{\"ID\":" + coord.ID + ", \"x\":" + x + ", \"y\":" + y + ", \"w\":" + w + ", \"h\":" + h + "}, ");
                    //json_file.Write("\"\":{ID:" + coord.ID + ", x:" + coord.x + ", y:" + coord.y + ", w:" + coord.w + ", h:" + coord.h + "}");
                }
            }
            json_file.Write("\"symbol\":" + symbol + ",");

            json_file.Write("\"joinerToEndSymbol\":\"" + pe.ToEnd + "\",");
            json_file.Write("\"joinerFromEndSymbol\":\"" + pe.FromEnd + "\",");
            json_file.Write("\"defaultWidth\":" + (pe.Width / cwLightDiagram.mmRatio) + ",");
            json_file.Write("\"defaultHeight\":" + (pe.Height / cwLightDiagram.mmRatio) + ",");
            json_file.Write("\"displayText\":" + pe.DisplayValue.ToString().ToLower() + ",");
            RegionDefs.HorizontalJustification horizontalJustification = (RegionDefs.HorizontalJustification)pe.HorizontalJustification;
            RegionDefs.VerticalJustification verticalJustification = (RegionDefs.VerticalJustification)pe.VerticalJustification;
            json_file.Write("\"horizontalAlignment\" : \"" + getVerticalJustificationString(verticalJustification) + "\",");
            json_file.Write("\"verticalAlignment\" : \"" + getHorizontalJustificationString(horizontalJustification) + "\",");

            cwWebDesignerStyle peStyle = stylesByID[pe.Style.Id];

            json_file.Write("\"style\":" + peStyle.toJSON());

            exportPaletteEntryRegions(json_file, pe, templateID);

            json_file.Write("}");
        }

        private cwPicturesCoord GetImagesForTemplate(int templateID, int symbolID, PaletteEntry.PaletteShapeRegion pe)
        {
            return GetImagesForTemplate(templateID, symbolID, Convert.ToInt32(pe.Region.Width), Convert.ToInt32(pe.Region.Height));
        }

        private cwPicturesCoord GetImagesForTemplate(int templateID, int symbolID, PaletteEntry pe)
        {
            return GetImagesForTemplate(templateID, symbolID, pe.WidthInMM, pe.HeightInMM);
        }

        private cwPicturesCoord GetImagesForTemplate(int templateID, int symbolID, float width, float height)
        {
            if (diagramLoader.plancheCreatedTemplateID.Contains(templateID))
            {
                return this.picturesCoordsByTemplateID[templateID].Get(pictureIDBySymbolIDAndTemplateID[templateID][symbolID], width, height);
            }
            if (!this.picturesCoordsByTemplateID.ContainsKey(templateID))
            {
                this.picturesCoordsByTemplateID[templateID] = new cwWebDesignerPicturesExports();
            }

            return this.picturesCoordsByTemplateID[templateID].Add(pictureIDBySymbolIDAndTemplateID[templateID][symbolID], width, height);
        }

        /// <summary>
        /// Exports the joiners.
        /// </summary>
        /// <param name="json_file">The json_file.</param>
        /// <param name="_dID">The _d ID.</param>
        /// <param name="d">The d.</param>
        /// <param name="_joinersByDiagram">The _joiners by diagram.</param>
        private void exportJoiners(StreamWriter json_file, int _dID, cwLightDiagram d, Dictionary<int, List<cwJoiner>> _joinersByDiagram)
        {
            json_file.WriteLine("\"joiners\" : [");
            if (!_joinersByDiagram.ContainsKey(_dID))
            {
                json_file.Write("]");
                return;
            }
            List<cwJoiner> joiners = _joinersByDiagram[_dID];

            int djNum = 0;
            int djCount = joiners.Count();

            foreach (cwJoiner dj in joiners)
            {
                json_file.Write("{");

                string cwLightObjectTypeScriptname = "";
                if (dj.objectID != 0)
                {
                    cwLightObjectTypeScriptname = currentLightModel.getObjectTypeByID(dj.cwLightObjectTypeID).ScriptName;
                }
                json_file.Write("\"objectPaletteEntryKey\" : \"" + getPaletteEntryKeyForJoiner(cwLightObjectTypeScriptname, dj) + "\",");


                Point min = getMinOfDiagram(d.ID);
                dj.nameZone.x += -min.X;
                dj.nameZone.y += -min.Y;

                if (dj.nameZone.isValid())
                {
                    json_file.Write(dj.nameZone.toJSON("nameZone") + ",");
                }


                json_file.Write("\"points\":[");
                int pNum = 0;
                int pCount = dj.getPoints().Count();
                Point lastPoint = dj.fromPoint;
                foreach (Point point in dj.getPoints())
                {
                    int x = point.X;
                    int y = point.Y;

                    x += -min.X;
                    y += -min.Y;
                    json_file.Write("{\"x\":" + x + ",\"y\":" + y + "}");
                    if (pNum < pCount - 1)
                    {
                        json_file.Write(",");
                    }
                    lastPoint = new Point(x, y);
                    pNum++;
                }
                json_file.Write("]");
                json_file.Write("}");
                if (djNum < djCount - 1)
                {
                    json_file.WriteLine(",");
                }
                djNum++;
            }
            json_file.Write("]");
        }


        /// <summary>
        /// Loads the picture symbol entries.
        /// </summary>
        /// <param name="dp">The dp.</param>
        /// <param name="templateID">The template ID.</param>
        private void loadPictureSymbolEntries(Palette dp, int templateID)
        {
            if (!pictureIDBySymbolIDAndTemplateID.ContainsKey(templateID))
            {
                pictureIDBySymbolIDAndTemplateID[templateID] = new Dictionary<int, int>();
            }

            foreach (Casewise.Shared.Definitions.SymbolEntry se in dp.Symbols)
            {
                int symbolID = Convert.ToInt32(se.PaletteID);
                if (!pictureIDBySymbolIDAndTemplateID[templateID].ContainsKey(symbolID))
                {
                    if (picturesNode.usedOTLightObjectsByUUID.ContainsKey(se.Picture))
                    {
                        pictureIDBySymbolIDAndTemplateID[templateID][symbolID] = picturesNode.usedOTLightObjectsByUUID[se.Picture].ID;
                    }

                }
            }
        }

        /// <summary>
        /// Exports the templates palettes.
        /// </summary>
        /// <param name="json_file">The json_file.</param>
        /// <param name="d">The d.</param>
        /// <param name="_templates">The _templates.</param>
        private void exportTemplatesPalettes(StreamWriter json_file, cwLightDiagram d, Dictionary<string, KeyValuePair<cwLightDiagram, Palette>> _templates)
        {
            json_file.WriteLine("\"objectTypesStyles\" : ");
            json_file.WriteLine("{");
            if (d.properties.ContainsKey("TYPE"))
            {
                if (!_templates.ContainsKey(d.properties["TYPE"]))
                {
                    json_file.WriteLine("}");
                    return;
                }
            }
            else
            {
                json_file.WriteLine("}");
                return;
            }
            Palette dp = _templates[d.properties["TYPE"]].Value;
            var t = _templates[d.properties["TYPE"]].Key;

            int paletteItemsCount = dp.Entries.Count;
            int i = 0;
            int templateID = _templates[d.properties["TYPE"]].Key.ID;

            loadPictureSymbolEntries(dp, templateID);

            foreach (PaletteEntry dpe in dp.Entries)
            {
                exportPaletteEntry(json_file, dpe, templateID);
                if (i < paletteItemsCount - 1)
                {
                    json_file.WriteLine(",");
                }
                i++;
            }
            json_file.WriteLine("}");
            if (!diagramLoader.plancheCreatedTemplateID.Contains(templateID))
            {
                diagramLoader.plancheCreatedTemplateID.Add(templateID);
            }
        }


        Dictionary<int, Dictionary<int, int>> pictureIDBySymbolIDAndTemplateID = new Dictionary<int, Dictionary<int, int>>();

        /// <summary>
        /// Gets the size of the real diagram.
        /// </summary>
        /// <param name="dID">The d ID.</param>
        /// <returns></returns>
        private Size getRealDiagramSize(int dID)
        {
            Point max = diagramLoader.diagramsMinMax[dID].Value;
            Point min = diagramLoader.diagramsMinMax[dID].Key;
            return new Size(max.X - min.X, max.Y - min.Y);
        }

        /// <summary>
        /// Gets the max of diagram.
        /// </summary>
        /// <param name="dID">The d ID.</param>
        /// <returns></returns>
        private Point getMaxOfDiagram(int dID)
        {
            Point max = diagramLoader.diagramsMinMax[dID].Value;
            max.X += 20;
            max.Y += 20;
            return max;
        }

        /// <summary>
        /// Gets the min of diagram.
        /// </summary>
        /// <param name="dID">The d ID.</param>
        /// <returns></returns>
        private Point getMinOfDiagram(int dID)
        {
            Point min = diagramLoader.diagramsMinMax[dID].Key;
            min.X -= 20;
            min.Y -= 20;
            return min;
        }

        /// <summary>
        /// Gets the palette entry category key.
        /// </summary>
        /// <param name="_lightObject">The _light object.</param>
        /// <returns></returns>
        private string getPaletteEntryCategoryKey(cwLightObject _lightObject)
        {
            if (_lightObject == null)
            {
                return "0";
            };
            string typeScriptName = _lightObject.getObjectType().getTypePropertyScriptName();
            if (_lightObject.properties.ContainsKey(typeScriptName + cwLookupManager.LOOKUPID_KEY))
            {
                return _lightObject.properties[typeScriptName + cwLookupManager.LOOKUPID_KEY];
            }
            else
            {
                return "0";
            }
        }

        /// <summary>
        /// Gets the palette entry key for joiner.
        /// </summary>
        /// <param name="cwLightObjectTypeScriptname">The cw light object type scriptname.</param>
        /// <param name="_joiner">The _joiner.</param>
        /// <returns></returns>
        private string getPaletteEntryKeyForJoiner(string cwLightObjectTypeScriptname, cwJoiner _joiner)
        {
            string objectPaletteEntryKey = cwLightObjectTypeScriptname + "|" + getPaletteEntryCategoryKey(_joiner.lightObject);
            return objectPaletteEntryKey;
        }

        /// <summary>
        /// Gets the palette entry key for shape.
        /// </summary>
        /// <param name="cwLightObjectTypeScriptname">The cw light object type scriptname.</param>
        /// <param name="_shape">The _shape.</param>
        /// <returns></returns>
        private string getPaletteEntryKeyForShape(string cwLightObjectTypeScriptname, cwShape _shape)
        {
            string objectPaletteEntryKey = cwLightObjectTypeScriptname + "|";

            if (cwLightObjectType.EVENTRESULT_SCRIPTNAME.Equals(cwLightObjectTypeScriptname))
            {
                // FLAGS AVAILABLE IN DATABASE (Corresponding to Event/Result Types in the diagram)
                if (0.Equals(_shape.flag2)) objectPaletteEntryKey += "0";
                if (131072.Equals(_shape.flag2)) objectPaletteEntryKey += "2";
                if (524288.Equals(_shape.flag2)) objectPaletteEntryKey += "1";
                if (655360.Equals(_shape.flag2)) objectPaletteEntryKey += "4";
            }
            else
            {
                objectPaletteEntryKey += getPaletteEntryCategoryKey(_shape.lightObject);
            }
            return objectPaletteEntryKey;
        }

        /// <summary>
        /// Creates the JSON file.
        /// </summary>
        private void exportShapes(StreamWriter json_file, List<cwShape> _shapes, cwLightDiagram diagram)
        {
            _shapes.Reverse();
            json_file.Write("\"shapes\":");
            json_file.Write("[");
            int shapesCountMax = _shapes.Count;
            int shapesCount = 0;

            foreach (cwShape shape in _shapes)
            {
                string objectLink = "";
                string cwLightObjectTypeScriptname = "";
                string objectName = "";
                if (shape.objectID != 0)
                {
                    cwLightObjectTypeScriptname = currentLightModel.getObjectTypeByID(shape.objectTypeID).ScriptName;
                    objectLink = cwLightObjectTypeScriptname.ToLower() + shape.objectID;
                    if (shape.lightObject != null)
                    {
                        objectName = shape.lightObject.ToString();
                    }
                }
                else if (7.Equals(shape.objectTypeID))
                {
                    objectName = diagram.properties["TITLE"];
                }

                objectName = cwTools.escapeChars(objectName);
                int x = shape.Position.X;
                int y = shape.Position.Y;
                Point min = getMinOfDiagram(diagram.ID);
                x += -min.X;
                y += -min.Y;
                int w = shape.Size.Width;
                int h = shape.Size.Height;
                json_file.Write("{\"objectPaletteEntryKey\":\"" + getPaletteEntryKeyForShape(cwLightObjectTypeScriptname, shape) + "\",");
                if (shape.customSymbol != 0)
                {
                    json_file.Write("\"customSymbol\" : " + shape.customSymbol + ",");
                }
                if (shape.customStyleID != 0)
                {
                    json_file.Write("\"customStyle\" : " + stylesByID[shape.customStyleID].toJSON() + ",");
                }

                json_file.Write("\"objectTypeName\" : \"" + cwLightObjectTypeScriptname + "\",");
                json_file.Write("\"link\" : \"" + objectLink + "\",");
                json_file.Write("\"name\" : \"" + objectName + "\",");

                if (shape.lightObject != null)
                {
                    cwLightObjectJSON shapeObject = new cwLightObjectJSON(shape.lightObject, diagramLoader.getObjectGroupNode(currentLightModel, shape.lightObject.OTID));

                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    string output = serializer.Serialize(shapeObject);
                    json_file.Write("\"cwObject\":" + output + ",");
                }


                json_file.Write("\"objectID\" : " + shape.objectID + ",");
                json_file.Write("\"x\" : " + x + ", \"y\" : " + y + ", \"w\" : " + w + ", \"h\" : " + h + ", \"seq\" : " + shape.sequence + "}");

                if (shapesCount < shapesCountMax - 1)
                {
                    json_file.Write(",");
                }
                shapesCount++;
            }
            json_file.Write("]");
        }

        /// <summary>
        /// Exports the pictures as planche.
        /// </summary>
        /// <param name="_fileManager">The _file manager.</param>
        private void exportPicturesAsPlanche(cwWebDesignerFileManager _fileManager, cwDiagramLoader diagramLoader)
        {
            cwLightObjectType PictureOT = currentLightModel.getObjectTypeByScriptName("PICTURE");
            cwLightNodeObjectType nodeOT = new cwLightNodeObjectType(PictureOT);
            nodeOT.addPropertyToSelect("ID");
            nodeOT.preloadLightObjects();

            string sitePath = _fileManager.addToSitePath("images/pictures/");

            foreach (var v in diagramLoader.templates)
            {
                // créer une planche
                int templateId = v.Value.Key.ID;
                string savePath = sitePath + "template" + templateId + ".png";
                if (this.picturesCoordsByTemplateID.ContainsKey(templateId))
                {
                    cwWebDesignerPicturesExports picturesCoords = this.picturesCoordsByTemplateID[templateId];
                    this.concatenateImages(sitePath, savePath, picturesCoords);
                }
            }
        }

        private const float MillimeterToPixel = 3.779f;

        /// <summary>
        /// Concatenates the images in planche, with right size.
        /// </summary>
        /// <param name="imagesPath">The images path.</param>
        /// <param name="savePath">The save path.</param>
        /// <param name="pictures">The pictures.</param>
        private void concatenateImages(string imagesPath, string savePath, cwWebDesignerPicturesExports pictures)
        {
            try
            {
                pictures.SetValues();
                int imageWidth = Convert.ToInt32(pictures.GetTotalWidth() * cwWebDesignerExportDiagrams.MillimeterToPixel);
                int imageHeight = Convert.ToInt32(pictures.GetTotalHeight() * cwWebDesignerExportDiagrams.MillimeterToPixel);
                Bitmap display = new Bitmap(imageWidth, imageHeight, PixelFormat.Format32bppArgb);
                Graphics g = Graphics.FromImage(display);
                g.PageUnit = GraphicsUnit.Millimeter;

                foreach (cwPicturesCoord picture in pictures.picturesCoords)
                {
                    Image image = Image.FromFile(imagesPath + "picture" + picture.ID + ".png");
                    if (image == null) continue;
                    g.DrawImage(image, picture.x, picture.y, picture.w, picture.h);
                }
                display.Save(savePath, ImageFormat.Png);
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Exports the pictures.
        /// </summary>
        private void exportPictures(cwWebDesignerFileManager _fileManager)
        {
            cwLightObjectType PictureOT = currentLightModel.getObjectTypeByScriptName("PICTURE");
            cwLightNodeObjectType nodeOT = new cwLightNodeObjectType(PictureOT);
            nodeOT.addPropertyToSelect("ID");
            nodeOT.preloadLightObjects();

            PictureLoader PL = new PictureLoader(currentLightModel.getConnection());
            string sitePath = _fileManager.addToSitePath("images/pictures/");
            foreach (cwLightObject p in nodeOT.usedOTLightObjects)
            {
                IDrawableImage image = PL.LoadImage(currentLightModel.getConnection(), p.ID, 1.0f);
                if (image.Image == null) continue;
                try
                {
                    string savePath = sitePath + "picture" + p.ID + ".png";
                    //image.Image.Save(savePath, System.Drawing.Imaging.ImageFormat.Png);
                    int _width = image.Image.Width / 4;
                    int _height = image.Image.Height / 4;
                    Bitmap _b = new Bitmap(image.Image, _width, _height);
                    _b.Save(savePath);

                    //string s = image.Image.PixelFormat.ToString();
                    //Bitmap b = new Bitmap(image.Image, new Size(image.Width, image.Height));                    
                    //b.Save(sitePath + "picture" + p.ID + ".bmp");
                }
                catch (Exception e)
                {
                    log.Error(String.Format("FOR PICTURE ID {0}", p.ID));
                    log.Error(e);

                    //throw new cwExceptionFatal(e.Message);
                }
            }
            PL.Dispose();
        }

        /// <summary>
        /// Does the exports.
        /// </summary>
        /// <param name="_fileManager">The _file manager.</param>
        private void doExports(cwWebDesignerFileManager _fileManager, bool exportDiagramsImages)
        {
            exportPictures(_fileManager);

            int diagramsCountMax = diagramLoader.diagramsShapes.Count();
            string imagesPath = _fileManager.addToSitePath("images/pictures/");

            foreach (var diagram in diagramLoader.diagramsShapes)
            {
                cwLightDiagram d = diagramLoader.allDiagrams[diagram.Key];
                string diagramKEY = "diagram" + diagram.Key;

                StreamWriter json_file = cwWebDesignerTools.getStreamWriterErase(_fileManager.addToGeneratedPath("diagram" + "/json/" + diagramKEY + "." + _fileManager.getJSONExtention()));
                Point min = getMinOfDiagram(d.ID);
                Point max = getMaxOfDiagram(d.ID);
                Size diagramSize = new Size(max.X - min.X, max.Y - min.Y);

                Size realSize = getRealDiagramSize(d.ID);
                // à commenter si on ne veut pas exporter les png des diagrammes
                if (exportDiagramsImages)
                {
                    d.saveImageWithoutBlankFrame(imagesPath + diagramKEY + ".png");
                }
                int templateID = 0;
                // vérifier que la property existe
                if (d.properties.ContainsKey("TYPE"))
                {
                    if (diagramLoader.templates.ContainsKey(d.properties["TYPE"]))
                    {
                        templateID = diagramLoader.templates[d.properties["TYPE"]].Key.ID;
                    }
                }

                json_file.Write("{");
                json_file.Write("\"diagram\" : {\"name\" : \"" + d.properties["NAME"] + "\", \"type\" : \"" + d.properties["TYPE"] + "\", \"model_filename\" : \"" + d.Model.FileName + "\", ");
                json_file.Write("\"model_name\" : \"" + d.Model.ToString() + "\", \"templateID\":" + templateID + ", \"object_id\":" + d.ID + ", \"size\":{\"w\":" + diagramSize.Width + ", \"h\":" + diagramSize.Height + "}, ");
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                List<int> _diagramIds = new List<int>();
                if (diagramLoader.parentDiagramIds.ContainsKey(diagram.Key))
                {
                    _diagramIds = diagramLoader.parentDiagramIds[diagram.Key];
                }
                json_file.Write("\"parentsId\" : " + serializer.Serialize(_diagramIds) + ", ");
                Dictionary<string, object> _propertiesToSerialize = new Dictionary<string, object>();
                cwLightObjectType diagramOT = d.Model.getObjectTypeByScriptName("DIAGRAM");
                foreach (var v in d.properties)
                {
                    if (diagramOT.hasProperty(v.Key))
                    {
                        cwLightProperty _prop = diagramOT.getProperty(v.Key);
                        switch (_prop.DataType)
                        {
                            case "Integer":
                                _propertiesToSerialize[v.Key] = Convert.ToInt32(v.Value);
                                break;
                            case "Double":
                                _propertiesToSerialize[v.Key] = Convert.ToDouble(v.Value);
                                break;
                            case "Boolean":
                                _propertiesToSerialize[v.Key] = Convert.ToBoolean(v.Value);
                                break;
                            case "Date":
                                _propertiesToSerialize[v.Key] = Convert.ToDateTime(v.Value);
                                break;
                            default:
                                _propertiesToSerialize[v.Key] = v.Value;
                                break;
                        }
                    }
                    else
                    {
                        _propertiesToSerialize[v.Key] = v.Value;
                    }
                }
                json_file.Write("\"properties\" : " + serializer.Serialize(_propertiesToSerialize));
                json_file.Write("},");

                exportShapes(json_file, diagramLoader.orderedShapes[diagram.Key], d);
                json_file.Write(",");

                exportJoiners(json_file, d.ID, d, diagramLoader.joinersByDiagram);
                json_file.Write(",");
                exportTemplatesPalettes(json_file, d, diagramLoader.templates);

                json_file.Write("}");
                json_file.Close();
            }
            // do not use
            //exportPicturesAsPlanche(_fileManager, diagramLoader);
        }

        private void exportTemplatesStandAlone(cwWebDesignerFileManager _fileManager, Dictionary<string, KeyValuePair<cwLightDiagram, Palette>> _templates)
        {
            DateTime start = DateTime.Now;
            foreach (var templateKey in _templates)
            {
                cwLightDiagram template = templateKey.Value.Key;
                Palette templatePalette = templateKey.Value.Value;
                string template_abbr = template.properties["TYPE" + cwLookupManager.LOOKUPABBR_KEY];
                if ("".Equals(template_abbr))
                {
                    log.Warn("ESCAPED;" + "Please provide a Abbreviation for template (" + templateKey.Key + ")");
                    continue;
                }
                StreamWriter json_template_file = cwWebDesignerTools.getStreamWriterErase(_fileManager.addToGeneratedPath("diagram" + "/json/template" + template_abbr + "." + _fileManager.getJSONExtention()));

                json_template_file.Write("{");
                exportTemplatesPalettes(json_template_file, template, diagramLoader.templates);
                json_template_file.Write("}");

                json_template_file.Close();
            }
            log.Info("TIME;STANDALONE TEMPLATES;" + DateTime.Now.Subtract(start).ToString());

        }

        private void doLoads(List<int> _diagramsToLoad, List<string> propertiesToLoad)
        {
            diagramLoader.doLoads(_diagramsToLoad, true, propertiesToLoad);

            DateTime start = DateTime.Now;
            loadFonts();
            log.Info("TIME;FONTS-LOADED;" + DateTime.Now.Subtract(start).ToString());

            start = DateTime.Now;
            loadStyles();
            log.Info("TIME;STYLES-LOADED;" + DateTime.Now.Subtract(start).ToString());

            picturesNode = new cwLightNodeObjectType(currentLightModel.getObjectTypeByScriptName("PICTURE"));
            picturesNode.addPropertyToSelect(cwLightObject.UNIQUEIDENTIFIER);
            picturesNode.addPropertyToSelect("ID");
            picturesNode.preloadLightObjects();
        }

        private void cleanLists()
        {
            diagramLoader.cleanLists();
            stylesByID.Clear();

        }

        public void exportDiagrams(cwLightModel _model, cwWebDesignerFileManager fileManager, List<int> _diagramsToLoad, bool exportDiagramsImages, List<string> propertiesToLoad)
        {
            if (0.Equals(_diagramsToLoad.Count()))
            {
                return;
            }

            currentLightModel = _model;
            cleanLists();

            if (!fileManager.checkWebStructureForPage("diagram"))
            {
                log.Error("Unable to create diagram Page, issue exits on the structure check");
                return;
            }
            //createDiagramHTMLFile("diagram", fileManager);

            doLoads(_diagramsToLoad, propertiesToLoad);
            doExports(fileManager, exportDiagramsImages);

            exportTemplatesStandAlone(fileManager, diagramLoader.templates);

            log.Info("DONE;DIAGRAMS EXPORTED");

        }

    }
}
