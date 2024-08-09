using System.Collections;
using System.Collections.Generic;

namespace Z3.NodeGraph.Core
{
    /// <summary>
    /// Used to find classes that have dependencies on <see cref="GraphSubAsset"/>.
    /// This allows the <see cref="Validator"/> to detect issues and apply fixes.
    /// </summary>
    /// <remarks>
    /// Classes that already inherit from <see cref="GraphSubAsset"/> do not need this interface.
    /// </remarks>
    /// <example> <see cref="Tasks.TaskList{T}"/> </example>
    public interface ISubAssetList : IList
    {
    }

    /// <summary>
    /// Provides the same functionality as <see cref="ISubAssetList"/>, 
    /// but reinforces its use for generic types that derive from <see cref="GraphSubAsset"/>.
    /// </summary>
    public interface ISubAssetList<T> : ISubAssetList, IList<T> where T : GraphSubAsset
    {
    }
}
