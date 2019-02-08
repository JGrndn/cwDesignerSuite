using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Casewise.GraphAPI.PSF;
using Casewise.webDesigner.GUI;
using Casewise.webDesigner.Nodes;

namespace Casewise.webDesigner.Libs
{
    internal class cwWebDesignerBehaviourManager
    {
        private Dictionary<string, List<cwWebDesignerTreeNodeBehaviour>> _behavioursByAllowedLayout = new Dictionary<string, List<cwWebDesignerTreeNodeBehaviour>>();

        private List<cwWebDesignerTreeNodeBehaviour> allBehaviours = new List<cwWebDesignerTreeNodeBehaviour>();
        /// <summary>
        /// Initializes a new instance of the <see cref="cwWebDesignerBehaviourManager"/> class.
        /// </summary>
        /// <param name="webDesignerGUI">The web designer GUI.</param>
        public cwWebDesignerBehaviourManager(cwWebDesignerGUI webDesignerGUI)
        {
            cwWebDesignerTreeNodeBehaviourTreeView treeview = new cwWebDesignerTreeNodeBehaviourTreeView(webDesignerGUI, null);
            cwWebDesignerTreeNodeBehaviourAccordion accordion = new cwWebDesignerTreeNodeBehaviourAccordion(webDesignerGUI, null);
            cwWebDesignerTreeNodeBehaviourWorldMap map = new cwWebDesignerTreeNodeBehaviourWorldMap(webDesignerGUI, null);
            
            addBehaviour(treeview);
            addBehaviour(accordion);
            addBehaviour(map);

            allBehaviours.Add(treeview);
            allBehaviours.Add(accordion);
            allBehaviours.Add(map);
        }

        public List<cwWebDesignerTreeNodeBehaviour> getAllBehaviours()
        {
            return this.allBehaviours;
        }

        /// <summary>
        /// Gets the behaviours for layout.
        /// </summary>
        /// <param name="layoutName">Name of the layout.</param>
        /// <returns></returns>
        public List<cwWebDesignerTreeNodeBehaviour> getBehavioursForLayout(string layoutName)
        {
            List<cwWebDesignerTreeNodeBehaviour> layoutBehaviours = new List<cwWebDesignerTreeNodeBehaviour>();
            if (_behavioursByAllowedLayout.ContainsKey(layoutName))
            {
                layoutBehaviours.AddRange(_behavioursByAllowedLayout[layoutName]);
            }
            return layoutBehaviours;
        }

        /// <summary>
        /// Adds the behaviour.
        /// </summary>
        /// <param name="behaviour">The behaviour.</param>
        public void addBehaviour(cwWebDesignerTreeNodeBehaviour behaviour)
        {
            foreach (string layoutName in behaviour.allowedLayouts)
            {
                if (!_behavioursByAllowedLayout.ContainsKey(layoutName))
                {
                    _behavioursByAllowedLayout[layoutName] = new List<cwWebDesignerTreeNodeBehaviour>();
                }
                _behavioursByAllowedLayout[layoutName].Add(behaviour);
            }
        }

    }
}
