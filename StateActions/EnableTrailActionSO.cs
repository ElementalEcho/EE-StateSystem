using EE.StateSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EE.Core.Actions {
    public class EnableTrailActionSO : GenericActionSO<EnableTrailAction> {
        public Color trailColor = new Color(191f, 151f, 94f);
        public float trailRange = 0;
    }
    public class EnableTrailAction : GenericAction {
        private EnableTrailActionSO OriginSO => (EnableTrailActionSO)_originSO;

        IHandComponent handComponent;
        public override void Init(IHasComponents controller) {
            handComponent = controller.GetComponent<IHandComponent>();
        }

        public override void Enter() {
            if (OriginSO.Reverse) {
                handComponent.DisableTrail();
            }
            else {
                handComponent.EnableTrail(OriginSO.trailRange, OriginSO.trailColor);
            }
        }
    }

}