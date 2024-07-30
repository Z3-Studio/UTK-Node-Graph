using System;
using System.Numerics;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    public static class OperationToolsExtensions
    {

        public static int Operate(this OperationMethod operation, int a, int b)
        {
            return operation switch
            {
                OperationMethod.Set => b,
                OperationMethod.Add => a + b,
                OperationMethod.Subtract => a - b,
                OperationMethod.Multiply => a * b,
                OperationMethod.Divide => a / b,
                _ => throw new NotImplementedException()
            };
        }

        public static float Operate(this OperationMethod operation, float a, float b)
        {
            return operation switch
            {
                OperationMethod.Set => b,
                OperationMethod.Add => a + b,
                OperationMethod.Subtract => a - b,
                OperationMethod.Multiply => a * b,
                OperationMethod.Divide => a / b,
                _ => throw new NotImplementedException()
            };
        }

        public static Vector2 Operate(this OperationMethod operation, Vector2 a, Vector2 b)
        {
            return operation switch
            {
                OperationMethod.Set => b,
                OperationMethod.Add => a + b,
                OperationMethod.Subtract => a - b,
                OperationMethod.Multiply => a * b,
                OperationMethod.Divide => a / b,
                _ => throw new NotImplementedException()
            };
        }
        public static Vector3 Operate(this OperationMethod operation, Vector3 a, Vector3 b)
        {
            return operation switch
            {
                OperationMethod.Set => b,
                OperationMethod.Add => a + b,
                OperationMethod.Subtract => a - b,
                OperationMethod.Multiply => a * b,
                OperationMethod.Divide => a / b,
                _ => throw new NotImplementedException()
            };
        }

        public static string GetString(this OperationMethod operation)
        {
            return operation switch
            {
                OperationMethod.Set => "=",
                OperationMethod.Add => "+=",
                OperationMethod.Subtract => "-=",
                OperationMethod.Multiply => "*=",
                OperationMethod.Divide => "/=",
                _ => throw new NotImplementedException()
            };
        }
    }
}