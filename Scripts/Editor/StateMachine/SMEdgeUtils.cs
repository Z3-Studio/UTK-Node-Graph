using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using Z3.NodeGraph.StateMachine;
using Z3.Utils;

namespace Z3.NodeGraph.Editor
{
    /// <summary> Shared by the transition edge, its ghost and the drag preview </summary>
    internal static class SMEdgeUtils
    {
        private const float ArrowSingleTransitionSize = 15f;
        private const float ArrowMultipleTransitionSize = 20f;
        private const float CircularConnectionOffset = 20f;

        internal static SMEdge FindEdge(NodeGraphModule module, StateMachineNode from, StateMachineNode to)
        {
            return module.edges.ToList()
                .OfType<SMEdge>()
                .FirstOrDefault(e => !e.isGhostEdge && e.input != null && e.output != null && e.OutputNode == from && e.InputNode == to);
        }

        /// <summary> Moves the preview two-way offset from <paramref name="current"/> to <paramref name="next"/> </summary>
        internal static void SetPreviewOffset(ref SMEdgeControl current, SMEdge next)
        {
            SMEdgeControl nextControl = next?.edgeControl as SMEdgeControl;

            current?.SetPreviewCircularOffset(false);
            current = nextControl;
            current?.SetPreviewCircularOffset(true);
        }

        internal static Vector2 GetCircularOffset(Vector2 from, Vector2 to)
        {
            Vector2 offsetDirection = (to - from).normalized * CircularConnectionOffset;
            return new Vector2(offsetDirection.y, -offsetDirection.x);
        }

        /// <summary> Arrow rotation from source to destination </summary>
        internal static float GetAngle(Vector2 from, Vector2 to)
        {
            Vector2 direction = from - to;
            return MathUtils.NormalizeAngle(Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f);
        }

        internal static VisualElement CreateArrow()
        {
            VisualElement arrow = new() { name = "arrow" };
            arrow.style.backgroundSize = new BackgroundSize(BackgroundSizeType.Contain);
            arrow.style.position = Position.Absolute;
            arrow.style.translate = new Translate(Length.Percent(-50f), Length.Percent(-50f));
            return arrow;
        }

        internal static void SetArrowIcon(VisualElement arrow, bool multipleTransitions)
        {
            float iconSize = multipleTransitions ? ArrowMultipleTransitionSize : ArrowSingleTransitionSize;
            Texture2D icon = multipleTransitions ? NodeGraphResources.MultipleArrowTransition : NodeGraphResources.ArrowTransition;

            arrow.style.height = iconSize;
            arrow.style.width = iconSize;
            arrow.style.backgroundImage = new StyleBackground(icon);
        }

        /// <summary>
        /// Returns a perimeter point from the angle
        /// </summary>
        internal static Vector2 GetPointInPerimeter(Vector2 start, Vector2 end, Vector2 boxSize)
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
