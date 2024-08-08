using Z3.NodeGraph.Core;

namespace Z3.NodeGraph.Sample.ThirdPerson.Character.States
{
    public class CheckCharacterEventPS : CharacterCondition
    {
        public Parameter<CharacterEvent> eventType;

        private bool actionCalled;
        public override string Info => $"CheckCharacterEvent: {eventType}";

        public override void StartCondition()
        {
            actionCalled = false;
            Agent.OnCharacterEvent += OnCharacterEvent;
        }

        public override void StopCondition()
        {
            Agent.OnCharacterEvent -= OnCharacterEvent;
        }

        private void OnCharacterEvent(CharacterEvent playerEvent)
        {
            if (playerEvent == eventType.Value)
            {
                actionCalled = true;
            }
        }

        public override bool CheckCondition()
        {
            bool value = actionCalled;
            actionCalled = false;
            return value;
        }
    }
}