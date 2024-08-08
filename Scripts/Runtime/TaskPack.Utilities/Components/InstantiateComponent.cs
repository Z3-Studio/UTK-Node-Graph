using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities.Components
{
    [NodeCategory(Categories.Components)]
    [NodeDescription("This class allows to use Quaterion")]
    public class InstantiateComponent<T> : ActionTask where T : Component
    {
        [Header("Spawn Pooled Object")]
        [SerializeField] private Parameter<T> prefab;
        [SerializeField] private Parameter<Vector3> position = Vector3.zero;
        [SerializeField] private Parameter<Quaternion> rotation = Quaternion.identity;
        [SerializeField] private Parameter<Transform> parent = null;

        [Header("Out")]
        [SerializeField] private Parameter<T> returnedObject;

        public override string Info => prefab.Value == null ?
            base.Info : $"Instantiate {prefab}";

        protected override void StartAction()
        {
            returnedObject = Instantiate(prefab.Value, position.Value, rotation.Value, parent.Value);
            EndAction();
        }
    }
}