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
        public Parameter<Vector3> initialVector;
        public Parameter<Vector3> otherVector;

        [Header("Config")]
        public OperationMethod operation = OperationMethod.Set;
        public Parameter<bool> setX;
        public Parameter<bool> setY;
        public Parameter<bool> setZ;

        [Header("Out")]
        public Parameter<Vector3> returnedVector;

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
            EndAction(true);
        }
    }
}