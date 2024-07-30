using UnityEngine;
using Z3.NodeGraph.Core;
using Z3.Utils;

namespace Z3.NodeGraph.Tasks
{
    public abstract class Task : GraphSubAsset
    {
        protected GraphEvents Events => GraphController.Events;
        protected CachedComponents Components => GraphController.CachedComponents;
        protected GameObject GameObject => Components.GameObject;
        protected Transform Transform => GameObject.transform;
    }
}
