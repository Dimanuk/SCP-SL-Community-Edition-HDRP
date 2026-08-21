using InventorySystem.Items.SwayControllers;
using UnityEngine;

namespace InventorySystem.Items.MicroHID
{
    // Restored from CODES/ClientCode/CPP2IL_ISIL_Client/IsilDump/.../MicroHIDViewmodel.txt.
    // This class does NOT touch the lightning visuals — the whole beam (Lightning_mesh etc.)
    // is driven by MicroHIDLaser scaling the "ShotFX" node, triggered by the "TriggerLaser"
    // AnimationEvent in Micro_FireStart.anim. Here only the animator state int and the
    // energy gauge needle are updated.
    public class MicroHIDViewmodel : AnimatedViewmodelBase
    {
        [SerializeField]
        private Transform _swayPivot;

        [SerializeField]
        private Transform _energyGauge;

        [SerializeField]
        private float _viewmodelFov;

        [SerializeField]
        private float _firstTimeInspectMaxTime;

        private GoopSway _goopSway;

        private float _pickupTime;

        private const float MaxSkipTime = 11f;

        private const float FiringGaugeLerpSpeed = 4f;

        private const float IdleGaugeLerpSpeed = 1.5f;

        private static readonly int StateHash;

        private static readonly int FirstTimePickupHash;

        public override IItemSwayController SwayController => _goopSway;

        public override float ViewmodelCameraFOV => _viewmodelFov;

        public HidState State
        {
            get
            {
                if (ParentItem is MicroHIDItem microHIDItem)
                {
                    return microHIDItem.State;
                }

                if (MicroHIDHandler.SyncStates.TryGetValue(ItemId.SerialNumber, out HidStatusMessage message))
                {
                    return (HidState)message.MessageCode;
                }

                return HidState.Idle;
            }
        }

        public float Energy
        {
            get
            {
                if (ParentItem is MicroHIDItem microHIDItem)
                {
                    return microHIDItem.RemainingEnergy;
                }

                if (MicroHIDHandler.SyncEnergy.TryGetValue(ItemId.SerialNumber, out float energy))
                {
                    return energy;
                }

                return 0f;
            }
        }

        static MicroHIDViewmodel()
        {
            StateHash = Animator.StringToHash("State");
            FirstTimePickupHash = Animator.StringToHash("FirstTimePickup");
        }

        public override void InitLocal(ItemBase parent)
        {
            base.InitLocal(parent);
            _pickupTime = Time.timeSinceLevelLoad;
        }

        public override void InitAny()
        {
            base.InitAny();

            var settings = new GoopSway.GoopSwaySettings(
                targetTransform: _swayPivot,
                swayIntensity: 1.6f,
                translationIntensity: 0.0015f,
                zAxisIntensity: 0.04f,
                swaySmoothness: 4f,
                translationSmoothness: 6.5f,
                bobIntensity: 0.025f,
                centrifugalIntensity: 2.6f
            );
            _goopSway = new GoopSway(settings, Hub);

            LerpGauge(_energyGauge, Energy, 1f);
        }

        public override void InitSpectator(ReferenceHub ply, ItemIdentifier id, bool wasEquipped)
        {
            base.InitSpectator(ply, id, wasEquipped);

            if (!wasEquipped)
            {
                return;
            }

            AnimatorForceUpdate(base.SkipEquipTime, true);

            if (MicroHIDHandler.SyncStates.TryGetValue(ItemId.SerialNumber, out HidStatusMessage message))
            {
                AnimatorSetInt(StateHash, message.MessageCode);
                AnimatorForceUpdate(Mathf.Min(MaxSkipTime, Time.timeSinceLevelLoad - message.Time), false);
            }
        }

        internal override void OnEquipped()
        {
            base.OnEquipped();

            bool isFirstTime = Energy == 1f
                && _pickupTime + _firstTimeInspectMaxTime > Time.timeSinceLevelLoad;

            AnimatorSetBool(FirstTimePickupHash, isFirstTime);
            _pickupTime = 0f;
        }

        private void Update()
        {
            float lerpSpeed = State == HidState.Firing ? FiringGaugeLerpSpeed : IdleGaugeLerpSpeed;
            LerpGauge(_energyGauge, Energy, Time.deltaTime * lerpSpeed);
            AnimatorSetInt(StateHash, (int)State);
        }

        public static void LerpGauge(Transform gauge, float energy, float l)
        {
            if (gauge == null) return;

            float targetAngle = Mathf.Lerp(-81.6f, 68.7f, energy);
            Quaternion targetRot = Quaternion.Euler(0f, 0f, targetAngle);

            gauge.localRotation = Quaternion.Lerp(gauge.localRotation, targetRot, l);
        }
    }
}
