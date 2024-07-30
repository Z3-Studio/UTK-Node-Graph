namespace Z3.NodeGraph.Core
{
    public static class NodeGraphExtensions
    {
        public static void SendEvent<T>(this GraphRunner graphRunner, string eventname, T value)
        {
            graphRunner.Events.SendCustomEvent(eventname, value);
        }

        public static void SendEvent(this GraphRunner graphRunner, string eventname, object value = null)
        {
            graphRunner.Events.SendCustomEvent(eventname, value);
        }
    }
}
