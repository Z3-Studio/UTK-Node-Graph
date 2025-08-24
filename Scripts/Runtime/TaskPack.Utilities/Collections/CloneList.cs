using System;
using System.Collections;
using UnityEngine;
using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Operations)]
    [NodeDescription("Clone List value to a new reference")]
    public class CloneList : ActionTask
    {
        [SerializeField] private Parameter<IList> list;
        [SerializeField] private Parameter<IList> cloneList;

        public override string Info => $"Clone {cloneList} = {list}";

        protected override void StartAction()
        {
            Type srcType = list.Value.GetType();
            IList newInstance = (IList)Activator.CreateInstance(srcType);

            foreach (object item in list.Value)
            {
                object copy = item;
                newInstance.Add(copy);
            }

            cloneList.Value = newInstance;
            EndAction();
        }
    }
}
