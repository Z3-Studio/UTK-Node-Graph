using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Transform)]
    public class CopyTransformValues : ActionTask
    {
        [SerializeField] private Parameter<Transform> transformToCopy;
        [SerializeField] private Parameter<Transform> transformToPaste;

        protected override void StartAction()
        {
            Transform copyTransform = transformToCopy.Value;
            Transform pasteTransform = transformToPaste.Value;

            pasteTransform.position = copyTransform.position;
            pasteTransform.rotation = copyTransform.rotation;
            pasteTransform.localScale = copyTransform.localScale;
            EndAction();
        }
    }
}
