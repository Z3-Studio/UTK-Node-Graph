using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities.AI
{
    [NodeCategory(Categories.Transform)]
    public class GetDistance : ActionTask
    {
        [ParameterDefinition(AutoBindType.FindSimilarVariable)]
        [SerializeField] private Parameter<Transform> transform;

        [SerializeField] private Parameter<Transform> target;        
        [SerializeField] private Parameter<float> result;

        public override string Info => $"Get Distance to {target}";

        protected override void StartAction()
        {
            result.Value = Vector3.Distance(transform.Value.position, target.Value.position);
            EndAction();
        }
    }
}