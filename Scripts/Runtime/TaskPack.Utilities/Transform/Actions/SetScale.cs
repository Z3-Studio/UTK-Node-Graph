using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities { 

    [NodeCategory(Categories.Transform)]
    [NodeDescription("Set transform.localScale")]
    public class SetScale : ActionTask 
    {
        [ParameterDefinition(AutoBindType.SelfBind)]
        [SerializeField] private Parameter<Transform> data;

        [SerializeField] private Parameter<Vector3> scale;

        protected override void StartAction() 
        {
            data.Value.localScale = scale.Value;
            EndAction();
        }
    }
}