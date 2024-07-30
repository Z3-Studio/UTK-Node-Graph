namespace Z3.NodeGraph.Core
{
    // https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.taskstatus?view=net-8.0
    public enum State // Review the namespace
    {
        /// <summary> Initial State </summary>
        Ready,

        /// <summary> Executing </summary>
        Running,

        /// <summary> Execution finish with failure </summary>
        Failure,

        /// <summary> Execution finish with success </summary>
        Success,

        /// <summary> Execution finish was interrupted before finishing </summary>
        Resting
        //Ready, // Resting
        //Error,
        //Optional
    }

    public enum StateResult
    {
        Success,
        Failure
    }
}
