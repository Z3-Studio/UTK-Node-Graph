using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Transform)]
    [NodeDescription("Compare the agent passed the target + offset.")]
    public class CheckExceed : ConditionTask
    {
        [ParameterDefinition(AutoBindType.SelfBind)]
        [SerializeField] private Parameter<Transform> data;
        [SerializeField] private Parameter<Axis3> axis;
        [SerializeField] private Parameter<Vector3> target;
        [SerializeField] private Parameter<float> offset;

        public override string Info => offset.Value == 0 ?
            $"Exceeded {axis} {target}":
            $"Exceeded {axis} {target} {offset}";

        public override bool CheckCondition() 
        {
            Vector3 inverse = data.Value.InverseTransformPoint(target.Value);
            
            return offset.Value > axis.Value switch
            {
                Axis3.X => inverse.x,
                Axis3.Y => inverse.y,
                Axis3.Z => inverse.z,
                _ => throw new System.NotImplementedException(),
            };
        }
    }
}