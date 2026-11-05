using UnityEngine;
using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Animations)]
    [NodeDescription("Set Animator Trigger Parameter")]
    public class SetAnimatorTrigger : ActionTask
    {
        [ParameterDefinition(AutoBindType.AnyWithSameType)]
        [SerializeField] private Parameter<Animator> animator;

        [SerializeField] private Parameter<string> parameterName;
        [SerializeField] private Parameter<bool> set;

        public override string Info => $"Set Trigger [{parameterName}]";

        protected override void StartAction()
        {
            if (set.Value)
            {
                animator.Value.SetTrigger(parameterName.Value);
            }
            else
            {
                animator.Value.ResetTrigger(parameterName.Value);
            }

            EndAction();
        }
    }
}
