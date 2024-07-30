using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using System;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Transform)]
    [NodeDescription("Rotate axis of a GameObject")]
    public class RotateDegreesOnAxis : ActionTask<Transform>
    {
        public Parameter<float> degrees;
        public Parameter<float> speed;
        public Parameter<Axis3> axis;

        public override string Info => $"Rotate {AgentInfo} {axis} {degrees} degrees";

        private float currentDegrees;
        private Vector3 initialAngle;

        private Func<Quaternion> updateRotation;
        protected override void StartAction()
        {
            currentDegrees = 0;
            initialAngle = Agent.eulerAngles;

            updateRotation = () => axis.Value switch
            {
                Axis3.X => Quaternion.Euler(currentDegrees + initialAngle.x, Agent.eulerAngles.y, Agent.eulerAngles.z),
                Axis3.Y => Quaternion.Euler(Agent.eulerAngles.x, currentDegrees + initialAngle.y, Agent.eulerAngles.z),
                Axis3.Z => Quaternion.Euler(Agent.eulerAngles.x, Agent.eulerAngles.y, currentDegrees + initialAngle.z),
                _ => throw new NotImplementedException(),
            };
        }

        protected override void UpdateAction()
        {
            currentDegrees = Mathf.MoveTowards(currentDegrees, degrees.Value, Time.fixedDeltaTime * speed.Value);
            Agent.rotation = updateRotation();

            if (currentDegrees == degrees.Value)
            {
                EndAction(true);
            }
        }
    }
}