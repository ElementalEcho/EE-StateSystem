using EE.Core;
using System;

namespace EE.StateSystem {
    [Serializable]
    public class ActionState : State {
        public GenericAction[] stateActions = new GenericAction[0];

        public bool LoopAfterSuccess = false;

        protected override void OnStart() {
            for (int i = 0; i < stateActions.Length; i++) {
                stateActions[i].Enter();
            }
        }

        protected override StateStatus OnUpdate(float tickSpeed) {
            int numberOfActions = stateActions.Length;

            for (int i = 0; i < stateActions.Length; i++) {
                if (stateActions[i].ExitCondition()) {
                    numberOfActions--;
                    if (numberOfActions <= 0) {
                        locked = false;
                        //Debug.Log("Lock removed", (UnityEngine.Object)Origin);
                        //return StateStatus.SUCCESS;
                    }
                }
            }

            if (numberOfActions > 0 || (LoopAfterSuccess && numberOfActions <= 0)) {
                for (int i = 0; i < stateActions.Length; i++) {
                    stateActions[i].Act(tickSpeed);
                }
            }

            return numberOfActions <= 0 ? StateStatus.SUCCESS : StateStatus.RUNNING;

        }

        protected override void OnStop() {
            for (int i = 0; i < stateActions.Length; i++) {
                stateActions[i].Exit();
            }
        }
    }

}




