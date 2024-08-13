using UnityEngine;
using Z3.NodeGraph.Core;

namespace Z3.NodeGraph.BehaviourTree
{
    [NodeIcon(GraphIcon.TimeOut)]
    public class Timeout : DecoratorNode
    {
        [SerializeField] private Parameter<float> timeout;

        private float time;

        public override string SubInfo => $"Duration {timeout}";

        protected override void StartNode()
        {
            time = 0f;
        }
        protected override State UpdateNode()
        {
            time += DeltaTime;
            if (time >= NodeRunningTime)
            {
                child.Interrupt();
                return State.Failure;
            }

            return child.Update();
        }
    }
}
