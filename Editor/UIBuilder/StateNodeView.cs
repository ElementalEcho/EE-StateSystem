using EE.AI;
using EE.StateSystem;
using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;

namespace EE {
    public class StateNodeView : Node {

        public Action<StateNodeView> OneNodeSelected;
        public StateSO stateSO;
        public IState state;

        public AIData aIDataSO;

        public Port input;
        public Port output;

        public StateNodeView(AIData aIDataSO,IState state) :base(BehaviorTreeEditor.FilPath +  "NodeView.uxml") {
            this.aIDataSO = aIDataSO;
            this.stateSO = (StateSO)state.Origin;
            this.state = state;

            this.viewDataKey = this.stateSO.name; //This should be guid
            var position = stateSO.position;
            style.left = position.x;
            style.top = position.y;

            if (this.stateSO is ActionStateSO) {
                this.title = ((ActionStateSO)stateSO).ActionNames();
            }
            else if (this.stateSO is DecisionStateSO) {
                this.title = ((DecisionStateSO)stateSO).DecisionNames();
            }
            else {
                this.title = this.stateSO.GetType().Name;
            }

            CreateInputPort();
            CreateOutputPort();
            SetUpClasses();
        }

        public override void SetPosition(Rect newPos) {
            base.SetPosition(newPos);
            Undo.RecordObject(stateSO, "AI Editor (Set Position)");
            stateSO.position = new Vector2(newPos.xMin, newPos.yMin);
            EditorUtility.SetDirty(stateSO);
        }
        public override void OnSelected() {
            base.OnSelected();
            OneNodeSelected?.Invoke(this);
            

        }
        private void CreateInputPort() {
            if (stateSO is ActionStateSO) {
                input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
            }
            if (stateSO is SequanceStateSO) {
                input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
            }
            if (stateSO is SelectorStateSO) {
                input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
            }
            if (stateSO is DecisionStateSO) {
                input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
            }
            if (stateSO is LoopStateSO) {
                input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
            }
            if (input != null) {
                input.portName = "";
                input.style.flexDirection = FlexDirection.Column;
                inputContainer.Add(input);
            }
        }
        private void CreateOutputPort() {
            if (stateSO is ActionStateSO) {
                //No ports
            }
            if (stateSO is SequanceStateSO) {
                output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(bool));
            }
            if (stateSO is SelectorStateSO) {
                output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(bool));
            }
            if (stateSO is DecisionStateSO) {
                output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));
            }
            if (stateSO is LoopStateSO) {
                output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));
            }
            if (output != null) {
                output.portName = "";
                input.style.flexDirection = FlexDirection.ColumnReverse;
                outputContainer.Add(output);
            }
        }

        private void SetUpClasses() {
            if (stateSO is ActionStateSO) {
                AddToClassList("actionstate");
            }
            if (stateSO is SequanceStateSO) {
                AddToClassList("sequancestate");
            }
            if (stateSO is SelectorStateSO) {
                AddToClassList("selectorstate");
            }
            if (stateSO is DecisionStateSO) {
                AddToClassList("decisionstate");
            }
        }
        public void UpdateState() {
            RemoveFromClassList("notstarted");
            RemoveFromClassList("running");
            RemoveFromClassList("success");
            RemoveFromClassList("failure");

            if (Application.isPlaying) {
                switch (state.Status) {
                    case StateStatus.NOTSTARTED:
                        break;
                    case StateStatus.RUNNING:
                        AddToClassList("running");
                        break;
                    case StateStatus.SUCCESS:
                        AddToClassList("success");
                        break;
                    case StateStatus.FAILED:
                        AddToClassList("failure");
                        break;
                    default:
                        break;
                }
            }
        }

        public void SortChildren() {
            stateSO.Children.Sort(SortByHorizontalPosition);
        }
        public int SortByHorizontalPosition(StateSO left, StateSO right) {
            return left.position.x < right.position.x ? -1: 1;
        }
    }
}
