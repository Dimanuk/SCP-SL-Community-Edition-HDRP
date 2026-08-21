using CameraShaking;
using InventorySystem.Items.SwayControllers;
using UnityEngine;
using UnityEngine.Serialization;

namespace InventorySystem.Items
{
    public class StandardAnimatedViemodel : AnimatedViewmodelBase
    {
        // Field used to be named "_handsPivot" - most item prefabs still store their
        // sway-pivot reference under that old name and were never resaved after the
        // rename, so without this attribute Unity silently drops the binding and every
        // one of those items loses its hand sway.
        [FormerlySerializedAs("_handsPivot")]
        [SerializeField]
        private Transform HandsPivot;

        [SerializeField]
        private Transform _trackerCamera;

        [SerializeField]
        private float _trackerForceScale = 1f;

        [SerializeField]
        private Vector3 _trackerOffset;

        [SerializeField]
        private float _fov = 50f;

        public IItemSwayController _swayController;

        public override IItemSwayController SwayController => _swayController;

        public override float ViewmodelCameraFOV => _fov;

        internal override void OnEquipped()
        {
            base.OnEquipped();
            _swayController ??= GetNewSwayController();

            CameraShakeController.AddEffect(new TrackerShake(_trackerCamera, Quaternion.Euler(_trackerOffset), _trackerForceScale));
        }

        public override void InitSpectator(ReferenceHub ply, ItemIdentifier id, bool wasEquipped)
        {
            base.InitSpectator(ply, id, wasEquipped);
            _swayController ??= GetNewSwayController();
        }

        public virtual IItemSwayController GetNewSwayController()
        {
            return new GoopSway(new GoopSway.GoopSwaySettings(HandsPivot, 0.65f, 0.0035f, 0.04f, 7f, 6.5f, 0.03f, 1.6f, invertSway: false), base.Hub);
        }
    }
}