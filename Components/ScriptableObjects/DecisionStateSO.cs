using EE.Core;
using Sirenix.OdinInspector;
using UnityEngine;
namespace EE.StateSystem {

    public class DecisionStateSO : StateSO<DecisionState> {
        [SerializeField, InlineProperty, HideLabel] 
        private DecisionGroupSO decisionGroup = new DecisionGroupSO();

        public override IState GetState(IHasComponents stateMachine) {
            return new DecisionState {
                decisions = decisionGroup.decisions.GetActions(stateMachine),
                Children = Children.GetStates(stateMachine),
                decideType = decisionGroup.decideType,
                Origin = this,
            };
        }

#if UNITY_EDITOR

        public string DecisionNames() {
            System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
            foreach (var item in decisionGroup.decisions) {
                stringBuilder.AppendLine(item.name);
            }
            return stringBuilder.ToString();
        }
#endif
    }
}