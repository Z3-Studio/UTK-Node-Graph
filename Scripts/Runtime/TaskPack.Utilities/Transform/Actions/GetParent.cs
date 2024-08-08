using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Transform)]
    [NodeDescription("Return the parent's transform from the Agent.")]
    public class GetParent : ActionTask
    {
        [ParameterDefinition(AutoBindType.SelfBind)]
        [SerializeField] private Parameter<Transform> data;

        [SerializeField] private Parameter<Transform> returnParent;

        protected override void StartAction()
        {
            returnParent.Value = data.Value.parent;
            EndAction();
        }
    }
}