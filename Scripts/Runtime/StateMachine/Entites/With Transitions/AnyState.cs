using System.Collections.Generic;
using UnityEngine;
using Z3.NodeGraph.Core;
using Z3.UIBuilder.Core;

namespace Z3.NodeGraph.StateMachine
{
    [NodeIcon(GraphIcon.ConditionTask)]
    public class AnyState : StateMachineNode, IOutputNode, IParallelState
    {
        [Tooltip("Thiink about it")]
        [SerializeField] private int priority = -1;

        [HideInGraphInspector, ReadOnly]
        [SerializeField] protected TransitionList transitions = new();

        public override string ClassStyle => "any-state";

        // IOutputNode
        public TransitionList Transitions => transitions;
        public bool Startable => false;

        // IParallelState
        public ParallelExecution ParallelExecution => ParallelExecution.BeforeUpdate;
        public int Priority => priority;

        public override void StartGraph()
        {
            NodeActivationTime = Time.time;
            State = State.Running;
            transitions.StartTransition();
        }

        public void UpdateParallel()
        {
            transitions.TryTransitionNew(GraphController);
        }

        public override void StopGraph()
        {
            State = State.Resting;
            transitions.StopTransitions();
        }

        protected override void SetupDependencies(Dictionary<string, GraphSubAsset> instances)
        {
            transitions.SetupDependencies(instances);
        }
    }
}
