using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Z3.NodeGraph.Core;
using Z3.UIBuilder.Core;

namespace Z3.NodeGraph.StateMachine
{
    [ClassStyle("any-state")]
    [NodeIcon(GraphIcon.ConditionTask)]
    public class AnyState : StateMachineNode, IOutputNode, IParallelState
    {
        [Tooltip("Bigger values execute first")]
        [SerializeField] private int priority = 1;
        [SerializeField] private ParallelUpdateMode parallelUpdateMode = ParallelUpdateMode.BeforeUpdate;
        [SerializeField] private bool canTransitionToSelf;

        [HideInGraphInspector, ReadOnly]
        [SerializeField] protected TransitionList transitions = new();

        // IOutputNode
        public TransitionList Transitions => transitions;
        public bool Startable => false;

        // IParallelState
        public ParallelExecution ParallelExecution => parallelUpdateMode == ParallelUpdateMode.BeforeUpdate ? ParallelExecution.BeforeUpdate : ParallelExecution.AfterUpdate;
        public int Priority => priority;

        private Action transitionToReset;

        public override void StartGraph()
        {
            NodeActivationTime = Time.time;
            State = State.Running;
            transitions.StartTransition();
        }

        public void UpdateParallel()
        {
            bool changeState = transitions.TryTransitionNew(GraphController, out Transition usedTransition);
            if (!changeState || canTransitionToSelf)
                return;

            List<Transition> transitionsToIgnore = new();

            // Turn off all transitions that have connection with the usedTransition while current node is active
            foreach (Transition transition in transitions.Where(t => t.Connection == usedTransition.Connection))
            {
                transition.StopTransitions();
                transitionsToIgnore.Add(transition);
            }

            // Wait change node to reactive Transions
            transitionToReset = OnReactiveTransitions;
            GraphController.OnChangeStage += transitionToReset;

            void OnReactiveTransitions()
            {
                GraphController.OnChangeStage -= transitionToReset;

                foreach (Transition transition in transitionsToIgnore)
                {
                    transition.StartTransitions();
                }
            }
        }

        public override void StopGraph()
        {
            GraphController.OnChangeStage -= transitionToReset;
            State = State.Resting;
            transitions.StopTransitions();
        }

        protected override void SetupDependencies(Dictionary<string, GraphSubAsset> instances)
        {
            transitions.ReplaceDependencies(instances);
        }

        public override void Paste(Dictionary<string, GraphSubAsset> copies)
        {
            transitions.Parse(copies);
        }
    }
}
