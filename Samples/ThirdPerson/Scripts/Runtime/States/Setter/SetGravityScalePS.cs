using Z3.NodeGraph.Core;

namespace Z3.NodeGraph.Sample.ThirdPerson.Character.States
{
    public class SetGravityScalePS : CharacterAction 
    {
        public Parameter<float> gravityScale;

        public override string Info => $"Gravity = {gravityScale}";

        protected override void StartAction()
        {
            Physics.SetGravityScale(gravityScale.Value);
            EndAction();
        }
    }
}
