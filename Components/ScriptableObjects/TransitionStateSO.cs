using EE.Core;
using EE.Core.Utils;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace EE.StateSystem {

    namespace EE.StateSystem {
        public class TransitionStateSO : StateSO<TransitionState> {

            [SerializeField] private List<TransitionSO> transitionSOs = new List<TransitionSO>();

            [SerializeField]
            [InfoBox("This should be type of TransitionStateSO.")]
            private List<TransitionSO> defaultTransitions = null;
            public override IState GetState(IHasComponents stateMachine) {
                var stateMachineComponent = stateMachine != null ? stateMachine.GetComponent<IStateMachine>() : null;

                var transitionState = new TransitionState {
                    Children = Children.GetStates(stateMachine),
                    Origin = this,
                    stateMachineComponent = stateMachineComponent,
                };

                stateMachineComponent.AddState(this,transitionState);

                transitionState.stateTransition = transitionSOs.GetTransitions(stateMachine, stateMachineComponent);
                transitionState.defaultTransitions = defaultTransitions.GetTransitions(stateMachine, stateMachineComponent);
                return transitionState;

            }

        }
        [System.Serializable]
        public class TransitionSO {
            [Header("Transition")]
            public DecisionGroupSO DecisionGroup;
            public StateSO TransitionState;
        }
    }
}

