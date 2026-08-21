using GameObjectPools;
using System.Collections.Generic;
using UnityEngine;

namespace Knife.DeferredDecals
{
    public class DecalPoolManager : MonoBehaviour
    {
        public static DecalPoolManager Singleton;

        public List<Decal> _decalPrefabs;

        public bool TrySpawnDecal(DecalPoolType decalType, out Decal decal)
        {
            decal = null;

            if (_decalPrefabs == null)
                return false;

            GameObjectPools.PoolManager poolManager = GameObjectPools.PoolManager.Singleton;

            foreach (Decal prefab in _decalPrefabs)
            {
                if (prefab == null)
                    continue;

                if (prefab.DecalPoolType != decalType)
                    continue;

                if (poolManager == null)
                    return false;

                GameObject go = prefab.gameObject;
                if (!poolManager.TryGetPoolObject(go, out PoolObject spawnedObj))
                    continue;

                if (spawnedObj == null)
                    continue;

                if (spawnedObj.TryGetComponent<Decal>(out Decal spawnedDecal))
                {
                    decal = spawnedDecal;
                    return true;
                }
            }

            return false;
        }

        public void ReturnDecal(Decal decal)
        {
            GameObjectPools.PoolManager poolManager = GameObjectPools.PoolManager.Singleton;
            if (decal == null || poolManager == null)
                return;

            poolManager.TryReturnPoolObject(decal.gameObject);
        }

        public void Awake()
        {
            Singleton = this;

            if (_decalPrefabs == null)
                return;

            GameObjectPools.PoolManager poolManager = GameObjectPools.PoolManager.Singleton;

            foreach (Decal prefab in _decalPrefabs)
            {
                if (prefab == null)
                    continue;

                if (poolManager == null)
                    return;

                poolManager.TryAddPool(prefab);
            }
        }
    }
}