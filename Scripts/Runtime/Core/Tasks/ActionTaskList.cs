using System;
using System.Collections.Generic;
using System.Linq;
using Z3.NodeGraph.Core;

namespace Z3.NodeGraph.Tasks
{
    public enum ExecutionPolicy
    {
        Sequential,
        Parallel
    }

    [Serializable]
    public class ActionTaskList : TaskList<ActionTask>
    {
        private Func<State> onUpdate = () => State.Success;

        public override void StartTaskList() => StartTaskList(default);

        public void StartTaskList(ExecutionPolicy executionPolicy)
        {
            if (executionPolicy == ExecutionPolicy.Sequential)
            {
                if (taskList.Count == 0)
                {
                    onUpdate = () => State.Success;
                    return;
                }

                int sequencerIndex = 0;
                onUpdate = () => SequentialExecution(ref sequencerIndex);
            }
            else
            {
                // Clone the list to be remove
                List<ActionTask> cloneList = new(taskList);
                onUpdate = () => ParallelExecution(cloneList);
            }
        }

        public State Update() => onUpdate();

        public override void StopTaskList()
        {
            foreach (ActionTask task in taskList)
            {
                task.StopTask();
            }
        }

        private State SequentialExecution(ref int sequencerIndex)
        {
            if (sequencerIndex >= taskList.Count) // TODO: Review it, single actions
            {
                sequencerIndex = 0;
            }

            State state = State.Success;

            while (state == State.Success && sequencerIndex < taskList.Count)
            {
                state = taskList[sequencerIndex].UpdateActionTask();

                if (state == State.Success)
                {
                    sequencerIndex++;
                }
            }

            return state;
        }

        public State ParallelExecution(List<ActionTask> taskList)
        {
            // Parallel Execution
            foreach (ActionTask task in taskList.ToList())
            {
                State actionResult = task.UpdateActionTask();

                if (actionResult == State.Failure)
                {
                    taskList.Remove(task);

                    // Stop others
                    StopTaskList();

                    return State.Failure;
                }
                if (actionResult == State.Success)
                {
                    taskList.Remove(task);
                }
            }

            return taskList.Count == 0 ? State.Success : State.Running;
        }
    }
}
