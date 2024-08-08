using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NgName("Set Vector2 Advanced Float")]
    [NodeCategory(Categories.Variables)]
    [NodeDescription("Easy way to set a specific axis")]
    public class SetVector2AdvancedFloat : ActionTask
    {
        [Header("In")]
        [SerializeField] private Parameter<Vector2> initialVector;
        [SerializeField] private Parameter<float> valueX;
        [SerializeField] private Parameter<float> valueY;

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
            Vector2 finalVector = initialVector.Value;

            if (setX.Value)
            {
                finalVector.x = operation.Operate(finalVector.x, valueX.Value);
            }
            if (setY.Value)
            {
                finalVector.y = operation.Operate(finalVector.y, valueY.Value);
            }

            returnedVector.Value = finalVector;
            EndAction();
        }
    }
}