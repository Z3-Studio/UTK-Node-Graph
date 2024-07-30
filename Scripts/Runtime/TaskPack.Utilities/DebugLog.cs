using UnityEngine;
using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Miscellaneous)]
    [NodeDescription("Show log")]
    public class DebugLog : ActionTask
    {
        [SerializeField] protected Parameter<string> message = "Hello World";
        [SerializeField] protected Parameter<LogType> logType = LogType.Log;

        public override string Info => $"Debug {message}";

        protected override void StartAction()
        {
            DebugLogger.Log(message.Value, this, true);
            EndAction();
        }
    }
}
