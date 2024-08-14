using UnityEngine;
using Z3.NodeGraph.Core;

namespace Z3.NodeGraph.BehaviourTree
{
    public class TryCatchDecorator : DecoratorNode
    {
        [SerializeField] private bool logError = true;

        protected override State UpdateNode()
        {
            try
            {
                return child.Update();
            }
            catch (System.Exception e)
            {
                if (logError)
                {
                    Debug.LogException(e);
                }

                return State.Failure;
            }
        }
    }
}
