using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;
using System;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NgName("Set Vector2 Advanced")]
    [NodeCategory(Categories.Variables)]
    [NodeDescription("Easy way to set a specific axis")]
    public class SetVector2Advanced : ActionTask
    {
        [Header("In")]
        [SerializeField] private Parameter<Vector2> initialVector;
        [SerializeField] private Parameter<Vector2> otherVector;

        [Header("Config")]
        public OperationMethod operation = OperationMethod.Set;
        [SerializeField] private Parameter<bool> setX;
        [SerializeField] private Parameter<bool> setY;

        [Header("Out")]
        [SerializeField] private Parameter<Vector2> returnedVector;

        public override string Info
        {
            get
            {
                string info = string.Empty;

                if (setX.Value)
                {
                    info = AddText(info, "X");
                }

                if (setY.Value)
                {
                    info = AddText(info, "Y");
                }

                if (string.IsNullOrEmpty(info))
                {
                    return name;
                }

                return info;
            }
        }

        private string AddText(string info, string axis)
        {
            axis = $"<b>{axis}</b>";
            if (string.IsNullOrEmpty(info))
            {
                return $"{returnedVector} {operation} {axis}";
            }

            return info + $", {axis}";
        }

        protected override void StartAction()
        {
            Vector3 finalVector = initialVector.Value;

            if (setX.Value)
            {
                finalVector.x = operation.Operate(finalVector.x, otherVector.Value.x);
            }
            if (setY.Value)
            {
                finalVector.y = operation.Operate(finalVector.y, otherVector.Value.y);
            }

            returnedVector.Value = finalVector;
            EndAction();
        }
    }
}