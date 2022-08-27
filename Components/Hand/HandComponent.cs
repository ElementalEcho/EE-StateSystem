using EE.Core;
using System;
using UnityEngine;
using EE.PhysicsSystem;
using EE.CameraManagement;
using EE.InventorySystem;
using System.Collections;

namespace EE.StateSystem {

    public class ProvideMousePosition : IHasPosition {
        public Vector2 Position => Input.mousePosition;
    }
    public interface IHandComponent {
        Vector2 FacingDirectionToRotationTarget { get; }
        Vector2 AttackOrigin { get; }
        float AimAngle { get; }
        Vector2 HandGameObjectPosition { get; }
        void EnableHand();
        void DisableHand();
        void EnableTrail(float distance, Color color);
        void DisableTrail();
        void UpdateHandRotation(float angle);
    }
}
namespace EE.StateSystem.Impl {
    public class HandComponent : EEMonobehavior, IPoolable, IHandComponent {
        [SerializeField]
        private float trailOffSet = 0.75f;
        [SerializeField]
        private CameraDataContainment cameraDataContainment = null;
        [SerializeField]
        private Transform handFollowTarget = null;
        [SerializeField]
        private Transform handGameObject = null;
        [SerializeField]
        private Transform attackOrigin = null;
        public Vector2 AttackOrigin => attackOrigin != null ? (Vector2)attackOrigin.transform.position : HandGameObjectPosition;
        [SerializeField]
        private TrailRenderer trail = null;
        [SerializeField]
        private SpriteRenderer spriteRenderer = null;

        public Vector2 FacingDirectionToRotationTarget => FacingDirectionUtils.GetFacingDirectionToTarget(HandGameObjectPosition, RotationTarget.Position);
        public float AimAngle => HandUtils.FollowDirectionProvider(RotationTarget.Position,HandGameObjectPosition);
        public Vector2 HandGameObjectPosition => handGameObject.transform.position;

        private IHasPosition rotationTarget;
        public IHasPosition RotationTarget => rotationTarget;

        private IPhysics2DComponent _myPhysicsHandler = null;
        private IInventoryComponent _myInventoryComponent = null;

        private void Awake() {
            _myInventoryComponent = GetComponent<IInventoryComponent>();
            _myPhysicsHandler = GetComponent<IPhysics2DComponent>();

            if (cameraDataContainment != null) {
                rotationTarget = new DirectionalCamera(cameraDataContainment);
            }
            else {
                rotationTarget = new StaticHandPosition(_myPhysicsHandler);

            }
            if (handFollowTarget != null) {
                handGameObject.SetParent(handFollowTarget);
            }
            _myInventoryComponent.AddInventoryAlteredEvent(ItemChanged);
        }
        public void UpdateHandRotation(float angle) {

            //handGameObject.eulerAngles = new Vector3(0, 0, angle);
            //handGameObject.localScale = _myPhysicsHandler.FacingDirection.x > 0 ? HandUtils.MirroredScale : Vector3.one;
        }

        public void ItemChanged() {
            if (_myInventoryComponent.CurrentItem != null && _myInventoryComponent.CurrentItem.ItemInfo != null) {
                spriteRenderer.sprite = _myInventoryComponent.CurrentItem.ItemInfo.ItemToDrop.GetComponent<SpriteRenderer>().sprite;
            }
            else {
                spriteRenderer.sprite = null;
            }
        }

        public void EnableTrail(float distance, Color color) {
            StartCoroutine(ResetTrailsIEnumerator());
            trail.startColor = color;
            trail.transform.localPosition = new Vector2(0, distance - trailOffSet);
            trail.gameObject.SetActive(true);
        }
        public void DisableTrail() {
            trail.gameObject.SetActive(false);
        }
        IEnumerator ResetTrailsIEnumerator() {
            trail.time = -1f;
            yield return new WaitForEndOfFrame();
            trail.time = 0.35f;
        }

        public void EnableHand() {
            handGameObject.gameObject.SetActive(true);
        }
        public void DisableHand() {
            handGameObject.gameObject.SetActive(false);
        }
#if UNITY_EDITOR
        [Header("This is only for debugging"),SerializeField]
        private bool showDebug = false;
        [SerializeField]
        private float _handAttackAngle = 90;

        public Vector2 DirFromViewAngle(float offset) {
            float angleInDegrees = -(handGameObject.eulerAngles.z + offset) + 90;
            return new Vector2(Mathf.Sin((angleInDegrees) * Mathf.Deg2Rad), Mathf.Cos((angleInDegrees) * Mathf.Deg2Rad));
        }
        void OnDrawGizmos() {
            if (!showDebug) {
                return;
            }
            Gizmos.color = Color.white;

            Vector2 viewAngleA = DirFromViewAngle(-_handAttackAngle / 2);
            Vector2 viewAngleB = DirFromViewAngle(_handAttackAngle / 2);
            Gizmos.DrawLine(handGameObject.transform.position, (Vector2)handGameObject.transform.position + viewAngleA * trail.transform.localPosition.x);
            Gizmos.DrawLine(handGameObject.transform.position, (Vector2)handGameObject.transform.position + viewAngleB * trail.transform.localPosition.x);
            ExtensionMethods.DrawWireArc(handGameObject.transform.position, Vector3.up, 360, trail.transform.localPosition.x);
        }
#endif

    }
    [Serializable]
    public class DirectionalCamera : IHasPosition {
        private CameraDataContainment camera;

        public DirectionalCamera(CameraDataContainment camera) {
            this.camera = camera;
        }

        public Vector2 Position => camera.GetMouseWorldPosition();
    }
    [Serializable]
    public class StaticHandPosition : IHasPosition {
        private IPhysics2DComponent physics2DComponent;

        public StaticHandPosition(IPhysics2DComponent physics2DComponent) {
            this.physics2DComponent = physics2DComponent;
        }

        public Vector2 Position {
            get {
                var XPosition = physics2DComponent.FacingDirection.x * 10;
                var YPosition = physics2DComponent.FacingDirection.y * 10;

                return new Vector2(physics2DComponent.Position.x + XPosition, physics2DComponent.Position.x + YPosition);
            }
        }
    }
}
