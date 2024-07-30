using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Transform)]
    [NodeDescription("Return the parent's transform from the Agent.")]
    public class GetParent : ActionTask <Transform>
    {
        public Parameter<Transform> returnParent;
        protected override void StartAction() {
            returnParent.Value = Agent.parent;
            EndAction(true);
        }
    }
}