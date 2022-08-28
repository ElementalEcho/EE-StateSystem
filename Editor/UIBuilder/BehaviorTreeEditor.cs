using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using EE.AI;
using UnityEditor.Callbacks;
using EE.AI.Impl;
using EE.StateSystem;

namespace EE {
    public class BehaviorTreeEditor : EditorWindow {

        public static string FilPath = "Packages/ee.statesystem/Editor/UIBuilder/";
        BehaviorTreeView behaviorTreeView;
        InspectorView inspectorView;

        [MenuItem("Tools/EE/BehaviorTreeEditor")]
        public static void OpenMenu() {
            BehaviorTreeEditor wnd = CreateInstance<BehaviorTreeEditor>();
            wnd.Show();
            wnd.titleContent = new GUIContent("BehaviorTreeEditor");
        }
        [OnOpenAsset]
        public static bool OnAssetOpen(int instanceId,int line) {
            if (Selection.activeObject is AIDataSO) {
                OpenMenu();
                return true;
            }
            return false;
        }

        public void CreateGUI() {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;

            // Import UXML
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(FilPath + "BehaviorTreeEditor.uxml");
            visualTree.CloneTree(root);

            // A stylesheet can be added to a VisualElement.
            // The style will be applied to the VisualElement and all of its children.
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(FilPath +  "BehaviorTreeEditor.uss");
            root.styleSheets.Add(styleSheet);

            behaviorTreeView = root.Q<BehaviorTreeView>();
            inspectorView = root.Q<InspectorView>();
            behaviorTreeView.OneNodeSelected = OnNodeSelectionChange;
            OnSelectionChange();
        }

        private void OnSelectionChange() {
            AIDataSO tree = Selection.activeObject as AIDataSO;
            if (tree) {
                var aiData = tree.GetAIData();
                behaviorTreeView.PopulateView(aiData);
            }

            if (Selection.activeGameObject && Selection.activeGameObject.TryGetComponent(out StateMachineComponent aIAgentComponent) && aIAgentComponent.AIData != null) {
                var aiData = aIAgentComponent.AIData;
                behaviorTreeView.PopulateView(aiData);
            }
        }

        void OnNodeSelectionChange(StateNodeView gOAPActionNodeView) {
            inspectorView.UpdateSelection(gOAPActionNodeView);
        }
        void OnInspectorUpdate() {
            if (behaviorTreeView != null) {
                behaviorTreeView.UpdateNodeStates();
            }
        }
    }
}