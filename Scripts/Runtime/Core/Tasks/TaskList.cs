using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Z3.NodeGraph.Core;
using Z3.Utils.ExtensionMethods;

namespace Z3.NodeGraph.Tasks
{
    [Serializable]
    public class TaskList<T> : ISubAssetList, IEnumerable<T> where T : Task
    {
        [SerializeField] protected List<T> taskList = new();

        public IList SubAssets => taskList;

        public virtual void StartTaskList() { }

        public virtual void StopTaskList() { }

        public void SetupDependencies(Dictionary<string, GraphSubAsset> subAssets)
        {
            List<T> newList = new List<T>();
            foreach (T child in taskList)
            {
                newList.Add(subAssets[child.Guid] as T);
            }

            taskList = newList;
        }

        public string GetInfo()
        {
            if (taskList.Count == 0)
                return "Empty".AddRichTextColor(Color.gray);

            string info = string.Empty;
            foreach (T task in taskList)
            {
                info += "\n" + task.ToString();
            }

            return info.Remove(0, 1);
        }

        IEnumerator IEnumerable.GetEnumerator() => taskList.GetEnumerator();
        public IEnumerator<T> GetEnumerator() => taskList.GetEnumerator();

        public static implicit operator List<T>(TaskList<T> taskList) => taskList.taskList;
    } 
}
