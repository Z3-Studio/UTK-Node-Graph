using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Transform)]
    [NodeDescription("GameObject.SetActive(active)")]
    public class SetActiveGameObject : ActionTask
    {
        [ParameterDefinition(AutoBindType.SelfBind)]
        [SerializeField] private Parameter<GameObject> gameObject;
        [SerializeField] private Parameter<bool> active;

        public override string Info => $"{gameObject}.Active = {active}";

        protected override void StartAction() 
        {
            gameObject.Value.SetActive(active.Value);
            EndAction();
        }
    }
}