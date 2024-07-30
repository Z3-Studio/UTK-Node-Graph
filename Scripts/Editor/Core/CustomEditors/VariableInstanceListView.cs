using System.Collections.Generic;
using UnityEngine.UIElements;
using Z3.NodeGraph.Core;
using Z3.UIBuilder.Editor;

namespace Z3.NodeGraph.Editor
{
    public class VariableInstanceListView : VisualElement
    {
        private List<VariableInstance> targetList;

        private ListViewBuilder<VariableInstance, VariableInstanceView> customListView;

        public VariableInstanceListView(VariableInstanceList source) : this(source.Values) { }

        public VariableInstanceListView(List<VariableInstance> source)
        {
            targetList = source;

            Z3ListViewConfig listConfig = new()
            {
                listName = "Variables Instance",
                showAddBtn = false,
                showRemoveButton = false,
                showReordable = false,
                showFoldout = false,
                selectable = false,
                onMakeItem = OnMake,
                onBind = OnBind
            };

            customListView = new ListViewBuilder<VariableInstance, VariableInstanceView>(source, listConfig);
            Add(customListView);
        }

        private VariableInstanceView OnMake() => new();

        private void OnBind(VisualElement visualElement, int i)
        {
            VariableInstanceView variableElement = visualElement as VariableInstanceView;

            variableElement.BuildItem(targetList[i]);
        }
    }
}