﻿using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NgName("Combine Vector2 Float")]
    [NodeCategory(Categories.Variables)]
    [NodeDescription("Easy way to combine vector and floats")]
    public class CombineVector2Float : ActionTask
    {
        [Header("In")]
        [SerializeField] private Parameter<Vector2> initialVector;
        [SerializeField] private Parameter<float> xPosition;
        [SerializeField] private Parameter<float> yPosition;

        [Header("Config")]
        [SerializeField] private Parameter<bool> useFloatX;
        [SerializeField] private Parameter<bool> useFloatY;

        [Header("Out")]
        [SerializeField] private Parameter<Vector2> returnedVector;

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

                string x = useFloatX.Value ? xPosition.ToString() : initialX;
                string y = useFloatY.Value ? yPosition.ToString() : initialY;

                return $"{returnedVector} = ({x}, {y})";
            }
        }

        protected override void StartAction()
        {
            Vector2 newVector = initialVector.Value;

            if (useFloatX.Value)
            {
                newVector.x = xPosition.Value;
            }
            if (useFloatY.Value)
            {
                newVector.y = yPosition.Value;
            }

            returnedVector.Value = newVector;
            EndAction();
        }
    }
}