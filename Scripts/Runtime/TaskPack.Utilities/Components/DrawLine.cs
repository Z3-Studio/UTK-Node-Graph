using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities.AI
{

    [NodeCategory(Categories.Components)]
    [NodeDescription("Draws a straight line between two local positions, it requires a LineRenderer.")]
    public class DrawLine : ActionTask
    {
        [SerializeField] private Parameter<Vector3> startPosition;
        [SerializeField] private Parameter<Vector3> endPosition;
        [SerializeField] private Parameter<LineRenderer> line;

        protected override void StartAction() {
            Draw();
            EndAction();
        }

        private void Draw()
        {
            line.Value.SetPosition(0, startPosition.Value);
            line.Value.SetPosition(1, endPosition.Value);
        }
    }
}