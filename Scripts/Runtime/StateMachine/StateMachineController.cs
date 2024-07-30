using System;
using System.Collections.Generic;
using System.Linq;
using Z3.NodeGraph.Core;

namespace Z3.NodeGraph.StateMachine
{
    public class StateMachineController : GraphController<StateMachineData>
    {
        private readonly List<IParallelState> enterStates = new();
        private readonly List<IParallelState> beforeUpdateState = new();
        private readonly List<IParallelState> afterUpdateState = new();
        private readonly List<IParallelState> exitStates = new();
        private readonly List<StateMachineNode> nodes = new();

        public TransitableStateNode CurrentState { get; private set; }
        private State state = State.Ready;

        public StateMachineController(IGraphRunner runner, StateMachineData data) : base(runner, data)
        {
            nodes = GraphData.SubAssets.OfType<StateMachineNode>().ToList();

            IEnumerable<IGrouping<ParallelExecution, IParallelState>> groups = nodes.OfType<IParallelState>()
                .OrderBy(a => a.Priority)
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

            CurrentState = GraphData.StartNode as TransitableStateNode;
            CurrentState.StartState();
        }

        public override void StopGraph()
        {
            if (CurrentState != null && CurrentState.Active)
            {
                CurrentState.StopState();
            }

            ExecuteParallel(exitStates);

            DoInAll(n => n.StopGraph());
        }

        public void SetNextState(TransitableStateNode nextState)
        {
            CurrentState.StopState();
            CurrentState = nextState;
            CurrentState.StartState();
        }

        public void ExecutionEnd(bool success)
        {
            state = success ? State.Success : State.Failure;
        }

        public override State OnUpdate()
        {
            ExecuteParallel(enterStates);
            ExecuteParallel(beforeUpdateState);

            CurrentState.UpdateState();

            if (state == State.Running)
            {
                ExecuteParallel(afterUpdateState);
            }
            else
            {
                CurrentState.StopState();
                CurrentState = null;
            }

            return state;
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
