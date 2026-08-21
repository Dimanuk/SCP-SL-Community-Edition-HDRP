using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameObjectPools
{
    public class PoolManager : MonoBehaviour
    {
        public void TryAddPool(PoolObject prefab)
        {
            if (this._poolLookup.ContainsKey(prefab.gameObject))
            {
                return;
            }
            Pool pool = new Pool
            {
                Prefab = prefab
            };
            this._poolLookup.Add(pool.Prefab.gameObject, pool);
            pool.Initialize();
        }

        public bool TryGetPoolObject(GameObject prefab, Transform parent, out PoolObject poolObject, bool autoSetup = true)
        {
            if (!this.TryGetPoolObject(prefab, out poolObject, false))
            {
                return false;
            }
            poolObject.transform.SetParent(parent);
            if (autoSetup)
            {
                poolObject.SetupPoolObject();
            }
            return true;
        }

        public bool TryGetPoolObject(GameObject prefab, Transform parent, bool worldPositionStays, out PoolObject poolObject, bool autoSetup = true)
        {
            if (!this.TryGetPoolObject(prefab, out poolObject, false))
            {
                return false;
            }
            poolObject.transform.SetParent(parent, worldPositionStays);
            if (autoSetup)
            {
                poolObject.SetupPoolObject();
            }
            return true;
        }

        public bool TryGetPoolObject(GameObject prefab, out PoolObject poolObject, bool autoSetup = true)
        {
            if (prefab == null)
            {
                poolObject = null;
                return false;
            }
            Pool pool;
            if (!this._poolLookup.TryGetValue(prefab, out pool))
            {
                poolObject = null;
                return false;
            }
            if (!pool.TryGetPoolableObject(out poolObject))
            {
                return false;
            }
            poolObject.gameObject.SetActive(true);
            if (autoSetup)
            {
                poolObject.SetupPoolObject();
            }
            return true;
        }

        public bool TryReturnPoolObject(GameObject poolGameObject)
        {
            PoolObject poolObject;
            if (poolGameObject == null || !this.PoolObjectLookup.TryGetValue(poolGameObject, out poolObject))
            {
                return false;
            }
            this.ReturnPoolObject(poolObject);
            return true;
        }

        public void ReturnPoolObject(PoolObject poolObject)
        {
            poolObject.ReturnToPool(true);
        }

        public void ReturnAllPoolObjects()
        {
            foreach (KeyValuePair<GameObject, PoolObject> keyValuePair in this.PoolObjectLookup)
            {
                PoolObject value = keyValuePair.Value;
                if (!(value == null) && !value.Pooled)
                {
                    value.ReturnToPool(true);
                }
            }
        }

        public void RestartRound()
        {
            foreach (KeyValuePair<GameObject, Pool> keyValuePair in this._poolLookup)
            {
                keyValuePair.Value.RestartRound();
            }
        }

        private void Awake()
        {
            if (PoolManager.Singleton != null)
            {
                UnityEngine.Object.Destroy(this);
                return;
            }
            PoolManager.Singleton = this;
            UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
            foreach (Pool pool in this.objectPools)
            {
                this._poolLookup.Add(pool.Prefab.gameObject, pool);
                pool.Initialize();
            }
        }

        private void OnDestroy()
        {
            Debug.LogWarning("--PoolManager - Average required pool objects per round--");
            foreach (KeyValuePair<GameObject, Pool> keyValuePair in this._poolLookup)
            {
                keyValuePair.Value.PrintDebug();
            }
            Debug.LogWarning("--End of Log--");
        }

        public static PoolManager Singleton;

        internal Dictionary<GameObject, PoolObject> PoolObjectLookup = new Dictionary<GameObject, PoolObject>();

        [SerializeField]
        private List<Pool> objectPools;

        private Dictionary<GameObject, Pool> _poolLookup = new Dictionary<GameObject, Pool>();
    }
}
