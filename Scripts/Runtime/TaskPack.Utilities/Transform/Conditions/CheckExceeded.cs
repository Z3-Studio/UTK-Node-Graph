using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Transform)]
    [NodeDescription("Compare the agent passed the target + offset.")]
    public class CheckExceed : ConditionTask<Transform> 
    {
        public Parameter<Axis3> axis;
        public Parameter<Vector3> target;
        public Parameter<float> offset;

        public override string Info => offset.Value == 0 ?
            $"Exceeded {axis} {target}":
            $"Exceeded {axis} {target} {offset}";

        public override bool CheckCondition() 
        {
            Vector3 inverse = Agent.InverseTransformPoint(target.Value);
            
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