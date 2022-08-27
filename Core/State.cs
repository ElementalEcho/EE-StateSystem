using EE.Core;
using System.Collections.Generic;

namespace EE.StateSystem {
    [System.Serializable]
    public abstract class State : IState {
        //ScriptableObject used to create this State
        public IHasState Origin { get; set; }

        protected StateStatus status = StateStatus.NOTSTARTED;
        public StateStatus Status => status;

        private List<IState> children = new List<IState>();
        public List<IState> Children { get => children; set => children = value; }

        public int priority = 100;

        public bool locked;
        public bool Lock { 
            get {
                if (locked) {
//#if UNITY_EDITOR    
//                    Debug.Log("State is locked : " + Origin, (Object)Origin);
//#endif
                    return true;
                }
                foreach (var item in children) {
                    if (item.Lock) {
                        return true;
                    }
                }
                return false;

            }
        } 
        public void Start() {
            status = StateStatus.RUNNING;
            locked = Origin != null && Origin.Lock;
            OnStart();
        }
        public StateStatus Update(float tickSpeed) {
            status = OnUpdate(tickSpeed);
            return status;
        }
        public void Stop() {
            OnStop();
            status = StateStatus.NOTSTARTED;
        }

        protected abstract void OnStart();
        protected abstract StateStatus OnUpdate(float tickSpeed);
        protected abstract void OnStop();
        public virtual int Priority { 
            get {
                foreach (var state in children) {
                    if (state.Priority <= 0) {
                        return 0;
                    }
                }
                return priority;
            }
        } 

    }
    public enum StateStatus {
        RUNNING,
        SUCCESS,
        FAILED,
        NOTSTARTED
    }
    public interface IState {
        int Priority { get; }
        bool Lock { get; }
        StateStatus Status { get; }
        IHasState Origin { set; get; }
        List<IState> Children { set; get; }
        void Start();
        StateStatus Update(float tickSpeed);
        void Stop();
    }



    public interface IHasState {
        IState GetState(IHasComponents stateMachine);
        bool Lock { get; }
    }
}




