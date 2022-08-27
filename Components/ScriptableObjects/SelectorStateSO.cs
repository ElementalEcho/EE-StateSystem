using EE.Core;

namespace EE.StateSystem {
    public class SelectorStateSO : StateSO<SelectorState> {
        public override IState GetState(IHasComponents stateMachine) {
            return new SelectorState {
                Children = Children.GetStates(stateMachine),
                Origin = this,
            };
        }
    }
}

