﻿using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Transform)]
    [NodeDescription("Get the transform.rotation.")]
    public class GetRotation : ActionTask
    {
        [ParameterDefinition(AutoBindType.SelfBind)]
        [SerializeField] private Parameter<Transform> data;

        [Header("Out")]
        [SerializeField] private Parameter<Quaternion> rotation;

        public override string Info => $"Get {data} Rotation";
        protected override void StartAction()
        {
            rotation.Value = data.Value.rotation;
            EndAction();
        }
    }
}