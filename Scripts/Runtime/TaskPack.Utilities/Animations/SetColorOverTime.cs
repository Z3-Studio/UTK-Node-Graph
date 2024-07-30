using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;


namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Animations)]
    [NodeDescription("Change Sprite Renderer Color Over Time")]
    public class SetColorOverTime : ActionTask<SpriteRenderer>
    {
        [Header("Input")]
        public Parameter<float> duration;

        [Header("Output")]
        public Parameter<Color> endColor;

        private Color startColor;
        private float timeStep;
        public override string Info => $"Change Color In Seconds: {duration}";

        protected override void StartAction()
        {
            startColor = Agent.color;
            timeStep = 0f;
        }

        protected override void UpdateAction()
        {
            if (Agent.color != endColor.Value && duration.Value != 0f)
            {
                timeStep += Time.fixedDeltaTime / duration.Value;
                Agent.color = Color.Lerp(startColor, endColor.Value, timeStep);
            }
            else
            {
                EndAction(true);
            }
        }
    }
}