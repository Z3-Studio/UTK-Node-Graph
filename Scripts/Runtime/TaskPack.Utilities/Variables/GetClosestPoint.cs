using System.Collections.Generic;
using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;


namespace Z3.NodeGraph.TaskPack.Utilities.NodeGraph.Unity
{
    [NodeCategory(Categories.Variables)]
    [NodeDescription("Gets the closest point from a transform inside a list")]
    public class GetClosestPoint<T> : ActionTask<Transform> where T : Component
    {
        [Header("In")]
        public Parameter<List<T>> points;
        
        [Header("Out")]
        public Parameter<int> outIndex;
        public Parameter<T> outPoint;

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
            EndAction(true);
        }

        private float GetSqrDistance(int index)
        {
            Vector3 point = points.Value[index].transform.position;
            return (point - Agent.position).sqrMagnitude;
        }
    }
}