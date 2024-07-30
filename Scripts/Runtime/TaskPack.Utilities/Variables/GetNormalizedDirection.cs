﻿using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities.Physic
{
    [NodeCategory(Categories.Variables)]
    [NodeDescription("Get (target.Value - from.Value).normalized")]
    public class GetNormalizedDirection : ActionTask
    {
        [Header("In")]
        public Parameter<Vector3> from;
        public Parameter<Vector3> target;

        [Header("Out")]
        public Parameter<Vector3> direction;

        public override string Info => $"Get Direction {from} to {target}";
        protected override void StartAction()
        {
            direction.Value = (target.Value - from.Value).normalized;
            EndAction(true);
        }
    }
}