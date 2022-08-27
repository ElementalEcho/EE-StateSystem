using System;

namespace EE.StateSystem {
    [Serializable]
    public class LoopState : State {
        public bool CheckPriorityBeforeLooping;

        protected override void OnStart() {
            Children[0].Start();
        }

        protected override StateStatus OnUpdate(float tickSpeed) {
            StateStatus stateStatus = Children[0].Update(tickSpeed);
            if (stateStatus == StateStatus.SUCCESS) {
                Children[0].Stop();
                if (!CheckPriorityBeforeLooping || (CheckPriorityBeforeLooping && Priority > 0)) {
                    Children[0].Start();
                }
            }

            return stateStatus == StateStatus.FAILED ? StateStatus.FAILED : StateStatus.RUNNING;
        }

        protected override void OnStop() {
            Children[0].Stop();

        }


    }


}




