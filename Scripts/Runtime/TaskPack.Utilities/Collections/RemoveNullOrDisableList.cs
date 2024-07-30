using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Collections)]
    [NodeDescription("Useful to check if the object went to the pool or was destroyed")]
    public class RemoveNullOrDisableList<T> : ActionTask where T : Component
    {
        //[BlackboardOnly]
        public Parameter<List<T>> list;

        public override string Info => $"Remove Null or Disabled from {list}";
        
        protected override void StartAction()
        {
            List<T> auxList = new List<T>(list.Value);
            foreach (var item in auxList)
            {
                if (item == null || !item.gameObject.activeSelf)
                    list.Value.Remove(item);
            }

            EndAction(true);
        }
    }
}