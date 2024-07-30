using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Components)]
    [NodeDescription("Sets a Sprite Renderer Sprite")]
    public class SetSprite : ActionTask<SpriteRenderer> 
    {
        public Parameter<Sprite> sprite;

        protected override void StartAction()
        {
            Agent.sprite = sprite.Value;
            EndAction(true);
        }
    }
}