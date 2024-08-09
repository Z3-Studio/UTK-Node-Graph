﻿using UnityEngine;
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
            texturePanel = NodeGraphResources.GetIGraphIcon(GraphIcon.GraphRunner);
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyItem;

            NodeGraphStartup.Init();
            Validator.Init();
        }

        private static void OnHierarchyItem(int instanceID, Rect selectionRect)
        {
            GameObject go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;

            if (go && go.GetComponent<IGraphRunner>() != null)
            {
                Rect r = new Rect(selectionRect);
                r.x = r.max.x - LabelWidth;
                r.width = LabelWidth;

                GUI.Label(r, texturePanel);
            }
        }
    }
}