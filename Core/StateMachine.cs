using System.Collections.Generic;

namespace EE.StateSystem {

    public interface IStateMachine {
        void TransitionToState(IState nextState);
        IState GetState(IHasState hasState);
        void AddState(IHasState hasState, IState state);
    }

    public class StateMachine : IStateMachine {
        private IState _currentState = null;
        private readonly Dictionary<IHasState, IState> _states = new Dictionary<IHasState, IState>();

        public void TransitionToState(IState nextState) {
            if (_currentState != null) {
                _currentState.Stop();
            }
            _currentState = nextState;
            nextState.Start();
        }

        public void Act(float tickSpeed) {
            if (_currentState != null) {
                _currentState.Update(tickSpeed);
            }        
        }

        public virtual bool IsInState(IState state) => _currentState == state;

        public IState GetState(IHasState hasState) {
            if (!_states.TryGetValue(hasState, out IState state)) {
                return null;
            }
            return state;
        }
        public void AddState(IHasState hasState, IState state) {
            _states.TryAdd(hasState, state);
        }
    }
}
