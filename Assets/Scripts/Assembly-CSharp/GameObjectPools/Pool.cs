using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameObjectPools
{
    [Serializable]
    internal class Pool
    {
        private Pool.OverflowModes OverflowMode
        {
            get
            {
                return this.Prefab.OverflowMode;
            }
        }

        private void AddNewPrefabToPool()
        {
            this._pool.Push(this.SpawnObject());
        }

        internal void Initialize()
        {
            this.parentTransform = new GameObject(this.Prefab.name).transform;
            this.parentTransform.SetParent(PoolManager.Singleton.transform, false);
            for (int i = 0; i < this.Prefab.InitialPoolSize; i++)
            {
                this.AddNewPrefabToPool();
            }
        }

        internal bool TryGetPoolableObject(out PoolObject poolableObject)
        {
            while (this._pool.Count > 0)
            {
                poolableObject = this._pool.Pop();
                if (!(poolableObject == null))
                {
                    if (this.OverflowMode == Pool.OverflowModes.RecycleOldPrefab)
                    {
                        this._spawnQueue.Enqueue(poolableObject);
                    }
                    return true;
                }
                Debug.LogError("Object Pooling: Found null object in " + this.parentTransform.gameObject.name + " pool.");
                this.AddNewPrefabToPool();
            }
            switch (this.OverflowMode)
            {
                case Pool.OverflowModes.RecycleOldPrefab:
                    while (this._spawnQueue.Count > 0)
                    {
                        poolableObject = this._spawnQueue.Dequeue();
                        if (!(poolableObject == null))
                        {
                            this._spawnQueue.Enqueue(poolableObject);
                            this.ResetObject(poolableObject);
                            return true;
                        }
                        Debug.LogError("Object Pooling: Found null object in " + this.parentTransform.gameObject.name + " pool.");
                        this.AddNewPrefabToPool();
                    }
                    poolableObject = null;
                    return false;
                case Pool.OverflowModes.DoNotSpawnPrefab:
                    poolableObject = null;
                    return false;
                case Pool.OverflowModes.CreateMorePrefabs:
                    poolableObject = this.SpawnObject();
                    return true;
                default:
                    poolableObject = null;
                    return false;
            }
        }

        internal void ReturnObject(PoolObject poolableObject)
        {
            if (poolableObject == null)
            {
                return;
            }
            this.ResetObject(poolableObject);
            this._pool.Push(poolableObject);
        }

        private void ResetObject(PoolObject poolableObject)
        {
            GameObject gameObject = poolableObject.gameObject;
            gameObject.SetActive(false);
            gameObject.transform.SetParent(this.parentTransform, false);
            poolableObject.ResetPoolObject();
        }

        internal void RestartRound()
        {
            if (this._currentMax != 0)
            {
                this._maxEachRound.Add(this._currentMax);
            }
            this._currentMax = 0;
        }

        internal void PrintDebug()
        {
            if (this.OverflowMode == Pool.OverflowModes.CreateMorePrefabs || this._maxEachRound.Count == 0)
            {
                return;
            }
            float num = 0f;
            float num2 = 0f;
            foreach (int num3 in this._maxEachRound)
            {
                num2 += (float)num3;
                if ((float)num3 > num)
                {
                    num = (float)num3;
                }
            }
            num2 /= (float)this._maxEachRound.Count;
            num2 = (float)((int)(num2 * 100f)) * 0.01f;
            Debug.LogWarning(string.Format("{0}: Initial Size: {1} | Average Used: {2} | Max Used: {3}", new object[]
            {
                this.parentTransform.gameObject.name,
                this.Prefab.InitialPoolSize,
                num2,
                num
            }));
        }

        private PoolObject SpawnObject()
        {
            PoolObject component = UnityEngine.Object.Instantiate<GameObject>(this.Prefab.gameObject, this.parentTransform).GetComponent<PoolObject>();
            PoolManager.Singleton.PoolObjectLookup.Add(component.gameObject, component);
            component.gameObject.SetActive(false);
            component.InitializePoolObject(this);
            return component;
        }

        [SerializeField]
        internal PoolObject Prefab;

        private Stack<PoolObject> _pool = new Stack<PoolObject>();

        private Queue<PoolObject> _spawnQueue = new Queue<PoolObject>();

        private Transform parentTransform;

        private int _currentMax;

        private List<int> _maxEachRound = new List<int>();

        internal enum OverflowModes
        {
            RecycleOldPrefab,
            DoNotSpawnPrefab,
            CreateMorePrefabs
        }
    }
}
