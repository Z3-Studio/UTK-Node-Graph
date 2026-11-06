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
        private readonly bool supportCopy;

        // Visual Elements
        private readonly ListViewBuilder<T, LabelView> customListView;
        private readonly Label inspectorTitle;
        private InspectorElement inspector;

        private static T itemClipboard;
        private static List<T> listClipboard = new();

        public GraphSubAssetListView(GraphSubAsset owner, IList<T> taskList, Z3ListViewConfig listConfig, bool supportCopy) : this(NodeGraphEditorUtils.GetGraphData(owner), owner, taskList, listConfig, supportCopy)
        {

        }

        public GraphSubAssetListView(GraphData graphData, GraphSubAsset owner, IList<T> taskList, Z3ListViewConfig listConfig, bool supportCopy) // editable == remove and reorder
        {
            style.backgroundColor = Color.black.SetAlpha(0.15f); // TEMP: Create a way to define the draw order

            // Add dependency
            this.graphData = graphData;
            assetParent = owner;
            source = taskList;
            this.supportCopy = supportCopy;

            if (listConfig.showAddBtn)
                listConfig.addEvent = () => TypeSelectorPopup<T>.OpenWindow(OnAdd);

            customListView = new ListViewBuilder<T, LabelView>((IList)taskList, listConfig); // TODO: Review this cast
            customListView.style.marginBottom = 8f;
            customListView.onBind += OnBind;

            customListView.OnDelete += OnDeleteActionTask;

            if (supportCopy)
            {
                customListView.AddManipulator(new ContextualMenuManipulator(evt =>
                {
                    if (evt.target is LabelView)
                        return;

                    DropdownMenu menu = evt.menu;
                    menu.AppendAction($"{typeof(T).Name} List", null, DropdownMenuAction.Status.Disabled);
                    menu.AppendSeparator();

                    menu.AppendAction($"Copy All ({source.Count})",
                        _ => 
                        { 
                            listClipboard = source.ToList(); 
                            itemClipboard = null; 
                        }, 
                        GetStatus(source.Count > 0)
                    );

                    List<T> copiedTasks = listClipboard.Where(CanPaste).ToList();
                    menu.AppendAction($"Paste All ({copiedTasks.Count})", _ => Paste(copiedTasks), GetStatus(copiedTasks.Count > 0));

                    menu.AppendSeparator();
                    menu.AppendAction("Paste as New", _ => Paste(new List<T> { itemClipboard }), GetStatus(CanPaste(itemClipboard)));
                }));
            }

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
            if (!supportCopy)
                return;

            view.AddManipulator(new ContextualMenuManipulator(evt =>
            {
                DropdownMenu menu = evt.menu;
                menu.AppendAction(typeof(T).Name, null, DropdownMenuAction.Status.Disabled);
                menu.AppendSeparator();

                menu.AppendAction($"Copy", action =>
                {
                    itemClipboard = element;
                    listClipboard = new();
                });

                menu.AppendAction($"Paste as New", _ => Paste(new List<T> { itemClipboard }), GetStatus(CanPaste(itemClipboard)));
                menu.AppendAction("Duplicate", _ => Paste(new List<T> { element }, element));
            }));
        }

        private bool CanPaste(T item) => item && graphData.CanCopy(item);

        private static DropdownMenuAction.Status GetStatus(bool enabled) => enabled ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled;

        // Inserts after anchor, or at the end when null
        private void Paste(List<T> items, T anchor = null)
        {
            int index = anchor ? source.IndexOf(anchor) + 1 : source.Count;

            foreach (T item in items)
            {
                NodeGraphEditorUtils.AddCopy(graphData, assetParent, source, item, index);
                index++;
            }

            customListView.Rebuild();
            Validator.Refresh(graphData);
        }

        private void OnAdd(string _, Type type) // TODO: Move it to NodeGraphModule
        {
            T newActionTask = ScriptableObject.CreateInstance(type) as T;

            string newAssetGuid = GUID.Generate().ToString();
            newActionTask.SetGuid(newAssetGuid, $"{assetParent.name}/");

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