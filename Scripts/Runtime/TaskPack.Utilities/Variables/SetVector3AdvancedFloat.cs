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
                string initialX;
                string initialY;
                string initialZ;

                if (initialVector.IsBinding)
                {
                    initialX = initialVector + ".X";
                    initialY = initialVector + ".Y";
                    initialZ = initialVector + ".Z";
                }
                else
                {
                    initialX = $"<b>{initialVector.Value.x}</b>";
                    initialY = $"<b>{initialVector.Value.y}</b>";
                    initialZ = $"<b>{initialVector.Value.z}</b>";
                }

                string xText = setX.Value ? valueX.ToString() : initialX;
                string yText = setY.Value ? valueY.ToString() : initialY;
                string zText = setZ.Value ? valueZ.ToString() : initialZ;

                return $"{returnedVector} {operation.GetOperationString()} ({xText}, {yText}, {zText})";
            }
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