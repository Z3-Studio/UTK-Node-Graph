using System;
using System.Collections.Generic;
using UnityEngine;
using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using Z3.UIBuilder.Core;

namespace Z3.NodeGraph.StateMachine
{
    [Flags]
    public enum AllowedTransitionStates
    {
        Failure = 1,
        Success = 2,
        Running = 4,
    }

    public class Transition : GraphSubAsset
    {
        [SerializeField] private AllowedTransitionStates allowedTransitionStates = (AllowedTransitionStates)7;
        [HideInGraphInspector, ReadOnly]
        [SerializeField] private TransitableStateNode connection;
        [SerializeField] private ConditionTaskList conditions = new();

        public TransitableStateNode Connection => connection;
        public State State { get; private set; }

        public override string Info => !string.IsNullOrEmpty(title) ? title : conditions.GetLabel();

        public void Setup(TransitableStateNode child)
        {
            connection = child;
        }

        public void StartTransitions()
        {
            conditions.StartTaskList(); 
            State = State.Running;
        }

        public bool CheckTransitions(State state)
        {
            switch (state)
            {
                case State.Failure:
                    if (!allowedTransitionStates.HasFlag(AllowedTransitionStates.Failure))
                        return false;
                    break;

                case State.Success:
                    if (!allowedTransitionStates.HasFlag(AllowedTransitionStates.Success))
                        return false;
                    break;

                default:
                    if (!allowedTransitionStates.HasFlag(AllowedTransitionStates.Running))
                        return false;
                    break;
            }

            bool result = conditions.CheckConditions();

            #if UNITY_EDITOR
            State = result ? State.Success : State.Failure;
            #endif

            return result;
        }

        public void StopTransitions()
        {
            State = State.Resting;
            conditions.StopTaskList();
        }

        protected override void SetupDependencies(Dictionary<string, GraphSubAsset> instances)
        {
            connection = instances[connection.Guid] as TransitableStateNode;
            conditions.ReplaceDependencies(instances);
        }

        public override void ValidatePaste(List<string> itemsToCopy)
        {
            if (!itemsToCopy.Contains(connection.Guid))
            {
                itemsToCopy.Remove(Guid);

                foreach (ConditionTask condition in conditions)
                {
                    itemsToCopy.Remove(condition.Guid);
                }
            }
        }

        public override void Paste(Dictionary<string, GraphSubAsset> copies)
        {
            connection = copies.GetValueOrDefault(connection.Guid) as TransitableStateNode;

            if (connection == null && conditions.Count > 0)
                throw new System.InvalidOperationException("Connection is null, but conditions are present.");

            conditions.Parse(copies);
        }
    }
}
