using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using Z3.NodeGraph.StateMachine;
using Z3.NodeGraph.Core;
using Z3.UIBuilder.Editor.ExtensionMethods;
using Z3.Utils.ExtensionMethods;

namespace Z3.NodeGraph.Editor
{
    /// <summary>
    /// Transition view component used to draw the line, arrow and condition labels
    /// </summary>
    public class SMEdgeControl : NgEdgeControl
    {
        private readonly VisualElement container;
        private readonly VisualElement arrow;
        private readonly VisualElement labelContainer;
        private SMEdge smEdge;

        // Labels above the nodes: a GraphView layer only applies to elements added to the graph, not inside the edge
        private readonly LabelGroup labelGroup;
        private const int LabelLayer = 10;

        private readonly Dictionary<Transition, TransitionView> labels = new();

        private float transitionActivation;

        // SM Edge
        private Port InputPort => smEdge.input;
        private Port OutputPort => smEdge.output;
        private TransitableStateNode InputNode => smEdge.InputNode;
        private StateMachineNode OutputNode => smEdge.OutputNode;
        private SMTransitableNodeView InputView => smEdge.InputNodeView;
        private SMNodeView OutputView => smEdge.OutputNodeView;

        private Color SelectedColor => smEdge.selectedColor;
        private readonly Color UnselectedColor = new Color(0.7843138f, 0.7843138f, 0.7843138f);
        private readonly Vector2 PortOffset = new(7.5f, 15f);

        private const float ArrowHoverScale = 1.6f;

        // Connection preview
        private SMEdgeControl previewReverse;
        private bool previewCircularOffset;

        private const float CapRadius = 4f;
        private const float InterceptWidth = 6f;

        public SMEdgeControl() : base() 
        {
            capRadius = CapRadius;
            interceptWidth = InterceptWidth;

            // Arrow
            arrow = SMEdgeUtils.CreateArrow();
            arrow.style.top = Length.Percent(50f);
            arrow.style.left = Length.Percent(50f);

            // Label
            labelContainer = new VisualElement() 
            { 
                name = "transitions", 
                pickingMode = PickingMode.Ignore,
                style =
                {
                    position = Position.Absolute,
                    unityTextAlign = TextAnchor.MiddleCenter,
                    alignItems = Align.Center,
                    justifyContent = Justify.SpaceAround,
                    flexBasis = Length.Percent(100f),
                    flexDirection = FlexDirection.ColumnReverse, // Display correct check order
                    minWidth = 1f
                }
            };

            labelContainer.style.SetPosition(Length.Percent(0f));

            container = new VisualElement() 
            { 
                name = "container", 
                pickingMode = PickingMode.Ignore 
            };

            container.Add(arrow);

            labelGroup = new LabelGroup
            {
                pickingMode = PickingMode.Ignore,
                layer = LabelLayer,
                style =
                {
                    position = Position.Absolute
                }
            };

            labelGroup.Add(labelContainer);

            SetArrowColor(UnselectedColor);

            this.RegisterUpdate(UpdateState);
        }

        internal void Init(SMEdge smEdge)
        {
            this.smEdge = smEdge;
            smEdge.Add(container);

            smEdge.RegisterCallback<DetachFromPanelEvent>(_ =>
            {
                SMEdgeUtils.SetPreviewOffset(ref previewReverse, null);
                labelGroup.RemoveFromHierarchy();
            });
        }

        internal void SetPreviewCircularOffset(bool value)
        {
            previewCircularOffset = value;
            smEdge.UpdateEdgeControl();
        }

        internal void Unselect() => SetArrowColor(UnselectedColor);

        internal void Select() => SetArrowColor(SelectedColor);

        internal void MouseEvent(bool enter)
        {
            float scale = enter ? ArrowHoverScale : 1f;
            arrow.style.scale = scale.ToVector2();
        }

        private void UpdateState()
        {
            if (!Application.isPlaying || smEdge.output == null || smEdge.input == null)
                return;

            TransitionList transitionList = smEdge.OutputNode.GetTransitionList();
            bool activeColor = transitionActivation != transitionList.ActivationTime || transitionList.Active;

            transitionActivation = transitionList.ActivationTime;

            TransitionView bestMatch = labels.Values.FirstOrDefault();

            foreach (TransitionView transitionView in labels.Values)
            {
                State transitionState = transitionView.UpdateState(activeColor);
                if (transitionState == State.Success)
                {
                    bestMatch = transitionView;
                }
            }

            // TODO: Find a better way
            Color color = UnselectedColor;

            if (bestMatch != null)
            {
                Color mainColor = bestMatch.GetColor();
                color = Color.Lerp(UnselectedColor, mainColor, mainColor.a);
            }

            // Set IO Color
            inputColor = color;
            outputColor = color;
            arrow.style.unityBackgroundImageTintColor = color;
        }

        protected override void ComputeControlPoints()
        {
            base.ComputeControlPoints();
            RemoveCurves();
        }

        /// <summary> Called by <see cref="Edge.UpdateEdgeControl"/> </summary>
        public override void UpdateLayout()
        {
            // TODO: Instead use update, you could use events
            base.UpdateLayout();

            if (OutputPort != null)
            {
                from = GetPortPosition(OutputPort, OutputView.Layout);
            }

            if (InputPort != null)
            {
                to = GetPortPosition(InputPort, InputView.Layout);
            }

            if (InputPort == null || OutputPort == null)
                return;

            SMTransitableNodeView input = smEdge.InputNodeView;

            List<Transition> transitions = OutputNode.GetTransitions().Where(t => t.Connection == InputNode).ToList();
            bool circularConnection = previewCircularOffset ||InputNode.GetTransitionsSafe().Any(t => t.Connection == OutputNode);

            if (smEdge.isGhostEdge)
            {
                // Hover look: visible over an existing edge between the same nodes
                MouseEvent(true);

                // The existing reverse edge also gets the offset
                SMEdge reverseEdge = circularConnection ? SMEdgeUtils.FindEdge(smEdge.References.Module, InputNode, OutputNode) : null;
                SMEdgeUtils.SetPreviewOffset(ref previewReverse, reverseEdge);
            }

            if (circularConnection)
            {
                // Apply Circular Offset, this prevents overlapping of the transition line when there is a circular connection
                Vector2 offset = SMEdgeUtils.GetCircularOffset(from, to);
                from += offset;
                to += offset;
            }

            // Get rotation
            Vector2 direction = from - to;
            float angle = SMEdgeUtils.GetAngle(from, to);

            UpdateContainer();

            // Ghost: counts the transitions being added
            int transitionCount = transitions.Count + (smEdge.isGhostEdge ? smEdge.GhostTransitionCount : 0);
            SMEdgeUtils.SetArrowIcon(arrow, transitionCount > 1); // TODO: Use event
            arrow.style.rotate = new Rotate(angle);

            // Ghost: the existing edge already shows the labels
            if (smEdge.isGhostEdge)
                return;

            UpdateLabelGroup();
            UpdateLabelContainer(input, direction, angle);
            UpdateLabels(transitions, angle);
        }

        private void RemoveCurves()
        {
            // Remove from default layout
            if (controlPoints != null && controlPoints.Length >= 4)
            {
                controlPoints[1] = controlPoints[0];
                controlPoints[2] = controlPoints[3];
            }
        }

        /// <summary> Used to move the connection point to the center of the node </summary>
        private Vector2 GetPortPosition(Port port, Rect layout)
        {
            Vector2 nodeOffset = layout.size * 0.5f;
            Vector2 position = port.GetGlobalCenter();
            return smEdge.WorldToLocal(position) + PortOffset + nodeOffset;
        }

        private void UpdateContainer()
        {
            Vector2 sizeC = (to - from).Abs();
            container.style.minWidth = sizeC.x;
            container.style.minHeight = sizeC.y;
            container.style.maxWidth = sizeC.x;
            container.style.maxHeight = sizeC.y;

            Vector2 position = from + (to - from - sizeC) * 0.5f;
            container.style.left = position.x;
            container.style.top = position.y;
        }

        /// <summary> Same box as the container, in the label layer </summary>
        private void UpdateLabelGroup()
        {
            if (labelGroup.parent == null)
            {
                smEdge.References.Graph.AddElement(labelGroup);
            }

            Vector2 size = (to - from).Abs();
            Vector2 position = from + (to - from - size) * 0.5f;
            // Layers sit at the content origin. The content already has a layout when the layer is new
            Vector2 layerPosition = smEdge.References.Graph.contentViewContainer.WorldToLocal(smEdge.LocalToWorld(position));

            labelGroup.style.minWidth = size.x;
            labelGroup.style.minHeight = size.y;
            labelGroup.style.maxWidth = size.x;
            labelGroup.style.maxHeight = size.y;
            labelGroup.style.left = layerPosition.x;
            labelGroup.style.top = layerPosition.y;
        }

        private void SetArrowColor(Color color)
        {
            arrow.style.unityBackgroundImageTintColor = color;
        }

        private void UpdateLabelContainer(SMTransitableNodeView input, Vector2 direction, float angle)
        {
            float directionMagnitude = direction.magnitude;
            float padding = input.Layout.size.x * 0.5f;

            float cosAngle = Mathf.Cos(angle * Mathf.Deg2Rad);
            float mappedValue = Mathf.InverseLerp(-1f, 1f, -Mathf.Abs(cosAngle));
            float y = mappedValue * -direction.magnitude;

            labelContainer.style.minHeight = directionMagnitude;
            labelContainer.style.maxHeight = directionMagnitude;
            labelContainer.style.paddingTop = padding;
            labelContainer.style.paddingBottom = padding;

            labelContainer.style.translate = new Translate(0, y);
            labelContainer.style.rotate = new Rotate(angle);
        }

        private void UpdateLabels(List<Transition> transitions, float angle)
        {
            float sinAngle = Mathf.Sin(angle * Mathf.Deg2Rad);

            foreach (Transition transition in transitions)
            {
                if (labels.TryGetValue(transition, out TransitionView existingLabel))
                {
                    existingLabel.UpdateLayout(angle, sinAngle);
                    continue;
                }

                TransitionView transitionView = new TransitionView(transition, smEdge);
                labelContainer.Add(transitionView);
                labels.Add(transition, transitionView);
            }
        }

        /// <summary> Holds the labels in a GraphView layer </summary>
        private class LabelGroup : GraphElement
        {
            public LabelGroup()
            {
                style.SetMargin(0f);
                style.SetPadding(0f);
            }
        }
    }
}