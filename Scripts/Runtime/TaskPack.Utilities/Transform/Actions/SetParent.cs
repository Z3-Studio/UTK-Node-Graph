using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Transform)]
    [NodeDescription("Set the Transform Parent")]
    public class SetParent : ActionTask<Component>
    {
        public Parameter<Transform> parent;
        public override string Info => $"Parent = {parent}";
        protected override void StartAction()
        {
            Agent.transform.SetParent(parent.Value);
            EndAction(true);
        }
    }
}