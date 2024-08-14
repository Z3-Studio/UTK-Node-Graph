using UnityEngine;
using Z3.Utils;

namespace Z3.NodeGraph.Core
{
    public interface IGraphRunner
    {
        /// <summary> Component associated GameObject </summary>
        Component Component { get; }

        /// <summary> Component pool of the associated GameObject </summary>
        CachedComponents CachedComponents { get; }

        /// <summary> Object where reference variable instances are stored </summary>
        VariableInstanceList ReferenceVariables { get; }

        /// <summary> Delta based on chosen time source (fixedDeltaTime, deltaTime or custom) </summary>
        float DeltaTime { get; }

        /// <summary> Last activation of the GraphRunner </summary>
        float OwnerActivationTime { get; }
    }
}
