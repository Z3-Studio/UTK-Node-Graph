using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities {

    [NodeCategory(Categories.Physics)]
    [NodeDescription("Create a OverlapPoint in the transform.position")]
    public class OverlapPoint : ConditionTask
    {
        [ParameterDefinition(AutoBindType.SelfBind)]
        [SerializeField] private Parameter<Transform> data;
        [SerializeField] private Parameter<Vector3> offset;
        [SerializeField] private Parameter<LayerMask> layerMask;

        public override bool CheckCondition() 
        {
            return Physics.CheckSphere(data.Value.position + offset.Value, float.MinValue, layerMask.Value);
        }
    }
}