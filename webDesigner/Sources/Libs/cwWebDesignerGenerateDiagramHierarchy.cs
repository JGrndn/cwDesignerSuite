using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Casewise.Data.ICM;
using Casewise.GraphAPI.API;
using System.Web.Script.Serialization;
using System.IO;
using System.Text.RegularExpressions;

namespace Casewise.webDesigner.Sources.Libs
{
    class cwWebDesignerGenerateDiagramHierarchy
    {
        public cwLightModel currentLightModel = null;

        public Dictionary<int, List<int>> fatherdiagList = new Dictionary<int, List<int>>();
        public Dictionary<int, List<int>> hierarchyCache = new Dictionary<int, List<int>>();
        public Dictionary<string, List<DiagramJson>> hierarchyJson = new Dictionary<string, List<DiagramJson>>();
        public string outputPath;
        public bool uuidAsFileName;
        public string jsonextension;
        public cwWebDesignerGenerateDiagramHierarchy(cwLightModel model, string output_path, string json_extension, bool uuid_As_Diag_FileName)
        {
            this.currentLightModel = model;
            this.outputPath = output_path;
            this.uuidAsFileName = uuid_As_Diag_FileName;
            this.jsonextension = json_extension;

        }


        public void GenerateHierarchy()
        {
            cwLightObjectType ot_diag = this.currentLightModel.getObjectTypeByScriptName("DIAGRAM");

            //cwLightObject DiagObject  = ot_diag.getObjectsByFilter(null);


            cwLightNodeObjectType node = new cwLightNodeObjectType(this.currentLightModel.getObjectTypeByScriptName("DIAGRAM"));

            List<string> _selectedProperties = new List<string>();
            _selectedProperties.Add("NAME");
            _selectedProperties.Add("ID");
            _selectedProperties.Add("UNIQUEIDENTIFIER");
            _selectedProperties.Add("TYPE");
            _selectedProperties.Add("DIAGRAMMER");
            _selectedProperties.Add("VALIDATED");
            //List<string> _selectedProperties = cwLightDiagram.getPropertiesToSelect(prop);

            node.addPropertiesToSelect(_selectedProperties.ToArray());
            node.preloadLightObjects();
            Dictionary<int, cwLightObject> _selectedDiagrams = node.usedOTLightObjectsByID;


            List<int> diags = new List<int>();
            // diags.Add(33);
            cwLightDiagram.loadAllParentDiagrams(this.currentLightModel, diags, fatherdiagList);
            foreach (var diag in _selectedDiagrams)
            {
                int id_diag = diag.Key;
                cwLightObject obj = diag.Value;

                List<cwLightProperty> lst_property = obj.getObjectType().getProperties();
                List<int> hrc = hierarchy_rec(id_diag,_selectedDiagrams);

                this.hierarchyCache[id_diag] = hrc;

                string uuid = obj.properties["UNIQUEIDENTIFIER"].ToString();

                if (this.uuidAsFileName)
                {
                    this.hierarchyJson[uuid] = hrc.Select(dId =>
                    {
                        cwLightObject di = _selectedDiagrams[dId];
                        string di_uuid = di.properties["UNIQUEIDENTIFIER"].ToString();
                        return new DiagramJson(di.ID, di_uuid, di.Text, di.properties["TYPE"].ToString());
                    }).ToList<DiagramJson>();
                }
                else {
                    this.hierarchyJson[id_diag.ToString()] = hrc.Select(dId =>
                    {
                        cwLightObject di = _selectedDiagrams[dId];
                        string di_uuid = di.properties["UNIQUEIDENTIFIER"].ToString();
                        return new DiagramJson(di.ID, di_uuid, di.Text, di.properties["TYPE"].ToString());
                    }).ToList<DiagramJson>();
                }
            }

            JavaScriptSerializer js = new JavaScriptSerializer();
            string c = js.Serialize(this.hierarchyJson);

            File.WriteAllText(outputPath + "hierarchy." + this.jsonextension, c, Encoding.UTF8);


        }

        public List<int> lst_hierachy_items = new List<int>();

        public List<int> hierarchy_rec(int diag_id, Dictionary<int, cwLightObject> _selectedDiagrams)
        {
            List<int> hierarchy = new List<int>();
            if (!hierarchyCache.ContainsKey(diag_id))
            {
                List<int> lst_parent_diag = new List<int>();

                this.fatherdiagList.TryGetValue(diag_id, out lst_parent_diag);

                if (lst_parent_diag != null && lst_parent_diag.Count > 0)
                {
                    lst_parent_diag.Reverse();

                    int bestFather = 0;
                    //some rules to choose the best father
                    foreach (int id_diag_father in lst_parent_diag)
                    {
                        cwLightObject father = _selectedDiagrams[id_diag_father];
                        cwLightObject diag = _selectedDiagrams[diag_id];

                        // not the same category
                        string type_diag_father = father.properties["TYPE"];
                        string type_diag = diag.properties["TYPE"];

                        // the diagram should be validated 
                        bool _notValidated = "0".Equals(father.properties["VALIDATED"]);

                        // if the diagram is matrix, it won't be taken into account 
                        string rule_diag = father.properties["DIAGRAMMER"];
                        bool _isMatrix = "Matrix".Equals(rule_diag);


                        //all the rules should be met to be a good Father :)
                        if (!_isMatrix && id_diag_father != diag_id && !_notValidated && type_diag_father != type_diag)
                        {
                            bestFather = id_diag_father;
                        }


                        // @todo : to check this rule
                        // the father level = child level + 1
                        // it works only if we use numnber to set tell the diagram level apart 
                        //for example, in the current diagram's level is 2, the father diagram is 1, then this father could be the best fahter :D
                        // check it, check it
                        string type_diag_father_level = Regex.Match(type_diag_father, @"\d+").Value;
                        string type_diag_level = Regex.Match(type_diag, @"\d+").Value;
                        if (!String.IsNullOrEmpty(type_diag_level) && !String.IsNullOrEmpty(type_diag_father_level))
                        {
                            int level_father = int.Parse(type_diag_father_level);
                            int level_child = int.Parse(type_diag_level);
                            if (level_father == level_child - 1)
                            {
                                bestFather = id_diag_father;
                                break;
                            }
                        }
                    }

                    // if the category containes a number
                    bool itemExistInHierarchy = lst_hierachy_items.Contains(diag_id);
                    if (!itemExistInHierarchy &&bestFather!=0)
                    {
                        lst_hierachy_items.Add(diag_id);
                        List<int> parentHierarchy = hierarchy_rec(bestFather, _selectedDiagrams);
                        hierarchy.AddRange(parentHierarchy);
                        hierarchy.Add(diag_id);
                        lst_hierachy_items = new List<int>();
                        return hierarchy;
                    }
                    //foreach(int id_diag_father in lst_parent_diag)
                    //{
                    //    cwLightObject father = _selectedDiagrams[id_diag_father];
                    //    cwLightObject diag = _selectedDiagrams[diag_id];

                    //    string type_diag_father = father.properties["TYPE"];
                    //    string type_diag = diag.properties["TYPE"];

                    //    string type_diag_father_level  = Regex.Match(type_diag_father, @"\d+").Value;
                    //    string type_diag_level = Regex.Match(type_diag, @"\d+").Value;

                    //    // if the diagram is matrix, it won't be taken into account 
                    //    string rule_diag = father.properties["DIAGRAMMER"];
                    //    bool isMatrix = "Matrix".Equals(rule_diag);

                    //    // if the category containes a number
                    //    bool itemExistInHierarchy = lst_hierachy_items.Contains(diag_id);
                    //    if (id_diag_father != diag_id && type_diag != type_diag_father && !itemExistInHierarchy && !isMatrix)
                    //    {    
                    //        lst_hierachy_items.Add(diag_id);
                    //        List<int> parentHierarchy = hierarchy_rec(id_diag_father, _selectedDiagrams);
                    //        hierarchy.AddRange(parentHierarchy);
                    //        hierarchy.Add(diag_id);
                    //        lst_hierachy_items = new List<int>();
                    //        return hierarchy;
                    //    }
                    //}
                }
                else
                {
                    // int id = diag.ID;
                    hierarchy.Add(diag_id);
                }
                lst_hierachy_items = new List<int>();
                return hierarchy;
            }
            else
            {   //hierarchyCache.ContainsKey(diag.ID)
                return this.hierarchyCache[diag_id];
            }
        }
        public class DiagramJson
        {
            public string UUID { get; set; }
            public int Id { get; set; }
            public string Name { get; set; }
            public string Type { get; set; }

            public DiagramJson()
            {
            }

            public DiagramJson(int id, string uuid, string name, string type)
            {
                this.Id = id;
                this.UUID = uuid;
                this.Name = name;
                this.Type = type;
            }
            public DiagramJson(int id, string uuid, string name)
            {
                this.Id = id;
                this.UUID = uuid;
                this.Name = name;
            }
        }

    }
}
