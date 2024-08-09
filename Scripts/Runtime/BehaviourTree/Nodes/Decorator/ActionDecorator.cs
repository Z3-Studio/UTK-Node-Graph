using System.Collections.Generic;
using UnityEngine;
using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;

namespace Z3.NodeGraph.BehaviourTree
{
    // Similar than Unreal Services
    /*
    [NodeIcon(GraphIcon.ActionTask)]
    public class ActionDecorator : DecoratorNode
    {
        [SerializeField] protected bool repeatSuccess;
        [SerializeField] protected ExecutionPolicy executionPolicy;
        [SerializeField] private ActionTaskList taskList = new();

        public override string Info => "Action List";
        public override string SubInfo => taskList.GetInfo();

        protected sealed override void StartNode()
        {
            taskList.StartTaskList(executionPolicy);
        }

        protected sealed override State UpdateNode()
        {
            State state = taskList.Update();
            if (state == State.Failure)
            {
                return State.Failure;
            }

            if (state == State.Success)
            {

            }

            return child.Update();
        }

        protected override void StopNode()
        {
            taskList.StopTaskList();
        }

        protected sealed override void SetupDependencies(Dictionary<string, GraphSubAsset> subAssets)
        {
            base.SetupDependencies(subAssets);
            taskList.ReplaceDependencies(subAssets);
        }

        public override void Parse(Dictionary<string, GraphSubAsset> copies)
        {
            base.Parse(copies);
            taskList.Parse(copies);
        }
    }*/
}
