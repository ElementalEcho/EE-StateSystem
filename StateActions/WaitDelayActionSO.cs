using EE.Core;
using UnityEngine;

namespace EE.StateSystem {
    public class WaitDelayActionSO : GenericActionSO<WaitDelayAction> {
        [SerializeField]
        public float waitTime = 4;
    }

    public class WaitDelayAction : GenericAction {
        private WaitDelayActionSO OriginSO => (WaitDelayActionSO)base._originSO;
        private float timer; 

        public override void Enter() {
            timer = OriginSO.waitTime;
        }

        public override void Act(float tickSpeed) {
            timer -= tickSpeed;
        }
        public override void Exit() {
            timer = 0;
        }
        public override bool ExitCondition() {
            return timer <= 0;
        }
        protected override bool Decide() {
            return timer <= 0;
        }
    }
}