using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using EE.PhysicsSystem;
using EE.Core.PoolingSystem;

namespace EE.Core.Old {
    /// <summary>
    /// Handles shootable objects created with attacks.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class ProjectileComponent : EEMonobehavior, IPoolable {
        #region Components
        [SerializeField]
        private AudioSource myAudioSource;
        public AudioSource MyAudioSource =>myAudioSource;

        [SerializeField]
        private Rigidbody2D myRigidbody;
        public Rigidbody2D MyRigidbody => myRigidbody;

        [SerializeField]
        private SpriteRenderer mySpriteRenderer;
        public SpriteRenderer MySpriteRenderer => mySpriteRenderer;

        [SerializeField]
        private Collider2D myCollider2D;
        public Collider2D MyCollider2D => myCollider2D;

        [SerializeField]
        private Transform myTransform;
        public Transform MyTransform => myTransform;
        [SerializeField]
        private PoolableComponent poolableComponent;
        public PoolableComponent PoolableComponent => poolableComponent;

        #endregion

        private bool audioPlayed = false;

        public void Spawned(ProjectileData projectileData, Vector2 shootDirection, Vector2 targetPosition) {
            //throwerCollider = thrower;
            //Physics2D.IgnoreCollision(thrower, myCollider2D);

            if (projectileData.InstantlyTravelToPosition) {
                transform.position = targetPosition;

            }
            else {
                myRigidbody.AddForce(shootDirection.normalized * projectileData.ProjectileSpeed);
            }

            if (targetPosition.x > transform.position.x) {
                MySpriteRenderer.flipX = true;
                projectileData.reverseRotation = true;
            }

            myCollider2D.enabled = false;
            //myAnimator.enabled = false;
        }


        //public IEnumerator ProjecttileExistance(ProjectileData projectileData, Vector2 shootDirection, Vector2 targetPosition, IPhysics2DComponent owner, List<Action<GameObject>> collisionEvents) {
        //    Spawned(projectileData, shootDirection, targetPosition);
        //    yield return new WaitForSeconds(projectileData.ActivationDelay);
        //    myCollider2D.enabled = true;
        //    //myAnimator.enabled = true;
        //    //myAnimator.PlayAnimation(projectileData.SpriteAnimation);


        //    float timer = projectileData.ProjectileDuration;
        //    while (timer > 0) {
        //        timer -= Time.deltaTime;

        //        if (projectileData.RotatingWhileMoving) {
        //            transform.Rotate(0, 0, projectileData.RotationSpeed * Time.deltaTime);                    
        //        }

        //        CheckForCollision(projectileData, owner, collisionEvents);
        //        yield return null;
        //    }
        //    if (projectileData.HitEffect != null) {
        //        PoolManager.SpawnObject(projectileData.HitEffect.PoolableComponent, transform.position, transform.rotation);
        //    }

        //    PoolManager.ReleaseObject(PoolableComponent);

        //}

        public void CheckForCollision(ProjectileData projectileData, IPhysics2DComponent owner, List<Action<GameObject>> collisionEvents) {

            RaycastHit2D[] hits = new RaycastHit2D[10];
            myCollider2D.Cast(Vector2.zero, hits);
            for (int i = 0; i < hits.Length; i++) {
                RaycastHit2D hit = hits[i];
                var target = hit.collider.GetComponent<IPhysics2DComponent>();
                if (target == null) {
                    return;
                }
                if (hit.collider && owner != target) {
                    HitEvent(projectileData, hit.transform.gameObject, collisionEvents);
                }
            }
        }

        //private void DoDamage(GameObject hittedObject) {
        //    if (!hittedObject.TryGetComponent(out IHurtable hurtable)) {
        //        return;
        //    }
        //    if (hurtable.CanTakeDamage(hurtSomething.DamagableTeams)) {
        //        hurtable.ReduceHealth(hurtSomething.CurrentDamage);
        //        hurtable.ActivateNormalHitEffects();
        //        hurtSomething.SomethingHit(hurtable);
        //    }
        //    else {
        //        hurtSomething.AttackBlocked(hurtable);
        //    }
        //}


        private void HitEvent(ProjectileData projectileData, GameObject hittedObject, List<Action<GameObject>> collisionEvents) {
            if (!audioPlayed) {
                //MyAudioSource.Play(projectileData.HitEffectSound);
                audioPlayed = true;
            }
            if (((1 << hittedObject.layer) & projectileData.CollisionLayerMask) != 0) {
                if (projectileData.HitEffect != null) {
                    PoolManager.SpawnObject(projectileData.HitEffect.PoolableComponent, transform.position, transform.rotation);
                }

                foreach (Action<GameObject> action in collisionEvents) {
                    action?.Invoke(hittedObject);
                }

                //MyAudioSource.Play(projectileData.HitEffectSound);

                if (projectileData.DestroyOnCollision) {
                    PoolManager.ReleaseObject(poolableComponent);

                }
            }
        }

        public void OnDisable() {
            audioPlayed = false;
            myCollider2D.enabled = false;
            //myAnimator.ResetAnimation();
        }
#if UNITY_EDITOR

        void OnValidate() {

            if (mySpriteRenderer == null) {
                mySpriteRenderer = GetComponent<SpriteRenderer>();
            }
            if (myAudioSource == null) {
                myAudioSource = GetComponent<AudioSource>();
            }
            if (myCollider2D == null) {
                myCollider2D = GetComponent<Collider2D>();
            }
            if (myRigidbody == null) {
                myRigidbody = GetComponent<Rigidbody2D>();
            }
            if (myTransform == null) {
                myTransform = GetComponent<Transform>();
            }
            if (poolableComponent == null) {
                poolableComponent = GetComponent<PoolableComponent>();
            }
        }



#endif
    }

    [System.Serializable]
    public class ProjectileData : IProjectileData {
        [SerializeField]
        private float projectileSpeed = 5;
        public float ProjectileSpeed => projectileSpeed;
        [SerializeField]
        private float projectileDuration = 1;
        public float ProjectileDuration => projectileDuration;

        [SerializeField]
        private bool rotatingWhileMoving = false;
        public bool RotatingWhileMoving => rotatingWhileMoving;

        [SerializeField]
        private float rotationSpeed = 5;
        public float RotationSpeed => reverseRotation ? -rotationSpeed : rotationSpeed;

        [HideInInspector]
        public bool reverseRotation = false;
        public bool ReverseRotation => reverseRotation;

        [SerializeField]
        private bool instantlyTravelToPosition = false;
        public bool InstantlyTravelToPosition => instantlyTravelToPosition;

        [SerializeField]
        private bool destroyOnCollision = false;
        public bool DestroyOnCollision => destroyOnCollision;

        [SerializeField]
        private ParticleSystemController hitEffect = null;
        public ParticleSystemController HitEffect => hitEffect;

        [SerializeField]
        private LayerMask collisionLayerMask = 1 << 8 | 1 << 11 | 1 << 15 | 1 << 17; // Default obstacle layers, Player8, Enemy 11,  Objects 15, Wall 17
        public LayerMask CollisionLayerMask => collisionLayerMask;


        [SerializeField]
        private float activationDelay = 0;
        public float ActivationDelay => activationDelay;
    }

    public interface IProjectileData {
        float ProjectileSpeed { get; }
        float ProjectileDuration { get; }
        float ActivationDelay { get; }
        bool RotatingWhileMoving { get; }
        bool InstantlyTravelToPosition { get; }
        bool DestroyOnCollision { get; }
        ParticleSystemController HitEffect { get; }
        LayerMask CollisionLayerMask { get; }

    }
}
