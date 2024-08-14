using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;
using Z3.Utils.ExtensionMethods;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Events)]
    [NodeDescription("Wait until anything is inside the Trigger area and return the Transform.")]
    public class CheckTriggerEvent : EventConditionTask
    {
        [ParameterDefinition(AutoBindType.SelfBind)]
        [SerializeField] private Parameter<GameObject> target;

        [SerializeField] private Parameter<bool> triggerEnter = true;
        [SerializeField] private Parameter<Collider> returnedCollider;

        public override string Info => $"Wait Until {target} Trigger {(triggerEnter.IsBinding ? triggerEnter.ToString() : triggerEnter.Value ? "Enter" : "Exit").ToBold()}";

        private MonoEventDispatcher monoEvents;

        protected override void Subscribe()
        {
            monoEvents = MonoEventDispatcher.ValidateEmmiter(monoEvents, target.Value);

            if (triggerEnter.Value)
                monoEvents.OnTriggerEnterEvent += OnTrigger;
            else
                monoEvents.OnTriggerExitEvent += OnTrigger;
        }

        protected override void Unsubscribe()
        {
            monoEvents.OnTriggerEnterEvent -= OnTrigger;
            monoEvents.OnTriggerExitEvent -= OnTrigger;
        }

        private void OnTrigger(Collider collider)
        {
            returnedCollider.Value = collider;
            EndEventCondition();
        }
    }
}