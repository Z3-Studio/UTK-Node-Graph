using Z3.NodeGraph.Core;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NgName("Set Vector3 Advanced")]
    [NodeDescription("Easy way to set a specific axis")]
    public class SetVector2 : SetOperation2<Vector2>
    {
        protected override void StartAction()
        {
            valueA.Value = operation.Operate(valueA.Value, valueB.Value);
            EndAction();
        }
    }
}
