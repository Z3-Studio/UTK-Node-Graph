using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities.AI
{
    [NodeCategory(Categories.Math)]
    [NodeDescription("TODO")]
    public class GetDeltaMovement : ActionTask
    {
        [ParameterDefinition(AutoBindType.SelfBind)]
        [SerializeField] private Parameter<Transform> data;

        [SerializeField] private Parameter<Vector3> velocity;

        public override string Info => $"Get {data} AI Velocity";

        private Vector3 previousPosition;
        private int previousFrame;
        protected override void StartAction()
        {
            previousFrame = Time.frameCount;
            previousPosition = data.Value.position;
        }

        protected override void UpdateAction()
        {
            if (Time.frameCount > previousFrame)
            {
                velocity.Value = (data.Value.position - previousPosition) / DeltaTime;
                EndAction();
            }
        }
    }
}