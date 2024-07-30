using UnityEngine.UIElements;
using Z3.NodeGraph.BehaviourTree;

namespace Z3.NodeGraph.Editor
{
    public class SubGraphDataBTNodeView : BTNodeView
    {
        private SubGraphNodeView subGraphView;

        public SubGraphDataBTNodeView(NodeGraphReferences references, SubGraphDataBT node) : base(references, node)
        {
        }

        public override VisualElement GetInspector()
        {
            VisualElement visualElement = base.GetInspector();
            return visualElement;

            BindableElement b = visualElement.Q<BindableElement>();

            subGraphView = new SubGraphNodeView(References, (SubGraphDataBT)Node, b);

            visualElement.Add(subGraphView);

            return visualElement;
        }

        protected override void OnUpdateUI()
        {
            base.OnUpdateUI();

            if (subGraphView == null)
                return;

            subGraphView.OnUpdateUI();
        }

        public override void OnSelected()
        {
            base.OnSelected();

            //subGraphView.OnSelect();
        }

        public override void OnUnselected()
        {
            base.OnUnselected();

            //subGraphView.OnUnselected();
        }
    }

}