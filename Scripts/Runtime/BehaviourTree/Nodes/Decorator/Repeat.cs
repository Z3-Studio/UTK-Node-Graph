using System;
using UnityEngine;
using Z3.NodeGraph.Core;
using Z3.Utils.ExtensionMethods;

namespace Z3.NodeGraph.BehaviourTree
{
    public enum RepeaterMode
    {
        Forever,
        Until,
        Times
    }

    [NodeIcon(GraphIcon.Repeater)]
    public class Repeat : DecoratorNode
    {
        [SerializeField] private RepeaterMode repeaterMode;
        [SerializeField] private StateResult stateResult;
        [SerializeField] private Parameter<int> times;

        public override string SubInfo => $"Mode: {repeaterMode.ToStringBold()}";

        private Func<State> update;

        protected override void StartNode()
        {
            update = repeaterMode switch
            {
                RepeaterMode.Forever => ForeverUpdate,
                RepeaterMode.Until => throw new NotImplementedException(),
                RepeaterMode.Times => throw new NotImplementedException(),
                _ => throw new NotImplementedException(),
            };
        }

        private State ForeverUpdate()
        {
            child.Update();
            return State.Running;
        }

        protected override State UpdateNode() => update();
    }
}
