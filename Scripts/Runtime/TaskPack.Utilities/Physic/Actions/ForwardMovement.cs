using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities.Physic
{
    [NodeCategory(Categories.Rigidbody)]
    [NodeDescription("Moves a rigidbody in the Transform.Forward direction. This movement preserves the current speed in Y.")]
    public class ForwardMovement : ActionTask
    {
        [ParameterDefinition(AutoBindType.FindSimilarVariable)]
        [SerializeField] private Parameter<Rigidbody> rigidbody;
        [SerializeField] private Parameter<float> speed;
        public override string Info => $"Forward Movement = {speed}";
        protected override void StartAction()
        {
            Vector3 forward = rigidbody.Value.transform.forward * speed.Value;
            rigidbody.Value.linearVelocity = new Vector3(forward.x, rigidbody.Value.linearVelocity.y, forward.z);
            EndAction();
        }
    }
}