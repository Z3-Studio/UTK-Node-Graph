using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Transform)]
    [NodeDescription("Return the parent's transform from the Agent.")]
    public class GetParent : ActionTask
    {
        [ParameterDefinition(AutoBindType.FindSimilarVariable)]
        [SerializeField] private Parameter<Transform> transform;

        [SerializeField] private Parameter<Transform> returnParent;

        protected override void StartAction()
        {
            returnParent.Value = transform.Value.parent;
            EndAction();
        }
    }
}