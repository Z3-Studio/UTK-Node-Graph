using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Variables)]
    [NodeDescription("Easy way to combine vectors")]
    public class CombineVector2 : ActionTask
    {
        [Header("In")]
        public Parameter<Vector2> initialVector;
        public Parameter<Vector2> otherVector;

        [Header("Config")]
        public Parameter<bool> useOtherX;
        public Parameter<bool> useOtherY;

        [Header("Out")]
        public Parameter<Vector2> returnedVector;

        public override string Info
        {
            get
            {
                string initialX;
                string initialY;

                if (initialVector.IsBinding)
                {
                    initialX = initialVector + ".X";
                    initialY = initialVector + ".Y";
                }
                else
                {
                    initialX = $"<b>{initialVector.Value.x}</b>";
                    initialY = $"<b>{initialVector.Value.y}</b>";
                }

                string otherX;
                string otherY;

                if (otherVector.IsBinding)
                {
                    otherX = otherVector + ".X";
                    otherY = otherVector + ".Y";
                }
                else
                {
                    otherX = $"<b>{otherVector.Value.x}</b>";
                    otherY = $"<b>{otherVector.Value.y}</b>";
                }

                string x = useOtherX.Value ? otherX : initialX;
                string y = useOtherY.Value ? otherY : initialY;

                return $"{returnedVector} = ({x}, {y})";
            }
        }

        protected override void StartAction()
        {
            Vector2 finalVector;

            finalVector.x = useOtherX.Value ? otherVector.Value.x : initialVector.Value.x;
            finalVector.y = useOtherY.Value ? otherVector.Value.y : initialVector.Value.y;

            returnedVector.Value = finalVector;

            EndAction(true);
        }
    }
}