using EE.Core;
namespace EE.StateSystem {

    namespace EE.StateSystem {
        [System.Serializable]
        public class Transition {
            public DecisionGroup DecisionGroup;
            public IState TransitionState;

            public Transition(DecisionGroup transitionCondition, IState transitionState) {
                this.DecisionGroup = transitionCondition;
                TransitionState = transitionState;
            }
            public bool CanTransition() => DecisionGroup.Decide();

        }
    }
}

