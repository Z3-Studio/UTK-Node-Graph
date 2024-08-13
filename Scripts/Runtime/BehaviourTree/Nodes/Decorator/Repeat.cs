using System;
using UnityEngine;
using Z3.NodeGraph.Core;
using Z3.Utils.ExtensionMethods;

namespace Z3.NodeGraph.BehaviourTree
{
    public enum RepeaterMode
    {
        Forever,
        Times,
        Until,
    }

    [NodeIcon(GraphIcon.Repeater)]
    public class Repeat : DecoratorNode
    {
        [Header("Repeat")]
        [SerializeField] private RepeaterMode repeaterMode;

        [Header("- Times Mode")]
        [SerializeField] private Parameter<int> times;

        [Header("- Until Mode")]
        [SerializeField] private StateResult untilResult;

        public override string SubInfo => $"Mode: {repeaterMode.ToStringBold()}";

        private Func<State> update;
        private int finishCounter;

        protected override void StartNode()
        {
            finishCounter = 0;

            update = repeaterMode switch
            {
                RepeaterMode.Forever => RepeatForever,
                RepeaterMode.Times => RepeatTimes,
                RepeaterMode.Until => RepeatUntil,
                _ => throw new NotImplementedException(),
            };
        }

        private State RepeatForever()
        {
            child.Update();
            return State.Running;
        }

        private State RepeatTimes()
        {
            State result = child.Update();
            if (result is State.Success or State.Failure)
            {
                finishCounter++;
                if (finishCounter > times.Value)
                {
                    return result;
                }
            }

            return State.Running;
        }

        private State RepeatUntil()
        {
            State state = child.Update();

            if (untilResult == StateResult.Success && state == State.Success)
                return State.Success;

            if (untilResult == StateResult.Failure && state == State.Failure)
                return State.Failure;

            return State.Running;
        }

        protected override State UpdateNode() => update();
    }
}
