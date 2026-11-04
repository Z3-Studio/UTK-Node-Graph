using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;
using Z3.Utils.ExtensionMethods;

namespace Z3.NodeGraph.TaskPack.Utilities.Physic
{
    [NodeCategory(Categories.Transform)]
    [NodeDescription("Set object to Socket")]
    public class GetSocket : ActionTask
    {
        [SerializeField] private Parameter<Transform> transformWithSocket;
        [SerializeField] private Parameter<string> socketName = "Socket Name";
        [Space]
        [SerializeField] private Parameter<Transform> returnedSocket;

        public override string Info => $"Get {socketName} from {transformWithSocket}";

        protected override void StartAction()
        {
            returnedSocket.Value = transformWithSocket.Value.FindInChildren(socketName);
            EndAction();
        }
    }
}