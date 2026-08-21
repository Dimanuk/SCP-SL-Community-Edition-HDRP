using System;
using Interactables.Interobjects.DoorUtils;
using InventorySystem.Items.Pickups;
using Mirror;
using UnityEngine;

namespace InventorySystem.Items.Keycards
{
    public class KeycardPickup : CollisionDetectionPickup
    {
        [SerializeField]
        private Material[] _keycardTextures;

        [SerializeField]
        private MeshRenderer _targetRenderer;

        public Material GetMaterialFromKeycardId(ItemType itemId)
        {
            int index = (int)itemId;
            if (_keycardTextures != null && index >= 0 && index < _keycardTextures.Length)
            {
                return _keycardTextures[index];
            }
            return null;
        }

        protected override void Start()
        {
            base.Start();

            UpdateMaterial();
        }

        public override void InfoReceived(PickupSyncInfo oldInfo, PickupSyncInfo newInfo)
        {
            base.InfoReceived(oldInfo, newInfo);
            UpdateMaterial();
        }

        private void UpdateMaterial()
        {
            if (_targetRenderer == null || _keycardTextures == null)
                return;

            ItemType itemId = base.Info.ItemId;
            Material material = GetMaterialFromKeycardId(itemId);

            if (material != null)
            {
                _targetRenderer.sharedMaterial = material;
            }
        }

        protected override void ProcessCollision(Collision collision)
        {
            base.ProcessCollision(collision);
            if (NetworkServer.active && collision.collider.TryGetComponent<RegularDoorButton>(out var component) &&
                component.Target is DoorVariant { ActiveLocks: 0 } doorVariant &&
                doorVariant.RequiredPermissions.RequiredPermissions != KeycardPermissions.None &&
                InventoryItemLoader.AvailableItems.TryGetValue(Info.ItemId, out var value) &&
                doorVariant.RequiredPermissions.CheckPermissions(value, null) &&
                doorVariant.AllowInteracting(null, component.ColliderId))
            {
                doorVariant.TargetState = !doorVariant.TargetState;
            }
        }
    }
}