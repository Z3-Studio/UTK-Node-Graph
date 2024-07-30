using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;


namespace Z3.NodeGraph.TaskPack.Utilities.Physic
{
    [NodeCategory(Categories.Transform)]
    [NodeDescription("Get transform.axis")]
    public class GetTransformAxisDirection : ActionTask<Transform>
    {
        [Header("In")]
        public Parameter<Direction> axisDirecition = Direction.Right;

        [Header("Out")]
        public Parameter<Vector3> returnedValue;

        public override string Info => $"Get Transform.{axisDirecition}";

        protected override void StartAction()
        {
            returnedValue.Value = axisDirecition.Value switch
            {
                Direction.Left => -Agent.transform.right,
                Direction.Right => Agent.transform.right,
                Direction.Up => Agent.transform.up,
                Direction.Down => -Agent.transform.up,
                Direction.Forward => Agent.transform.forward,
                Direction.Back => -Agent.transform.forward,
                _ => throw new System.NotImplementedException(),
            };

            EndAction(true);
        }
    }
}