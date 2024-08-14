using UnityEngine;
using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;

namespace Z3.NodeGraph.TaskPack.Utilities.Utils
{
    [NodeCategory(Categories.Events)]
    public class SendEventoToMediator : ActionTask
    {
        [SerializeField] private Parameter<EventMediator> eventMediator;

        public override string Info => $"Invoke {eventMediator}";

        protected override void StartAction()
        {
            eventMediator.Value.Invoke();
            EndAction();
        }
    }

    [NodeCategory(Categories.Events)]
    public class SendEventoToMediator<T> : ActionTask
    {
        [SerializeField] private Parameter<EventMediator> eventMediator;
        [SerializeField] private Parameter<T> value;

        public override string Info => $"Invoke {eventMediator} with {value}";

        protected override void StartAction()
        {
            eventMediator.Value.Invoke(value);
            EndAction();
        }
    }
}