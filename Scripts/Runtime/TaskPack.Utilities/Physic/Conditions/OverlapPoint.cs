using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities {

    [NodeCategory(Categories.Physics)]
    [NodeDescription("Create a OverlapPoint in the transform.position")]
    public class OverlapPoint : ConditionTask<Transform> 
    {
        public Parameter<Vector3> offset;
        public Parameter<LayerMask> layerMask;

        public override bool CheckCondition() 
        {
            return Physics.CheckSphere(Agent.position + offset.Value, float.MinValue, layerMask.Value);
        }

        //public override void OnDrawGizmosSelected() {
        //    Gizmos.color = Color.green;
        //    Gizmos.DrawWireSphere(Agent.position + offset.Value, .2f);
        //}
    }
}