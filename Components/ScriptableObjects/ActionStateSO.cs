using EE.Core;
using Sirenix.OdinInspector;
using UnityEngine;

namespace EE.StateSystem {
    public class ActionStateSO : StateSO<ActionState> {
        [SerializeField]
        private bool loopAfterSuccess = false;

        [SerializeField]
        [InlineEditor]
        private GenericActionSO[] _actions = new GenericActionSO[0];

        public override IState GetState(IHasComponents stateMachine) {
            return new ActionState {
                stateActions = _actions.GetActions(stateMachine),
                Origin = this,
                LoopAfterSuccess = loopAfterSuccess
            };
        }

#if UNITY_EDITOR
        public string ActionNames() {
            System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
            foreach (var item in _actions) {
                stringBuilder.AppendLine(item.name);
            }
            return stringBuilder.ToString();
        }
#endif
    }

}




