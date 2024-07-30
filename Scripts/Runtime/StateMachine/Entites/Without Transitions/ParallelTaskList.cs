using System.Collections.Generic;
using UnityEngine;
using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;

namespace Z3.NodeGraph.StateMachine
{
    public abstract class ParallelTaskList : StateMachineNode, IParallelState
    {
        [Tooltip("Review")]
        [SerializeField] private int priority = 0;
        [SerializeField] protected ExecutionPolicy executionPolicy;
        [SerializeField] protected ActionTaskList taskList = new();

        public int Priority => priority;
        public List<ActionTask> TaskList => taskList;

        public override string SubInfo => taskList.GetInfo();
        public abstract ParallelExecution ParallelExecution { get; }

        public override void StartGraph()
        {
            State = State.Running;
            taskList.StartTaskList(executionPolicy);
        }

        public abstract void UpdateParallel();

        public override void StopGraph()
        {
            if (State == State.Running)
            {
                taskList.StopTaskList();
            }

            State = State.Resting;
        }

        protected override void SetupDependencies(Dictionary<string, GraphSubAsset> instanceDict)
        {
            taskList.SetupDependencies(instanceDict);
        }
    }
}
