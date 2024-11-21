using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities.Physic
{
    [NodeCategory(Categories.Rigidbody)]
    [NodeDescription("Clamp Velocity to a set of values")]
    public class ClampVelocity : ActionTask
    {
        [ParameterDefinition(AutoBindType.FindSimilarVariable)]
        [SerializeField] private Parameter<Rigidbody> rigidbody;
        [Header("Inputs")]
        [SerializeField] private Parameter<Vector2> range;
        public override string Info => $"Clamp Velocity, Range: {range}";
        protected override void StartAction()
        {
            rigidbody.Value.linearVelocity = new Vector3()
            {
                x = Mathf.Clamp(rigidbody.Value.linearVelocity.x, range.Value.x, range.Value.y),
                y = Mathf.Clamp(rigidbody.Value.linearVelocity.y, range.Value.x, range.Value.y),
                z = Mathf.Clamp(rigidbody.Value.linearVelocity.z, range.Value.x, range.Value.y)
            };
            EndAction();
        }
    }
}