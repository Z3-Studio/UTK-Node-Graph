using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using Z3.NodeGraph.StateMachine;
using Z3.NodeGraph.Core;
using Z3.UIBuilder.Core;
using Z3.UIBuilder.ExtensionMethods;

namespace Z3.NodeGraph.Editor
{
    // TODO: inherit from GraphElement
    public class TransitionView : VisualElement, INodeGraphElement
    {
        [UIElement] private readonly Label label;
        [UIElement] private readonly VisualElement stateBorder;
        [UIElement] private readonly VisualElement selectionBorder;

        private bool active;
        private float delayColor;
        private State lastState = State.Ready;

        private const float Delay = SMNodeView.Delay;
        private const float LabelOffset = -10f;

        private Transition Transition { get; }
        private IOutputNode Owner { get; }
        public NodeGraphReferences References { get; }

        public GraphElement Self => throw new System.NotImplementedException();
        private StateMachineGraphModule Module => References.Module as StateMachineGraphModule;
        private static VisualTreeAsset SmTransitionLabel => NodeGraphResources.SmTransitionLabel;

        public TransitionView(Transition transition, IOutputNode owner, NodeGraphReferences references) 
        {
            Transition = transition;
            Owner = owner;
            References = references;

            SmTransitionLabel.CloneTree(this);

            pickingMode = PickingMode.Ignore;
            this.BindUIElements();

            SetLabelInfo();
            selectionBorder.AddManipulator(new Clickable(() =>
            {
                Module.Inspect(GetInspector());
            }));
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
            throw new System.NotImplementedException("Test it");
            Owner.Transitions.SubAssets.Remove(Transition); // Review it
            Module.DeleteAsset(Transition);
        }

        public VisualElement GetInspector() => Transition.CreateNgInspector();
    }
}