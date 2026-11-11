using System;
using System.Collections.Generic;
using System.Linq;
using Z3.NodeGraph.Core;

namespace Z3.NodeGraph.StateMachine
{
    public class StateMachineController : GraphController<StateMachineData>
    {
        public event Action OnChangeStage;

        public TransitableStateNode CurrentState { get; private set; }

        private readonly List<IParallelState> enterStates = new();
        private readonly List<IParallelState> beforeUpdateState = new();
        private readonly List<IParallelState> afterUpdateState = new();
        private readonly List<IParallelState> exitStates = new();
        private readonly List<StateMachineNode> nodes = new();

        private State state = State.Ready;
        public bool ChangedStateThisFrame { get; private set; } // Used to avoid multiple any state transite

        private bool stopped;

        public StateMachineController(IGraphRunner runner, StateMachineData data) : base(runner, data)
        {
            nodes = GraphData.SubAssets.OfType<StateMachineNode>().ToList();

            IEnumerable<IGrouping<ParallelExecution, IParallelState>> groups = nodes.OfType<IParallelState>()
                .OrderByDescending(a => a.Priority)
                .GroupBy(a => a.ParallelExecution);

            foreach (IGrouping<ParallelExecution, IParallelState> group in groups)
            {
                switch (group.Key)
                {
                    case ParallelExecution.Enter:
                        enterStates = group.ToList();
                        break;
                    case ParallelExecution.BeforeUpdate:
                        beforeUpdateState = group.ToList();
                        break;
                    case ParallelExecution.AfterUpdate:
                        afterUpdateState = group.ToList();
                        break;
                    case ParallelExecution.Exit:
                        exitStates = group.ToList();
                        break;
                }
            }
        }

        public override void StartGraph()
        {
            DoInAll(n => n.StartGraph());

            state = State.Running;
            stopped = false;

            CurrentState = GraphData.StartNode as TransitableStateNode;
            CurrentState.StartState();
        }

        public override void StopGraph() => StopStates();

        public void SetNextState(TransitableStateNode nextState)
        {
            CurrentState.StopState();
            CurrentState = nextState;
            OnChangeStage?.Invoke(); // Used in AnyState
            ChangedStateThisFrame = true;
            CurrentState.StartState();
        }

        public void ExecutionEnd(State newState) // Should be Success or Failure
        {
            state = newState;
        }

        public override State OnUpdate()
        {
            if (state is State.Failure or State.Success)
                return state;

            ChangedStateThisFrame = false;
            ExecuteParallel(enterStates);
            ExecuteParallel(beforeUpdateState);

            CurrentState.UpdateState();

            if (state == State.Running)
            {
                ExecuteParallel(afterUpdateState);
            }
            else
            {
                StopStates();
            }

            return state;
        }

        private void StopStates()
        {
            if (stopped)
                return;

            stopped = true;

            if (CurrentState.Active)
            {
                CurrentState.StopState();
            }

            ExecuteParallel(exitStates);
            DoInAll(n => n.StopGraph());
        }

        private void ExecuteParallel(List<IParallelState> parallelNodes)
        {
            parallelNodes.ForEach(n => n.UpdateParallel());
        }

        private void DoInAll(Action<StateMachineNode> action)
        {
            nodes.ForEach(action);
        }
    }
}
