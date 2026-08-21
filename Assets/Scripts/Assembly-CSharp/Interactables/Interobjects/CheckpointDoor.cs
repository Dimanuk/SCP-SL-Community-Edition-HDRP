using Interactables.Interobjects.DoorUtils;
using Mirror;
using UnityEngine;

namespace Interactables.Interobjects
{
    public class CheckpointDoor : DoorVariant, IDamageableDoor
    {
        private enum CheckpointSequenceStage
        {
            Idle = 0,
            Granted = 1,
            Open = 2,
            Closing = 3
        }

        private enum CheckpointErrorType : byte
        {
            Denied = 0,
            LockedDown = 1,
            Destroyed = 2
        }

        [SerializeField]
        private DoorVariant[] _subDoors;

        [SerializeField]
        private float _openingTime;

        [SerializeField]
        private float _waitTime;

        [SerializeField]
        private float _warningTime;

        [SerializeField]
        private PanelVisualSettings _panelSettings;

        [SerializeField]
        private Material _warningMat;

        [SerializeField]
        private RegularDoorButton[] _buttons;

        [SerializeField]
        private AudioClip _warningSound;

        [SerializeField]
        private AudioClip _loudThingySound;

        [SerializeField]
        private AudioClip _beepSound;

        [SerializeField]
        private AudioSource _loudSource;

        [SerializeField]
        private AudioSource _quietSource;

        private float _remainingBeepCooldown;

        private float _mainTimer;

        private bool _permanentDestroyment;

        private CheckpointSequenceStage _currentSequence;

        private string _warningText;

        public DoorVariant[] SubDoors => _subDoors;

        public bool IsDestroyed
        {
            get => GetHealthPercent() == 0f;
            set => throw new System.NotImplementedException();
        }

        public override bool AllowInteracting(ReferenceHub ply, byte colliderId)
        {
            int destroyedCount = 0;
            foreach (DoorVariant door in _subDoors)
            {
                if (door is IDamageableDoor damageable && damageable.IsDestroyed)
                {
                    destroyedCount++;
                }
                else if (!door.AllowInteracting(ply, colliderId))
                {
                    return false;
                }
            }

            if (destroyedCount >= _subDoors.Length)
            {
                RpcPlayBeepSound((byte)CheckpointErrorType.Destroyed);
                return false;
            }

            return _currentSequence == CheckpointSequenceStage.Idle;
        }

        public override float GetExactState()
        {
            if (_subDoors.Length == 0)
                return 0f;

            float maxState = 0f;
            foreach (DoorVariant door in _subDoors)
            {
                float state = door.GetExactState();
                if (state > maxState)
                    maxState = state;
            }
            return maxState;
        }

        public override bool IsConsideredOpen()
        {
            foreach (DoorVariant door in _subDoors)
            {
                if (door.IsConsideredOpen())
                    return true;
            }
            return false;
        }

        public override void LockBypassDenied(ReferenceHub ply, byte colliderId)
        {
            RpcPlayBeepSound((byte)CheckpointErrorType.LockedDown);
        }

        public override void PermissionsDenied(ReferenceHub ply, byte colliderId)
        {
            RpcPlayBeepSound((byte)CheckpointErrorType.Denied);
        }

        public override bool AnticheatPassageApproved()
        {
            foreach (DoorVariant door in _subDoors)
            {
                if (door.AnticheatPassageApproved())
                    return true;
            }
            return false;
        }

        protected override void Start()
        {
            base.Start();
            SetButtons(_panelSettings.TextClosed, _panelSettings.PanelClosedMat);
            _warningText = TranslationReader.Get("Doors", 9, "<color=orange>CLOSING</color>");
        }

        protected override void LockChanged(ushort prevValue)
        {
            if (ActiveLocks > 0)
                SetButtons(_panelSettings.TextLockedDown, _panelSettings.PanelErrorMat);
            else
                SetButtons(_panelSettings.TextClosed, _panelSettings.PanelClosedMat);
        }

        protected override void Update()
        {
            base.Update();

            if (_remainingBeepCooldown > 0f)
            {
                _remainingBeepCooldown -= Time.deltaTime;
                if (_remainingBeepCooldown <= 0f)
                {
                    if (ActiveLocks > 0)
                        SetButtons(_panelSettings.TextLockedDown, _panelSettings.PanelErrorMat);
                    else
                        SetButtons(_panelSettings.TextClosed, _panelSettings.PanelClosedMat);
                }
            }

            UpdateSequence();
        }

        private void UpdateSequence()
        {
            bool isDecontLockdown = DoorLockUtils.HasFlagFast(
                (DoorLockReason)ActiveLocks, DoorLockReason.DecontLockdown);

            bool isForceOpen = DoorLockUtils.HasFlagFast(
                (DoorLockReason)ActiveLocks, DoorLockReason.DecontEvacuate)
                || DoorLockUtils.HasFlagFast(
                (DoorLockReason)ActiveLocks, DoorLockReason.Warhead);

            if (TargetState && _currentSequence == CheckpointSequenceStage.Idle)
            {
                if (NetworkServer.active)
                {
                    ToggleAllDoors(true);
                }

                _currentSequence = CheckpointSequenceStage.Granted;
                _mainTimer = 0f;
                _loudSource.PlayOneShot(_loudThingySound);
                SetButtons(_panelSettings.TextMoving, _panelSettings.PanelMovingMat);
                return;
            }

            switch (_currentSequence)
            {
                case CheckpointSequenceStage.Granted:
                    _mainTimer += Time.deltaTime;
                    if (_mainTimer > _openingTime)
                    {
                        _currentSequence = CheckpointSequenceStage.Open;
                        _mainTimer = 0f;
                        SetButtons(_panelSettings.TextOpen, _panelSettings.PanelOpenMat);
                    }
                    break;

                case CheckpointSequenceStage.Open:
                    if (NetworkServer.active)
                    {
                        if (!isForceOpen)
                            _mainTimer += Time.deltaTime;

                        if (_mainTimer > _waitTime || isDecontLockdown)
                        {
                            _mainTimer = 0f;
                            base.TargetState = false;
                        }
                    }

                    if (!TargetState)
                    {
                        _currentSequence = CheckpointSequenceStage.Closing;
                        _quietSource.PlayOneShot(_warningSound);
                        SetButtons(_warningText, _warningMat);
                    }
                    break;

                case CheckpointSequenceStage.Closing:
                    if (NetworkServer.active)
                    {
                        _mainTimer += Time.deltaTime;
                        if (_mainTimer > _warningTime || isDecontLockdown)
                        {
                            _currentSequence = CheckpointSequenceStage.Idle;
                            ToggleAllDoors(false);
                            SetButtons(_panelSettings.TextClosed, _panelSettings.PanelClosedMat);

                            DoorLockMode mode = DoorLockUtils.GetMode((DoorLockReason)ActiveLocks);
                            if (!mode.HasFlagFast(DoorLockMode.CanClose)
                                && mode.HasFlagFast(DoorLockMode.CanOpen))
                            {
                                base.TargetState = true;
                            }
                        }
                    }
                    else
                    {
                        foreach (DoorVariant door in _subDoors)
                        {
                            if ((!(door is IDamageableDoor dmg) || !dmg.IsDestroyed)
                                && door.GetExactState() < 1f)
                            {
                                return;
                            }
                        }
                        _currentSequence = CheckpointSequenceStage.Idle;

                        if (ActiveLocks > 0)
                            SetButtons(_panelSettings.TextLockedDown, _panelSettings.PanelErrorMat);
                        else
                            SetButtons(_panelSettings.TextClosed, _panelSettings.PanelClosedMat);
                    }
                    break;
            }
        }

        private void ToggleAllDoors(bool newState)
        {
            foreach (DoorVariant door in _subDoors)
            {
                if (!(door is IDamageableDoor dmg) || !dmg.IsDestroyed)
                {
                    door.TargetState = newState;
                }
            }
        }

        [ClientRpc]
        private void RpcPlayBeepSound(byte deniedType)
        {
            if (_remainingBeepCooldown > 0f)
                return;

            _remainingBeepCooldown = 1f;
            _quietSource.PlayOneShot(_beepSound);

            if (deniedType == (byte)CheckpointErrorType.Denied)
            {
                SetButtons(_panelSettings.TextDenied, _panelSettings.PanelDeniedMat);
            }
            else if (deniedType == (byte)CheckpointErrorType.Destroyed)
            {
                SetButtons(_panelSettings.TextError, _panelSettings.PanelErrorMat);
                _permanentDestroyment = true;
            }
        }

        private void SetButtons(string text, Material mat)
        {
            if (_permanentDestroyment)
                return;

            foreach (RegularDoorButton btn in _buttons)
            {
                btn.SetupButton(text, mat);
            }
        }

        public bool ServerDamage(float hp, DoorDamageType type)
        {
            bool result = false;
            foreach (DoorVariant door in _subDoors)
            {
                if (door is IDamageableDoor damageable)
                {
                    result |= damageable.ServerDamage(hp, type);
                }
            }
            return result;
        }

        public void ClientDestroyEffects() { }

        public float GetHealthPercent()
        {
            float health = 1f;
            foreach (DoorVariant door in _subDoors)
            {
                if (door is IDamageableDoor damageable)
                {
                    health *= damageable.GetHealthPercent();
                }
            }
            return health;
        }
    }
}