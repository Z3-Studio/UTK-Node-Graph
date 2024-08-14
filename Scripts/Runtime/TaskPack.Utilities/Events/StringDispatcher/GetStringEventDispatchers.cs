using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;

namespace Z3.NodeGraph.TaskPack.Utilities.Utils
{
    [NodeCategory(Categories.Events)]
    [NodeDescription("Converts a graph reference to a list of graph owners")]
    public class GetStringEventDispatchers : ActionTask
    {
        [Header("In")]
        [SerializeField] private Parameter<StringDispatcherReferences> dispatcherReferences;

        [Header("Out")]
        [SerializeField] private Parameter<List<StringEventDispatcher>> dispachers;

        public override string Info => $"Get String Event Dispatchers from {dispatcherReferences}";

        protected override void StartAction()
        {
            dispachers.Value = dispatcherReferences.Value.GetAllDispatcher().ToList();
            EndAction();
        }
    }
}