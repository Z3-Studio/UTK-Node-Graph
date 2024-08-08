using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Transform)]
    [NodeDescription("Check the distance beetwen the agent to the target comparing the distance.")]
    public class CheckXDistance : ConditionTask
    {
        [ParameterDefinition(AutoBindType.SelfBind)]
        [SerializeField] private Parameter<Transform> data;

        [SerializeField] private Parameter<Vector3> target;
        [SerializeField] private Parameter<float> distance;
        public CompareMethod checkType = CompareMethod.LessOrEqualTo;

        public override string Info => $"{data}.X - {target}.X" + checkType.GetString() + distance;

        public override bool CheckCondition()
        {
            float xDistance = Mathf.Abs(data.Value.position.x - target.Value.x);
            return checkType.Compare(xDistance, distance.Value);
        }
    }
}