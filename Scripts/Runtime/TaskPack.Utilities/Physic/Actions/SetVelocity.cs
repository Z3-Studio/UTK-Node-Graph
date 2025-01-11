using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities {

    [NodeCategory(Categories.Rigidbody)]
    [NodeDescription("Set Rigidbody velocity")]
    public class SetVelocity : ActionTask 
    {
        [ParameterDefinition(AutoBindType.FindSimilarVariable)]
        [SerializeField] private Parameter<Rigidbody> rigidbody;

        [SerializeField] private Parameter<Vector3> velocity;
        public override string Info => $"Velocity = {velocity}";
        protected override void StartAction() 
        {
#if UNITY_6000_0_OR_NEWER
            rigidbody.Value.linearVelocity = velocity.Value;
#else
            rigidbody.Value.velocity = velocity.Value;
#endif
            EndAction();
        }
    }
}