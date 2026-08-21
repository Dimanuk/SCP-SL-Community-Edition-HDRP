using System.Collections.Generic;
using UnityEngine;

namespace MapGeneration.Distributors
{
    public class ItemSpawnpoint : global::MapGeneration.Distributors.DistributorSpawnpointBase
    {
        public static readonly HashSet<ItemSpawnpoint> AutospawnInstances = new HashSet<ItemSpawnpoint>();
        public static readonly HashSet<ItemSpawnpoint> RandomInstances = new HashSet<ItemSpawnpoint>();

        public string TriggerDoorName;
        public ItemType AutospawnItem = ItemType.None;

        [SerializeField] private int _maxUses = 1;
        [SerializeField] private ItemType[] _acceptedItems;
        [SerializeField] private Transform[] _positionVariants;

        private int _uses;

        public bool CanSpawn(ItemType[] items)
        {
            if (items == null) return false;

            foreach (ItemType itemType in items)
            {
                if (itemType != ItemType.None && this.CanSpawn(itemType))
                {
                    return true; 
                }
            }
            return false;
        }

        public bool CanSpawn(ItemType targetItem)
        {
            if (_uses >= _maxUses || targetItem == ItemType.None)
            {
                return false;
            }

            for (int i = 0; i < _acceptedItems.Length; i++)
            {
                if (_acceptedItems[i] == targetItem)
                {
                    return true;
                }
            }
            return false;
        }

        public Transform Occupy()
        {
            _uses++;
            return _positionVariants[Random.Range(0, _positionVariants.Length)];
        }

        private void Start()
        {
            if (AutospawnItem == ItemType.None)
                RandomInstances.Add(this);
            else
                AutospawnInstances.Add(this);
        }

        private void OnDestroy()
        {
            RandomInstances.Remove(this);
            AutospawnInstances.Remove(this);
        }
    }
}