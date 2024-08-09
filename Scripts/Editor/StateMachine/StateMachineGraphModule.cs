using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using Z3.NodeGraph.Core;
using Z3.NodeGraph.StateMachine;
using Z3.Utils.ExtensionMethods;
using Z3.Utils;

namespace Z3.NodeGraph.Editor
{
    public class StateMachineGraphModule : NodeGraphModule<StateMachineData>
    {
        public override void OnInitialize()
        {
            List<StateMachineNode> nodeList = CurrentGraph.SubAssets.OfType<StateMachineNode>().ToList();
            InitializeNodes(nodeList);
        }

        protected override void OnEdgesToCreate(List<NGEdge> edgesToCreate)
        {
            edgesToCreate.Cast<SMEdge>().ForEach(smEdge =>
            {
                SMNodeView parent = smEdge.OutputNodeView;
                SMTransitableNodeView child = smEdge.InputNodeView;

                // Instantiate and setup
                Type type = typeof(Transition);
                Transition newTransition = ScriptableObject.CreateInstance(type) as Transition;

                newTransition.SetGuid(GUID.Generate().ToString(), $"{parent.Node.name}/");
                newTransition.Setup(child.Node);

                // Before to add and remember
                UndoRecorder.AddUndo(parent.Node, "Add Transition");
                CurrentGraph.AddTransition(parent.Node, newTransition);

                // Add and remember
                if (!Application.isPlaying)
                {
                    AssetDatabase.AddObjectToAsset(newTransition, CurrentGraph);
                }
                UndoRecorder.AddCreation(newTransition, "Create Transition");

                AssetDatabase.SaveAssets();
            });
        }

        public override void BuildGraphMenu(ContextualMenuPopulateEvent evt)
        {
            base.BuildGraphMenu(evt);

            IEnumerable<Type> types = ReflectionUtils.GetDeriveredConcreteTypes<StateMachineNode>();

            IEnumerable<Type> transitable = types.Where(t => t.IsSubclassOf(typeof(TransitableStateNode)));
            IEnumerable<Type> parallel = types.Where(t => typeof(IParallelState).IsAssignableFrom(t));

            CreateContextItem("Transitable", transitable, evt);
            CreateContextItem("Parallel", parallel, evt);
        }

        public override void BuildNodeMenu(ContextualMenuPopulateEvent evt, NodeView node)
        {
            if (node.Node is IOutputNode outputNode && outputNode.Startable)
            {
                base.BuildNodeMenu(evt, node);
            }
        }

        protected void CreateContextItem(string subName, IEnumerable<Type> types, ContextualMenuPopulateEvent evt)
        {
            Vector2 localMousePosition = evt.localMousePosition;

            foreach (Type type in types)
            {
                string typeName = type.GetTypeNiceString();
                evt.menu.AppendAction($"Create SM Node/{subName}/{typeName}", (_) => CreateNode(type, localMousePosition));
            }
        }

        protected override NodeView MakeNodeView(Node node)
        {
            if (node is TransitableStateNode transitableNode)
            {
                return new SMTransitableNodeView(References, transitableNode);
            }

            return new SMNodeView(References, node as StateMachineNode);
        }

        [Obsolete("TODO: Use IGraphElement")]
        internal void Inspect(VisualElement transitionView)
        {
            VisualElement inspector = References.Inspector;

            inspector.Clear();
            inspector.Add(transitionView);
        }
    }
}