using EE.Core;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using EE.AI;

namespace EE.StateSystem {

    public class StateMachineComponent : EEMonobehavior, IStateMachine, IPoolable, IUpdater {

        private StateMachine _stateMachine;

        [SerializeField]
        private StateSO defaultState;
        [SerializeField]
        private AIDataSO aiData;
        [SerializeField]
        private AIData aIData;
        public AIData AIData {
            get {
                if (aiData == null) {
                    return null;
                }
                if (aIData == null || aIData.Origin == null) {
                    aIData = aiData.GetAIData(this);
                }

                return aIData;
            }
        }

        [SerializeField]
        private bool showTransitionLogs = false;

        public void StartIEnumerator(IEnumerator enumerator) {
            StartCoroutine(enumerator);
        }

        public Vector2 Position => transform.position;


        private IState defaultIState;

        public readonly Dictionary<IHasState, IState> _states = new Dictionary<IHasState, IState>();

        private void Awake() {
            _stateMachine = new StateMachine();
        }

        private void Start() {
            if (aiData != null && _stateMachine.IsInState(null)) {
                aIData = aiData.GetAIData(this);
                defaultIState = aIData.State;
                AddState(defaultState, defaultIState);
                TransitionToState(defaultIState);
            }
            else if (defaultState != null && _stateMachine.IsInState(null)) {
                defaultIState = defaultState.GetState(this);
                AddState(defaultState, defaultIState);
                TransitionToState(defaultIState);
            }
#if UNITY_EDITOR
            else {
                Debug.LogWarning("Has already state when spawned.", this);
            }
#endif
        }

        public void CustomUpdate(float tickSpeed) {
            Act(tickSpeed);
        }

        public void TransitionToState(IState nextState) {
            if (showTransitionLogs) {
                UnityEngine.Debug.Log("Transitioned State: " + nextState.Origin);
            }
            if (nextState == null) {
                nextState = defaultIState;
            }
            _stateMachine.TransitionToState(nextState);
        }

        public void Act(float tickSpeed) => _stateMachine.Act(tickSpeed);

        public bool IsInState(IState state) => _stateMachine.IsInState(state);


        public IState GetState(IHasState state) {
            return _stateMachine.GetState(state);
        }

        public void AddState(IHasState hasState, IState state) {
            _stateMachine.AddState(hasState, state);
        }
#if UNITY_EDITOR

        [Button]
        void OnValidate() {
            if (!base.gameObject.activeSelf) {
                return;
            }      
        }



#endif


    }
}


