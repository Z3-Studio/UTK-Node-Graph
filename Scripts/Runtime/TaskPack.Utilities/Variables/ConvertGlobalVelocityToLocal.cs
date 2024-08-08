using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Transform)]
    [NodeDescription("Convert the global velocity to the velocity the transform is directed")]
    public class ConvertGlobalVelocityToLocal : ActionTask
    {
        [ParameterDefinition(AutoBindType.SelfBind)]
        [SerializeField] private Parameter<Transform> data;

        [SerializeField] private Parameter<Vector3> globalVelocity;
        [SerializeField] private Parameter<Vector3> localVelocity;

        public override string Info => $"Convert {globalVelocity} to Velocity";

        protected override void StartAction()
        {
            localVelocity.Value = new Vector3()
            {
                x = Vector3.Dot(data.Value.right, globalVelocity.Value),
                y = Vector3.Dot(data.Value.up, globalVelocity.Value),
                z = Vector3.Dot(data.Value.forward, globalVelocity.Value)
            };

            EndAction();
        }
    }
}