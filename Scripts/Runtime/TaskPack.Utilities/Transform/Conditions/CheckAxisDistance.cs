using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Transform)]
    [NodeDescription("Check the distance beetwen the reference to the target comparing the selected axis.")]
    public class CheckAxisDistance : ConditionTask
    {
        public Parameter<Vector3> reference;
        public Parameter<Vector3> target;
        public Parameter<float> distance;
        public Parameter<Axis3Flags> axis;
        public CompareMethod checkType = CompareMethod.LessOrEqualTo;

        public override string Info
        {
            get
            {
                return $"{axis} Distance {reference} to {target}" + checkType.GetString() + distance;
            }
        }

        public override bool CheckCondition()
        {
            float axisDistance = Distance(axis.Value, reference.Value, target.Value);
            return checkType.Compare(axisDistance, distance.Value);
        }

        public static float Distance(Axis3Flags axis, Vector3 a, Vector3 b)
        {
            float squaredDifference = 0;

            if (axis.HasFlag(Axis3Flags.X))
            {
                float aux = a.x - b.x;
                squaredDifference += aux * aux;
            }
            if (axis.HasFlag(Axis3Flags.Y))
            {
                float aux = a.y - b.y;
                squaredDifference += aux * aux;

            }
            if (axis.HasFlag(Axis3Flags.Z))
            {
                float aux = a.z - b.z;
                squaredDifference += aux * aux;
            }

            return Mathf.Sqrt(squaredDifference);
        }
    }
}