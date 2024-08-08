using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using System;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Transform)]
    [NodeDescription("Rotate axis of a GameObject")]
    public class RotateDegreesOnAxis : ActionTask
    {
        [ParameterDefinition(AutoBindType.SelfBind)]
        [SerializeField] private Parameter<Transform> data;

        [SerializeField] private Parameter<float> degrees;
        [SerializeField] private Parameter<float> speed;
        [SerializeField] private Parameter<Axis3> axis;

        public override string Info => $"Rotate {data} {axis} {degrees} degrees";

        private float currentDegrees;
        private Vector3 initialAngle;

        private Func<Quaternion> updateRotation;
        protected override void StartAction()
        {
            currentDegrees = 0;
            initialAngle = data.Value.eulerAngles;

            updateRotation = () => axis.Value switch
            {
                Axis3.X => Quaternion.Euler(currentDegrees + initialAngle.x, data.Value.eulerAngles.y, data.Value.eulerAngles.z),
                Axis3.Y => Quaternion.Euler(data.Value.eulerAngles.x, currentDegrees + initialAngle.y, data.Value.eulerAngles.z),
                Axis3.Z => Quaternion.Euler(data.Value.eulerAngles.x, data.Value.eulerAngles.y, currentDegrees + initialAngle.z),
                _ => throw new NotImplementedException(),
            };
        }

        protected override void UpdateAction()
        {
            currentDegrees = Mathf.MoveTowards(currentDegrees, degrees.Value, Time.fixedDeltaTime * speed.Value);
            data.Value.rotation = updateRotation();

            if (currentDegrees == degrees.Value)
            {
                EndAction();
            }
        }
    }
}