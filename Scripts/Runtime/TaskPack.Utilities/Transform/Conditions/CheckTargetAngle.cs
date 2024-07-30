using UnityEngine;
using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using Z3.UIBuilder.Core;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Transform)]
    [NodeDescription("TODO")]
    public class CheckTargetAngle : ConditionTask
    {
        [ParameterDefinition(AutoBindType.SelfBind)]
        public Parameter<Transform> transform;

        public Parameter<Vector3> target;
        [Slider(0f, 180f)]
        public Parameter<float> angle;
        public CompareMethod checkType = CompareMethod.LessThan;

        public override string Info => $"{target} Angle {checkType.GetString()} {angle}";

        public override bool CheckCondition()
        {
            Vector3 directionToCheck = target.Value - transform.Value.position;
            float targetAngle = Vector3.Angle(transform.Value.forward, directionToCheck);
            return checkType.Compare(targetAngle, angle.Value);
        }
    }
}