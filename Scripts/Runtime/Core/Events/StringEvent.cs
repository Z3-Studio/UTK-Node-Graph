namespace Z3.NodeGraph.Core
{
    public class StringEvent : IStringEvent
    {
        public object Sender { get; }
        public string EventName { get; }

        public StringEvent(object sender, string eventName)
        {
            EventName = eventName;
            Sender = sender;
        }
    }

    public class StringEvent<T> : StringEvent, IStringEvent<T>
    {
        public T Payload { get; }

        public StringEvent(object sender, string eventName, T value) : base(sender, eventName)
        {
            Payload = value;
        }
    }
}
