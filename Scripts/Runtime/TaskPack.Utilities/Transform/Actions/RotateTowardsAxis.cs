using UnityEngine;
using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Transform)]
    [NodeDescription("Rotate the agent towards the target per frame")]
    public class RotateTowardsAxis : ActionTask
    {
        [ParameterDefinition(AutoBindType.FindSimilarVariable)]
        [SerializeField] private Parameter<Transform> transform;

        [SerializeField] private Parameter<Vector3> target;
        [SerializeField] private Parameter<float> speed = 15f;
        [SerializeField] private Parameter<Axis3Flags> axis;
        //[Range(0, 180)]
        [SerializeField] private Parameter<float> angleDifference = 5f;

        public override string Info => $"Rotate {axis} Towards {target}";

        protected override void StartAction()
        {
            if (Vector3.Distance(transform.Value.position, target.Value) <= 0.01f)
            {
                EndAction();
            }
        }

        protected override void UpdateAction()
        {
            // Get Rotation
            Vector3 lookPos = target.Value - transform.Value.position;
            Quaternion targetRotation = Quaternion.LookRotation(lookPos);

            Vector3 currentEuler = transform.Value.rotation.eulerAngles;
            Vector3 targetEuler = targetRotation.eulerAngles;

            if (!axis.Value.HasFlag(Axis3Flags.X))
            {
                targetEuler.x = currentEuler.x;
            }

            if (!axis.Value.HasFlag(Axis3Flags.Y))
            {
                targetEuler.y = currentEuler.y;
            }

            if (!axis.Value.HasFlag(Axis3Flags.Z))
            {
                targetEuler.z = currentEuler.z;
            }

            // Apply Rotation
            Quaternion rotation = Quaternion.Euler(targetEuler);
            transform.Value.rotation = Quaternion.RotateTowards(transform.Value.rotation, rotation, DeltaTime * speed.Value * 10f);

            if (Quaternion.Angle(transform.Value.rotation, rotation) <= angleDifference.Value)
            {
                EndAction();
            }
        }
    }
}
