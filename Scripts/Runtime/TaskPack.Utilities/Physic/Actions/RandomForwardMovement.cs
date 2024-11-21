using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;
using Z3.Utils.ExtensionMethods;

namespace Z3.NodeGraph.TaskPack.Utilities.Physic
{
    [NodeCategory(Categories.Movement)]
    [NodeDescription("Moves a rigidbody in the Transform.Right direction with random speed within a range. This movement preserves the current speed in Y.")]
    public class RandomForwardMovement : ActionTask
    {
        [ParameterDefinition(AutoBindType.FindSimilarVariable)]
        [SerializeField] private Parameter<Rigidbody2D> rigidbody;
        [SerializeField] private Parameter<Vector2> speedRange;
        public override string Info => $"Random Forward Movement = {speedRange}";
        protected override void StartAction()
        {
            Vector3 forward = rigidbody.Value.transform.forward * speedRange.Value.RandomRange();
            rigidbody.Value.linearVelocity = new Vector3(forward.x, rigidbody.Value.linearVelocity.y, forward.z);
            EndAction();
        }
    }
}