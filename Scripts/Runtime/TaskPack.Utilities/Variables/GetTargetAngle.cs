using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;
using Z3.Utils;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Variables)]
    [NodeDescription("Get angle based from center to target")]
    public class GetTargetAngle : ActionTask {

        [Header("In")]
        [SerializeField] private Parameter<Vector3> center;
        [SerializeField] private Parameter<Vector3> target;

        [Header("Out")]
        [SerializeField] private Parameter<float> angle;

        public override string Info => $"{angle} = Between {center} to {target}";

        protected override void StartAction() {
            angle.Value = MathUtils.TargetAngle(center.Value, target.Value);
            EndAction();
        }
    }
}