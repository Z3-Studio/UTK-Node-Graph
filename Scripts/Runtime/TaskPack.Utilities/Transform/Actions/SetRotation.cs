using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{

    [NodeCategory(Categories.Transform)]
    [NodeDescription("Set Transform.position")]
    public class SetRotation : ActionTask
    {
        [ParameterDefinition(AutoBindType.FindSimilarVariable)]
        [SerializeField] private Parameter<Transform> transform;

        [SerializeField] private Parameter<Quaternion> rotation = Quaternion.identity;

        public override string Info => $"Rotation = {rotation}";

        protected override void StartAction()
        {
            transform.Value.rotation = rotation.Value;
            EndAction();
        }
    }
}