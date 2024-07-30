using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Z3.NodeGraph.Core;

namespace Z3.NodeGraph.StateMachine
{
    public enum TransitionCheckType
    {
        BeforeUpdate,
        AfterUpdate,
        OnlyWhenFinished
    }

    [Serializable]
    public class TransitionList : ISubAssetList, IEnumerable<Transition>
    {
        [SerializeField] protected List<Transition> transitions = new();

        public float ActivationTime { get; private set; }
        public bool Active { get; private set; }
        public IList SubAssets => transitions;

        public TransitionList() { }

        public TransitionList(TransitionList original)
        {
            transitions = new(original.transitions);
        }

        public List<Transition> Get() => transitions;

        public IEnumerator<Transition> GetEnumerator() => transitions.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => transitions.GetEnumerator();

        public void SetupDependencies(Dictionary<string, GraphSubAsset> subAssets)
        {
            List<Transition> newList = new();
            foreach (Transition child in transitions)
            {
                newList.Add(subAssets[child.Guid] as Transition);
            }

            transitions = newList;
        }

        public void StartTransition()
        {
            Active = true;
            ActivationTime = Time.time;

            foreach (Transition transition in transitions)
            {
                transition.StartTransitions();
            }
        }

        public void StopTransitions()
        {
            foreach (Transition transition in transitions)
            {
                transition.StopTransitions();
            }

            Active = false;
        }

        public void TryTransition(StateMachineController graphController)
        {
            TryTransition(graphController, delegate { });
        }

        // TODO: Improve the arguments
        public void TryTransition(StateMachineController graphController, TransitionCheckType transitionCheckType, Func<State> state, Action callback)
        {
            if (transitionCheckType == TransitionCheckType.BeforeUpdate)
            {
                TryTransition(graphController, callback);
                return;
            }

            if (state() == State.Running)
            {
                callback();
            }

            if (transitionCheckType == TransitionCheckType.OnlyWhenFinished && state() == State.Running)
                return;

            TryTransition(graphController, delegate { });
        }

        private void TryTransition(StateMachineController graphController, Action afterFailTransition)
        {
            foreach (Transition transition in transitions)
            {
                bool sucess = transition.CheckTransitions();
                if (sucess)
                {
                    graphController.SetNextState(transition.Connection);
                    return;
                }
            }

            afterFailTransition();
        }

        public void TryTransitionNew(StateMachineController graphController)
        {
            foreach (Transition transition in transitions.Where(t => t.Connection != graphController.CurrentState))
            {
                bool sucess = transition.CheckTransitions();
                if (sucess)
                {
                    graphController.SetNextState(transition.Connection);
                    return;
                }
            }
        }

        public static implicit operator List<Transition>(TransitionList transitions) => transitions.transitions;
    }
}
