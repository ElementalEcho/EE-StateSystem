using EE.Core;
using System.Collections.Generic;
namespace EE.StateSystem {

    namespace EE.StateSystem {
        public static class TransitionUtils {
            public static List<Transition> GetTransitions(this List<TransitionSO> transitionSOs, IHasComponents hasComponents, IStateMachine stateMachine) {
                List<Transition> transitions = new List<Transition>();
                foreach (var transitionSO in transitionSOs) {

                    DecisionGroup TransitionCondition = transitionSO.DecisionGroup.GetDecisionGroup(hasComponents);


                    var state = stateMachine.GetState(transitionSO.TransitionState);
                    if (state == null) {
                        state = transitionSO.TransitionState.GetState(hasComponents);
                        stateMachine.AddState(transitionSO.TransitionState,state);
                    }
                    Transition transition = new Transition(TransitionCondition, state);
                    transitions.Add(transition);
                }
                return transitions;
            }
        }
    }
}

