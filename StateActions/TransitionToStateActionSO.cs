using EE.Core;
using EE.StateSystem;
using UnityEngine;

namespace EE.InputManagement.Actions {
    public class TransitionToStateActionSO : GenericActionSO<TransitionToStateAction> {
        [SerializeField]
        private StateSO stateSO = null;
        public StateSO StateSO => stateSO;

    }
    public class TransitionToStateAction : GenericAction {
        private TransitionToStateActionSO OriginSO => (TransitionToStateActionSO)base._originSO;

        IStateMachine stateMachine;
        IState state;

        public override void Init(IHasComponents controller) {
            stateMachine = controller.GetComponent<IStateMachine>();
            state = OriginSO.StateSO.GetState(controller);
        }

        public override void Enter() {
            stateMachine.TransitionToState(state);
        }

    }

}