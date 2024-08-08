using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Physics)]
    [NodeDescription("Get Character Controller Velocity")]
    public class GetCharacterControllerVelocity : ActionTask
    {
        [ParameterDefinition(AutoBindType.SelfBind)]
        [SerializeField] private Parameter<CharacterController> characterController;
        [SerializeField] private Parameter<Vector3> velocity;

        public override string Info => $"Get {characterController} Velocity";

        protected override void StartAction()
        {
            velocity.Value = characterController.Value.velocity;
            EndAction();
        }
    }
}