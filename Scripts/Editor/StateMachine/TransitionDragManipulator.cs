using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Z3.NodeGraph.StateMachine;

namespace Z3.NodeGraph.Editor
{
    /// <summary> Moves a transition end to another node. Edge: all transitions. Label: only its transition </summary>
    /// <remarks> Replaces GraphView's edge drag, which recreates the edge and loses the conditions </remarks>
    internal class TransitionDragManipulator : Manipulator
    {
        private readonly TransitionView label; // Null on the edge

        private bool IsLabel => label != null;
        private SMEdge Edge => IsLabel ? label.Edge : (SMEdge)target;

        private bool pointerPressed;
        private bool canDrag;
        private int pointerId;
        private Vector2 pressPosition;

        private bool dragging;
        private bool moveTarget;
        private Vector2 fixedEnd;
        private List<Transition> draggedTransitions;
        private VisualElement draggedElement;
        private SMNodeView dropNode;
        private TransitionDragPreview preview;
        private VisualElement root;
        private SMEdge ghost;

        // Label only
        private TransitionSideHandles sideHandles;

        private const float StartDragDistance = 10f;
        private const float DraggedOpacity = 0.35f;
        private const string DropTargetClass = "transition-drop-target";

        public TransitionDragManipulator(TransitionView label = null)
        {
            this.label = label;
        }

        protected override void RegisterCallbacksOnTarget()
        {
            // TrickleDown: before the label click and GraphView's EdgeManipulator
            target.RegisterCallback<PointerDownEvent>(OnPointerDown, TrickleDown.TrickleDown);
            target.RegisterCallback<PointerMoveEvent>(OnPointerMove, TrickleDown.TrickleDown);
            target.RegisterCallback<PointerUpEvent>(OnPointerUp, TrickleDown.TrickleDown);
            target.RegisterCallback<MouseMoveEvent>(OnMouseMove, TrickleDown.TrickleDown);
            target.RegisterCallback<PointerCaptureOutEvent>(OnPointerCaptureOut);
            target.RegisterCallback<PointerLeaveEvent>(OnPointerLeave);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<PointerDownEvent>(OnPointerDown, TrickleDown.TrickleDown);
            target.UnregisterCallback<PointerMoveEvent>(OnPointerMove, TrickleDown.TrickleDown);
            target.UnregisterCallback<PointerUpEvent>(OnPointerUp, TrickleDown.TrickleDown);
            target.UnregisterCallback<MouseMoveEvent>(OnMouseMove, TrickleDown.TrickleDown);
            target.UnregisterCallback<PointerCaptureOutEvent>(OnPointerCaptureOut);
            target.UnregisterCallback<PointerLeaveEvent>(OnPointerLeave);
        }

        private void OnPointerDown(PointerDownEvent evt)
        {
            pointerPressed = true;
            pointerId = evt.pointerId;
            pressPosition = evt.position;

            // Alt: pan. Ctrl/Shift: selection
            canDrag = evt.button == (int)MouseButton.LeftMouse
                && !evt.altKey && !evt.shiftKey && !evt.actionKey
                && !Application.isPlaying
                && Edge.input != null && Edge.output != null;

            UpdateSideHandles(pressPosition);
        }

        private void OnPointerMove(PointerMoveEvent evt)
        {
            // Disables GraphView's EdgeManipulator drag
            evt.StopImmediatePropagation();

            if (pointerPressed && evt.pointerId == pointerId && evt.pressedButtons == 0)
            {
                // Released outside
                StopDrag();
                pointerPressed = false;
            }

            // Hover: the side that a press here would move
            if (!pointerPressed)
            {
                UpdateSideHandles(evt.position);
                return;
            }

            if (evt.pointerId != pointerId || !canDrag)
                return;

            Vector2 position = evt.position;

            if (!dragging)
            {
                if ((position - pressPosition).sqrMagnitude < StartDragDistance * StartDragDistance)
                    return;

                StartDrag();
            }

            UpdateDrag(position);
        }

        private void OnMouseMove(MouseMoveEvent evt)
        {
            // Also used by GraphView's EdgeManipulator
            if (!IsLabel)
            {
                evt.StopImmediatePropagation();
            }
        }

        private void OnPointerUp(PointerUpEvent evt)
        {
            if (!pointerPressed || evt.pointerId != pointerId)
                return;

            pointerPressed = false;

            if (!dragging)
                return;

            evt.StopImmediatePropagation();

            SMNodeView node = dropNode;
            bool movingTarget = moveTarget;
            List<Transition> transitions = draggedTransitions;
            StateMachineNode owner = Edge.OutputNode;
            StateMachineGraphModule module = (StateMachineGraphModule)Edge.References.Module;

            StopDrag();

            if (node == null)
                return;

            // Delayed: the refresh rebuilds this edge
            EditorApplication.delayCall += () => module.MoveTransitions(owner, transitions, node.Node, movingTarget);
        }

        private void OnPointerCaptureOut(PointerCaptureOutEvent evt)
        {
            if (dragging && evt.target == target)
            {
                StopDrag();
            }
        }

        private void OnPointerLeave(PointerLeaveEvent evt)
        {
            if (evt.target == target)
            {
                HideSideHandles();
            }
        }

        private void OnKeyDown(KeyDownEvent evt)
        {
            if (evt.keyCode != KeyCode.Escape)
                return;

            evt.StopImmediatePropagation();
            StopDrag();
        }

        private void StartDrag()
        {
            target.CapturePointer(pointerId);
            dragging = true;
            HideSideHandles();

            Vector2 source = Edge.LocalToWorld(Edge.edgeControl.from);
            Vector2 destination = Edge.LocalToWorld(Edge.edgeControl.to);

            moveTarget = IsLabel
                ? IsDestinationSide(pressPosition)
                : (pressPosition - destination).sqrMagnitude < (pressPosition - source).sqrMagnitude;

            fixedEnd = moveTarget ? source : destination;

            if (IsLabel)
            {
                draggedTransitions = new List<Transition> { label.Transition };
                draggedElement = label;
            }
            else
            {
                draggedTransitions = Edge.OutputNode.GetTransitions().Where(t => t.Connection == Edge.InputNode).ToList();
                draggedElement = Edge;
            }

            draggedElement.style.opacity = DraggedOpacity;

            preview = new TransitionDragPreview(draggedTransitions.Count > 1);

            // Behind the graph content, like the edges
            VisualElement content = Edge.References.Graph.contentViewContainer;
            VisualElement container = content.hierarchy.parent;
            container.hierarchy.Insert(container.hierarchy.IndexOf(content), preview);

            root = target.panel.visualTree;
            root.RegisterCallback<KeyDownEvent>(OnKeyDown, TrickleDown.TrickleDown);
        }

        private void UpdateDrag(Vector2 position)
        {
            SetDropNode(FindValidNode(position));

            // Snapped: the ghost shows the result
            preview.visible = dropNode == null;

            if (moveTarget)
            {
                preview.SetPoints(fixedEnd, position);
            }
            else
            {
                preview.SetPoints(position, fixedEnd);
            }
        }

        private void StopDrag()
        {
            if (!dragging)
                return;

            dragging = false;
            canDrag = false; // Escape cancels until the next press

            if (target.HasPointerCapture(pointerId))
            {
                target.ReleasePointer(pointerId);
            }

            draggedElement.style.opacity = StyleKeyword.Null;
            preview.RemoveFromHierarchy();
            SetDropNode(null);
            root.UnregisterCallback<KeyDownEvent>(OnKeyDown, TrickleDown.TrickleDown);

            draggedTransitions = null;
            draggedElement = null;
            preview = null;
            root = null;
        }

        private SMNodeView FindValidNode(Vector2 position)
        {
            SMNodeView node = Edge.References.Module.nodes.ToList()
                .OfType<NGNode>()
                .Where(n => n.worldBound.Contains(position))
                .Select(n => n.NodeView as SMNodeView)
                .LastOrDefault(); // Topmost

            return node != null && IsValidDrop(node) ? node : null;
        }

        private bool IsValidDrop(SMNodeView node)
        {
            StateMachineNode owner = Edge.OutputNode;
            StateMachineNode destination = Edge.InputNode;

            if (moveTarget)
                return node is SMTransitableNodeView && node.Node != owner && node.Node != destination;

            return node.Node is IOutputNode && node.Node != destination && node.Node != owner;
        }

        private void SetDropNode(SMNodeView node)
        {
            if (dropNode == node)
                return;

            dropNode?.NGNode.RemoveFromClassList(DropTargetClass);
            dropNode = node;
            dropNode?.NGNode.AddToClassList(DropTargetClass);

            SetGhost(node);
        }

        /// <summary> Same ghost GraphView shows when creating a transition </summary>
        private void SetGhost(SMNodeView node)
        {
            // RemoveElement, not DeleteElements: deleting would delete the transitions
            if (ghost != null)
            {
                Edge.References.Graph.RemoveElement(ghost);
                ghost = null;
            }

            if (node == null)
                return;

            ghost = new SMEdge
            {
                isGhostEdge = true,
                pickingMode = PickingMode.Ignore,
                GhostTransitionCount = draggedTransitions.Count
            };

            Edge.References.Graph.AddElement(ghost);
            ghost.output = moveTarget ? Edge.output : node.Output;
            ghost.input = moveTarget ? ((SMTransitableNodeView)node).Input : Edge.input;
        }

        /// <summary> Label half facing the destination </summary>
        private bool IsDestinationSide(Vector2 worldPosition)
        {
            return Vector2.Dot(worldPosition - target.worldBound.center, GetLineDirection()) >= 0f;
        }

        /// <summary> Source to destination, world space </summary>
        private Vector2 GetLineDirection()
        {
            return Edge.LocalToWorld(Edge.edgeControl.to) - Edge.LocalToWorld(Edge.edgeControl.from);
        }

        private void UpdateSideHandles(Vector2 worldPosition)
        {
            if (!IsLabel || Application.isPlaying || Edge.input == null || Edge.output == null)
                return;

            Vector2 direction = GetLineDirection();

            if (direction.sqrMagnitude < 1f)
                return;

            sideHandles ??= new TransitionSideHandles();

            if (sideHandles.parent != target)
            {
                target.Add(sideHandles);
            }

            sideHandles.SetSides(direction, IsDestinationSide(worldPosition));
        }

        private void HideSideHandles()
        {
            sideHandles?.RemoveFromHierarchy();
        }
    }

    /// <summary> Bars around the label towards each transition end. Highlights the end a drag would move </summary>
    public class TransitionSideHandles : VisualElement
    {
        private const float BarLength = 10f;
        private const float BarThickness = 2f;
        private const float BarGap = 6f;
        private const float BarOutline = 1f;

        private static readonly Color InactiveColor = new(0.85f, 0.85f, 0.85f);
        private static readonly Color OutlineColor = new(0.1f, 0.1f, 0.1f);

        private const float BarOffset = BarGap + BarThickness * 0.5f;
        private const float Extent = BarGap + BarThickness + BarOutline; // Hit area, covers the gap
        private const float LayoutMargin = 4f; // Label border grows on hover/focus

        private Vector2 direction;
        private bool destinationActive;

        public TransitionSideHandles()
        {
            name = "transition-side-handles";

            const float layoutExtent = -(Extent + LayoutMargin);
            style.position = Position.Absolute;
            style.left = layoutExtent;
            style.top = layoutExtent;
            style.right = layoutExtent;
            style.bottom = layoutExtent;

            generateVisualContent = OnGenerateVisualContent;
            RegisterCallback<GeometryChangedEvent>(_ => MarkDirtyRepaint());
        }

        public override bool ContainsPoint(Vector2 localPoint)
        {
            if (parent == null)
                return false;

            Rect label = GetLabelRect();
            return Inflate(label, Extent).Contains(localPoint) && !label.Contains(localPoint);
        }

        public void SetSides(Vector2 lineDirection, bool destinationActive)
        {
            direction = lineDirection;
            this.destinationActive = destinationActive;
            MarkDirtyRepaint();
        }

        private void OnGenerateVisualContent(MeshGenerationContext context)
        {
            if (parent == null || direction.sqrMagnitude < 1f)
                return;

            Rect border = Inflate(GetLabelRect(), BarOffset);

            Painter2D painter = context.painter2D;
            painter.lineJoin = LineJoin.Miter;
            painter.lineCap = LineCap.Butt;

            DrawBar(painter, border, -direction, !destinationActive);
            DrawBar(painter, border, direction, destinationActive);
        }

        // Via world: the label border width changes
        private Rect GetLabelRect() => this.WorldToLocal(parent.worldBound);

        private static void DrawBar(Painter2D painter, Rect border, Vector2 towards, bool active)
        {
            Vector2 center = SMEdgeUtils.GetPointInPerimeter(border.center, border.center + towards, border.size);

            // Outline
            painter.strokeColor = OutlineColor;
            painter.lineWidth = BarThickness + BarOutline * 2f;
            StrokeAlongBorder(painter, border, center, BarLength + BarOutline * 2f);

            painter.strokeColor = active ? TransitionDragPreview.LineColor : InactiveColor;
            painter.lineWidth = BarThickness;
            StrokeAlongBorder(painter, border, center, BarLength);
        }

        /// <summary> Border piece centered on the point, bending at the corners </summary>
        private static void StrokeAlongBorder(Painter2D painter, Rect border, Vector2 center, float length)
        {
            float perimeter = 2f * (border.width + border.height);
            length = Mathf.Min(length, perimeter);

            float middle = GetBorderDistance(border, center);
            float start = middle - length * 0.5f;
            float end = middle + length * 0.5f;

            float[] corners = { 0f, border.width, border.width + border.height, 2f * border.width + border.height };

            painter.BeginPath();
            painter.MoveTo(GetBorderPoint(border, start));

            // Wraps at the top left corner
            for (int lap = -1; lap <= 1; lap++)
            {
                foreach (float corner in corners)
                {
                    float distance = corner + lap * perimeter;

                    if (distance > start && distance < end)
                    {
                        painter.LineTo(GetBorderPoint(border, distance));
                    }
                }
            }

            painter.LineTo(GetBorderPoint(border, end));
            painter.Stroke();
        }

        /// <summary> Clockwise from the top left corner </summary>
        private static float GetBorderDistance(Rect border, Vector2 point)
        {
            float top = Mathf.Abs(point.y - border.yMin);
            float right = Mathf.Abs(point.x - border.xMax);
            float bottom = Mathf.Abs(point.y - border.yMax);
            float left = Mathf.Abs(point.x - border.xMin);
            float nearest = Mathf.Min(Mathf.Min(top, right), Mathf.Min(bottom, left));

            if (nearest == top)
                return point.x - border.xMin;

            if (nearest == right)
                return border.width + point.y - border.yMin;

            if (nearest == bottom)
                return border.width + border.height + border.xMax - point.x;

            return 2f * border.width + border.height + border.yMax - point.y;
        }

        private static Vector2 GetBorderPoint(Rect border, float distance)
        {
            distance = Mathf.Repeat(distance, 2f * (border.width + border.height));

            if (distance <= border.width)
                return new Vector2(border.xMin + distance, border.yMin);

            distance -= border.width;

            if (distance <= border.height)
                return new Vector2(border.xMax, border.yMin + distance);

            distance -= border.height;

            if (distance <= border.width)
                return new Vector2(border.xMax - distance, border.yMax);

            distance -= border.width;
            return new Vector2(border.xMin, border.yMax - distance);
        }

        private static Rect Inflate(Rect rect, float amount)
        {
            return new Rect(rect.x - amount, rect.y - amount, rect.width + amount * 2f, rect.height + amount * 2f);
        }
    }

    /// <summary> Line and arrow while dragging </summary>
    public class TransitionDragPreview : VisualElement
    {
        internal static readonly Color LineColor = new(0f, 0.82f, 1f);
        private const float LineWidth = 3f;

        private readonly VisualElement arrow;
        private Vector2 worldStart;
        private Vector2 worldEnd;

        public TransitionDragPreview(bool multipleTransitions)
        {
            pickingMode = PickingMode.Ignore;

            style.position = Position.Absolute;
            style.left = 0f;
            style.top = 0f;
            style.right = 0f;
            style.bottom = 0f;

            arrow = SMEdgeUtils.CreateArrow();
            arrow.pickingMode = PickingMode.Ignore;
            arrow.style.unityBackgroundImageTintColor = LineColor;
            SMEdgeUtils.SetArrowIcon(arrow, multipleTransitions);
            Add(arrow);

            generateVisualContent = OnGenerateVisualContent;
        }

        public void SetPoints(Vector2 start, Vector2 end)
        {
            worldStart = start;
            worldEnd = end;

            // Via parent: it already has a layout when the preview is new
            Vector2 middle = hierarchy.parent.WorldToLocal((start + end) * 0.5f);

            arrow.style.left = middle.x;
            arrow.style.top = middle.y;
            arrow.style.rotate = new Rotate(SMEdgeUtils.GetAngle(start, end));

            MarkDirtyRepaint();
        }

        private void OnGenerateVisualContent(MeshGenerationContext context)
        {
            Vector2 start = this.WorldToLocal(worldStart);
            Vector2 end = this.WorldToLocal(worldEnd);

            if ((end - start).sqrMagnitude < 1f)
                return;

            Painter2D painter = context.painter2D;
            painter.strokeColor = LineColor;
            painter.lineWidth = LineWidth;
            painter.lineCap = LineCap.Round;

            painter.BeginPath();
            painter.MoveTo(start);
            painter.LineTo(end);
            painter.Stroke();
        }
    }
}
