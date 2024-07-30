using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Transform)]
    [NodeDescription("Convert the global velocity to the velocity the transform is directed")]
    public class ConvertGlobalVelocityToLocal : ActionTask<Transform>
    {
        public Parameter<Vector3> globalVelocity;
        public Parameter<Vector3> localVelocity;

        public override string Info => $"Convert {globalVelocity} to Velocity";

        protected override void StartAction()
        {
            localVelocity.Value = new Vector3()
            {
                x = Vector3.Dot(Agent.right, globalVelocity.Value),
                y = Vector3.Dot(Agent.up, globalVelocity.Value),
                z = Vector3.Dot(Agent.forward, globalVelocity.Value)
            };

            EndAction();
        }
    }
}