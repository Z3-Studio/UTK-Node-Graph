using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;
using Z3.Utils.ExtensionMethods;
using Z3.Utils;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Variables)]
    [NodeDescription("Get a quatertion with Z rotation inside of a opening angle")]
    public class GetRotationWithinOpeningAngle : ActionTask
    {
        [Header("In")]
        public Parameter<Transform> directedTransform;
        public Parameter<float> desiredAngle;
        public Parameter<float> openingAngle;

        [Header("Out")]
        public Parameter<Quaternion> rotation;

        public override string Info => $"{name} = {openingAngle}";

        protected override void StartAction()
        {
            float angleZ = MathUtils.DirectionToAngle(directedTransform.Value.right);
            
            float halfAngle = openingAngle.Value / 2;
            float minAngle = (angleZ - halfAngle).NormalizeAngle();
            float maxAngle = (angleZ + halfAngle).NormalizeAngle();

            float eulerZ = desiredAngle.Value;
            Vector2 angleRange = new Vector2(minAngle, maxAngle);
            if (!angleRange.InsideRange(eulerZ))
            {
                float a = MathUtils.AngleDiference(desiredAngle.Value, angleRange.x);
                float b = MathUtils.AngleDiference(desiredAngle.Value, angleRange.y);

                eulerZ = a < b ? angleRange.x : angleRange.y;
            }

            rotation.Value = Quaternion.Euler(0f,0f, eulerZ);
            EndAction(true);
        }
    }
}