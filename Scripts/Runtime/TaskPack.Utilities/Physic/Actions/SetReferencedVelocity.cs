using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities 
{    
    [NodeCategory(Categories.Rigidbody)]
    [NodeDescription("Set Rigidbody velocity based on Transform direction")]
    public class SetReferencedVelocity : ActionTask
    {
        [ParameterDefinition(AutoBindType.FindSimilarVariable)]
        [SerializeField] private Parameter<Rigidbody> rigidbody;
        [SerializeField] private Parameter<Vector3> velocity;
        public override string Info => $"Referenced velocity = {velocity}";
        protected override void StartAction() 
        {
            Vector3 result =
                rigidbody.Value.transform.right * velocity.Value.x +
                rigidbody.Value.transform.up * velocity.Value.y +
                rigidbody.Value.transform.forward * velocity.Value.z;

#if UNITY_6000_0_OR_NEWER
            rigidbody.Value.linearVelocity = result;
#else
            rigidbody.Value.velocity = result;
#endif

            EndAction();
        }
    }
}