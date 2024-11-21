using System.Collections.Generic;
using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;


namespace Z3.NodeGraph.TaskPack.Utilities.NodeGraph.Unity
{
    [NodeCategory(Categories.Variables)]
    [NodeDescription("Gets the closest point from a transform inside a list")]
    public class GetClosestPoint<T> : ActionTask where T : Component
    {
        [ParameterDefinition(AutoBindType.FindSimilarVariable)]
        [SerializeField] private Parameter<Transform> transform;

        [Header("In")]
        [SerializeField] private Parameter<List<T>> points;
        
        [Header("Out")]
        [SerializeField] private Parameter<int> outIndex;
        [SerializeField] private Parameter<T> outPoint;

        protected override void StartAction()
        {
            float closestDistance = GetSqrDistance(0);
            int closestIndex = 0;
            
            for (int i = 1; i < points.Value.Count; i++)
            {
                float currentDistance = GetSqrDistance(i);
                
                if (currentDistance < closestDistance)
                {
                    closestDistance = currentDistance;
                    closestIndex = i;
                }
            }

            outIndex.Value = closestIndex;
            outPoint.Value = points.Value[closestIndex];
            EndAction();
        }

        private float GetSqrDistance(int index)
        {
            Vector3 point = points.Value[index].transform.position;
            return (point - transform.Value.position).sqrMagnitude;
        }
    }
}