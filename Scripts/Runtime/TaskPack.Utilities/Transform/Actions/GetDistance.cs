using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities.AI
{
    [NodeCategory(Categories.Transform)]
    public class GetDistance : ActionTask<Transform>
    {
        public Parameter<Transform> target;        
        public Parameter<float> result;

        public override string Info => $"Get Distance to {target}";

        protected override void StartAction()
        {
            result.Value = Vector3.Distance(Agent.position, target.Value.position);
            EndAction();
        }
    }
}