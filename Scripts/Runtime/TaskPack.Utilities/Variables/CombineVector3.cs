using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Variables)]
    [NodeDescription("Easy way to combine vectors")]
    public class CombineVector3 : ActionTask
    {
        [Header("In")]
        [SerializeField] private Parameter<Vector3> initialVector;
        [SerializeField] private Parameter<Vector3> otherVector;

        [Header("Config")]
        [SerializeField] private Parameter<bool> useOtherX;
        [SerializeField] private Parameter<bool> useOtherY;
        [SerializeField] private Parameter<bool> useOtherZ;

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

                string x = useOtherX.Value ? otherX : initialX;
                string y = useOtherY.Value ? otherY : initialY;
                string z = useOtherZ.Value ? otherZ : initialZ;

                return $"{returnedVector} = ({x}, {y}, {z})";
            }
        }

        protected override void StartAction()
        {
            Vector3 finalVector;

            finalVector.x = useOtherX.Value ? otherVector.Value.x : initialVector.Value.x;
            finalVector.y = useOtherY.Value ? otherVector.Value.y : initialVector.Value.y;
            finalVector.z = useOtherZ.Value ? otherVector.Value.z : initialVector.Value.z;

            returnedVector.Value = finalVector;

            EndAction();
        }
    }
}