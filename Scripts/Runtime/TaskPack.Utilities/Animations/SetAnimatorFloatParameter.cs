using UnityEngine;
using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Animations)]
    [NodeDescription("Set Animator Float Parameter")]
    public class SetAnimatorFloatParameter : ActionTask
    {
        [ParameterDefinition(AutoBindType.AnyWithSameType)]
        [SerializeField] private Parameter<Animator> animator;

        [SerializeField] private Parameter<string> parameterName;
        [SerializeField] private Parameter<float> value;

        public override string Info => $"Set {parameterName} = {value}";

        protected override void StartAction()
        {
            animator.Value.SetFloat(parameterName.Value, value.Value);
            EndAction();
        }
    }
}
