using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Z3.NodeGraph.Core
{
    [Flags]
    public enum LogInfo
    {
        None = 0,
        Frame = 1,
        Timestamp = 2,
        TaskName = 4,
    }

    public static class DebugLogger // TODO: NgLogger
    {
        private static readonly Regex TagPattern = new(@"\[.*?\]", RegexOptions.Compiled);

        public static void Log(string message, GraphSubAsset context, LogType logType = LogType.Log, LogInfo logInfo = LogInfo.None)
        {
            string nodeName = TagPattern.Replace(context.name, string.Empty);
            string finalMessage = message;
            if (logInfo.HasFlag(LogInfo.Frame))
            {
                finalMessage = $"[Frame: {Time.frameCount}] {finalMessage}";
            }
            if (logInfo.HasFlag(LogInfo.Timestamp))
            {
                finalMessage = $"[{DateTime.Now:HH:mm:ss}] {finalMessage}";
            }
            if (logInfo.HasFlag(LogInfo.TaskName))
            {
                finalMessage = $" {finalMessage}\n{nodeName}";
            }
            LogObj(finalMessage, context, logType);
        }

        public static void LogWarning(string message, GraphSubAsset context, LogInfo logInfo = LogInfo.None)
        {
            Log(message, context, LogType.Warning, logInfo);
        }

        public static void LogError(string message, GraphSubAsset context, LogInfo logInfo = LogInfo.None)
        {
            Log(message, context, LogType.Error, logInfo);
        }

        private static void LogObj(string message, GraphSubAsset context, LogType logType = LogType.Log)
        {
            Debug.unityLogger.Log(logType, (object)message, context);
        }
    }
}
