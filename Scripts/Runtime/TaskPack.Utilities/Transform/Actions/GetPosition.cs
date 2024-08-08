using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Transform)]
    [NodeDescription("Get the transform.position.")]
    public class GetPosition : ActionTask
    {
        [Header("In")]
        [ParameterDefinition(AutoBindType.SelfBind)]
        [SerializeField] private Parameter<Transform> transform;

        [Header("Out")]
        [SerializeField] private Parameter<Vector3> position;

        public override string Info => $"Get {transform} Position";

        protected override void StartAction() 
        {
            position.Value = transform.Value.position;
            EndAction();
        }
    }
}