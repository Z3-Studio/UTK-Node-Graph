using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;
using Z3.UIBuilder.Core;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Transform)]
    [NodeDescription("Rotate axis of a GameObject")]
    public class LookAt : ActionTask
    {
        [ParameterDefinition(AutoBindType.SelfBind)]
        [SerializeField] private Parameter<Transform> transform;

        public bool useSpeed = true;
        [SerializeField] private Parameter<Axis3Flags> modifiedAxis = Axis3Flags.Y;
        [SerializeField] private Parameter<Vector3> target;

        //[ShowIf(nameof(useSpeed), 1)]
        [SerializeField] private Parameter<float> speed;
        //[ShowIf(nameof(useSpeed), 1)]
        [Slider(0, 180)]
        [SerializeField] private Parameter<float> angleDifference = 10f;

        public override string Info => $"Look At {modifiedAxis}" + (useSpeed ? $" Speed {speed}" : string.Empty);

        private Transform Agent => transform;

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
            Agent.rotation = Quaternion.Slerp(Agent.rotation, eulerRotation, speed.Value * Time.fixedDeltaTime);

            if (Vector3.Angle(eulerRotation * Vector3.forward, Agent.forward) <= angleDifference.Value)
            {
                EndAction();
            }
        }

        private Quaternion GetRotation()
        {
            Vector3 targetDirection = target.Value - Agent.position;
            Vector3 eules = Quaternion.LookRotation(targetDirection).eulerAngles;

            if (!modifiedAxis.Value.HasFlag(Axis3Flags.X))
            {
                eules.x = Agent.eulerAngles.x;
            }
            if (!modifiedAxis.Value.HasFlag(Axis3Flags.Y))
            {
                eules.y = Agent.eulerAngles.y;
            }
            if (!modifiedAxis.Value.HasFlag(Axis3Flags.Z))
            {
                eules.z = Agent.eulerAngles.z;
            }

           return Quaternion.Euler(eules);
        }
    }
}