using System;
using UnityEngine;

namespace Z3.NodeGraph.Core
{
    [Serializable]
    public class GraphPreference
    {
        [SerializeField] private GraphData graph;
        [SerializeField] private Vector2 position;
        [SerializeField] private float scale;

        public GraphData Graph => graph;
        public Vector2 Position { get => position; set => position = value; }
        public float Scale { get => scale; set => scale = value; }

        public GraphPreference(GraphData graph)
        {
            this.graph = graph;
            position = Vector2.zero;
            scale = 1f;
        }
    }
}