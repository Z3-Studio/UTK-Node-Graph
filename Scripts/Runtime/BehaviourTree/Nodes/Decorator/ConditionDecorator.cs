using System.Collections.Generic;
using UnityEngine;
using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;

namespace Z3.NodeGraph.BehaviourTree
{
    public enum CheckType
    {
        OnlyFirst,
        Always
    }

    [NodeIcon(GraphIcon.ConditionTask)]
    public class ConditionDecorator : DecoratorNode
    {
        [SerializeField] private CheckType checkType;
        [SerializeField] private ConditionTaskList conditions = new();

        public override string Info => "Condition";
        public override string SubInfo => conditions.GetInfo();

        private bool running;

        protected override void StartNode()
        {
            running = false;
            conditions.StartTaskList();
        }

        protected override State UpdateNode()
        {
            if (!running || checkType == CheckType.Always)
            {
                if (!conditions.CheckConditions())
                {
                    child.Interrupt();
                    return State.Failure;
                }

                running = true;
            }

            return child.Update();
        }

        protected override void StopNode()
        {
            conditions.StopTaskList();
        }

        protected override void SetupDependencies(Dictionary<string, GraphSubAsset> subAssets)
        {
            base.SetupDependencies(subAssets);

            conditions.SetupDependencies(subAssets);
        }
    }
}
