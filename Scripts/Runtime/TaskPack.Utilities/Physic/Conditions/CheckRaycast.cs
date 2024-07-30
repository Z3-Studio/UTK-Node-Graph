using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Physics)]
    [NodeDescription("Launch a Raycast and return true or false if something was hit.")]
    public class CheckRaycast : ConditionTask
    {
        [Header("Out")]
        public Parameter<Vector3> rayOrigin;
        public Parameter<Vector3> direction;
        public Parameter<LayerMask> layerMask;
        public Parameter<float> distance;
        
        [Header("Out")]
        public Parameter<Vector2> positionHit;

        public override bool CheckCondition()
        {
            bool successful = Physics.Raycast(rayOrigin.Value, direction.Value, out RaycastHit hit, distance.Value, layerMask.Value);

            positionHit.Value = hit.point;
            
            return successful;
        }
    }
}