using Interactables.Interobjects.DoorUtils;
using Mirror;
using UnityEngine;

namespace Interactables.Interobjects
{
    public class BreakableDoor : BasicDoor, IDamageableDoor, INonInteractableDoor, IScp106PassableDoor
    {
        [SyncVar]
        private bool _destroyed;

        private bool _prevDestroyed;

        [SerializeField]
        private float _maxHealth = 80f;

        [SerializeField]
        private BrokenDoor _brokenPrefab;

        [SerializeField]
        private DoorDamageType _ignoredDamageSources;

        [SerializeField]
        private GameObject _objectToReplace;

        [SerializeField]
        private bool _nonInteractable;

        public float RemainingHealth { get; set; }

        public float MaxHealth
        {
            get => _maxHealth;
            set => _maxHealth = value;
        }

        public DoorDamageType IgnoredDamageSources
        {
            get => _ignoredDamageSources;
            set => _ignoredDamageSources = value;
        }

        public bool IsDestroyed
        {
            get => _destroyed;
            set
            {
                if (value)
                    ServerDamage(_maxHealth, DoorDamageType.ServerCommand);
                else
                    Debug.LogError("You cannot unset the IsDestroyed value.");
            }
        }

        public bool IgnoreLockdowns => _nonInteractable;

        public bool IgnoreRemoteAdmin => _nonInteractable;

        public bool IsScp106Passable => true;

        [Server]
        public bool ServerDamage(float hp, DoorDamageType type)
        {
            if (!NetworkServer.active)
            {
                Debug.LogWarning("[Server] function 'System.Boolean Interactables.Interobjects.BreakableDoor::ServerDamage(System.Single,Interactables.Interobjects.DoorUtils.DoorDamageType)' called when server was not active");
                return false;
            }

            if (_destroyed)
                return false;

            if (DamageableDoorUtils.HasFlagFast(_ignoredDamageSources, type))
                return false;

            if (_brokenPrefab == null || _objectToReplace == null)
                return false;

            RemainingHealth -= hp;

            if (RemainingHealth <= 0f)
            {
                _destroyed = true;
                DoorEvents.TriggerAction(this, DoorAction.Destroyed, null);
            }

            return true;
        }

        public override float GetExactState()
        {
            if (!_destroyed)
                return base.GetExactState();

            return 1f;
        }

        public override bool AllowInteracting(ReferenceHub ply, byte colliderId)
        {
            if (!_destroyed)
                return base.AllowInteracting(ply, colliderId);

            return false;
        }

        internal override void TargetStateChanged()
        {
            if (!_destroyed)
                base.TargetStateChanged();
        }

        protected override void Update()
        {
            base.Update();

            if (!_prevDestroyed && _destroyed)
            {
                _prevDestroyed = true;
                ClientDestroyEffects();
            }
        }

        protected override void Start()
        {
            base.Start();
            RemainingHealth = _maxHealth;
        }

        public void ClientDestroyEffects()
        {
            if (_objectToReplace == null)
                return;

            _objectToReplace.SetActive(false);

            PanelVisualSettings panelSettings = PanelSettings;
            if (panelSettings != null)
            {
                base.SetButtons(panelSettings.TextError, panelSettings.PanelErrorMat, true);
            }

            if (_brokenPrefab == null || _objectToReplace == null)
                return;

            Transform parent = _objectToReplace.transform.parent;
            BrokenDoor brokenDoor = Object.Instantiate(_brokenPrefab, parent);

            Transform brokenTransform = brokenDoor.transform;
            Transform originalTransform = _objectToReplace.transform;

            brokenTransform.position = originalTransform.position;
            brokenTransform.rotation = originalTransform.rotation;
            brokenTransform.localScale = originalTransform.localScale;
        }

        public float GetHealthPercent()
        {
            return Mathf.Clamp01(RemainingHealth / _maxHealth);
        }
    }
}