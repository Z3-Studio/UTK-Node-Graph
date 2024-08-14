using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Rigidbody)]
    public class GetRigidbodyFromCollision : ActionTask
    {
        [SerializeField] private Parameter<Collision> collision;
        [SerializeField] private Parameter<Rigidbody> attachedRigidbody;

        public override string Info => $"Get Ridigbody from {collision}";

        protected override void StartAction()
        {
            attachedRigidbody.Value = collision.Value.rigidbody;
            EndAction();
        }
    }
}