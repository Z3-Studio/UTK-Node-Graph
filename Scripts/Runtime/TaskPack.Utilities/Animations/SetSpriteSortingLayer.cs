using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Animations)]
    [NodeDescription("Sets the sprite layer")]
    public class SetSpriteSortingLayer : ActionTask<SpriteRenderer>
    {
        [Header("Layer")]
        public Parameter<int> sortingLayerIndex;
        public Parameter<string> sortingLayerName;

        [Header("Parameters")]
        public Parameter<bool> useName;

        public override string Info => $"Set sprite layer to {(useName.Value ? sortingLayerName.Value : SortingLayer.layers[sortingLayerIndex.Value].name)}";

        protected override void StartAction()
        {
            string layerName = useName.Value 
                ? sortingLayerName.Value 
                : SortingLayer.layers[sortingLayerIndex.Value].name;

            int layerID = SortingLayer.NameToID(layerName);
            
            if(!SortingLayer.IsValid(layerID))
                EndAction(false);
            
            Agent.sortingLayerID = layerID;
            EndAction(true);
        }
    }
}