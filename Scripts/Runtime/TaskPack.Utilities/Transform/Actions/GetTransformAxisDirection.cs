using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities.Physic
{
    [NodeCategory(Categories.Transform)]
    [NodeDescription("Get transform.axis")]
    public class GetTransformAxisDirection : ActionTask
    {
        [ParameterDefinition(AutoBindType.FindSimilarVariable)]
        [SerializeField] private Parameter<Transform> transform;

        [Header("In")]
        [SerializeField] private Parameter<Direction> axisDirecition = Direction.Right;

        [Header("Out")]
        [SerializeField] private Parameter<Vector3> returnedValue;

        public override string Info => $"Get Transform.{axisDirecition}";

        protected override void StartAction()
        {
            returnedValue.Value = axisDirecition.Value switch
            {
                Direction.Left => -transform.Value.transform.right,
                Direction.Right => transform.Value.transform.right,
                Direction.Up => transform.Value.transform.up,
                Direction.Down => -transform.Value.transform.up,
                Direction.Forward => transform.Value.transform.forward,
                Direction.Back => -transform.Value.transform.forward,
                _ => throw new System.NotImplementedException(),
            };

            EndAction();
        }
    }
}