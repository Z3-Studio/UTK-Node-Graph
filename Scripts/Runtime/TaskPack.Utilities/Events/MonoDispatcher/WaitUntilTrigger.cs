using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Rigidbody)]
    [NodeDescription("Wait until anything is inside the Trigger area and return the Transform.")]
    public class WaitUntilTrigger : ActionTask
    {
        [ParameterDefinition(AutoBindType.SelfBind)]
        [SerializeField] private Parameter<GameObject> target;

        [SerializeField] private Parameter<bool> triggerExit;
        [SerializeField] private Parameter<Collider> returnedCollider;

        public override string Info => $"Wait Until Trigger {(triggerExit.Value ? "Exit" : "Enter")} 'AgentInfo'";

        private MonoEventDispatcher monoEvents;

        protected override void StartAction()
        {
            monoEvents = MonoEventDispatcher.ValidateEmmiter(monoEvents, target.Value);

            if (!triggerExit.Value)
                monoEvents.OnTriggerEnterEvent += OnTrigger;
            else
                monoEvents.OnTriggerExitEvent += OnTrigger;
        }

        protected override void StopAction()
        {
            monoEvents.OnTriggerEnterEvent -= OnTrigger;
            monoEvents.OnTriggerExitEvent -= OnTrigger;
        }

        private void OnTrigger(Collider collider)
        {
            returnedCollider.Value = collider;
            EndAction();
        }
    }
}