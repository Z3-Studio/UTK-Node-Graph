using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Transform)]
    [NodeDescription("Rotate the agent towards the target per frame")]
    public class RotateTowards : ActionTask
    {
        [ParameterDefinition(AutoBindType.FindSimilarVariable)]
        [SerializeField] private Parameter<Transform> transform;

        [SerializeField] private Parameter<Vector3> target;
        [SerializeField] private Parameter<float> speed = 15f;
        //[Range(0, 180)]
        [SerializeField] private Parameter<float> angleDifference = 5f;

        protected override void UpdateAction()
        {
            Vector3 lookPos = target.Value - transform.Value.position;
            Quaternion rotation = Quaternion.LookRotation(lookPos);
            transform.Value.rotation = Quaternion.RotateTowards(transform.Value.rotation, rotation, DeltaTime * speed.Value * 10f);

            if (Vector3.Angle(lookPos, transform.Value.forward) <= angleDifference.Value)
            {
                EndAction();
            }
        }
    }
}