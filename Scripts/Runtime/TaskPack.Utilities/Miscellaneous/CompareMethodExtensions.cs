using System;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    public static class CompareMethodExtensions
    {
        public static bool Compare(this CompareMethod checkType, float a, float b, float threshHold)
        {
            return checkType.Compare(a, b + threshHold) || checkType.Compare(a, b - threshHold);
        }

        public static bool Compare(this CompareMethod checkType, float a, float b)
        {
            return checkType switch
            {
                CompareMethod.LessOrEqualTo => a <= b,
                CompareMethod.LessThan => a < b,
                CompareMethod.EqualTo => a == b,
                CompareMethod.BiggerOrEqualTo => a >= b,
                CompareMethod.BiggerThan => a > b,
                _ => throw new NotImplementedException()
            };
        }

        public static bool Compare(this CompareMethod checkType, int a, int b)
        {
            return checkType switch
            {
                CompareMethod.LessOrEqualTo => a <= b,
                CompareMethod.LessThan => a < b,
                CompareMethod.EqualTo => a == b,
                CompareMethod.BiggerOrEqualTo => a >= b,
                CompareMethod.BiggerThan => a > b,
                _ => throw new NotImplementedException()
            };
        }

        public static string GetString(this CompareMethod checkType)
        {
            return checkType switch
            {
                CompareMethod.LessOrEqualTo => "<=",
                CompareMethod.LessThan => "<",
                CompareMethod.EqualTo => "==",
                CompareMethod.BiggerOrEqualTo => "=>",
                CompareMethod.BiggerThan => ">",
                _ => throw new NotImplementedException()
            };
        }
    }
}