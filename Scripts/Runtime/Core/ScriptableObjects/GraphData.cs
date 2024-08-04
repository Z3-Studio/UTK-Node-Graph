using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Z3.UIBuilder.Core;

namespace Z3.NodeGraph.Core
{
    [ReadOnly]
    public abstract class GraphData : ScriptableObject// TODO? : GraphData<T> where T : Node or Transition
    {
        [SerializeField] protected GraphVariables referenceVariables;
        [Hide]
        [SerializeField] protected List<Variable> localVariables;
        [Hide]
        [SerializeField] protected Node startNode;
        [Hide]
        [SerializeField] protected List<GraphSubAsset> subAssets = new List<GraphSubAsset>();

        public const string ReferenceVariablesField = nameof(referenceVariables);
        public Node StartNode => startNode;
        public List<GraphSubAsset> SubAssets => subAssets;
        public VariablesAsset ReferenceVariables => referenceVariables;
        public List<Variable> LocalVariables => localVariables;

        protected virtual void OnValidate()
        {
            if (!Validator.Initalized)
                return;

            Validator.Validate(this);
        }

        public List<Variable> GetVariables()
        {
            List<Variable> list = new();

            if (ReferenceVariables != null)
            {
                list.AddRange(ReferenceVariables.GetOriginalVariables());
            }

            list.AddRange(LocalVariables);

            return list;
        }

        /// <summary> Create a instance, copying the the GraphData instead change the original </summary>
        public abstract GraphController CreateInstance(IGraphRunner runner); // CLONE


        /// <summary> Called after right click </summary>
        public virtual void AddSubAsset(GraphSubAsset newNode)
        {
            subAssets.Add(newNode);

            if (startNode == null && newNode is Node node && node.StartableNode)
            {
                startNode = node;
            }
        }

        /// <summary> Called after press delete </summary>
        public virtual void RemoveSubAsset(GraphSubAsset node)
        {
            subAssets.Remove(node);

            if (startNode == node)
            {
                startNode = subAssets.FirstOrDefault(x => x is Node) as Node;
            }
        }

        /// <summary> Called after right click </summary>
        public virtual void SetStartNode(Node node) => startNode = node;

        /// <summary> Called when connect a edge </summary>
        //public abstract void AddConnection(Node parent, Node child);

        /// <summary> Called when delete a edge </summary>
        //public abstract void RemoveConnection(Node parent, Node child);

        /// <summary> Used to draw </summary>
        //public abstract List<Node> GetConnections(Node parent);
    }
}
