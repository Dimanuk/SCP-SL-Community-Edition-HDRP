using System;
using System.Collections.Generic;
using InventorySystem.Items.Pickups;
using Mirror;
using UnityEngine;

namespace InventorySystem.Items.Usables.Scp330
{
    public class Scp330Pickup : CollisionDetectionPickup
    {
        [Serializable]
        private struct IndividualCandy
        {
            [SerializeField]
            private CandyKindID _kind;

            [SerializeField]
            private GameObject _candyObject;

            public void Refresh(CandyKindID exposed)
            {
                if (_candyObject != null)
                {
                    _candyObject.SetActive(exposed == _kind);
                }
            }
        }

        public List<CandyKindID> StoredCandies = new List<CandyKindID>();

        [SyncVar]
        public CandyKindID ExposedCandy;

        [SerializeField]
        private IndividualCandy[] _candyTypes;

        private int _prevExposed = -1;

        private void Update()
        {
            if (_prevExposed != (int)ExposedCandy)
            {
                foreach (var candy in _candyTypes)
                {
                    candy.Refresh(ExposedCandy);
                }
                
                _prevExposed = (int)ExposedCandy;
                
                if (NetworkServer.active && StoredCandies.Count == 0)
                {
                    DestroySelf();
                }
            }
        }

        public Scp330Pickup()
        {
            StoredCandies = new List<CandyKindID>();
            _prevExposed = -1;
        }
    }
}