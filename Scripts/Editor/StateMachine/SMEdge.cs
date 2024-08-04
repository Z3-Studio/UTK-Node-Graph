using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;
using Z3.NodeGraph.StateMachine;
using Z3.UIBuilder.Editor;

namespace Z3.NodeGraph.Editor
{
    /// <summary>
    /// Used to reference input and output ports
    /// </summary>
    public class SMEdge : NGEdge<SMEdgeControl>
    {
        internal TransitableStateNode InputNode => InputNodeView.Node;
        internal StateMachineNode OutputNode => OutputNodeView.Node;

        internal SMTransitableNodeView InputNodeView => input.GetNodeView<SMTransitableNodeView>();
        internal SMNodeView OutputNodeView => output.GetNodeView<SMNodeView>();

        private NodeGraphReferences references;
        internal NodeGraphReferences References
        {
            get
            {
                // TEMP: Try to find a way to save this variable at constructor or init method
                references ??= InputNodeView.References;
                return references;
            } 
        }

        public SMEdge() : base()
        {
            NgEdgeControl.Init(this);

            RegisterCallback<MouseEnterEvent>((e) => NgEdgeControl.MouseEvent(true));
            RegisterCallback<MouseLeaveEvent>((e) => NgEdgeControl.MouseEvent(false));
        }

        public override void OnSelected()
        {
            base.OnSelected();
            NgEdgeControl.Select();
            References.InvokeUpdateSelection(this);
        }

        public override void OnUnselected()
        {
            base.OnUnselected();
            NgEdgeControl.Unselect();
        }

        public override VisualElement GetInspector()
        {
            List<Transition> transitions = OutputNode.GetTransitions().Where(t => t.Connection == InputNode).ToList();

            Z3ListViewConfig transition = new("Transitions")
            {
                showAddBtn = false,
                toStringWithPrefix = false,
                showReordable = false,
                showRemoveButton = false, // TODO: Review it
            };

            GraphSubAssetListView<Transition> transitionListElement = new(References.Data, OutputNode, transitions, transition);
            VisualElement transitionContainer = new();
            transitionListElement.Add(transitionContainer);

            return transitionListElement;
        }

        public override void DeleteElement()
        {
            // Delete transition
            if (OutputNode is IOutputNode fsmParent)
            {
                List<Transition> transitions = fsmParent.Transitions.Get();
                List<Transition> itemsToRemove = transitions.Where(c => c.Connection == InputNode).ToList();

                foreach (Transition transition in itemsToRemove)
                {
                    transitions.Remove(transition);
                }

                NodeGraphUtils.DeleteAssets(References.Data, itemsToRemove);
            }
        }
    }
}