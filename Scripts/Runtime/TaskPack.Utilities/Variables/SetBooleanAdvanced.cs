using System;
using UnityEngine;
using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;

namespace Z3.NodeGraph.TaskPack.Utilities.NodeGraph.Unity.Variables
{
    [NodeCategory(Categories.Variables)]
    [NodeDescription("Performs boolean calculations")]
    public class SetBooleanAdvanced : ActionTask
    {
        public enum BooleanOperations
        {
            Or = 1,
            And = 2,
            Not = 3,
            Xor = 4,
            Xand = 5,
            Nor = 6,
            Nand = 7
        }

        [Header("Input")]
        public Parameter<bool> booleanA;
        public Parameter<bool> booleanB;
        public BooleanOperations operation = BooleanOperations.Or;
        
        [Header("Output")]
        public Parameter<bool> output;

        public override string Info => operation switch
        {
            BooleanOperations.Or => $"{output} = {booleanA} OR {booleanB}",
            BooleanOperations.And => $"{output} = {booleanA} AND {booleanB}",
            BooleanOperations.Not => $"{output} = !{booleanA}",
            BooleanOperations.Xor => $"{output} = {booleanA} != {booleanB}",
            BooleanOperations.Xand => $"{output} = {booleanA} == {booleanB}",
            BooleanOperations.Nor => $"{output} = !({booleanA} OR {booleanB})",
            BooleanOperations.Nand => $"{output} = !({booleanA} AND {booleanB})",
            _ => throw new NotImplementedException()
        };

        protected override void StartAction()
        {
            output.Value = operation switch
            {
                BooleanOperations.Or => booleanA.Value || booleanB.Value,
                BooleanOperations.And => booleanA.Value && booleanB.Value,
                BooleanOperations.Not => !booleanA.Value,
                BooleanOperations.Xor => booleanA.Value != booleanB.Value,
                BooleanOperations.Xand => booleanA.Value == booleanB.Value,
                BooleanOperations.Nor => !(booleanA.Value || booleanB.Value),
                BooleanOperations.Nand => !(booleanA.Value && booleanB.Value),
                _ => throw new NotImplementedException()
            };
            
            EndAction(true);
        }
    }
}