using System.Collections;
using UnityEngine;
using Z3.NodeGraph.Core;

namespace Z3.NodeGraph.BehaviourTree
{
    [NodeIcon(GraphIcon.ForEach)]
    public class ForEach : DecoratorNode
    {
        [SerializeField] private Parameter<IEnumerable> list;

        public override string SubInfo => $"{list}";

        protected override State UpdateNode()
        {

            foreach (var item in list.Value)
            {

            }
            // Give to the black board each element

            return child.Update();
        }
    }
}
