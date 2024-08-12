﻿using System.Collections.Generic;
using UnityEngine;
using Z3.NodeGraph.Core;
using Z3.UIBuilder.Core;

namespace Z3.NodeGraph.StateMachine
{
    [ClassStyle("pass")]
    [NodeIcon(GraphIcon.Sequencer)]
    public class Pass : TransitableStateNode, IOutputNode
    {
        [HideInGraphInspector, ReadOnly]
        [SerializeField] protected TransitionList transitions = new();

        public bool Startable => true;

        public TransitionList Transitions => transitions;

        public override void StartState()
        {
            base.StartState();
            transitions.StartTransition();
        }

        public override void UpdateState()
        {
            transitions.TryTransition(GraphController);
        }

        public override void StopState()
        {
            base.StopState();
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
