using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using Z3.NodeGraph.BehaviourTree;
using Z3.Utils;
using Z3.Utils.ExtensionMethods;
using Node = Z3.NodeGraph.Core.Node;

namespace Z3.NodeGraph.Editor
{
    public class BehaviourTreeGraphModule : NodeGraphModule<BehaviourTreeData>
    {
        public override void OnInitialize()
        {
            List<Node> nodeList = CurrentGraph.SubAssets.OfType<Node>().ToList();
            InitializeNodes(nodeList);
        }

        protected override void OnMovedElements(List<GraphElement> movedElements)
        {
            nodes.ForEach(n => n.GetNodeView<BTNodeView>().SortChildren());
        }

        protected override void OnElementsToRemove(List<GraphElement> elementsToRemove)
        {
            base.OnElementsToRemove(elementsToRemove);
            // TODO: Check output and input is valid, and connect to each other
        }

        protected override void OnEdgesToCreate(List<NGEdge> edgesToCreate)
        {
            edgesToCreate.ForEach(edge =>
            {
                BTNodeView parent = edge.output.GetNodeView<BTNodeView>();
                BTNodeView child = edge.input.GetNodeView<BTNodeView>();
                UndoRecorder.AddUndo(parent.Node, "Add Child");
                CurrentGraph.AddConnection(parent.Node, child.Node);
                EditorUtility.SetDirty(parent.Node);
            });
        }

        public override void BuildGraphMenu(ContextualMenuPopulateEvent evt)
        {
            base.BuildGraphMenu(evt);

            CreateContextItem<LeafNode>(evt);
            CreateContextItem<DecoratorNode>(evt);
            CreateContextItem<CompositeNode>(evt);
        }

        protected void CreateContextItem<T>(ContextualMenuPopulateEvent evt) where T : BehaviourTreeNode
        {
            IEnumerable<Type> types = ReflectionUtils.GetDeriveredConcreteTypes<T>();
            string subName = StringUtils.GetTypeNiceString<T>();

            Vector2 localMousePosition = evt.localMousePosition;

            foreach (Type type in types)
            {
                string typeName = type.Name.GetNiceString();
                evt.menu.AppendAction($"Create BT Node/{subName}/{typeName}", (_) => CreateNode(type, localMousePosition));
            }
        }

        protected override NodeView MakeNodeView(Node node)
        {
            if (node is SubGraphDataBT subGraphDataBT)
            {
                return new SubGraphDataBTNodeView(References, subGraphDataBT);
            }

            return new BTNodeView(References, node as BehaviourTreeNode);
        }
    }
}