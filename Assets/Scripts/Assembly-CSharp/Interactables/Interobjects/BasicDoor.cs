using System.Collections.Generic;
using System.Linq;
using Interactables.Interobjects.DoorUtils;
using UnityEngine;

namespace Interactables.Interobjects
{
    public class BasicDoor : DoorVariant
    {
        private static readonly int AnimHash = global::UnityEngine.Animator.StringToHash("isOpen");

        [SerializeField]
        internal Animator MainAnimator;

        [SerializeField]
        internal AudioSource MainSource;

        [SerializeField]
        private float _cooldownDuration;

        [SerializeField]
        private float _consideredOpenThreshold = 0.7f;

        [SerializeField]
        private float _anticheatPassableThreshold = 0.2f;

        [SerializeField]
        private RegularDoorButton[] _buttons;

        [SerializeField]
        internal PanelVisualSettings PanelSettings;

        [SerializeField]
        internal DoorAudioSettings AudioSettings;

        [SerializeField]
        private Transform _stateMoveable;

        [SerializeField]
        private Transform _stateStator;

        [SerializeField]
        private float _stateMinDis;

        [SerializeField]
        private float _stateMaxDis;

        [HideInInspector]
        public bool UpdateAnimations;

        public List<Collider> Scp106Colliders;

        private float _remainingAnimCooldown;

        private float _remainingBeepCooldown;

        private bool _permanentPanels;

        public override bool AllowInteracting(ReferenceHub ply, byte colliderId)
        {
            return _remainingAnimCooldown <= 0f;
        }

        public override float GetExactState()
        {
            global::UnityEngine.Vector3 position = _stateMoveable.position;
            global::UnityEngine.Vector3 position2 = _stateStator.position;
            float value = global::UnityEngine.Mathf.Abs(position.x - position2.x)
                        + global::UnityEngine.Mathf.Abs(position.y - position2.y)
                        + global::UnityEngine.Mathf.Abs(position.z - position2.z);
            return global::UnityEngine.Mathf.Clamp01(global::UnityEngine.Mathf.InverseLerp(_stateMinDis, _stateMaxDis, value));
        }

        public override bool IsConsideredOpen()
        {
            return GetExactState() > _consideredOpenThreshold;
        }

        public override bool AnticheatPassageApproved()
        {
            if (!IsConsideredOpen())
            {
                if (!TargetState)
                {
                    return GetExactState() > _anticheatPassableThreshold;
                }
                return false;
            }
            return true;
        }

        public override void LockBypassDenied(ReferenceHub ply, byte colliderId)
        {
            RpcPlayBeepSound(denied: false);
        }

        public override void PermissionsDenied(ReferenceHub ply, byte colliderId)
        {
            RpcPlayBeepSound(denied: true);
        }

        [global::Mirror.ClientRpc]
        private void RpcPlayBeepSound(bool denied)
        {
            if (_remainingBeepCooldown <= 0f)
            {
                _remainingBeepCooldown = 1f;
                MainSource.PlayOneShot(AudioSettings.AccessDenied);
                if (denied)
                {
                    SetButtons(PanelSettings.TextDenied, PanelSettings.PanelDeniedMat);
                    UpdateAnimations = true;
                }
            }
        }

        protected override void Update()
        {
            base.Update();

            if (_remainingBeepCooldown > 0f)
                _remainingBeepCooldown -= global::UnityEngine.Time.deltaTime;

            if (global::Mirror.NetworkServer.active && _remainingAnimCooldown > 0f)
                _remainingAnimCooldown -= global::UnityEngine.Time.deltaTime;

            if (!UpdateAnimations)
                return;

            if (ActiveLocks > 0)
            {
                SetButtons(PanelSettings.TextLockedDown, PanelSettings.PanelErrorMat);
                UpdateAnimations = false;
                return;
            }

            // The panel follows the actual animation progress, not the server-only interaction
            // cooldown - otherwise clients would never see the MOVING stage.
            float exactState = GetExactState();
            bool stillMoving = TargetState ? exactState < 1f : exactState > 0f;

            if (stillMoving)
            {
                SetButtons(PanelSettings.TextMoving, PanelSettings.PanelMovingMat);
                return;
            }

            if (_remainingBeepCooldown > 0f)
                return;

            SetButtons(
                exactState == 0f ? PanelSettings.TextClosed : PanelSettings.TextOpen,
                exactState == 0f ? PanelSettings.PanelClosedMat : PanelSettings.PanelOpenMat);

            UpdateAnimations = false;
        }

        private void Awake()
        {
            Scp106Colliders = GetComponentsInChildren<Collider>().ToList();
            for (int i = Scp106Colliders.Count - 1; i >= 0; i--)
            {
                if (Scp106Colliders[i].GetComponent<RegularDoorButton>() != null)
                    Scp106Colliders.RemoveAt(i);
            }
        }

        protected override void Start()
        {
            base.Start();
            MainAnimator.SetBool(AnimHash, TargetState);
            SetButtons(
                TargetState ? PanelSettings.TextOpen : PanelSettings.TextClosed,
                TargetState ? PanelSettings.PanelOpenMat : PanelSettings.PanelClosedMat,
                permanent: false);
        }

        internal void SetButtons(string text, Material mat, bool permanent = false)
        {
            if (_permanentPanels)
                return;

            foreach (RegularDoorButton btn in _buttons)
            {
                btn.SetupButton(text, mat);
            }
            _permanentPanels = permanent;
        }

        internal override void TargetStateChanged()
        {
            MainAnimator.SetBool(AnimHash, TargetState);
            if (global::Mirror.NetworkServer.active)
            {
                _remainingAnimCooldown = _cooldownDuration;
            }
            MainSource.PlayOneShot(TargetState ? AudioSettings.RandomOpeningSound : AudioSettings.RandomClosingSound);
            UpdateAnimations = true;
        }

        protected override void LockChanged(ushort prevValue)
        {
            UpdateAnimations = true;
        }
    }
}