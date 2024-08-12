using System.Collections.Generic;
using UnityEngine;
using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;

namespace Z3.NodeGraph.BehaviourTree
{
    [NodeIcon(GraphIcon.WaitUntil)]
    public class WaitUntilDecorator : DecoratorNode
    {
        [SerializeField] private ConditionTaskList conditions;

        private bool running;

        public override string Info => "Wait Until";

        public override string SubInfo => conditions.GetInfo();

        protected override void StartNode()
        {
            running = false;
            conditions.StartTaskList();
        }

        protected override State UpdateNode()
        {
            if (!running)
            {
                if (!conditions.CheckConditions())
                {
                    return State.Running;
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
            conditions.ReplaceDependencies(subAssets);
        }

        public override void Paste(Dictionary<string, GraphSubAsset> copies)
        {
            base.Paste(copies);
            conditions.Parse(copies);
        }
    }
}
