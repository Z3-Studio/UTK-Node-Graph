using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Transform)]
    [NodeDescription("Rotate the agent towards the target per frame")]
    public class RotateTowards : ActionTask
    {
        [ParameterDefinition(AutoBindType.SelfBind)]
        [SerializeField] private Parameter<Transform> data;

        [SerializeField] private Parameter<Vector3> target;
        [SerializeField] private Parameter<float> speed = 2;
        [Range(0, 180)]
        [SerializeField] private Parameter<float> angleDifference = 5;

        protected override void UpdateAction()
        {
            Vector3 lookPos = target.Value - data.Value.position;
            Quaternion rotation = Quaternion.LookRotation(lookPos);
            data.Value.rotation = Quaternion.Slerp(data.Value.rotation, rotation, Time.fixedDeltaTime * speed.Value);

            if (Vector3.Angle(lookPos, data.Value.forward) <= angleDifference.Value)
            {
                EndAction();
            }
        }
    }
}