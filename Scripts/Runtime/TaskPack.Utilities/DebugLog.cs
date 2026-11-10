using UnityEngine;
using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Miscellaneous)]
    [NodeDescription("Show log")]
    public class DebugLog : ActionTask
    {
        [SerializeField] private Parameter<string> message = "Debug Action";
        [SerializeField] private Parameter<LogType> logType = LogType.Log;
        [SerializeField] private Parameter<LogInfo> logInfo = LogInfo.None;

        public override string Info => $"Debug {message}";

        protected override void StartAction()
        {
            DebugLogger.Log(message.Value, this, logType.Value, logInfo.Value);
            EndAction();
        }
    }
}
