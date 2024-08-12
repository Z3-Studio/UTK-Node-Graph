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

            customListView = new ListViewBuilder<T, LabelView>(taskList, listConfig);
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
                IParameter parameter = field.GetValue(newActionTask) as IParameter;
                if (parameter == null)
                {
                    parameter = Activator.CreateInstance(field.FieldType) as IParameter;
                    field.SetValue(newActionTask, parameter);
                }

                ParameterDefinitionAttribute attribute = field.GetCustomAttribute<ParameterDefinitionAttribute>();
                AutoBindType bindType = UserPreferences.DefaultAutoBindType;

                if (attribute != null)
                {
                    bindType = attribute.AutoBindType;
                }

                if (bindType == AutoBindType.SelfBind)
                {
                    if (parameter.CanSelfBind())
                    {
                        parameter.SelfBind();
                    }
                    else
                    {
                        Debug.LogError($"Self-binding is not supported for type '{parameter.GenericType.Name}'. Check the '{nameof(ParameterDefinitionAttribute)}' in class '{type.Name}'.");
                    }
                }
                else if (bindType == AutoBindType.FindSameVariable)
                {
                    Variable variable = graphData.GetVariables().FirstOrDefault(v => v.name == field.Name);

                    if (variable != null && TypeResolver.CanConvert(parameter, variable))
                    {
                        parameter.Bind(variable);
                    }
                }
                else if (bindType == AutoBindType.FindSimilarVariable)
                {
                    string similarName = field.Name.ToLower().Replace(" ", string.Empty);
                    Variable variable = graphData.GetVariables().FirstOrDefault(v => v.name.ToLower().Replace(" ", string.Empty) == similarName);

                    if (variable != null && TypeResolver.CanConvert(parameter, variable))
                    {
                        parameter.Bind(variable);
                    }
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
            NodeGraphEditorUtils.DeleteAsset(graphData, task);
            onDelete?.Invoke(task);
        }
    }
}