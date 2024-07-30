using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities.AI
{

    [NodeCategory(Categories.Components)]
    [NodeDescription("Draws a straight line between two local positions, it requires a LineRenderer.")]
    public class DrawLine : ActionTask
    {
        /*[RequiredField]*/ public Parameter<Vector3> startPosition;
        /*[RequiredField]*/ public Parameter<Vector3> endPosition;
        /*[RequiredField]*/ public Parameter<LineRenderer> line;

        protected override void StartAction() {
            Draw();
            EndAction(true);
        }

        private void Draw()
        {
            line.Value.SetPosition(0, startPosition.Value);
            line.Value.SetPosition(1, endPosition.Value);
        }
    }
}