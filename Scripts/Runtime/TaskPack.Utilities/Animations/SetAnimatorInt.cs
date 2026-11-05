using UnityEngine;
using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Animations)]
    [NodeDescription("Set Animator Int Parameter")]
    public class SetAnimatorInt : ActionTask
    {
        [ParameterDefinition(AutoBindType.AnyWithSameType)]
        [SerializeField] private Parameter<Animator> animator;

        [SerializeField] private Parameter<string> parameterName;
        [SerializeField] private Parameter<int> value;

        public override string Info => $"Set Int [{parameterName}] = {value}";

        protected override void StartAction()
        {
            animator.Value.SetInteger(parameterName.Value, value.Value);
            EndAction();
        }
    }
}
