using PlayerRoles.FirstPersonControl;
using UnityEngine;

namespace PlayerRoles.PlayableScps.Scp049.Zombies
{
    public class ZombieMovementModule : FirstPersonMovementModule
    {
        private const float MaxTargetTime = 5f;
        private const float MinTargetTime = 1f;
        private const float SpeedPerTick  = 0.05f;
        [SerializeField]
        private ZombieRole _role;

        private ZombieBloodlustAbility _visionTracker; 
        private float _lookingTimer;                  
        private float _speedTickTimer;               
        private bool  _bloodlustActive;              

        public bool CanMove { get; set; }             
        public float BloodlustSpeed { get; private set; }
        public float NormalSpeed    { get; private set; } 
        private float MovementSpeed
        {
            get => WalkSpeed;
            set
            {
                WalkSpeed   = value;
                SprintSpeed = value;
            }
        }

        private void Awake()
        {
            NormalSpeed    = WalkSpeed;
            BloodlustSpeed = SprintSpeed;

            _role.SubroutineModule.TryGetSubroutine(out _visionTracker);
        }

        protected override void UpdateMovement()
        {
            float deltaTime = Time.deltaTime;

            UpdateBloodlustState(deltaTime);
            UpdateSpeed(deltaTime);

            base.UpdateMovement();
        }

        private void UpdateBloodlustState(float deltaTime)
        {
            float change = _visionTracker.LookingAtTarget ? deltaTime : -deltaTime;
            _lookingTimer = Mathf.Clamp(_lookingTimer + change, 0f, MaxTargetTime);
            if (_lookingTimer > MinTargetTime)
            {
                _bloodlustActive = true;
            }
            else if (_lookingTimer == 0f)
            {
                _bloodlustActive = false;
            }
        }
        private void UpdateSpeed(float deltaTime)
        {
            _speedTickTimer += deltaTime;

            if (_speedTickTimer < MinTargetTime)
                return;

            _speedTickTimer = 0f;

            float delta = _bloodlustActive ? SpeedPerTick : -0.1f;
            float value = MovementSpeed + delta;

            MovementSpeed = Mathf.Clamp(value, NormalSpeed, BloodlustSpeed);
        }
    }
}