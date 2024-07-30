using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace Z3.NodeGraph.Editor
{
    public class NGPort : Port
    {
        public NGPort(Orientation portOrientation, Direction portDirection, Capacity portCapacity, Type portType) : base(portOrientation, portDirection, portCapacity, portType) { }

        /// <summary> This will add drag and drop connections functionality between Output Ports to Input Ports </summary>
        public void AddPortManipulator<TEdge>() where TEdge : NGEdge, new()
        {
            CustomEdgeConnectorListener listener = new CustomEdgeConnectorListener();
            m_EdgeConnector = new EdgeConnector<TEdge>(listener);
            this.AddManipulator(m_EdgeConnector);
        }

        /// <summary> Manual connection, useful for initialization </summary>
        public TEdge InstantiateEdge<TEdge>(NGPort other, NodeGraphReferences references) where TEdge : NGEdge, new()
        {
            TEdge edge = ConnectTo<TEdge>(other);
            edge.InjectDependencies(references);
            references.Module.AddElement(edge);
            return edge;
        }
    }
}