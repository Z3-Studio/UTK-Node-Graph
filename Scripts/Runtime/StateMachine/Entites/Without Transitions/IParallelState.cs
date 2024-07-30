namespace Z3.NodeGraph.StateMachine
{
    public interface IParallelState
    {
        ParallelExecution ParallelExecution { get; }
        int Priority { get; }
        void UpdateParallel();
    }

    public enum ParallelUpdateMode
    {
        BeforeUpdate,
        AfterUpdate
    }
    public enum ParallelExecution
    {
        Enter,
        BeforeUpdate,
        AfterUpdate,
        Exit
    }
}
