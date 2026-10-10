using UnityEngine;
using UnityEditor;
using Z3.NodeGraph.Core;

namespace Z3.NodeGraph.Editor
{
    /// <summary>
    /// Initialize depedencies in editor
    /// </summary>
    [InitializeOnLoad]
    public static class NodeGraphEditorStartup
    {
        private const float LabelWidth = 20f;
        private static readonly Texture2D texturePanel;

        static NodeGraphEditorStartup()
        {
            NodeGraphResources.Init();
            UserPreferences.Init();
            NodeGraphStartup.Init(); // TypeResolver
            Validator.Init();

            WelcomeWindow.Init();

            // Draw icons in Hierarchy
            texturePanel = NodeGraphResources.GetIGraphIcon(GraphIcon.GraphRunner);
#if UNITY_6000_4_OR_NEWER
            EditorApplication.hierarchyWindowItemByEntityIdOnGUI += OnHierarchyItem;
#else
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyItem;
#endif
        }

#if UNITY_6000_4_OR_NEWER
        private static void OnHierarchyItem(EntityId entityId, Rect selectionRect)
        {
            DrawRunnerIcon(EditorUtility.EntityIdToObject(entityId) as GameObject, selectionRect);
        }
#else
        private static void OnHierarchyItem(int instanceID, Rect selectionRect)
        {
            DrawRunnerIcon(EditorUtility.InstanceIDToObject(instanceID) as GameObject, selectionRect);
        }
#endif

        private static void DrawRunnerIcon(GameObject go, Rect selectionRect)
        {
            if (!go || go.GetComponent<IGraphRunner>() == null)
                return;

            Rect r = selectionRect;
            r.xMin = r.xMax - LabelWidth;
            GUI.Label(r, texturePanel);
        }
    }
}