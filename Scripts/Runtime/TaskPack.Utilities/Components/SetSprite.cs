using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Components)]
    [NodeDescription("Sets a Sprite Renderer Sprite")]
    public class SetSprite : ActionTask
    {
        [ParameterDefinition(AutoBindType.FindSimilarVariable)]
        [SerializeField] private Parameter<SpriteRenderer> spriteRenderer;
        [SerializeField] private Parameter<Sprite> sprite;

        protected override void StartAction()
        {
            spriteRenderer.Value.sprite = sprite.Value;
            EndAction();
        }
    }
}