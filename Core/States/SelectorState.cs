using System;
using UnityEngine;

namespace EE.StateSystem {
    [Serializable]
    public class SelectorState : State {

        protected int _stateIndex = -1;

        protected override void OnStart() {
            for (int i = 0; i < Children.Count; i++) {
                if (Children[i].Priority > 0) {
                    _stateIndex = i;
                    Children[_stateIndex].Start();
                    return;
                }
            }
        }

        protected override StateStatus OnUpdate(float tickSpeed) {
            if (_stateIndex < 0) {
                GetStateWithHighestPriority();
            }
            if (_stateIndex < 0) {
                return StateStatus.FAILED;
            }

            StateStatus stateStatus = Children[_stateIndex].Update(tickSpeed);
            if (_stateIndex >= Children.Count || _stateIndex < 0) {
                return StateStatus.FAILED;
            }
            if (Children[_stateIndex].Lock && stateStatus != StateStatus.SUCCESS) {
                return StateStatus.RUNNING;
            }
            for (int i = 0; i < Children.Count; i++) {
                if (Children[i].Priority > 0) {
                    if (i == _stateIndex) {
                        return stateStatus;
                    }
                    Children[_stateIndex].Stop();
                    _stateIndex = i;
                    if (_stateIndex < Children.Count) {
                        Children[_stateIndex].Start();
                        return Children[_stateIndex].Update(tickSpeed);
                    }
                }
            }
            return StateStatus.FAILED;
        }
        public override int Priority => Children.Count >= _stateIndex && _stateIndex  < 0 ? 0 : Children[_stateIndex].Priority;

        protected override void OnStop() {
            if (_stateIndex >= 0) {
                Children[_stateIndex].Stop();
            }
            _stateIndex = -1;
        }

        private void GetStateWithHighestPriority() {
            for (int i = 0; i < Children.Count; i++) {
                if (Children[i].Priority > 0) {
                    _stateIndex = i;
                    Children[_stateIndex].Start();
                    return;
                }
            }
        }
    }
}

