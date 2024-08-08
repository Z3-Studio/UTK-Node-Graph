using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Animations)]
    [NodeDescription("Sets the sprite layer")]
    public class SetSpriteSortingLayer : ActionTask
    {
        [ParameterDefinition(AutoBindType.SelfBind)]
        [SerializeField] private Parameter<SpriteRenderer> data;

        [Header("Layer")]
        [SerializeField] private Parameter<int> sortingLayerIndex;
        [SerializeField] private Parameter<string> sortingLayerName;

        [Header("Parameters")]
        [SerializeField] private Parameter<bool> useName;

        public override string Info => $"Set sprite layer to {(useName.Value ? sortingLayerName.Value : SortingLayer.layers[sortingLayerIndex.Value].name)}";

        protected override void StartAction()
        {
            string layerName = useName.Value 
                ? sortingLayerName.Value 
                : SortingLayer.layers[sortingLayerIndex.Value].name;

            int layerID = SortingLayer.NameToID(layerName);
            
            if(!SortingLayer.IsValid(layerID))
                EndAction(false);
            
            data.Value.sortingLayerID = layerID;
            EndAction();
        }
    }
}