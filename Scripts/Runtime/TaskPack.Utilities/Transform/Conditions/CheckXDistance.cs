using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Transform)]
    [NodeDescription("Check the distance beetwen the agent to the target comparing the distance.")]
    public class CheckXDistance : ConditionTask
    {
        [ParameterDefinition(AutoBindType.FindSimilarVariable)]
        [SerializeField] private Parameter<Transform> transform;

        [SerializeField] private Parameter<Vector3> target;
        [SerializeField] private Parameter<float> distance;
        public CompareMethod checkType = CompareMethod.LessOrEqualTo;

        public override string Info => $"{transform}.X - {target}.X" + checkType.GetString() + distance;

        public override bool CheckCondition()
        {
            float xDistance = Mathf.Abs(transform.Value.position.x - target.Value.x);
            return checkType.Compare(xDistance, distance.Value);
        }
    }
}