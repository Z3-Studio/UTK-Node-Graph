using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using Z3.NodeGraph.Core;
using Z3.UIBuilder.Editor;
using Z3.Utils.ExtensionMethods;
using Z3.Utils;
using System.Collections;

namespace Z3.NodeGraph.Editor
{
    /// <summary>
    /// Display lists inside the inspector panel. Task list and Transitions
    /// </summary>
    public class GraphSubAssetListView<T> : VisualElement where T : GraphSubAsset
    {
        [Obsolete("TEMP")]
        public event Action<T> onDelete;

        // Core
        private readonly GraphData graphData;
        private readonly GraphSubAsset assetParent;
        private readonly IList<T> source;

        // Visual Elements
        private readonly ListViewBuilder<T, LabelView> customListView;
        private readonly Label inspectorTitle;
        private InspectorElement inspector;

        private static T clipboard;

        public GraphSubAssetListView(GraphSubAsset owner, IList<T> taskList, Z3ListViewConfig listConfig) : this(NodeGraphEditorUtils.GetGraphData(owner), owner, taskList, listConfig)
        {

        }

        public GraphSubAssetListView(GraphData graphData, GraphSubAsset owner, IList<T> taskList, Z3ListViewConfig listConfig) // editable == remove and reorder
        {
            style.backgroundColor = Color.black.SetAlpha(0.15f); // TEMP: Create a way to define the draw order

            // Add dependency
            this.graphData = graphData;
            assetParent = owner;
            source = taskList;

            if (listConfig.showAddBtn)
                listConfig.addEvent = () => TypeSelectorPopup<T>.OpenWindow(OnAdd);

            customListView = new ListViewBuilder<T, LabelView>((IList)taskList, listConfig); // TODO: Review this cast
            customListView.style.marginBottom = 8f;
            customListView.onBind += OnBind;

            customListView.OnDelete += OnDeleteActionTask;

            // Add Visual Elements
            Add(customListView);

            // Create components and set style

            inspectorTitle = new TitleView();
            inspectorTitle.style.marginBottom = 8f;

            Add(inspectorTitle);

            // Update Selection
            customListView.OnSelectChange += OnChangeSelection;
            OnChangeSelection(taskList.FirstOrDefault());
        }

        private void OnBind(LabelView view, T element, int i)
        {
            view.AddManipulator(new ContextualMenuManipulator(evt =>
            {
                DropdownMenu menu = evt.menu;
                menu.AppendAction(typeof(T).Name, null, DropdownMenuAction.Status.Disabled);
                menu.AppendSeparator();

                menu.AppendAction($"Copy", action =>
                {
                    clipboard = element;
                });

                bool canPaste = clipboard && graphData.CanCopy(clipboard);

                menu.AppendAction($"Paste as New", actionEvent =>
                {
                    NodeGraphEditorUtils.AddCopy(graphData, source, clipboard);
                    customListView.Rebuild();
                    Validator.Refresh(graphData);

                }, canPaste ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);
            }));
        }

        private void OnAdd(string _, Type type) // TODO: Move it to NodeGraphModule
        {
            T newActionTask = ScriptableObject.CreateInstance(type) as T;
            newActionTask.SetGuid(GUID.Generate().ToString(), $"{assetParent.name}/");

            IEnumerable<FieldInfo> fields = ReflectionUtils.GetAllFieldsTypeOf<IParameter>(newActionTask);

            foreach (FieldInfo field in fields)
            {
                NodeGraphEditorUtils.TryAutoBind(graphData, newActionTask, field);
            }

            source.Add(newActionTask);

            graphData.AddSubAsset(newActionTask);

            // After creation, the list must rebuild
            customListView.Rebuild();

            // Before to add and remember
            AssetDatabase.AddObjectToAsset(newActionTask, graphData);
            AssetDatabase.SaveAssets();
        }

        public void SetSelection(T item) => customListView.SetSelection(item);

        private void OnChangeSelection(T taskToDraw)
        {
            // Note: Use Add and Remove have a better performance than change the style.display
            if (inspector != null)
            {
                Remove(inspector);
                inspector = null;
            }

            if (taskToDraw == null)
            {
                inspectorTitle.text = "Empty";
                return;
            }

            inspector = new InspectorElement();
            inspector.style.SetPadding(0f);
            Add(inspector);

            inspectorTitle.text = taskToDraw.GetTypeNiceString();
            SerializedObject serializedObject = new SerializedObject(taskToDraw);
            inspector.Bind(serializedObject);
            HideInGraphBuilder.HideObjects(taskToDraw, inspector);
        }

        protected virtual void OnDeleteActionTask(T task)
        {
            NodeGraphEditorUtils.DeleteAsset(graphData, task);
            onDelete?.Invoke(task);
        }
    }
}