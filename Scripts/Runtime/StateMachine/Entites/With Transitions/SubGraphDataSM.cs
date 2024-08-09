using System.Collections.Generic;
using UnityEngine;
using Z3.NodeGraph.Core;
using Z3.UIBuilder.Core;
using Z3.Utils.ExtensionMethods;

namespace Z3.NodeGraph.StateMachine
{
    [ClassStyle("sub-graph")]
    [NodeIcon(GraphIcon.SubGraph)]
    public class SubGraphDataSM : TransitableStateNode, ISubGraphNode, IOutputNode
    {
        [SerializeField] private TransitionCheckType transitionCheck = TransitionCheckType.OnlyWhenFinished;
        [SerializeField] private bool repeat;
        [HideInGraphInspector, ReadOnly]
        [SerializeField] protected TransitionList transitions = new();

        [SerializeField] private GraphData subGraph;

        public override string Info => "Sub Graph";

        public override string SubInfo
        {
            get
            {
                if (!subGraph)
                    return "Empty".AddRichTextColor(Color.gray);

                return $"Run {subGraph.name.ToBold()}";
            }
        }

        public bool Startable => true;

        public GraphData SubGraph => subGraph;
        public GraphController SubController => subController ??= GraphController.BuildSubGraph(subGraph);
        public TransitionList Transitions => transitions;

        private GraphController subController;

        public override void StartState()
        {
            base.StartState();

            State = State.Running;

            transitions.StartTransition();

            subController ??= GraphController.BuildSubGraph(subGraph);
            subController.StartGraph();
        }

        public override void UpdateState()
        {
            transitions.TryTransition(GraphController, transitionCheck, () => State, () => State = subController.OnUpdate());
        }

        public override void StopState()
        {
            base.StopState();
            transitions.StopTransitions();
            subController.StopGraph();

            if (State == State.Running)
            {
                State = State.Resting;
            }
        }

        protected override void SetupDependencies(Dictionary<string, GraphSubAsset> instances)
        {
            transitions.ReplaceDependencies(instances);
        }

        public override void Parse(Dictionary<string, GraphSubAsset> copies)
        {
            transitions.Parse(copies);
        }
    }
}
