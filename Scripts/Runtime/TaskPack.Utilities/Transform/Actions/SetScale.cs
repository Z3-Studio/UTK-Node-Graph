using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities { 

    [NodeCategory(Categories.Transform)]
    [NodeDescription("Set transform.localScale")]
    public class SetScale : ActionTask 
    {
        [ParameterDefinition(AutoBindType.FindSimilarVariable)]
        [SerializeField] private Parameter<Transform> transform;

        [SerializeField] private Parameter<Vector3> scale;

        protected override void StartAction() 
        {
            transform.Value.localScale = scale.Value;
            EndAction();
        }
    }
}