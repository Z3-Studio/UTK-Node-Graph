using UnityEditor;
using UnityEngine.UIElements;
using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using Z3.UIBuilder.Editor;

namespace Z3.NodeGraph.Editor
{
    public class TaskListPropertyDrawer<TTask> : Z3PropertyDrawer<TaskList<TTask>> where TTask : Task 
    {
        protected override VisualElement CreateVisualElement()
        {
            GraphSubAsset assetParent = GetTarget<GraphSubAsset>();

            if (assetParent == null)
                return base.CreateVisualElement();

            // Build list
            Z3ListViewConfig listConfig = new Z3ListViewConfig(SerializedProperty.displayName);
            return new GraphSubAssetListView<TTask>(assetParent, ResolvedValue, listConfig);
        }
    }

    [CustomPropertyDrawer(typeof(ConditionTaskList), true)]
    public class ConditionTaskListDrawer : TaskListPropertyDrawer<ConditionTask> { }

    [CustomPropertyDrawer(typeof(ActionTaskList), true)]
    public class ActionTaskListDrawer : TaskListPropertyDrawer<ActionTask> { }
}