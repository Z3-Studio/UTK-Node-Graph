using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Operations)]
    public abstract class SetOperation<T> : ActionTask
    {
        [SerializeField] protected Parameter<T> valueA;
        [SerializeField] protected Parameter<T> valueB;
        [SerializeField] protected OperationMethod operation = OperationMethod.Set;

        public override string Info => $"{valueA} {operation.GetOperationString()} {valueB}";
    }

    [NodeCategory(Categories.Operations)]
    public abstract class SetOperation2<T> : ActionTask
    {
        [SerializeField] protected Parameter<T> valueA;
        [SerializeField] protected Parameter<T> valueB;
        [SerializeField] protected Parameter<T> result;
        [SerializeField] protected OperationMethod operation = OperationMethod.Set;

        public override string Info => $"{result} = {valueA} {operation.GetString()} {valueB}";
    }
}
