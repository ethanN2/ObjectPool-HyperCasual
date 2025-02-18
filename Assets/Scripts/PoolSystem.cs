/*******************************************************************
* Copyright         : 2025 Ethan
* File Name         : PoolSystem
* Description       : this is MonoBehaviour template i created.
*                     If you want to improve it. Just go to
*                     https://github.com/ethanN2/Unity_Script_Template
/******************************************************************/

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game
{
    [AddComponentMenu("Default/PoolSystem")]
    public class PoolSystem : MonoBehaviour
    {
        #region Fields

        [FormerlySerializedAs("_poolPrefab")] [Header("Pool Settings")] [SerializeField]
        private GameObject poolPrefab;

        [SerializeField] private int amountInitOnScene;

        private static PoolSystem       _instance;
        private        List<GameObject> _poolObjects;

        #endregion

        #region Properties

        public static PoolSystem Instance    => _instance;
        public static bool       HasInstance => _instance != null;

        public bool IsInitialized => _poolObjects != null && _poolObjects.Count != 0;

        #endregion

        #region Events / Delegates

        #endregion

        #region Unity Methods

        // Awake is called once before the first execution of Update after the MonoBehaviour is created
        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
            InitPoolObjects();
        }

        // These functions will be called when the attached GameObject is enabled.
        private void OnEnable()
        {
        }

        // Update is called once per frame
        private void Update()
        {
        }

        // These functions will be called when the attached GameObject is toggled.
        private void OnDisable()
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

        public void ReturnObjectToPool(GameObject obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            obj.SetActive(false);
        }

        public GameObject GetObject()
        {
            foreach (var obj in _poolObjects)
            {
                if (obj.activeInHierarchy) continue;
                return obj;
            }

            if (poolPrefab == null)
            {
                throw new InvalidOperationException("Pool prefab is null");
            }

            var newObj = Instantiate(poolPrefab);
            _poolObjects.Add(newObj);
            return newObj;
        }

        #endregion

        #region Private Methods

        private void InitPoolObjects()
        {
            if (poolPrefab == null)
            {
                throw new InvalidOperationException("Pool prefab is null");
            }

            _poolObjects ??= new List<GameObject>();
            for (int i = 0; i < amountInitOnScene; i++)
            {
                var obj = Instantiate(poolPrefab);
                obj.SetActive(false);
                _poolObjects.Add(obj);
            }

            Debug.Log("Init pool objects done!");
        }

        #endregion
    }
}