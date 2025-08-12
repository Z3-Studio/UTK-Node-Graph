using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NgName("Combine Vector3 Float")]
    [NodeCategory(Categories.Variables)]
    [NodeDescription("Easy way to combine vector and floats")]
    public class CombineVector3Float : ActionTask 
    {
        [Header("In")]
        [SerializeField] private Parameter<Vector3> initialVector;
        [SerializeField] private Parameter<float> x;
        [SerializeField] private Parameter<float> y;
        [SerializeField] private Parameter<float> z;

        [Header("Config")]
        [SerializeField] private Parameter<bool> useFloatX;
        [SerializeField] private Parameter<bool> useFloatY;
        [SerializeField] private Parameter<bool> useFloatZ;

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

                string xText = useFloatX.Value ? x.ToString() : initialX;
                string yText = useFloatY.Value ? y.ToString() : initialY;
                string zText = useFloatZ.Value ? z.ToString() : initialZ;

                return $"{returnedVector} = ({xText}, {yText}, {zText})";
            } 
        }

        protected override void StartAction() 
        {
            Vector3 newVector = initialVector.Value;

            if (useFloatX.Value)
            {
                newVector.x = x.Value;
            }
            if (useFloatY.Value)
            {
                newVector.y = y.Value;
            }
            if (useFloatZ.Value)
            {
                newVector.z = z.Value;
            }

            returnedVector.Value = newVector;
            EndAction();
        }
    }
}