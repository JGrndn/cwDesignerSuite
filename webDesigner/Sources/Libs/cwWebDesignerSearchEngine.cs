using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Casewise.GraphAPI.PSF;
using Casewise.webDesigner.Nodes;

namespace Casewise.webDesigner.Libs
{
    internal class cwWebDesignerSearchEngine
    {

        public static string createSearchEngineRequirements(cwWebDesignerTreeNodePage _Page, int level, string pageName)
        {
            string output = "";

            int rule_num = 0;
            foreach (cwPSFTreeNodeObjectNode nodeOG in _Page.getChildrenObjectNodes())
            {
                output += createSearchEngineRequirements_rec(nodeOG, 0, pageName);
                if (rule_num < _Page.getChildrenObjectNodes().Count() - 1)
                {
                    output += @",";
                }
                rule_num++;
            }
            return output;
        }

        /// <summary>
        /// Creates the search engine requirements.
        /// </summary>
        /// <param name="nodeOG">The node OG.</param>
        /// <param name="level">The level.</param>
        /// <param name="pageName">Name of the page.</param>
        /// <returns></returns>
        private static string createSearchEngineRequirements_rec(cwPSFTreeNodeObjectNode nodeOG, int level, string pageName)
        {
            string output = "";
            // output = "";

            string levelSpace = "";
            for (int i = level; i >= 0; --i)
            {
                levelSpace += "   ";
            }

            var rules = @"
";

            //if (nodeOG.getChildrenObjectNodes().Count > 0)
            {
                int og_num = 0;
                //foreach (cwObjectGroup OG in rule.getObjectGroups())
                {

                    rules += levelSpace;
                    rules += @"{'id':'" + nodeOG.ID + "', 'properties' : [" + cwWebDesignerTools.getPropertiesSepartedByComa(nodeOG) + "], ";
                    rules += @"children:[";

                    int rNum = 0;
                    if (nodeOG.getChildrenObjectNodes().Count() > 0)
                    {
                        foreach (cwPSFTreeNodeObjectNode r in nodeOG.getChildrenObjectNodes())
                        {
                            rules += createSearchEngineRequirements_rec(r, level + 1, pageName);

                            if (rNum < nodeOG.getChildrenObjectNodes().Count() - 1)
                            {
                                rules += @",";
                            }
                            rNum++;
                        }

                    }
                    else
                    {
                        //rules += "empty";
                    }

                    if (rNum > 0)
                    {
                        rules += levelSpace;
                        rules += @"
" + levelSpace + "]";
                    }
                    else
                    {
                        rules += "]";
                    }

                    rules += @"}";
                    if (og_num < nodeOG.getChildrenObjectNodes().Count() - 1)
                    {
                        rules += @",";
                    }
                    og_num++;

                }

                //rules = rules.Substring(0, rules.Length - 2);
                output += rules;
            }
            //else
            //{
            //    output += "[empty" + rules + "]";
            //}


            return output;
        }
    }
}
