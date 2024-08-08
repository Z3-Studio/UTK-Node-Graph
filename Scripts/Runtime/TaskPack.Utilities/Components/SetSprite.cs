using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Components)]
    [NodeDescription("Sets a Sprite Renderer Sprite")]
    public class SetSprite : ActionTask
    {
        [ParameterDefinition(AutoBindType.SelfBind)]
        [SerializeField] private Parameter<SpriteRenderer> data;
        [SerializeField] private Parameter<Sprite> sprite;

        protected override void StartAction()
        {
            data.Value.sprite = sprite.Value;
            EndAction();
        }
    }
}