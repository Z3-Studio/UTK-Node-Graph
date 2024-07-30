using UnityEngine;

namespace Z3.NodeGraph.Core
{
    public interface IGraphRunner
    {
        float DeltaTime { get; }
        float OwnerActivationTime { get; }
        Component Component { get; }
        GraphEvents Events { get; }
        VariableInstanceList ReferenceVariables { get; }
    }
}
