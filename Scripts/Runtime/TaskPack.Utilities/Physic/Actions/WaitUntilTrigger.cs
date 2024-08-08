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
        [SerializeField] private Parameter<Transform> data;

        [SerializeField] private Parameter<bool> triggerExit;
        [SerializeField] private Parameter<Collider> returnedCollider;

        public override string Info => $"Wait Until Trigger {(triggerExit.Value ? "Exit" : "Enter")} 'AgentInfo'";

        protected override void StartAction()
        {
            //if (!triggerExit.Value)
            //    router.onTriggerEnter += OnTrigger;
            //else
            //    router.onTriggerExit += OnTrigger;
        }

        protected override void StopAction()
        {
            //router.onTriggerExit -= OnTrigger;
            //router.onTriggerEnter -= OnTrigger;
        }

        //private void OnTrigger(EventData<Collider> data)
        //{
        //    returnedCollider.Value = data.Value;
        //    EndAction();
        //}
    }
}