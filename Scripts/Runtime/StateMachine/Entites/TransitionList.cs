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
                bool success = transition.CheckTransitions();
                if (success)
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
                bool success = transition.CheckTransitions();
                if (success)
                {
                    graphController.SetNextState(transition.Connection);
                    return;
                }
            }
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
