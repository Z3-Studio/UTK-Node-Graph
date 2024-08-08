using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Variables)]
    [NodeDescription("Convert Euler Angles in a Quaterion")]
    public class EulerToQuaterion : ActionTask
    {
        [SerializeField] private Parameter<Vector3> euler;
        [SerializeField] private Parameter<Quaternion> quaterion;

        public override string Info => $"{quaterion} = Quaternion.Euler({euler})";

        protected override void StartAction()
        {
            quaterion.Value = Quaternion.Euler(euler.Value);
            EndAction();
        }
    }
}