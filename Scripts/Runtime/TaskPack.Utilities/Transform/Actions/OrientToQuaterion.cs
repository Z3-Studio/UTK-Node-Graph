using UnityEngine;
using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using Z3.UIBuilder.Core;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Transform)]
    [NodeDescription("Rotate to a target Quaternion")]
    public class OrientToQuaterion : ActionTask
    {
        [ParameterDefinition(AutoBindType.FindSimilarVariable)]
        [SerializeField] private Parameter<Transform> transform;
        [SerializeField] private Parameter<Quaternion> target;
        [SerializeField] private Parameter<Axis3Flags> modifiedAxis = Axis3Flags.Y;

        [Space, Tooltip("If false, it will rotate in the same frame")]
        [SerializeField] private bool useSpeed = true;
        [ShowIf(nameof(useSpeed))]
        [SerializeField] private Parameter<float> speed;
        [ShowIf(nameof(useSpeed)), Slider(0, 180)]
        [SerializeField] private Parameter<float> angleDifference = 10f;

        public override string Info => $"Rotate {modifiedAxis} To {target}" + (useSpeed ? $" Speed {speed}" : string.Empty);

        private Transform Agent => transform;

        private const float MinAngleDiff = 0.1f;

        protected override void StartAction()
        {
            if (!useSpeed)
            {
                Agent.rotation = GetRotation();
                EndAction();
            }
        }

        protected override void UpdateAction()
        {
            Quaternion eulerRotation = GetRotation();
            Agent.rotation = Quaternion.RotateTowards(Agent.rotation, eulerRotation, speed.Value * DeltaTime);

            float angleDiff = Quaternion.Angle(Agent.rotation, eulerRotation);
            float finishAngle = Mathf.Max(angleDifference.Value, MinAngleDiff);

            if (angleDiff <= finishAngle)
            {
                if (angleDiff <= MinAngleDiff)
                {
                    Agent.rotation = eulerRotation;
                }

                EndAction();
            }
        }

        private Quaternion GetRotation()
        {
            Vector3 targetEuler = target.Value.eulerAngles;
            Vector3 currentEuler = Agent.eulerAngles;
            Axis3Flags axes = modifiedAxis.Value;

            if (!axes.HasFlag(Axis3Flags.X))
            {
                targetEuler.x = currentEuler.x;
            }
            if (!axes.HasFlag(Axis3Flags.Y))
            {
                targetEuler.y = currentEuler.y;
            }
            if (!axes.HasFlag(Axis3Flags.Z))
            {
                targetEuler.z = currentEuler.z;
            }

            return Quaternion.Euler(targetEuler);
        }
    }
}
