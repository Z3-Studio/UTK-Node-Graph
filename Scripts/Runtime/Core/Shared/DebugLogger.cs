using System.Text.RegularExpressions;
using UnityEngine;

namespace Z3.NodeGraph.Core
{
    public static class DebugLogger // TODO: NgLogger
    {
        static readonly Regex TagPattern = new(@"\[.*?\]", RegexOptions.Compiled);

        public static void Log(string message, GraphSubAsset context, bool force = false, LogType logType = LogType.Log)
        {
            if (!force)
                return;

            string nodeName = TagPattern.Replace(context.name, string.Empty);
            LogObj($"{nodeName}: {message}", context, logType);
        }

        public static void LogWarning(string message, GraphSubAsset context, bool force = false)
        {
            Log(message, context, force, LogType.Warning);
        }

        public static void LogError(string message, GraphSubAsset context, bool force = false)
        {
            Log(message, context, force, LogType.Error);
        }

        public static void LogObj(string message, GraphSubAsset context, LogType logType = LogType.Log)
        {
            Debug.unityLogger.Log(logType, (object)message, context);
        }
    }
}
