using System;
using System.Collections.Generic;

namespace Z3.NodeGraph.StateMachine
{ 
    public static class StateMachineExtensions
    {
        public static List<Transition> GetTransitionsSafe(this StateMachineNode node)
        {
            return node.GetTransitionListSafe();
        }

        public static List<Transition> GetTransitions(this StateMachineNode node)
        {
            return node.GetTransitionList();
        }

        public static List<Transition> GetTransitionListSafe(this StateMachineNode node)
        {
            if (node is IOutputNode output)
                return output.Transitions;

            return new();
        }

        public static TransitionList GetTransitionList(this StateMachineNode node)
        {
            if (node is IOutputNode output)
                return output.Transitions;

            throw new InvalidCastException();
        }

        public static bool HasTransition(this StateMachineNode node)
        {
            return node is IOutputNode;
        }
    }
}
