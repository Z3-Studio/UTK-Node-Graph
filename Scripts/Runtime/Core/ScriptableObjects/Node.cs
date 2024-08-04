using UnityEngine;
using Z3.UIBuilder.Core;

namespace Z3.NodeGraph.Core
{
    public abstract class Node : GraphSubAsset
    {
        [ReadOnly, HideInGraphInspector]
        [SerializeField] private Vector2 position;

        public virtual string SubInfo => string.Empty;

        public virtual bool StartableNode => true;
        public float NodeActivationTime { get; protected set; }
        public float NodeRunningTime => Time.time - NodeActivationTime;

        protected float Delta;


        public Vector2 Position { get => position; set => position = value; }

        /// <summary> NodeView Color </summary>
        public virtual string ClassStyle => string.Empty;
    }
}
