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
        [SerializeField] private ParallelUpdateMode updateMode;
        [SerializeField] private GraphData subGraph;

        public int Priority => priority;
        public ParallelExecution ParallelExecution => updateMode == ParallelUpdateMode.BeforeUpdate ? ParallelExecution.BeforeUpdate : ParallelExecution.AfterUpdate;
        public GraphData SubGraph => subGraph;
        public GraphController SubController { get; private set; }

        public override string SubInfo
        {
            get
            {
                if (!subGraph)
                    return "Empty".AddRichTextColor(Color.gray);

                return $"Run {subGraph.name.ToBold()}";
            }
        }

        public void UpdateParallel()
        {
            throw new System.NotImplementedException();
        }
    }
}
