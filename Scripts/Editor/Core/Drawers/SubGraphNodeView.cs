using UnityEngine.UIElements;
using Z3.NodeGraph.Core;
using Object = UnityEngine.Object;

namespace Z3.NodeGraph.Editor
{
    public class SubGraphNodeView : VisualElement
    {
        public ISubGraphNode Node;

        private VisualElement bindingContainer;

        private VariablesAsset graphVariableCache;
        private VariablesAsset subGraphVariableCache;

        public NodeGraphReferences References { get; }

        private BindableElement bindable;

        public SubGraphNodeView(NodeGraphReferences references, ISubGraphNode node, BindableElement b)
        {
            References = references;
            Node = node;
            bindable = b;
        }

        public void OnSelect()
        {

            bindingContainer = new VisualElement();

            Add(bindingContainer);

            RedrawBindingContainer();

            // Review it
            bindable.RegisterCallback<ChangeEvent<Object>>(OnBoolChangedEvent);

            void OnBoolChangedEvent(ChangeEvent<Object> evt)
            {
                RedrawBindingContainer();
            }
        }

        public void OnUpdateUI()
        {
            // Note: It could be removed from UpdateUI if find ChangeEvent for InspectorElement

            // Check if is using inspector
            if (bindingContainer == null)
                return;

            VariablesAsset graphVariable = References.Data.ReferenceVariables;
            VariablesAsset subGraphVariable = Node.SubGraph != null ? Node.SubGraph.ReferenceVariables : null;

            // Check if the cache is updated, if is not, redraw
            if (graphVariable == graphVariableCache && subGraphVariable == subGraphVariableCache)
                return;

            // Update cache
            graphVariableCache = graphVariable;
            subGraphVariableCache = subGraphVariable;

            RedrawBindingContainer();
        }

        public void OnUnselected()
        {
            bindingContainer = null;
        }

        private void RedrawBindingContainer()
        {
            bindingContainer.Clear();

            if (Node.SubGraph == null)
                return;

            if (subGraphVariableCache == null)
            {
                bindingContainer.Add(new Label("Your sub graph need variable if you want to propagate the variables to sub graph"));
                return;
            }

            if (graphVariableCache == null)
            {
                bindingContainer.Add(new Label("Your graph need variable if you want to propagate the variables to sub graph"));
                return;
            }

            if (graphVariableCache == subGraphVariableCache)
            {
                bindingContainer.Add(new Label("Full compatibility!"));
                return;
            }

            foreach (Variable variable in subGraphVariableCache.GetOriginalVariables())
            {
                bindingContainer.Add(new Label($"TODO: {variable.Name}"));
            }
        }
    }
}