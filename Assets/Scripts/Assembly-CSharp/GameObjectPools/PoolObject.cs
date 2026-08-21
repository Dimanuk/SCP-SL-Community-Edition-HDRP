using System;
using UnityEngine;

namespace GameObjectPools
{
    public class PoolObject : MonoBehaviour
    {
        public bool Pooled { get; set; } = true;

        private Pool _myPool { get; set; }

        public void ReturnToPool(bool checkChildren = true)
        {
            if (this.Pooled)
            {
                return;
            }
            if (this._myPool == null)
            {
                Debug.LogError(base.gameObject.name + " does not have an initialized pool.");
                return;
            }
            if (checkChildren)
            {
                PoolObject[] componentsInChildren = base.GetComponentsInChildren<PoolObject>();
                for (int i = componentsInChildren.Length - 1; i >= 0; i--)
                {
                    componentsInChildren[i].ReturnToPool(false);
                }
                return;
            }
            this.Pooled = true;
            this._myPool.ReturnObject(this);
        }

        protected virtual void OnInstantiated()
        {
        }

        internal void InitializePoolObject(Pool poolOwner)
        {
            this._myPool = poolOwner;
            this._poolResetables = base.GetComponentsInChildren<IPoolResettable>();
            this._poolSpawnables = base.GetComponentsInChildren<IPoolSpawnable>();
            this.OnInstantiated();
        }

        internal void ResetPoolObject()
        {
            IPoolResettable[] poolResetables = this._poolResetables;
            for (int i = 0; i < poolResetables.Length; i++)
            {
                poolResetables[i].ResetObject();
            }
        }

        public void SetupPoolObject()
        {
            this.Pooled = false;
            IPoolSpawnable[] poolSpawnables = this._poolSpawnables;
            for (int i = 0; i < poolSpawnables.Length; i++)
            {
                poolSpawnables[i].SpawnObject();
            }
        }

        [SerializeField]
        internal int InitialPoolSize = 10;

        [SerializeField]
        internal Pool.OverflowModes OverflowMode = Pool.OverflowModes.CreateMorePrefabs;

        private IPoolResettable[] _poolResetables;

        private IPoolSpawnable[] _poolSpawnables;
    }
}
