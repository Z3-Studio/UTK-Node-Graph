using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;
using Z3.Utils.ExtensionMethods;
using Z3.Utils;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Events)]
    [NodeDescription("Wait until anything is inside the Trigger area and return the Transform.")]
    public class WaitUntilTrigger : ActionTask
    {
        [ParameterDefinition(AutoBindType.SelfBind)]
        [SerializeField] private Parameter<GameObject> target;

        [SerializeField] private Parameter<bool> triggerEnter = true;
        [SerializeField] private Parameter<Collider> returnedCollider;

        public override string Info => $"Wait Until {target} Trigger {(triggerEnter.IsBinding ? triggerEnter.ToString() : triggerEnter.Value ? "Enter" : "Exit").ToBold()}";

        private MonoEventDispatcher monoEvents;

        protected override void StartAction()
        {
            UnityUtils.AddComponentIfNeeded(ref monoEvents, target.Value);

            if (triggerEnter.Value)
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