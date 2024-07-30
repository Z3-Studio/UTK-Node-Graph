using UnityEditor;
using UnityEngine;
using Z3.NodeGraph.BehaviourTree;
using Z3.UIBuilder.Editor.ExtensionMethods;

namespace Z3.NodeGraph.Editor
{
    public class BTEdge : NGEdge
    {

        private Color color;

        public BTEdge() : base()
        {
            this.RegisterUpdate(SetColor);
        }

        public override bool UpdateEdgeControl()
        {
            bool value = base.UpdateEdgeControl();

            if (Application.isPlaying)
            {
                edgeControl.inputColor = color;
                edgeControl.outputColor = color;
            }
            //SetColor();

            return value;
        }

        public override void OnSelected()
        {
            base.OnSelected();
            selected = true;
        }

        public override void OnUnselected()
        {
            base.OnUnselected();
            selected = false;
        }

        private void SetColor()
        {
            if (input == null || !Application.isPlaying)
                return;

            BTNodeView btInput = input.GetNodeView<BTNodeView>();

            color = btInput.ColorState;

            float t = Mathf.Lerp(1, 0, color.a);

            color.a = 1;

            color = Color.Lerp(color, Color.gray, t);

            edgeControl.inputColor = color;
            edgeControl.outputColor = color;
        }

        public override void DeleteElement()
        {
            BTNodeView parent = output.GetNodeView<BTNodeView>();
            BTNodeView child = input.GetNodeView<BTNodeView>();
            UndoRecorder.AddUndo(parent.Node, "Remove Child");
            (References.Data as BehaviourTreeData).RemoveConnection(parent.Node, child.Node);
            EditorUtility.SetDirty(parent.Node);
        }
    }
}