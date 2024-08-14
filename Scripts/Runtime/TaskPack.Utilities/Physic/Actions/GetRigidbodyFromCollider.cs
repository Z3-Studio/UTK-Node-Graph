using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Rigidbody)]
    public class GetRigidbodyFromCollider : ActionTask
    {
        [SerializeField] private Parameter<Collider> collider;
        [SerializeField] private Parameter<Rigidbody> attachedRigidbody;

        public override string Info => $"Get Ridigbody from {collider}";

        protected override void StartAction()
        {
            attachedRigidbody.Value = collider.Value.attachedRigidbody;
            EndAction();
        }
    }
}