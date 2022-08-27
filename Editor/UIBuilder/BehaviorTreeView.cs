using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor;
using EE.AI;
using System;
using EE.Core;
using System.Linq;
using EE.StateSystem;
using EE.Core.Utils;

namespace EE
{
    public class BehaviorTreeView : GraphView
    {
        public Action<StateNodeView> OneNodeSelected;

        AIData aIData;

        public new class UxmlFactory : UxmlFactory<BehaviorTreeView, GraphView.UxmlTraits> { }
        public BehaviorTreeView() {
            Insert(0, new GridBackground());


            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(BehaviorTreeEditor.FilPath + "BehaviorTreeEditor.uss");
            styleSheets.Add(styleSheet);

            Undo.undoRedoPerformed += OnUndoRedo;
        }

        private void OnUndoRedo() {
            if (aIData == null) {
                return;
            }
            var aiData = aIData.Origin.GetAIData();
            PopulateView(aiData);
            Debug.Log("Undo");
            AssetDatabase.SaveAssets();
        }

        public void PopulateView(AIData aIData) {
            this.aIData = aIData;
            graphViewChanged -= OnGraphViewChanged;
            DeleteElements(graphElements);
            graphViewChanged += OnGraphViewChanged;
            DrawActionView(aIData.State, null);
        }

        public void DrawActionView(IState state,StateNodeView gOAPActionNodeView) {
            if (state == null || state.Origin == null) {
                return;
            }
            var view = CreateActionView(state);

            if (gOAPActionNodeView != null && gOAPActionNodeView.output != null) {
                var edge = gOAPActionNodeView.output.ConnectTo(view.input);

                AddElement(edge);
            }
            foreach (var child in state.Children) {
                DrawActionView(child, view);
            }
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter) {
            return ports.ToList().Where(endPort =>
                endPort.direction != startPort.direction &&
                endPort.node != startPort.node
            ).ToList();
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange) {
            if (graphViewChange.elementsToRemove != null) {
                graphViewChange.elementsToRemove.ForEach(elem => {
                    StateNodeView stateNodeView = elem as StateNodeView;
                    if (stateNodeView != null) {
                        DeleteNode(stateNodeView.stateSO);
                    }

                    Edge edge = elem as Edge;
                    if (edge != null) {
                        StateNodeView parentView = edge.output.node as StateNodeView;
                        StateNodeView childView = edge.input.node as StateNodeView;
                        aIData.Origin.RemoveState(parentView.stateSO,childView.stateSO);
                    }

                });
            }
            if (graphViewChange.edgesToCreate != null) {
                graphViewChange.edgesToCreate.ForEach(edge => {
                    StateNodeView parentView = edge.output.node as StateNodeView;
                    StateNodeView childView = edge.input.node as StateNodeView;
                    aIData.Origin.AddChild(parentView.stateSO, childView.stateSO);
                });
            }
            if (graphViewChange.movedElements != null) {
                nodes.ForEach((n) => {
                    StateNodeView stateNodeView = n as StateNodeView;
                    stateNodeView.SortChildren();
                });                
            }
            return graphViewChange;
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt) {
            {
                var types = TypeCache.GetTypesDerivedFrom<StateSO>();
                foreach (var type in types) {
                    evt.menu.AppendAction($"Create GOAP Nodee {type.Name}", (a) => CreateNode(type));
                }
            }

        }
        void CreateNode(Type type) {
            StateSO stateSO = ScriptableObject.CreateInstance(type) as StateSO;
            stateSO.name = type.Name;
            Undo.RecordObject(aIData.Origin, "AI Editor (CreateNode)");

            AssetDatabase.AddObjectToAsset(stateSO, aIData.Origin);
            Undo.RegisterCreatedObjectUndo(stateSO, "AI Editor (CreateNode)");

            AssetDatabase.SaveAssets();
            CreateActionView(stateSO.GetState(null));
        }
        void DeleteNode(StateSO node) {
            Undo.RecordObject(aIData.Origin, "AI Editor (DeleteNode)");
            Undo.DestroyObjectImmediate(node);
            AssetDatabase.SaveAssets();
        }
        StateNodeView CreateActionView(IState state) {
            StateNodeView nodeView = new StateNodeView(aIData, state) {
                OneNodeSelected = OneNodeSelected
            };
            AddElement(nodeView);

            return nodeView;
        }

        public void UpdateNodeStates() {
            nodes.ForEach(n => {
                StateNodeView stateNodeView = n as StateNodeView;
                stateNodeView.UpdateState();
            });
        }



    }
}
