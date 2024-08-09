using UnityEngine;
using Z3.UIBuilder.Core;

namespace Z3.NodeGraph.Core
{
    public abstract class Node : GraphSubAsset
    {
        [ReadOnly, HideInGraphInspector]
        [SerializeField] private Vector2 position;

        public Vector2 Position { get => position; set => position = value; }
        public virtual string SubInfo => string.Empty;

        public float NodeActivationTime { get; protected set; }
        public float NodeRunningTime => Time.time - NodeActivationTime;

    }
}
