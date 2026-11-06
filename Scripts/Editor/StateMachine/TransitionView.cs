using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using Z3.NodeGraph.StateMachine;
using Z3.NodeGraph.Core;
using Z3.UIBuilder.Core;
using Z3.UIBuilder.ExtensionMethods;
using Z3.Utils.ExtensionMethods;

namespace Z3.NodeGraph.Editor
{
    public class TransitionView : GraphElement, INodeGraphElement
    {
        [UIElement] private readonly Label label;
        [UIElement] private readonly VisualElement stateBorder;
        [UIElement] private readonly VisualElement selectionBorder;

        public Transition Transition { get; }
        public NodeGraphReferences References { get; }
        internal SMEdge Edge { get; } // The label is in a layer above the nodes, outside the edge
        private IOutputNode Owner { get; }

        private bool active;
        private float delayColor;
        private State lastState = State.Ready;

        private const float Delay = SMNodeView.Delay;
        private const float LabelOffset = -10f;

        public GraphElement Self => this;
        private static VisualTreeAsset SmTransitionLabel => NodeGraphResources.SmTransitionLabel;

        public TransitionView(Transition transition, SMEdge edge)
        {
            Transition = transition;
            Edge = edge;
            Owner = edge.OutputNode as IOutputNode;
            References = edge.References;

            SmTransitionLabel.CloneTree(this);

            style.SetMargin(0f);
            style.SetPadding(0f);

            pickingMode = PickingMode.Ignore;
            this.BindUIElements();

            SetLabelInfo();

            // Not movable: the selection dragger would move it
            capabilities = Capabilities.Selectable | Capabilities.Deletable;

            selectionBorder.AddManipulator(new TransitionDragManipulator(this));
            selectionBorder.AddManipulator(new Clickable(() =>
            {
                // The click doesn't reach the ClickSelector
                References.Graph.ClearSelection();
                References.Graph.AddToSelection(this);
            }));
        }

        public override void OnSelected()
        {
            base.OnSelected();
            References.InvokeUpdateSelection(this);
        }

        public override void OnUnselected()
        {
            base.OnUnselected();
            References.InvokeUpdateSelection(null);
        }

        private void SetLabelInfo()
        {
            // Adds line break for each transition
            const string c = " && </color>";
            const string n = c + "\n";
            label.text = Transition.Info.Replace(c, n);
        }

        internal void UpdateLayout(float angle, float sinAngle)
        {
            SetLabelInfo();

            Vector2 translateOffset = -label.layout.size * 0.5f;
            translateOffset += new Vector2(LabelOffset, LabelOffset);
            float value = Mathf.Lerp(translateOffset.x, translateOffset.y, Mathf.Abs(sinAngle));

            // Set as l
            style.width = label.layout.width;
            style.height = label.layout.height;
            style.rotate = new Rotate(-angle);
            style.translate = new Translate(value, 0f);
        }

        internal State UpdateState(bool activeColor)
        {
            State newState = Transition.State;

            if (newState != lastState)
            {
                RemoveFromClassList(lastState.ToString().ToLower());
                AddToClassList(newState.ToString().ToLower());

                lastState = newState;
                delayColor = Time.time + Delay;
            }
            else if (!active && activeColor)
            {
                AddToClassList(newState.ToString().ToLower());
                delayColor = Time.time + Delay;
            }
            else if (!activeColor && Time.time > delayColor)
            {
                RemoveFromClassList(newState.ToString().ToLower());
            }

            active = activeColor;
            return newState;
        }

        [System.Obsolete("Temp")]
        internal Color GetColor() => stateBorder.resolvedStyle.backgroundColor;

        public void DeleteElement()
        {
            UndoRecorder.AddUndo(Owner as Object, "Delete Transition");
            Owner.Transitions.Get().Remove(Transition);
            NodeGraphEditorUtils.DeleteAsset(References.Data, Transition);

            // The edge may have no transition left, or a different arrow
            EditorApplication.delayCall += References.Refresh;
        }

        public VisualElement GetInspector() => Transition.CreateNgInspector();
    }
}