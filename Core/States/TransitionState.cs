using EE.Core;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace EE.StateSystem {

    namespace EE.StateSystem {
        [Serializable]
        public class TransitionState : State {
            public IStateMachine stateMachineComponent;
            public IHasComponents hasComponents;

            public List<Transition> stateTransition = new List<Transition>();
            public List<Transition> defaultTransitions = new List<Transition>();

            protected override void OnStart() => Children.ForEach(state => state.Start());

            protected override void OnStop() => Children.ForEach(state => state.Stop());


            protected override StateStatus OnUpdate(float tickSpeed) {
                foreach (var transition in stateTransition) {
                    if (transition.CanTransition()) {
                        stateMachineComponent.TransitionToState(transition.TransitionState);
                        return StateStatus.SUCCESS;
                    }
                }
                if (Children[0].Update(tickSpeed) == StateStatus.SUCCESS) {

                    foreach (var defaultTransition in defaultTransitions) {
                        if (defaultTransition.CanTransition()) {
                            stateMachineComponent.TransitionToState(defaultTransition.TransitionState);
                            return StateStatus.SUCCESS;
                        }
                    }
                }
                return StateStatus.RUNNING;
            }

        }
    }
}

