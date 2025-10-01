using UnityEngine;
using Z3.NodeGraph.Core;
using Z3.UIBuilder.Core;
using Z3.Utils.ExtensionMethods;

namespace Z3.NodeGraph.StateMachine
{
    [NodeIcon(GraphIcon.SubGraph)]
    public class ParallelSubGraph : StateMachineNode, ISubGraphNode, IParallelState
    {
        [DesignOnly]
        [SerializeField] private int priority;
        [SerializeField] private bool autoRestart = true;
        [SerializeField] private ParallelUpdateMode updateMode;
        [SerializeField] private GraphData subGraph;

        public int Priority => priority;
        public ParallelExecution ParallelExecution => updateMode == ParallelUpdateMode.BeforeUpdate ? ParallelExecution.BeforeUpdate : ParallelExecution.AfterUpdate;
        public GraphData SubGraph => subGraph;
        public GraphController SubController => subController ??= GraphController.BuildSubGraph(subGraph);
        private GraphController subController;

        public override string SubInfo
        {
            get
            {
                if (!subGraph)
                    return "Empty".AddRichTextColor(Color.gray);

                return $"Run {subGraph.name.ToBold()}";
            }
        }

        public override void StartGraph()
        {
            State = State.Running;

            subController ??= GraphController.BuildSubGraph(subGraph);
            subController.StartGraph();
        }

        public void UpdateParallel()
        {
            if (State != State.Running)
                return;

            State newState = subController.OnUpdate();
            if (newState != State.Running && autoRestart)
            {
                subController.StopGraph();
                subController.StartGraph();
            }
            else
            {
                State = newState;
            }
        }

        public override void StopGraph()
        {
            subController.StopGraph();

            if (State == State.Running)
            {
                State = State.Resting;
            }
        }
    }
}
