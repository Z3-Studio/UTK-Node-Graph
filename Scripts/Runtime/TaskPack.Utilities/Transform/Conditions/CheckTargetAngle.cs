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
        [SerializeField] private Parameter<Transform> transform;

        [SerializeField] private Parameter<Vector3> target;
        [Slider(0f, 180f)]
        [SerializeField] private Parameter<float> angle;
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