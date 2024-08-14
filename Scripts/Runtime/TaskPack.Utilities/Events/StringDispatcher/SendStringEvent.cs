using CodiceApp.EventTracking.Plastic;
using UnityEngine;
using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;

/*
Box Collider envia sinal para MonoBehaviour do mesmo objeto
Todos MonoBehaviour que implementarem on Trigger, vão receber o eventos.

MonoEvent distruibui o evento para quem queira ouvir
Task ao iniciar, adiciona ou get MonoEvent pra ouvir seus eventos

-------

Task envia sinal adiciona ou get em um NamedEvent, enviando self GraphRunner/GameObject como argumento

A outra Task que está interessada em ouvir, adiciona ou get em um NamedEvent

*/
namespace Z3.NodeGraph.TaskPack.Utilities.Utils
{
    [NodeCategory(Categories.Events)]
    [NodeDescription("Sends an event to Dispatcher")]
    public class SendStringEvent : ActionTask
    {
        [SerializeField] private Parameter<GameObject> target;
        [SerializeField] private Parameter<string> eventName;

        public override string Info => $"Send Event [{eventName}] to {target}";

        protected override void StartAction()
        {
            StringEventDispatcher.SendEvent(GraphRunner, target.Value, eventName.Value);
            EndAction();
        }
    }

    [NodeCategory(Categories.Events)]
    [NodeDescription("Sends an event to Dispatcher with payload")]
    public class SendStringEvent<T> : ActionTask
    {
        [SerializeField] private Parameter<GameObject> target;
        [SerializeField] private Parameter<string> eventName;
        [SerializeField] private Parameter<T> valueToSend;

        public override string Info => $"Send Event [{eventName}] to {target} with {valueToSend}";

        protected override void StartAction()
        {
            StringEventDispatcher.SendEvent(GraphRunner, target.Value, eventName.Value, valueToSend.Value);
            EndAction();
        }
    }
}