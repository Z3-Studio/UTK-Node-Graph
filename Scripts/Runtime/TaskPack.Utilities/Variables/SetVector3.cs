using Z3.NodeGraph.Core;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NgName("Set Vector3")]
    [NodeDescription("Easy way to set a specific axis")]
    public class SetVector3 : SetOperation2<Vector3>
    {
        protected override void StartAction()
        {
            valueA.Value = operation.Operate(valueA.Value, valueB.Value);
            EndAction();
        }
    }
}
