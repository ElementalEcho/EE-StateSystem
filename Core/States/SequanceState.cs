using System;

namespace EE.StateSystem {
    [Serializable]
    public class SequanceState : State {
        protected int _stateIndex = 0;
        public bool failedIsSuccess = false;

        protected override void OnStart() {
            _stateIndex = 0;
            Children[_stateIndex].Start();
        }

        protected override StateStatus OnUpdate(float tickSpeed) {
            if(_stateIndex >= Children.Count) {
                return StateStatus.SUCCESS;               
            }
            var child = Children[_stateIndex];
            switch (child.Update(tickSpeed)) {
                case StateStatus.RUNNING:
                    return StateStatus.RUNNING;
                case StateStatus.FAILED:
                    return failedIsSuccess ? HandleStateSuccess(child) : StateStatus.FAILED;                   
                case StateStatus.SUCCESS:
                    return HandleStateSuccess(child);
            }

            return StateStatus.RUNNING;
        }
        private StateStatus HandleStateSuccess(IState child) {
            child.Stop();
            _stateIndex++;
            if (_stateIndex < Children.Count) {
                child = Children[_stateIndex];
                child.Start();
                return StateStatus.RUNNING;
            }
            else {
                locked = false;
                return StateStatus.SUCCESS;
            }
        }
        protected override void OnStop() {
            if (_stateIndex < Children.Count) {
                Children[_stateIndex].Stop();
            }
            _stateIndex = 0;

        }
    }

}

