using System.Text.RegularExpressions;
using UnityEngine;

namespace Z3.NodeGraph.Core
{
    public static class DebugLogger
    {
        public static void Log(string message, GraphSubAsset context, bool force = false)
        {
            if (!force)
                return;

            string pattern = @"\[.*?\]";
            string nodeName = context.name;
            if (Regex.IsMatch(nodeName, pattern))
            {
                nodeName = Regex.Replace(context.name, pattern, string.Empty);
            }

            LogObj($"{nodeName}: {message}", context);
        }

        public static void LogObj(string message, GraphSubAsset context)
        {
            Debug.Log(message, context);
        }
    }
}
