    using Z3.NodeGraph.Core;
using Z3.Utils.ExtensionMethods;

namespace Z3.NodeGraph.Sample.ThirdPerson.Character.States
{
    public class IdlePS : CharacterAction
    {
        public Parameter<string> idleState;
        public Parameter<string> overrideIdleState;

        protected override void StartAction()
        {
            string stateName = string.IsNullOrEmpty(overrideIdleState.Value) ? idleState.Value : overrideIdleState.Value;
            Animator.PlayStateAllLayers(stateName);
            overrideIdleState.Value = string.Empty;
            EndAction();
        }
    }
}