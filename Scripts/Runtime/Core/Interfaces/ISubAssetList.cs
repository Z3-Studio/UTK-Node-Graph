using System.Collections;

namespace Z3.NodeGraph.Core
{
    /// <summary>
    /// Used to delete sub items when the GraphSubAsset owner is deleted
    /// </summary>
    public interface ISubAssetList
    {
        /// <summary> It must to be list of <see cref="GraphSubAsset"/> </summary>
        IList SubAssets { get; }
    }
}
