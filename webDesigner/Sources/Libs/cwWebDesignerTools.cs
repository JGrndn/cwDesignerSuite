using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Casewise.GraphAPI.API;
using Casewise.GraphAPI.PSF;
using Casewise.webDesigner.Nodes;

namespace Casewise.webDesigner.Libs
{
    internal class cwWebDesignerTools
    {
        
        /// <summary>
        /// Gets the properties separted by coma.
        /// </summary>
        /// <param name="nodeOG">The node OG.</param>
        /// <returns></returns>
        public static string getPropertiesSepartedByComa(cwPSFTreeNodeObjectNode nodeOG)
        {
            string propertiesSeparatedByComa = "";
            if (nodeOG.selectedCheckedProperties.Count() > 0)
            {
                foreach (cwLightProperty p in nodeOG.selectedCheckedProperties)
                {
                    if ("Memo".Equals(p.DataType)) {
                        continue;
                    }
                    string p_to_lower = p.ScriptName.ToLower();
                    propertiesSeparatedByComa += "'" + p_to_lower + "', ";
                }
                propertiesSeparatedByComa = propertiesSeparatedByComa.Substring(0, propertiesSeparatedByComa.Length - 2);
            }
            return propertiesSeparatedByComa;
        }

       

        /// <summary>
        /// Creates the JSON documentation_ rec.
        /// </summary>
        /// <param name="objectGroupNode">The object group node.</param>
        /// <param name="json_file">The json_file.</param>
        /// <param name="_space">The _space.</param>
        private static void createJSONDocumentation_Rec(cwPSFTreeNodeObjectNode objectGroupNode, StreamWriter json_file, string _space)
        {
            json_file.Write(_space + "." + objectGroupNode.ID + " : ");
            foreach (cwLightProperty p in objectGroupNode.propertiesBoxes.getPropertyBox<cwPSFPropertyBoxTreeViewProperties>(cwPSFTreeNodeObjectNode.CONFIG_SELECTED_PROPERTIES).getCheckedPropertiesList())
            {
                json_file.Write(p.ScriptName.ToLower() + ", ");
            }
            json_file.Write(" object_id, link_id");
            json_file.WriteLine();
            if (objectGroupNode.hasAtLeastOnChildNode<cwWebDesignerTreeNodeObjectNodeAssociationType>())
            {
                foreach (cwWebDesignerTreeNodeObjectNodeAssociationType ATChild in objectGroupNode.getChildrenNodes<cwWebDesignerTreeNodeObjectNodeAssociationType>())
                {
                    createJSONDocumentation_Rec(ATChild, json_file, _space + " ");
                }
            }
        }


        /// <summary>
        /// Adds to output.
        /// </summary>
        /// <param name="_output">The _output.</param>
        /// <param name="_tabulation">The _tabulation.</param>
        /// <param name="_text">The _text.</param>
        public static void addToOutput(StreamWriter _output, string _tabulation, string _text)
        {
            _output.WriteLine(_tabulation + "output.push(\"" + _text + "\");");
        }

        /// <summary>
        /// Gets the stream writer.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static StreamWriter getStreamWriterErase(string path)
        {
            Encoding myUTF8 = new UTF8Encoding(false);
            return new StreamWriter(path, false, myUTF8);
        }

        /// <summary>
        /// Creates the JSON documentation.
        /// </summary>
        /// <param name="_Page">The _ page.</param>
        /// <param name="json_file">The json_file.</param>
        /// <param name="_pageName">Name of the _page.</param>
        public static void createJSONDocumentation(cwWebDesignerTreeNodePage _Page, StreamWriter json_file, string _pageName)
        {
            json_file.WriteLine("/*");

            if (_Page.hasAtLeastOnChildNode<cwWebDesignerTreeNodeObjectNodeObjectType>())
            {
                foreach (cwWebDesignerTreeNodeObjectNodeObjectType OGOT in _Page.getChildrenNodes<cwWebDesignerTreeNodeObjectNodeObjectType>())
                {
                    json_file.WriteLine(_pageName);
                    createJSONDocumentation_Rec(OGOT, json_file, " ");
                }
            }


            json_file.WriteLine("*/");
        }
    }
}
