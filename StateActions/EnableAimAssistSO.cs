using EE.Core.PoolingSystem;
using EE.PhysicsSystem;
using EE.StateSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EE.Core.Actions {
    public class EnableAimAssistSO : GenericActionSO<EnableAimAssist> {
        [SerializeField]
        private PoolableComponent poolableComponent = null;
        public PoolableComponent PoolableComponent => poolableComponent;
        [SerializeField]
        private Vector2 aimAssitOffset = new Vector2(0,0.5f);
        public Vector2 AimAssitOffset => aimAssitOffset;
    }
    public class EnableAimAssist : GenericAction {
        private EnableAimAssistSO OriginSO => (EnableAimAssistSO)_originSO;

        IHandComponent handComponent;
        IPhysics2DComponent physics2DComponent;
        Transform transform;

        PoolableComponent aimAssist;
        public override void Init(IHasComponents controller) {
            handComponent = controller.GetComponent<IHandComponent>();
            physics2DComponent = controller.GetComponent<IPhysics2DComponent>();
            transform = controller.GetComponent<Transform>();
        }

        public override void Enter() {
            aimAssist = PoolManager.SpawnObjectAsChild(OriginSO.PoolableComponent, transform,OriginSO.AimAssitOffset);
        }

        public override void Act(float tickSpeed) {
            aimAssist.transform.eulerAngles = new Vector3(0, 0, handComponent.AimAngle);

            int direction = physics2DComponent.FacingDirection.x > 0 ? -1 : 1;
            Vector3 scale = new Vector3(direction * Mathf.Abs(aimAssist.transform.localScale.x), 1, 1);

            aimAssist.transform.localScale = scale;
        }
        public override void Exit() {
            PoolManager.ReleaseObject(aimAssist);
        }
    }

}