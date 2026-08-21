using System;
using CameraShaking;
using InventorySystem.Items;
using InventorySystem.Items.SwayControllers;
using PlayerRoles;
using UnityEngine;

namespace PlayerRoles.PlayableScps.HUDs
{
    public abstract class ScpViewmodelBase : MonoBehaviour
    {
        private bool _destroyed;
        private GoopSway _sway;

        [SerializeField]
        private Vector3 _localRotation;

        [SerializeField]
        private Vector3 _trackerOffset;

        [SerializeField]
        private Transform _cameraBone;

        [SerializeField]
        private GoopSway.GoopSwaySettings _swaySettings;

        public abstract float CamFOV { get; }

        protected ScpHudBase Hud;

        protected ReferenceHub Owner;

        protected PlayerRoleBase Role;

        [field: SerializeField]
        protected Animator Anim { get; private set; }

        protected virtual void Start()
        {
            Transform current = base.transform;
            ScpHudBase hud;
            while (!current.TryGetComponent(out hud))
            {
                Transform parent = current.parent;
                if (parent == null)
                {
                    throw new NullReferenceException(
                        $"{this} failed to get component ScpHudBase for game object {base.name}");
                }
                current = parent;
            }

            this.Hud = hud;
            this.Owner = hud.Hub;
            this.Role = this.Owner.roleManager.CurrentRole;

            Transform transform = base.transform;
            Transform hands = SharedHandsController.Singleton.transform;

            transform.SetParent(hands, false);
            transform.localScale = Vector3.one;
            transform.localEulerAngles = _localRotation;

            PrimeAnimatorForAlignment();

            transform.position += hands.position - _cameraBone.position;
            hud.OnDestroyed += DestroySelf;
            PlayerRoleManager.OnRoleChanged += OnRoleChanged;

            this._sway = new GoopSway(_swaySettings, this.Owner);

            CameraShakeController.AddEffect(new TrackerShake(_cameraBone, _trackerOffset));
        }

        protected virtual void PrimeAnimatorForAlignment()
        {
            if (Anim != null)
                Anim.Update(0f);
        }

        protected virtual void SkipAnimations(float totalTime, int steps = 3)
        {
            float deltaTime = Time.deltaTime;

            if (Anim == null)
                return;

            Anim.Update(Time.deltaTime);
            UpdateAnimations();

            float stepDelta = (totalTime - deltaTime) / steps;
            for (int i = 0; i < steps; i++)
            {
                if (Anim == null)
                    return;

                Anim.Update(stepDelta);
            }
        }

        protected virtual void OnDestroy()
        {
            ScpHudBase hud = this.Hud;
            this._destroyed = true;

            if (hud != null)
            {
                hud.OnDestroyed -= DestroySelf;
                PlayerRoleManager.OnRoleChanged -= OnRoleChanged;
            }
        }

        protected virtual void LateUpdate()
        {
            UpdateAnimations();
            _sway?.UpdateSway();
        }

        protected abstract void UpdateAnimations();

        private void OnRoleChanged(ReferenceHub hub, PlayerRoleBase oldRole, PlayerRoleBase newRole)
        {
            if (hub == this.Owner && !this._destroyed)
            {
                UnityEngine.Object.Destroy(base.gameObject);
                this._destroyed = true;
            }
        }

        private void DestroySelf()
        {
            if (!this._destroyed)
            {
                UnityEngine.Object.Destroy(base.gameObject);
                this._destroyed = true;
            }
        }
    }
}
