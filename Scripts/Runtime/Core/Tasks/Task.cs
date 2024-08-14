using UnityEngine;
using Z3.NodeGraph.Core;
using Z3.Utils;

namespace Z3.NodeGraph.Tasks
{
    public abstract class Task : GraphSubAsset
    {
        protected GameObject GameObject => GraphRunner.Component.gameObject;
        protected Transform Transform => GraphRunner.Component.transform;
        protected CachedComponents CachedComponents => GraphRunner.CachedComponents;
        protected StringEventDispatcher StringEvents => CachedComponents.GetOrAddCachedComponent<StringEventDispatcher>();
        protected MonoEventDispatcher MonoEvents => CachedComponents.GetOrAddCachedComponent<MonoEventDispatcher>();
    }
}
