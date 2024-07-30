using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using Z3.Utils.ExtensionMethods;

namespace Z3.NodeGraph.Editor
{
    public class SMPort : NGPort
    {
        public SMPort(Orientation portOrientation, Direction portDirection, Capacity portCapacity, Type portType) : base(portOrientation, portDirection, portCapacity, portType)
        {
            if (portDirection == Direction.Input)
            {
                // Only output can drag
                pickingMode = PickingMode.Ignore;
            }
            else
            {
                // Only output can connect
                AddPortManipulator<SMEdge>();
            }

            ResizePort();

            // Hide display name
            portName = string.Empty;

            // Hide connection point
            m_ConnectorBoxCap.visible = false;
            m_ConnectorBox.visible = false;
        }

        private void ResizePort()
        {
            style.height = new Length(100, LengthUnit.Percent);
            style.width = new Length(100, LengthUnit.Percent);
            style.position = Position.Absolute;
            style.SetPosition(0f);

            style.paddingLeft = 0;
            style.paddingRight = 0;

            style.alignItems = Align.FlexStart; 
            style.flexDirection = FlexDirection.Column;
        }
    }
}