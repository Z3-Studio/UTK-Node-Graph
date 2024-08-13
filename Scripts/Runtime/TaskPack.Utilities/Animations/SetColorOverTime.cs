using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Animations)]
    [NodeDescription("Change Sprite Renderer Color Over Time")]
    public class SetColorOverTime : ActionTask
    {
        [ParameterDefinition(AutoBindType.SelfBind)]
        [SerializeField] private Parameter<SpriteRenderer> data;

        [Header("Input")]
        [SerializeField] private Parameter<float> duration;

        [Header("Output")]
        [SerializeField] private Parameter<Color> endColor;

        private Color startColor;
        private float timeStep;
        public override string Info => $"Change Color In Seconds: {duration}";

        protected override void StartAction()
        {
            startColor = data.Value.color;
            timeStep = 0f;
        }

        protected override void UpdateAction()
        {
            if (data.Value.color != endColor.Value && duration.Value != 0f)
            {
                timeStep += DeltaTime / duration.Value;
                data.Value.color = Color.Lerp(startColor, endColor.Value, timeStep);
            }
            else
            {
                EndAction();
            }
        }
    }
}