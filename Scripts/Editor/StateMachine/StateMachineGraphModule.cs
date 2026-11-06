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

            Undo.undoRedoEvent += OnUndoRedo;
        }

        public override void Dispose()
        {
            base.Dispose();
            Undo.undoRedoEvent -= OnUndoRedo;
        }

        private void OnUndoRedo(in UndoRedoInfo undo)
        {
            if (undo.undoName != UndoRecorder.Prefix + UndoRecorder.MoveTransition)
                return;

            References.Refresh();
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
                newTransition.SetupConnection(child.Node);

                // Before to add and remember
                UndoRecorder.AddUndo(parent.Node, "Add Transition");
                CurrentGraph.AddTransition(parent.Node, newTransition);

                // If an edge visual element already exists, delete the old
                SMEdge existingEdge = SMEdgeUtils.FindEdge(this, parent.Node, child.Node);

                if (existingEdge != null)
                {
                    existingEdge.input.Disconnect(existingEdge);
                    existingEdge.output.Disconnect(existingEdge);
                    Controller.RemoveElement(existingEdge);
                }

                // Refresh circular offset removed with the ghost, before this transition existed
                SMEdgeUtils.FindEdge(this, child.Node, parent.Node)?.UpdateEdgeControl();

                // Add and remember
                if (!Application.isPlaying)
                {
                    AssetDatabase.AddObjectToAsset(newTransition, CurrentGraph);
                }
                UndoRecorder.AddCreation(newTransition, "Create Transition");

                AssetDatabase.SaveAssets();
            });
        }

        /// <summary> Drag a node transition and drop to another node, replacing the incoming or outgoing target </summary>
        public void MoveTransitions(StateMachineNode owner, List<Transition> transitions, StateMachineNode newNode, bool moveTarget)
        {
            Undo.IncrementCurrentGroup();
            int undoGroup = Undo.GetCurrentGroup();

            if (!moveTarget)
            {
                // Changed the incoming owner
                List<Transition> ownerTransitions = owner.GetTransitions();
                List<Transition> newOwnerTransitions = newNode.GetTransitions();

                UndoRecorder.AddUndo(owner, UndoRecorder.MoveTransition);
                UndoRecorder.AddUndo(newNode, UndoRecorder.MoveTransition);

                foreach (Transition transition in transitions)
                {
                    ownerTransitions.Remove(transition);
                    newOwnerTransitions.Add(transition);
                    NodeGraphEditorUtils.UpdateSubAssetName(transition, newNode);
                }

                EditorUtility.SetDirty(owner);
                EditorUtility.SetDirty(newNode);
            }
            else
            {
                // Changed the outgoing target
                TransitableStateNode newTarget = (TransitableStateNode)newNode;

                foreach (Transition transition in transitions)
                {
                    UndoRecorder.AddUndo(transition, UndoRecorder.MoveTransition);
                    transition.SetupConnection(newTarget);
                    EditorUtility.SetDirty(transition);
                }
            }

            Undo.SetCurrentGroupName(UndoRecorder.Prefix + UndoRecorder.MoveTransition);
            Undo.CollapseUndoOperations(undoGroup);

            AssetDatabase.SaveAssets();
            References.Refresh();
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