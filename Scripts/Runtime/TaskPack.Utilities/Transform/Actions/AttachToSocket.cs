using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;
using Z3.Utils.ExtensionMethods;

namespace Z3.NodeGraph.TaskPack.Utilities.Physic
{
    [NodeCategory(Categories.Transform)]
    [NodeDescription("Set object to Socket")]
    public class AttachToSocket : ActionTask
    {
        [SerializeField] private Parameter<Transform> transformToSet;
        [Space]
        [SerializeField] private Parameter<Transform> transformWithSocket;
        [SerializeField] private Parameter<string> socketName = "Socket Name";
        [Space]
        [SerializeField] private Parameter<Vector3> localPosition;
        [SerializeField] private Parameter<Vector3> localEuler;

        private Transform cachedSocket;

        public override string Info => $"Attach {transformToSet} to {socketName}";

        protected override void StartAction()
        {
            if (!cachedSocket)
            {
                cachedSocket = transformWithSocket.Value.FindInChildren(socketName);
            }

            transformToSet.Value.SetParent(cachedSocket);
            transformToSet.Value.SetLocalPositionAndRotation(localPosition, Quaternion.Euler(localEuler));
            EndAction();
        }
    }
}