using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using Z3.NodeGraph.StateMachine;
using Z3.NodeGraph.Core;
using Z3.UIBuilder.Editor.ExtensionMethods;
using Z3.Utils.ExtensionMethods;
using Z3.Utils;

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

        private const float ArrowSingleTransitionSize = 15f;
        private const float ArrowMultipleTransitionSize = 20f;
        private const float ArrowHoverScale = 1.6f;
        private const float CircularConnectionOffset = 20f;

        private const float CapRadius = 4f;
        private const float InterceptWidth = 6f;

        public SMEdgeControl() : base() 
        {
            capRadius = CapRadius;
            interceptWidth = InterceptWidth;

            // Arrow
            arrow = new VisualElement() { name = "arrow" };
            arrow.style.backgroundSize = new BackgroundSize(BackgroundSizeType.Contain);

            arrow.style.position = Position.Absolute;
            arrow.style.translate = new Translate(Length.Percent(-50f), Length.Percent(-50f));
            arrow.style.top = Length.Percent(50f);
            arrow.style.left = Length.Percent(50f);

            // Label
            labelContainer = new VisualElement() 
            { 
                name = "transitions", 
                pickingMode = PickingMode.Ignore 
            };

            labelContainer.style.SetPosition(Length.Percent(0f));
            labelContainer.style.position = Position.Absolute;

            labelContainer.style.unityTextAlign = TextAnchor.MiddleCenter;
            labelContainer.style.alignItems = Align.Center;
            labelContainer.style.justifyContent = Justify.SpaceAround;
            labelContainer.style.flexBasis = Length.Percent(100f);
            labelContainer.style.flexDirection = FlexDirection.ColumnReverse; // Display correct check order
            labelContainer.style.minWidth = 1f;

            container = new VisualElement() 
            { 
                name = "container", 
                pickingMode = PickingMode.Ignore 
            };

            container.Add(arrow);
            container.Add(labelContainer);

            SetArrowColor(UnselectedColor);

            this.RegisterUpdate(UpdateState);
        }

        internal void Init(SMEdge smEdge)
        {
            this.smEdge = smEdge;
            smEdge.Add(container);
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
            bool circularConnection = InputNode.GetTransitionsSafe().Any(t => t.Connection == OutputNode);

            if (circularConnection)
            {
                ApplyCircularOffset();
            }

            // Get rotation
            Vector2 direction = from - to;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            angle = MathUtils.NormalizeAngle(angle);

            UpdateContainer();
            UpdateArrow(transitions, angle);
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

        /// <summary> 
        /// This prevents overlapping of the transition line when there is a circular connection, moving and separating the lines to ensure visibility in graph
        /// </summary>
        /// <remarks>
        /// In other words, it will add a space when the Input is connected to the Output at the same time the Output is connected to the Input
        /// </remarks>
        private void ApplyCircularOffset()
        {
            Vector2 outputStart = from;
            Vector2 outputEnd = to;

            Vector2 offsetDirection = (outputEnd - outputStart).normalized * CircularConnectionOffset;
            Vector2 perpendicular = new(offsetDirection.y, -offsetDirection.x);

            outputStart += perpendicular;
            outputEnd += perpendicular;

            from = outputStart;
            to = outputEnd;
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

        private void SetArrowColor(Color color)
        {
            arrow.style.unityBackgroundImageTintColor = color;
        }

        private void UpdateArrow(List<Transition> transition, float angle)
        {
            Texture2D icon;
            float iconSize;
            if (transition.Count > 1) // TODO: Use event
            {
                iconSize = ArrowMultipleTransitionSize;
                icon = NodeGraphResources.MultipleArrowTransition;
            }
            else
            {
                iconSize = ArrowSingleTransitionSize;
                icon = NodeGraphResources.ArrowTransition;
            }

            // Arrow Icon
            arrow.style.height = iconSize;
            arrow.style.width = iconSize;
            arrow.style.backgroundImage = new StyleBackground(icon);
            arrow.style.rotate = new Rotate(angle);
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

                TransitionView transitionView = new TransitionView(transition, OutputNode as IOutputNode, smEdge.References);
                labelContainer.Add(transitionView);
                labels.Add(transition, transitionView);
            }
        }

        /// <summary>
        /// Returns a perimeter point from the angle
        /// </summary>
        public static Vector2 GetPointInPerimeter(Vector2 start, Vector2 end, Vector2 boxSize)
        {
            Vector2 direction = end - start;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            float halfWidth = boxSize.x * 0.5f;
            float halfHeight = boxSize.y * 0.5f;
            float radians = angle * Mathf.Deg2Rad;
            float cos = Mathf.Cos(radians);
            float sin = Mathf.Sin(radians);

            float x, y;
            if (Mathf.Abs(cos) > Mathf.Abs(sin)) // use horizontal edge
            {
                x = halfWidth * Mathf.Sign(cos);
                y = x * sin / cos;

                if (Mathf.Abs(y) > halfHeight) // adjust if near vertical edge
                {
                    y = Mathf.Sign(y) * halfHeight;
                    x = y * cos / sin;
                }
            }
            else // use vertical edge
            {
                y = halfHeight * Mathf.Sign(sin);
                x = y * cos / sin;

                if (Mathf.Abs(x) > halfWidth) // adjust if near horizontal edge
                {
                    x = Mathf.Sign(x) * halfWidth;
                    y = x * sin / cos;
                }
            }

            return start + new Vector2(x, y);
        }
    }
}