using UnityEngine;
using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Miscellaneous)]
    [NodeDescription("Show log")]
    public class DebugLogCondition : ConditionTask
    {
        [SerializeField] private Parameter<string> message = "Debug Condition";
        [SerializeField] private Parameter<LogType> logType = LogType.Log;
        [SerializeField] private Parameter<LogInfo> logInfo = LogInfo.None;

        public override bool CheckCondition()
        {
            DebugLogger.Log(message.Value, this, logType.Value, logInfo.Value);
            return true;
        }
    }
}
