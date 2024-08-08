using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities.Physic
{
    [NodeCategory(Categories.Rigidbody)]
    [NodeDescription("Clamp Velocity to a set of values")]
    public class ClampVelocity : ActionTask
    {
        [ParameterDefinition(AutoBindType.SelfBind)]
        [SerializeField] private Parameter<Rigidbody> data;
        [Header("Inputs")]
        [SerializeField] private Parameter<Vector2> range;
        public override string Info => $"Clamp Velocity, Range: {range}";
        protected override void StartAction()
        {
            data.Value.velocity = new Vector3()
            {
                x = Mathf.Clamp(data.Value.velocity.x, range.Value.x, range.Value.y),
                y = Mathf.Clamp(data.Value.velocity.y, range.Value.x, range.Value.y),
                z = Mathf.Clamp(data.Value.velocity.z, range.Value.x, range.Value.y)
            };
            EndAction();
        }
    }
}