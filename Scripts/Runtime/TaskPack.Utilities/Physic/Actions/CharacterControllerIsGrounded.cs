using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Physics)]
    [NodeDescription("Check if Character Controller is Grounded")]
    public class CharacterControllerIsGrounded : ConditionTask
    {
        [ParameterDefinition(AutoBindType.SelfBind)]
        [SerializeField] private Parameter<CharacterController> characterController;

        public override string Info => $"{characterController}.isGrounded";

        public override bool CheckCondition()
        {
            return characterController.Value.isGrounded;
        }
    }
}