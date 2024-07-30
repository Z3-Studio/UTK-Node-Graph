using System;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    public enum BoolOperation
    {
        True,
        False,
        Toggle
    }

    public enum Direction
    {
        Left,
        Right,
        Up,
        Down,
        Forward,
        Back
    }

    /// <summary> Used with <see cref="CompareMethodExtensions"/> </summary>
    public enum CompareMethod
    {
        LessOrEqualTo,
        LessThan,
        EqualTo,
        BiggerOrEqualTo,
        BiggerThan,
    }

    /// <summary> Used with <see cref="OperationToolsExtensions"/> </summary>
    public enum OperationMethod
    {
        Set,
        Add,
        Subtract,
        Multiply,
        Divide
    }

    /// <summary> Used with <see cref="AxisExtesions"/> </summary>
    public enum Axis2
    {
        X,
        Y
    }

    /// <summary> Used with <see cref="AxisExtesions"/> </summary>
    [Flags]
    public enum Axis2Flags
    {
        X = 1,
        Y = 2
    }

    /// <summary> Used with <see cref="AxisExtesions"/> </summary>
    public enum Axis3
    {
        X,
        Y,
        Z
    }

    /// <summary> Used with <see cref="AxisExtesions"/> </summary>
    [Flags]
    public enum Axis3Flags
    {
        X = 1,
        Y = 2,
        Z = 4
    }
}