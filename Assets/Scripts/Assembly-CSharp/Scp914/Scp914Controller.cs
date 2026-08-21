using System;
using GameCore;
using Interactables;
using Interactables.Interobjects.DoorUtils;
using Interactables.Verification;
using Mirror;
using UnityEngine;
using Utils.ConfigHandler;

namespace Scp914
{
    public class Scp914Controller : NetworkBehaviour, IServerInteractable, IInteractable
    {
        [SerializeField]
        [SyncVar]
        private Scp914KnobSetting _knobSetting;

        [SerializeField]
        private AudioSource _knobSoundSource;

        [SerializeField]
        private AudioSource _upgradingSoundSource;

        [SerializeField]
        private Transform _knobTransform;

        [SerializeField]
        private float _knobChangeCooldown;

        [SerializeField]
        private float _totalSequenceTime;

        [SerializeField]
        private float _doorCloseTime;

        [SerializeField]
        private float _itemUpgradeTime;

        [SerializeField]
        private float _doorOpenTime;

        [SerializeField]
        private DoorVariant[] _doors;

        [SerializeField]
        private Vector3 _chamberSize;

        [SerializeField]
        private float _knobTurningSpeed;

        [SerializeField]
        private float _knobNotchAngle;

        private float _remainingCooldown;

        private bool _isUpgrading;

        private bool _itemsAlreadyUpgraded;

        private ConfigEntry<Scp914Mode> _configMode;

        public static Scp914Controller Singleton { get; private set; }

        [field: SerializeField]
        public Transform IntakeChamber { get; private set; }

        [field: SerializeField]
        public Transform OutputChamber { get; private set; }

        private Vector3 IntakeChamberSize
        {
            get
            {
                Vector3 vector = IntakeChamber.rotation * _chamberSize / 2f;
                return new Vector3(Mathf.Abs(vector.z), Mathf.Abs(vector.y), Mathf.Abs(vector.x));
            }
        }

        public IVerificationRule VerificationRule => StandardDistanceVerification.Default;

        public Scp914KnobSetting KnobSetting => _knobSetting;

        public void ServerInteract(ReferenceHub ply, byte colliderId)
        {
            if (_remainingCooldown > 0f)
            {
                return;
            }

            switch ((Scp914InteractCode)colliderId)
            {
                case Scp914InteractCode.ChangeMode:
                    {
                        Scp914KnobSetting nextSetting = _knobSetting + 1;
                        if (!Enum.IsDefined(typeof(Scp914KnobSetting), nextSetting))
                        {
                            nextSetting = Scp914KnobSetting.Rough;
                        }

                        _knobSetting = nextSetting;
                        _remainingCooldown = _knobChangeCooldown;
                        RpcPlaySound(0);
                        break;
                    }
                case Scp914InteractCode.Activate:
                    {
                        _remainingCooldown = _totalSequenceTime;
                        _isUpgrading = true;
                        _itemsAlreadyUpgraded = false;
                        RpcPlaySound(1);
                        break;
                    }
            }
        }

        private void Start()
        {
            Singleton = this;

            if (Scp914Upgrader.SolidObjectMask == 0)
            {
                Scp914Upgrader.SolidObjectMask = LayerMask.GetMask("Default", "Door");
            }

            if (NetworkServer.active)
            {
                _configMode = new ConfigEntry<Scp914Mode>("914_mode", Scp914Mode.DroppedAndPlayerTeleport, "SCP-914 Mode", "The behavior SCP-914 should use when upgrading items.");
                ConfigFile.ServerConfig.RegisterConfig(_configMode);
            }
        }

        private void OnDestroy()
        {
            if (NetworkServer.active && _configMode != null)
            {
                ConfigFile.ServerConfig.UnRegisterConfig(_configMode);
            }
        }

        private void Update()
        {
            UpdateClientsideEffects();

            if (NetworkServer.active)
            {
                UpdateServerside();
            }
        }

        private void UpdateClientsideEffects()
        {
            if (_knobTransform == null) return;

            Quaternion targetRotation = Quaternion.Euler(Vector3.forward * (((int)_knobSetting - 2) * _knobNotchAngle));

            _knobTransform.localRotation = Quaternion.Lerp(
                _knobTransform.localRotation,
                targetRotation,
                Time.deltaTime * _knobTurningSpeed
            );
        }

        [Server]
        private void UpdateServerside()
        {
            if (!NetworkServer.active) return;

            if (_isUpgrading)
            {
                float timePassed = _totalSequenceTime - _remainingCooldown;

                if (timePassed >= _doorCloseTime && timePassed < _doorOpenTime && _doors[0].TargetState)
                {
                    foreach (DoorVariant door in _doors)
                    {
                        door.TargetState = false;
                    }
                }
                else if (timePassed >= _itemUpgradeTime && !_itemsAlreadyUpgraded)
                {
                    Collider[] colliders = Physics.OverlapBox(IntakeChamber.position, IntakeChamberSize);

                    Vector3 moveVector = OutputChamber.position - IntakeChamber.position;

                    Scp914Upgrader.Upgrade(colliders, moveVector, _configMode.Value, _knobSetting);
                    _itemsAlreadyUpgraded = true;
                }
                else if (timePassed >= _doorOpenTime && !_doors[0].TargetState)
                {
                    foreach (DoorVariant door in _doors)
                    {
                        door.TargetState = true;
                    }
                }
            }

            if (_remainingCooldown >= 0f)
            {
                _remainingCooldown -= Time.deltaTime;
                if (_remainingCooldown < 0f)
                {
                    _isUpgrading = false;
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (IntakeChamber != null && OutputChamber != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawCube(IntakeChamber.position, IntakeChamberSize * 2f);
                Gizmos.DrawCube(OutputChamber.position, IntakeChamberSize * 2f);
            }
        }

        [ClientRpc]
        private void RpcPlaySound(byte soundId)
        {
            switch ((Scp914Sound)soundId)
            {
                case Scp914Sound.KnobChange:
                    if (_knobSoundSource != null)
                    {
                        _knobSoundSource.Stop();
                        _knobSoundSource.Play();
                    }
                    break;
                case Scp914Sound.Upgrading:
                    if (_upgradingSoundSource != null)
                    {
                        _upgradingSoundSource.Stop();
                        _upgradingSoundSource.Play();
                    }
                    break;
            }
        }
    }
}
