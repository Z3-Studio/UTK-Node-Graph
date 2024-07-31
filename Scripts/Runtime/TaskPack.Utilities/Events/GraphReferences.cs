using Z3.NodeGraph.Core;
using System.Linq;
using UnityEngine;

namespace Z3.NodeGraph.TaskPack.Utilities.Utils
{
    [System.Serializable]
    public struct GraphReference
    {
        public string keyName;
        public GraphRunner graphOwner;
    }

    /// <summary>
    /// Stores a list of graph owners
    /// </summary>
    //[AddComponentMenu(GraphPath.ComponentMenu + "Graph References")]
    public class GraphReferences : MonoBehaviour 
    {
        [Header("GraphReferences")]
        [SerializeField] private GraphReference[] references;
        public GraphReference[] Graphs => references;

        public GraphRunner GetGraph(string name)
        {
            return references.First(r => r.keyName == name).graphOwner;
        }
    }   
}