﻿using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Transform)]
    [NodeDescription("Compare the Inverse Transform Point to target + offset.")]
    public class CheckInversePointAxis : ConditionTask
    {
        [ParameterDefinition(AutoBindType.FindSimilarVariable)]
        [SerializeField] private Parameter<Transform> transform;

        [SerializeField] private Parameter<Axis3> axis;
        [SerializeField] private Parameter<Vector3> target;
        [SerializeField] private Parameter<float> offset;
        [SerializeField] private Parameter<float> value;
        public CompareMethod checkType = CompareMethod.EqualTo;

        public override string Info => $"Inverse Point {axis} {target} {checkType.GetString()} {value}";

        public override bool CheckCondition()
        {
            Vector3 inverse = transform.Value.InverseTransformPoint(target.Value);

            float axisDistance = offset.Value + axis.Value switch
            {
                Axis3.X => inverse.x,
                Axis3.Y => inverse.y,
                Axis3.Z => inverse.z,
                _ => throw new System.NotImplementedException(),
            };

            return checkType.Compare(axisDistance, value.Value);
        }
    }
}