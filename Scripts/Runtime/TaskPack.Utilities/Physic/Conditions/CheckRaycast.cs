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
        [SerializeField] private Parameter<Vector3> rayOrigin;
        [SerializeField] private Parameter<Vector3> direction;
        [SerializeField] private Parameter<LayerMask> layerMask;
        [SerializeField] private Parameter<float> distance;
        
        [Header("Out")]
        [SerializeField] private Parameter<Vector2> positionHit;

        public override bool CheckCondition()
        {
            bool successful = Physics.Raycast(rayOrigin.Value, direction.Value, out RaycastHit hit, distance.Value, layerMask.Value);

            positionHit.Value = hit.point;
            
            return successful;
        }
    }
}