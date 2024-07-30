using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities.Math
{
    [NodeCategory(Categories.Math)]
    public class CheckVectorDistance : ConditionTask
    {
        [SerializeField] private Parameter<Vector3> vectorA;
        [SerializeField] private Parameter<Vector3> vectorB;
        [SerializeField] private CompareMethod comparison = CompareMethod.EqualTo;
        [SerializeField] private Parameter<float> distance;

        public override string Info => $"Distance ({vectorA}, {vectorB}) {comparison.GetString()} {distance}";

        public override bool CheckCondition()
        {
            float d = Vector3.Distance(vectorA.Value, vectorB.Value);
            return comparison.Compare(d, distance.Value);
        }
    }
}