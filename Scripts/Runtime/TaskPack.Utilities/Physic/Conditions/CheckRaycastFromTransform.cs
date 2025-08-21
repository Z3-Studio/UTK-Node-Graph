using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Physics)]
    [NodeDescription("Launch a Raycast and return true or false if something was hit. Use a Transform.forward as direction")]
    public class CheckRaycastFromTransform : ConditionTask
    {
        [Header("Out")]
        [SerializeField] private Parameter<Transform> rayOrigin;
        [SerializeField] private Parameter<LayerMask> layerMask;
        [SerializeField] private Parameter<float> distance;
        [SerializeField] private Parameter<Direction> direction = Direction.Forward;
        
        [Header("Out")]
        [SerializeField] private Parameter<Vector3> positionHit;

        public override bool CheckCondition()
        {
            Vector3 rayDirection = direction.Value.GetDirection(rayOrigin.Value);
            bool successful = Physics.Raycast(rayOrigin.Value.position, rayDirection, out RaycastHit hit, distance.Value, layerMask.Value);

            positionHit.Value = hit.point;

            return successful;
        }
    }
}
