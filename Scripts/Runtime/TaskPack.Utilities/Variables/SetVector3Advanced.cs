using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NgName("Set Vector3 Advanced")]
    [NodeCategory(Categories.Variables)]
    [NodeDescription("Easy way to set a specific axis")]
    public class SetVector3Advanced : ActionTask 
    {
        [Header("In")]
        [SerializeField] private Parameter<Vector3> initialVector;
        [SerializeField] private Parameter<Vector3> otherVector;

        [Header("Config")]
        [SerializeField] private OperationMethod operation = OperationMethod.Set;
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

                string otherX;
                string otherY;
                string otherZ;
                string operationS = operation.GetString();

                if (otherVector.IsBinding)
                {
                    otherX = otherVector + ".X";
                    otherY = otherVector + ".Y";
                    otherZ = otherVector + ".Z";
                }
                else
                {
                    otherX = $"<b>{otherVector.Value.x}</b>";
                    otherY = $"<b>{otherVector.Value.y}</b>";
                    otherZ = $"<b>{otherVector.Value.z}</b>";
                }

                string xText;
                string yText;
                string zText;

                if (operation == OperationMethod.Set)
                {
                    xText = setX.Value ? $"{otherX}" : initialX;
                    yText = setY.Value ? $"{otherY}" : initialY;
                    zText = setZ.Value ? $"{otherZ}" : initialZ;
                }
                else
                {
                    xText = setX.Value ? $"{initialX} {operationS} {otherX}" : initialX;
                    yText = setY.Value ? $"{initialY} {operationS} {otherY}" : initialY;
                    zText = setZ.Value ? $"{initialZ} {operationS} {otherZ}" : initialZ;
                }

                return $"{returnedVector} = ({xText}, {yText}, {zText})";
            }
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
            if (setZ.Value)
            {
                finalVector.z = operation.Operate(finalVector.z, otherVector.Value.z);
            }

            returnedVector.Value = finalVector;
            EndAction();
        }
    }
}