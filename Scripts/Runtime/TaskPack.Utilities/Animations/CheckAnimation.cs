using UnityEngine;
using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using Z3.Utils.ExtensionMethods;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Animations)]
    [NodeDescription("Check the current animation by state name")]
    public class CheckAnimation : ConditionTask
    {
        [ParameterDefinition(AutoBindType.FindSimilarVariable)]
        [SerializeField] private Parameter<Animator> animator;
        [SerializeField] private Parameter<string> stateName;
        [SerializeField] private Parameter<int> layerIndex;

        public override string InfoC => $"Animation == {stateName}";

        public override bool CheckCondition()
        {
            return animator.Value.IsState(stateName, layerIndex);
        }
    }
}
