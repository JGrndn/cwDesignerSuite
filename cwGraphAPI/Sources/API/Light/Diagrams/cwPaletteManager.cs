using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Casewise.GraphAPI.API;
using Casewise.GraphAPI.Exceptions;
using Casewise.Services.Entities;
using System.Drawing;
using Casewise.GraphAPI.PSF;


namespace Casewise.GraphAPI.API
{
    /// <summary>
    /// cwPaletteManager
    /// </summary>
    public class cwPaletteManager
    {
        private Dictionary<string, Dictionary<int, Size>> paletteEntries = new Dictionary<string, Dictionary<int, Size>>();
        cwLightDiagram diagram = null;


        /// <summary>
        /// Initializes a new instance of the <see cref="cwPaletteManager"/> class.
        /// </summary>
        /// <param name="diagramObject">The diagram object.</param>
        public cwPaletteManager(cwLightObject diagramObject)
        {
            cwLightModel model = diagramObject.Model;
            diagram = new cwLightDiagram(model, diagramObject.ID, diagramObject.ToString(), diagramObject.ToString(), diagramObject.properties[cwLightObject.UNIQUEIDENTIFIER]);
            diagram.Load(model);

            foreach (PaletteEntry dpe in diagram.diagram.Palette.Entries)
            {
                Size size = new Size(dpe.Width, dpe.Height);
                if (!paletteEntries.ContainsKey(dpe.ObjectType.ScriptName))
                {
                    paletteEntries[dpe.ObjectType.ScriptName] = new Dictionary<int, Size>();
                }
                if (!paletteEntries[dpe.ObjectType.ScriptName].ContainsKey(dpe.Category))
                {
                    paletteEntries[dpe.ObjectType.ScriptName][dpe.Category] = size;
                }
            }
            diagram.Dispose();
        }


        /// <summary>
        /// Objects the type script name is in palette.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns></returns>
        private bool objectTypeScriptNameIsInPalette(cwLightObjectType objectType)
        {
            if (!paletteEntries.ContainsKey(objectType.ScriptName))
            {
                throw new cwExceptionNodeValidation(String.Format(Casewise.GraphAPI.Properties.Resources.ERROR_OBJETTYPE_IS_NOT_IN_PALETTEENTRY, objectType.ToString(), diagram.ToString()), null);
            }
            return true;
        }

        /// <summary>
        /// Checks if object type is inside palette entry.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns></returns>
        public bool checkIfObjectTypeIsInsidePaletteEntry(cwPSFTreeNodeObjectNode node)
        {
            try
            {
                objectTypeScriptNameIsInPalette(node.getSelectedObjectType());
                foreach (cwPSFTreeNodeObjectNode nodeATChild in node.getChildrenObjectNodes())
                {
                    checkIfObjectTypeIsInsidePaletteEntry(nodeATChild);
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return true;
        }

        /// <summary>
        /// Gets the default size for object.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public Size getDefaultSizeForObject(cwLightObject item)
        {
            cwLightObjectType OT = item.getObjectType();
            if (paletteEntries.ContainsKey(OT.ScriptName))
            {
                int categoryID = Convert.ToInt32(item.properties["TYPE" + cwLookupManager.LOOKUPID_KEY]);
                if (paletteEntries[OT.ScriptName].ContainsKey(categoryID))
                {
                    return paletteEntries[OT.ScriptName][categoryID];
                }
                if (paletteEntries[OT.ScriptName].ContainsKey(0))
                {
                    return paletteEntries[OT.ScriptName][0];
                }
                Size defaultSize = new Size(cwLightDiagram.one_pixel * 2, cwLightDiagram.one_pixel);
                return defaultSize;
            }
            else
            {
                throw new cwExceptionWarning(String.Format(Properties.Resources.ERROR_OBJETTYPE_IS_NOT_IN_PALETTEENTRY, item.getObjectType().ToString(), diagram.ToString()));
            }
        }

        /// <summary>
        /// Gets the object type script names.
        /// </summary>
        public List<string> objectTypeScriptNames
        {
            get
            {
                return paletteEntries.Keys.ToList<string>();
            }
        }

    }
}
