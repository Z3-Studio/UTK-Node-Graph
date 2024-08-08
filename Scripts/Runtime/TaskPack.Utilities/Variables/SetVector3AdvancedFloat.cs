using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NgName("Set Vector3 Advanced Float")]
    [NodeCategory(Categories.Variables)]
    [NodeDescription("Easy way to set a specific axis")]
    public class SetVector3AdvancedFloat : ActionTask
    {
        [Header("In")]
        [SerializeField] private Parameter<Vector3> initialVector;
        [SerializeField] private Parameter<float> valueX;
        [SerializeField] private Parameter<float> valueY;
        [SerializeField] private Parameter<float> valueZ;

        [Header("Config")]
        public OperationMethod operation = OperationMethod.Set;
        [SerializeField] private Parameter<bool> setX;
        [SerializeField] private Parameter<bool> setY;
        [SerializeField] private Parameter<bool> setZ;

        [Header("Out")]
        [SerializeField] private Parameter<Vector3> returnedVector;

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

                if (setZ.Value)
                {
                    info = AddText(info, "Z");
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
                finalVector.x = operation.Operate(finalVector.x, valueX.Value);
            }
            if (setY.Value)
            {
                finalVector.y = operation.Operate(finalVector.y, valueY.Value);
            }
            if (setZ.Value)
            {
                finalVector.z = operation.Operate(finalVector.z, valueZ.Value);
            }

            returnedVector.Value = finalVector;
            EndAction();
        }
    }
}