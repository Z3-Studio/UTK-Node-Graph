﻿using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities.AI.Pathfinding
{
    [NodeCategory(Categories.Math)]
    public class RandomPointInCircle : ActionTask
    {
        [Header("In")]
        //[SerializeField] private Parameter<Axis> axis = Axis.Y;
        [SerializeField] private Parameter<Vector3> center;
        [SerializeField] private Parameter<float> radius;

        [Header("Out")]
        [SerializeField] private Parameter<Vector3> targetPoint;

        protected override void StartAction()
        {
            int angle = Random.Range(0, 360);
            float area = Random.Range(0, radius.Value);
            Vector3 offset = new Vector3()
            {
                x = area * Mathf.Cos(angle * Mathf.Deg2Rad),
                y = 0f,
                z = area * Mathf.Sin(angle * Mathf.Deg2Rad)
            };

            targetPoint.Value = center.Value + offset;
            EndAction();
        }
    }
}