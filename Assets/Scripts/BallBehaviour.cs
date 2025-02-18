/*******************************************************************
* Copyright         : 2025 Ethan
* File Name         : BallBehaviour
* Description       : this is MonoBehaviour template i created.
*                     If you want to improve it. Just go to
*                     https://github.com/ethanN2/Unity_Script_Template
/******************************************************************/

using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Game
{
    [AddComponentMenu("Default/BallBehaviour")]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(SphereCollider))]
    public class BallBehaviour : MonoBehaviour, IBall
    {
        #region Enums

        #endregion

        #region Fields

        [Header("Ball Settings")] [SerializeField]
        private float forceExplosion;

        private Rigidbody      _rigidbody;
        private SphereCollider _collider;
        private bool           _isShoot;
        private List<Collider> _colliders;
        private Coroutine      _coroutine;

        #endregion

        #region Properties

        public float   ForceExplosion => forceExplosion;
        public Vector3 Position       => transform.position;

        #endregion

        #region Events / Delegates

        #endregion

        #region Unity Methods

        // Awake is called once before the first execution of Update after the MonoBehaviour is created
        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _collider  = GetComponent<SphereCollider>();
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
        }

        // These functions will be called when the attached GameObject is enabled.
        private void OnEnable()
        {
        }

        // Update is called once per frame
        private void Update()
        {
            _colliders = Physics.OverlapSphere(transform.position, _collider.radius + 0.001f).ToList();

            if (_coroutine != null) return;

            // TODO: if touch wall then it will dissapere in 3 sec
            foreach (var otherCollider in _colliders)
            {
                if (otherCollider.gameObject.layer == LayerMask.NameToLayer("Wall"))
                {
                    _coroutine = StartCoroutine(AutomateDisableIn(3));
                }
            }
        }

        // These functions will be called when the attached GameObject is toggled.
        private void OnDisable()
        {
            _isShoot = false;
        }

        private void OnCollisionEnter(Collision collision)
        {
        }

        #endregion

        #region Gizmos

        private void OnDrawGizmos()
        {
        }

        private void OnDrawGizmosSelected()
        {
        }

        #endregion

        #region Public Methods

        public void SetPosition(Vector3 position)
        {
            transform.position = position;
        }

        public void Shoot(Vector3 direction, float force)
        {
            if (_isShoot) return;
            if (_rigidbody == null && _collider == null)
            {
                throw new Exception("BallBehaviour has no rigidbody or SphereCollider!");
            }

            gameObject.SetActive(true);
            _rigidbody.AddForce(direction * force, ForceMode.Impulse);
            _isShoot = true;
        }

        public void BeingInteractedWith(IBall otherBall)
        {
            if (otherBall == null)
            {
                throw new Exception("BallBehaviour has no ball!");
            }

            var direction = transform.position - otherBall.Position;
            _rigidbody.AddForce(direction.normalized * otherBall.ForceExplosion);
        }

        #endregion

        #region Private Methods

        private IEnumerator AutomateDisableIn(uint seconds)
        {
            yield return new WaitForSeconds(seconds);

            // TODO: add explosion

            gameObject.SetActive(false);
            foreach (var otherBall in _colliders)
            {
                if (otherBall.gameObject.layer != LayerMask.NameToLayer("Ball")) continue;
                var ballComponent = otherBall.GetComponent<IBall>();
                ballComponent?.BeingInteractedWith(this);
            }

            if (PoolSystem.HasInstance && PoolSystem.Instance.IsInitialized)
            {
                PoolSystem.Instance.ReturnObjectToPool(gameObject);
            }

            _colliders.Clear(); // don't forget clear list -> will grow your memory
            _coroutine = null;
        }

        #endregion
    }

    public interface IBall
    {
        float   ForceExplosion { get; }
        Vector3 Position       { get; }

        void Shoot(Vector3 direction, float force);
        void BeingInteractedWith(IBall otherBall);
        void SetPosition(Vector3 position);
    }
}