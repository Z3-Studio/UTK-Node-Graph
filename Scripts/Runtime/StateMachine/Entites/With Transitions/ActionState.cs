using System.Collections.Generic;
using UnityEngine;
using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using Z3.UIBuilder.Core;

namespace Z3.NodeGraph.StateMachine
{
    [NodeIcon(GraphIcon.ActionTask)]
    public class ActionState : TransitableStateNode, IOutputNode
    {
        [SerializeField] private TransitionCheckType transitionCheck = TransitionCheckType.OnlyWhenFinished;
        [SerializeField] private bool repeat;
        [HideInGraphInspector, ReadOnly]
        [SerializeField] protected TransitionList transitions = new();
        [Space]
        [SerializeField] protected ExecutionPolicy executionPolicy;
        [SerializeField] private ActionTaskList taskList = new();

        public bool Startable => true;
        public TransitionList Transitions => transitions;

        public override string Info => "Action List";
        public override string SubInfo => taskList.GetInfo();

        public override string ClassStyle => "action";

        public override void StartState()
        {
            base.StartState();
            State = State.Running;
            transitions.StartTransition();

            taskList.StartTaskList(executionPolicy);
        }

        public override void UpdateState()
        {
            transitions.TryTransition(GraphController, transitionCheck, () => State, () => State = taskList.Update());
        }

        public override void StopState()
        {
            base.StopState();
            if (State == State.Running)
            {
                taskList.StopTaskList();
                State = State.Resting;
            }
            transitions.StopTransitions();
        }

        protected override void SetupDependencies(Dictionary<string, GraphSubAsset> instances)
        {
            transitions.SetupDependencies(instances);
            taskList.SetupDependencies(instances);
        }
    }
}