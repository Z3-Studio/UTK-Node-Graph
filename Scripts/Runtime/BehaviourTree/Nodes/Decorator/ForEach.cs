using System.Collections;
using UnityEngine;
using Z3.NodeGraph.Core;

namespace Z3.NodeGraph.BehaviourTree
{
    [NodeIcon(GraphIcon.ForEach)]
    public class ForEach : DecoratorNode
    {
        [SerializeField] private Parameter<IList> list;
        [SerializeField] private Parameter<object> element;
        [SerializeField] private Parameter<int> index;

        public override string SubInfo => $"Foreach {list}";

        private bool pickElement;

        protected override void StartNode()
        {
            pickElement = true;
        }

        protected override State UpdateNode()
        {
            if (index.Value >= list.Value.Count)
                return State.Success;

            if (pickElement)
            {
                pickElement = false;
                element.Value = list.Value[index];
            }

            State result = child.Update();

            // Child Fail
            if (result == State.Failure)
                return State.Failure;

            if (result == State.Success)
            {
                // Finish iteration
                if (index.Value >= list.Value.Count)
                    return State.Success;

                // Continue with next index
                index.Value++;
                pickElement = true;
                return UpdateNode();
            }

            // Continue processing child
            return State.Running;
        }
    }
}
