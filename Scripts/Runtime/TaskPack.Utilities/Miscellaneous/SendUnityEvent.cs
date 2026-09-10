using UnityEngine;
using UnityEngine.Events;
using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Miscellaneous)]
    [NodeDescription("Send Unity Event")]
    public class SendUnityEvent : ActionTask
    {
        [SerializeField] private Parameter<UnityEvent> unityEvent;

        public override string Info => $"{unityEvent} Invoke";

        protected override void UpdateAction()
        {
            unityEvent.Value.Invoke();
            EndAction();
        }
    }
}
