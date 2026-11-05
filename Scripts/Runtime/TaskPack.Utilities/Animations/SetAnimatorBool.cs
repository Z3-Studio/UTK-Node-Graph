using UnityEngine;
using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Animations)]
    [NodeDescription("Set Animator Bool Parameter")]
    public class SetAnimatorBool : ActionTask
    {
        [ParameterDefinition(AutoBindType.AnyWithSameType)]
        [SerializeField] private Parameter<Animator> animator;

        [SerializeField] private Parameter<string> parameterName;
        [SerializeField] private Parameter<bool> value;

        public override string Info => $"Set Bool [{parameterName}] = {value}";

        protected override void StartAction()
        {
            animator.Value.SetBool(parameterName.Value, value.Value);
            EndAction();
        }
    }
}
