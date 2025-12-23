using UnityEngine;
using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using Z3.UIBuilder.Core;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Transform)]
    [NodeDescription("Compare the Angle in the selected Axis")]
    public class CheckDirectionAxisAngle : ConditionTask
    {
        [ParameterDefinition(AutoBindType.FindSimilarVariable)]
        [SerializeField] private Parameter<Transform> transform;

        [SerializeField] private Parameter<Vector3> target;
        [SerializeField] private Parameter<Axis3Flags> axis = Axis3Flags.X | Axis3Flags.Z;
        [Slider(0f, 180f)]
        [SerializeField] private Parameter<float> angle;
        [SerializeField] private CompareMethod checkType = CompareMethod.LessThan;

        public override string InfoC => $"{target} Angle {checkType.GetString()} {angle}";

        public override bool CheckCondition()
        {
            Vector3 directionToCheck = (target.Value - transform.Value.position).normalized;

            directionToCheck = axis.Value.Reset(directionToCheck);
            Vector3 from = axis.Value.Reset(transform.Value.forward);

            float targetAngle = Vector3.Angle(from, directionToCheck);
            return checkType.Compare(targetAngle, angle.Value);
        }
    }
}
