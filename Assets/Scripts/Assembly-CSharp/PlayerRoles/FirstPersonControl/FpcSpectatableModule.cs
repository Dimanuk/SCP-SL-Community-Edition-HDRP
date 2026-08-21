using InventorySystem;
using InventorySystem.Items;
using PlayerRoles.FirstPersonControl.Thirdperson;
using PlayerRoles.Spectating;
using UnityEngine;

namespace PlayerRoles.FirstPersonControl
{
    public class FpcSpectatableModule : SpectatableModuleBase, IViewmodelRole
    {
        private FirstPersonMovementModule FpcModule => (MainRole as IFpcRole).FpcModule;

        public ItemViewmodelBase SpawnedViewmodel { get; private set; }

        public override Vector3 CameraPosition
        {
            get
            {
                if (MainRole is ICameraController cameraController)
                {
                    return cameraController.CameraPosition;
                }

                return TargetHub.PlayerCameraReference.position;
            }
        }

        public override Vector3 CameraRotation
        {
            get
            {
                if (MainRole is ICameraController cameraController)
                {
                    float roll = (cameraController is IAdvancedCameraController advanced)
                        ? advanced.RollRotation
                        : 0f;

                    return new Vector3(cameraController.VerticalRotation, cameraController.HorizontalRotation, roll);
                }

                return TargetHub.PlayerCameraReference.rotation.eulerAngles;
            }
        }

        internal override void OnBeganSpectating()
        {
            FpcModule.CharacterModelInstance.SetVisibility(false);
            Inventory.OnCurrentItemChanged += OnCurrentItemChanged;

            ItemIdentifier current = TargetHub.inventory.CurItem;
            OnCurrentItemChanged(TargetHub, current, current);

            SharedHandsController.SetRoleGloves(PlayerRolesUtils.GetRoleId(TargetHub));

            if (CameraShaking.HeadbobShake.IsEnabled && FpcModule.CharacterModelInstance is AnimatedCharacterModel animatedModel)
            {
                var headbob = new CameraShaking.HeadbobShake(animatedModel);
                CameraShaking.CameraShakeController.AddEffect(headbob);
            }
        }

        internal override void OnStoppedSpectating()
        {
            var model = FpcModule.CharacterModelInstance;
            if (model != null)
            {
                model.SetVisibility(true);
            }

            Inventory.OnCurrentItemChanged -= OnCurrentItemChanged;

            if (SpawnedViewmodel != null)
            {
                Destroy(SpawnedViewmodel.gameObject);
                SpawnedViewmodel = null;
            }

            SharedHandsController.UpdateInstance(null);
        }

        public override void ResetObject()
        {
            base.ResetObject();
            Inventory.OnCurrentItemChanged -= OnCurrentItemChanged;

            if (SpawnedViewmodel != null)
            {
                Destroy(SpawnedViewmodel.gameObject);
                SpawnedViewmodel = null;
            }
        }

        private void OnDestroy()
        {
            Inventory.OnCurrentItemChanged -= OnCurrentItemChanged;
        }

        private void OnCurrentItemChanged(ReferenceHub hub, ItemIdentifier oldItem, ItemIdentifier newItem)
        {
            if (this == null)
            {
                Inventory.OnCurrentItemChanged -= OnCurrentItemChanged;
                return;
            }

            if (hub != TargetHub)
                return;

            if (SpawnedViewmodel != null)
            {
                Destroy(SpawnedViewmodel.gameObject);
                SpawnedViewmodel = null;
            }

            if (newItem.TypeId == ItemType.None)
            {
                SharedHandsController.UpdateInstance(null);
                return;
            }

            if (InventoryItemLoader.TryGetItem<ItemBase>(newItem.TypeId, out var itemBase) && itemBase.ViewModel != null)
            {
                SpawnedViewmodel = Instantiate(itemBase.ViewModel, SharedHandsController.Singleton.transform);
                SharedHandsController.UpdateInstance(SpawnedViewmodel);
                SpawnedViewmodel.InitSpectator(hub, newItem, oldItem == newItem);
            }
            else
            {
                SharedHandsController.UpdateInstance(null);
            }
        }

        public bool TryGetViewmodelFov(out float fov)
        {
            if (SpawnedViewmodel != null)
            {
                fov = SpawnedViewmodel.ViewmodelCameraFOV;
                return true;
            }

            if (MainRole is IViewmodelRole viewmodelRole)
            {
                return viewmodelRole.TryGetViewmodelFov(out fov);
            }

            fov = 0f;
            return false;
        }
    }
}