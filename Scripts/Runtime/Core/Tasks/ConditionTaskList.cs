using System;
using UnityEngine;
using Z3.Utils.ExtensionMethods;

namespace Z3.NodeGraph.Tasks
{
    [Serializable]
    public class ConditionTaskList : TaskList<ConditionTask>
    {
        public override void StartTaskList()
        {
            foreach (ConditionTask task in taskList)
            {
                task.StartCondition();
            }
        }

        public bool CheckConditions()
        {
            foreach (ConditionTask task in taskList)
            {
                bool successful = task.EvaluateCondition();
                if (!successful)
                    return false;
            }

            return true;
        }

        public override void StopTaskList()
        {
            foreach (ConditionTask task in taskList)
            {
                task.StopCondition();
            }
        }

        public string GetLabel()
        {
            if (taskList.Count == 0)
                return "Finish";

            string text = string.Empty;
            string and = " && ".AddRichTextColor(Color.magenta);

            foreach (ConditionTask task in taskList)
            {
                if (!task)
                {
                    text += "Missing".AddRichTextColor(Color.red);
                }
                else if (task.InvertCondition)
                {
                    text += $"!({task})";
                }
                else
                {
                    text += task.ToString();
                }

                text += and;
            }

            // Remove last "and"
            text = text[..^and.Length];

            return text;
        }
    }
}
