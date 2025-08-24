using System;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    public static class AxisExtesions
    {
        public static float Distance(this Axis3Flags axis, Vector3 a, Vector3 b)
        {
            float squaredDifference = 0f;

            if (axis.HasFlag(Axis3Flags.X))
            {
                float aux = a.x - b.x;
                squaredDifference += aux * aux;
            }
            if (axis.HasFlag(Axis3Flags.Y))
            {
                float aux = a.y - b.y;
                squaredDifference += aux * aux;

            }
            if (axis.HasFlag(Axis3Flags.Z))
            {
                float aux = a.z - b.z;
                squaredDifference += aux * aux;
            }

            return Mathf.Sqrt(squaredDifference);
        }

        public static Vector3 GetDirection(this Direction direction, Transform transform)
        {
            return direction switch
            {
                Direction.Forward => transform.forward,
                Direction.Back => -transform.forward,
                Direction.Up => transform.up,
                Direction.Down => -transform.up,
                Direction.Right => transform.right,
                Direction.Left => -transform.right,
                _ => throw new NotImplementedException(),
            };
        }
    }
}