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
    public class TransitionList : ISubAssetList<Transition>
    {
        [SerializeField] protected List<Transition> transitions = new();

        public float ActivationTime { get; private set; }
        public bool Active { get; private set; }

        // IList implementation
        bool IList.IsFixedSize => ((IList)transitions).IsFixedSize;
        bool ICollection.IsSynchronized => ((ICollection)transitions).IsSynchronized;
        object ICollection.SyncRoot => ((ICollection)transitions).SyncRoot;
        bool IList.IsReadOnly => ((IList)transitions).IsReadOnly;
        int ICollection.Count => transitions.Count;
        object IList.this[int index]
        {
            get => ((IList)transitions)[index];
            set => ((IList)transitions)[index] = value;
        }

        // IList<T> implementation
        int ICollection<Transition>.Count => transitions.Count;
        bool ICollection<Transition>.IsReadOnly => ((ICollection<Transition>)transitions).IsReadOnly;
        Transition IList<Transition>.this[int index]
        {
            get => transitions[index];
            set => transitions[index] = value;
        }

        // TransitionList implementation
        public TransitionList() { }

        public List<Transition> Get() => transitions;

        private Action onProcessTransitions;

        public void StartTransition()
        {
            Active = true;
            ActivationTime = Time.time;

            foreach (Transition transition in transitions)
            {
                transition.StartTransitions();
            }
        }

        public void StartTransitionProcessor(StateMachineController graphController, TransitionCheckType checkType, Func<State> getState, Action runningCallback, bool repeat, Action repeatCallback)
        {
            switch (checkType)
            {
                case TransitionCheckType.BeforeUpdate:
                    StartTransition();
                    onProcessTransitions = () => CheckBeforeUpdate(graphController, getState, runningCallback, repeat, repeatCallback);
                    break;

                case TransitionCheckType.AfterUpdate:
                    StartTransition();
                    onProcessTransitions = () => CheckAfterUpdate(graphController, getState, runningCallback, repeat, repeatCallback);
                    break;

                case TransitionCheckType.OnlyWhenFinished:
                    onProcessTransitions = () => CheckOnlyWhenFinished(graphController, getState, runningCallback, repeat, repeatCallback);
                    break;
            }
        }

        public void StopTransitions()
        {
            if (!Active)
                return;

            Active = false;

            foreach (Transition transition in transitions)
            {
                transition.StopTransitions();
            }
        }

        public void TryTransition(StateMachineController graphController)
        {
            TryTransition(graphController, State.Success);
        }

        public void ProcessTransitions() => onProcessTransitions();

        private void CheckBeforeUpdate(StateMachineController graphController, Func<State> getState, Action runningCallback, bool repeat, Action repeatCallback)
        {
            State state = getState();
            if (TryTransition(graphController, state))
                return;

            if (state != State.Running)
            {
                if (!repeat)
                    return;

                repeatCallback();
                // Note: Should I read the state again after the repeat callback?
            }

            runningCallback();
        }

        private void CheckAfterUpdate(StateMachineController graphController, Func<State> getState, Action runningCallback, bool repeat, Action repeatCallback)
        {
            State state = getState();
            if (state == State.Running)
            {
                runningCallback();
                state = getState();
            }

            if (TryTransition(graphController, state))
                return;

            if (repeat && state != State.Running)
            {
                repeatCallback();
            }
        }

        private void CheckOnlyWhenFinished(StateMachineController graphController, Func<State> getState, Action runningCallback, bool repeat, Action repeatCallback)
        {
            State state = getState();
            if (state == State.Running)
            {
                runningCallback();
                state = getState();
                if (state == State.Running)
                    return;
            }

            if (!Active)
            {
                StartTransition();
            }

            bool transitioned = TryTransition(graphController, state);

            if (repeat && !transitioned)
            {
                // Closes after a single try
                StopTransitions();
                repeatCallback();
            }
        }

        private bool TryTransition(StateMachineController graphController, State state)
        {
            foreach (Transition transition in transitions)
            {
                bool success = transition.CheckTransitions(state);
                if (success)
                {
                    graphController.SetNextState(transition.Connection);
                    return true;
                }
            }

            return false;
        }

        public bool TryTransitionNew(StateMachineController graphController, out Transition transitionUsed)
        {
            foreach (Transition transition in transitions.Where(t => t.State != State.Resting))
            {
                bool success = transition.CheckTransitions(State.Success);
                if (success)
                {
                    graphController.SetNextState(transition.Connection);
                    transitionUsed = transition;
                    return true;
                }
            }

            transitionUsed = null;
            return false;
        }

        // IList implementation
        int IList.Add(object value) => ((IList)transitions).Add(value);
        void IList.Clear() => transitions.Clear();
        bool IList.Contains(object value) => transitions.Contains((Transition)value);
        int IList.IndexOf(object value) => transitions.IndexOf((Transition)value);
        void IList.Insert(int index, object value) => transitions.Insert(index, (Transition)value);
        void IList.Remove(object value) => transitions.Remove((Transition)value);
        void IList.RemoveAt(int index) => transitions.RemoveAt(index);
        void ICollection.CopyTo(Array array, int index) => ((ICollection)transitions).CopyTo(array, index);
        IEnumerator IEnumerable.GetEnumerator() => transitions.GetEnumerator();

        // IList<T> implementation
        int IList<Transition>.IndexOf(Transition item) => transitions.IndexOf(item);
        void IList<Transition>.Insert(int index, Transition item) => transitions.Insert(index, item);
        void IList<Transition>.RemoveAt(int index) => transitions.RemoveAt(index);
        void ICollection<Transition>.Add(Transition item) => transitions.Add(item);
        void ICollection<Transition>.Clear() => transitions.Clear();
        bool ICollection<Transition>.Contains(Transition item) => transitions.Contains(item);
        void ICollection<Transition>.CopyTo(Transition[] array, int arrayIndex) => transitions.CopyTo(array, arrayIndex);
        bool ICollection<Transition>.Remove(Transition item) => transitions.Remove(item);
        IEnumerator<Transition> IEnumerable<Transition>.GetEnumerator() => transitions.GetEnumerator();

        // Implicit conversion operator
        public static implicit operator List<Transition>(TransitionList transitions) => transitions.transitions;
    }
}
