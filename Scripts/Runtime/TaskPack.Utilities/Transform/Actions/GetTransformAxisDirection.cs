using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities.Physic
{
    [NodeCategory(Categories.Transform)]
    [NodeDescription("Get transform.axis")]
    public class GetTransformAxisDirection : ActionTask
    {
        [ParameterDefinition(AutoBindType.SelfBind)]
        [SerializeField] private Parameter<Transform> data;

        [Header("In")]
        [SerializeField] private Parameter<Direction> axisDirecition = Direction.Right;

        [Header("Out")]
        [SerializeField] private Parameter<Vector3> returnedValue;

        public override string Info => $"Get Transform.{axisDirecition}";

        protected override void StartAction()
        {
            returnedValue.Value = axisDirecition.Value switch
            {
                Direction.Left => -data.Value.transform.right,
                Direction.Right => data.Value.transform.right,
                Direction.Up => data.Value.transform.up,
                Direction.Down => -data.Value.transform.up,
                Direction.Forward => data.Value.transform.forward,
                Direction.Back => -data.Value.transform.forward,
                _ => throw new System.NotImplementedException(),
            };

            EndAction();
        }
    }
}