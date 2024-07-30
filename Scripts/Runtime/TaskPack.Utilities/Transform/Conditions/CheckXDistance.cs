using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Transform)]
    [NodeDescription("Check the distance beetwen the agent to the target comparing the distance.")]
    public class CheckXDistance : ConditionTask<Transform> {

        public Parameter<Vector3> target;
        public Parameter<float> distance;
        public CompareMethod checkType = CompareMethod.LessOrEqualTo;

        public override string Info => $"{AgentInfo}.X - {target}.X" + checkType.GetString() + distance;

        public override bool CheckCondition() {
            float xDistance = Mathf.Abs(Agent.position.x - target.Value.x);
            return checkType.Compare(xDistance, distance.Value);
        }
    }
}