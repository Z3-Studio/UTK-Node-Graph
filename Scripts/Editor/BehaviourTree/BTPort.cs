using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using Z3.Utils.ExtensionMethods;

namespace Z3.NodeGraph.Editor
{
    public class BTPort : NGPort
    {
        public BTPort(Orientation portOrientation, Direction portDirection, Capacity portCapacity, Type portType) : base(portOrientation, portDirection, portCapacity, portType)
        {
            // Only output can connect
            AddPortManipulator<BTEdge>();

            if (portDirection == Direction.Input)
            {
                style.flexDirection = FlexDirection.Column;
                style.alignItems = Align.FlexEnd;
            }
            else
            {
                style.flexDirection = FlexDirection.ColumnReverse;
                style.alignItems = Align.FlexStart;
            }

            ResizePort();

            // Hide display name
            portName = string.Empty;

            // Hide connection point
            m_ConnectorBox.visible = false;
            m_ConnectorBoxCap.visible = false;
        }

        private void ResizePort()
        {
            // height is 24 Pixels
            style.width = new Length(100, LengthUnit.Percent);

            style.paddingLeft = 0;
            style.paddingRight = 0;

            m_ConnectorBox.style.width = new Length(100, LengthUnit.Percent);
            m_ConnectorBox.style.SetMargin(0f);
        }
    }
}