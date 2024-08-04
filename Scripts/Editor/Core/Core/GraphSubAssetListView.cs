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
        private readonly List<T> source;

        // Visual Elements
        private readonly ListViewBuilder<T, ParameterView> customListView;
        private readonly Label inspectorTitle;
        private InspectorElement inspector;

        public GraphSubAssetListView(GraphSubAsset owner, List<T> taskList, Z3ListViewConfig listConfig) : this(NodeGraphUtils.GetGraphData(owner), owner, taskList, listConfig)
        {

        }

        public GraphSubAssetListView(GraphData graphData, GraphSubAsset owner, List<T> taskList, Z3ListViewConfig listConfig) // editable == remove and reorder
        {
            style.backgroundColor = Color.black.SetAlpha(0.15f); // TEMP: Create a way to define the draw order

            // Add dependency
            this.graphData = graphData;
            assetParent = owner;
            source = taskList;

            if (listConfig.showAddBtn)
                listConfig.addEvent = () => TypeSelectorPopup<T>.OpenWindow(OnAdd);

            customListView = new ListViewBuilder<T, ParameterView>(taskList, listConfig);
            customListView.style.marginBottom = 8f;

            customListView.OnDelete += OnDeleteActionTask;

            // Add Visual Elements
            Add(customListView);

            // Create components and set style

            inspectorTitle = TitleBuilder.GetTitle();
            inspectorTitle.style.marginBottom = 8f;

            Add(inspectorTitle);

            // Update Selection
            customListView.OnSelectChange += OnChangeSelection;
            OnChangeSelection(taskList.FirstOrDefault());
        }

        private void OnAdd(string _, Type type) // TODO: Move it to NodeGraphModule
        {
            T newActionTask = ScriptableObject.CreateInstance(type) as T;
            newActionTask.Guid = GUID.Generate().ToString();
            newActionTask.name = $"{assetParent.name}/{type.Name} [{newActionTask.Guid}]"; // TODO: It could be a static method

            IEnumerable<FieldInfo> fields = ReflectionUtils.GetAllFieldsTypeOf<IParameter>(newActionTask);

            foreach (FieldInfo field in fields)
            {
                IParameter newParameter = Activator.CreateInstance(field.FieldType) as IParameter;
                field.SetValue(newActionTask, newParameter);

                ParameterDefinitionAttribute attribute = field.GetCustomAttribute<ParameterDefinitionAttribute>();
                if (attribute == null)
                    continue;

                switch (attribute.AutoBindType)
                {
                    case AutoBindType.SelfBind:
                        if (typeof(Component).IsAssignableFrom(field.FieldType))
                        {
                            newParameter.SelfBind();
                        }
                        break;

                    case AutoBindType.FindSameVariable:
                        // TODO
                        break;

                    case AutoBindType.FindSimilarVariable:
                        // TODO
                        break;
                }
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
            NodeGraphUtils.DeleteAsset(graphData, task);
            onDelete?.Invoke(task);
        }
    }
}