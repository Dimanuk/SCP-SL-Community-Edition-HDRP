using AudioPooling;
using PlayerRoles.Spectating;
using UnityEngine;

namespace InventorySystem.Items.Firearms
{
    public abstract class FirearmAnimatorEventsBase : MonoBehaviour
    {
        private Firearm _cachedFirearm;

        private bool _cacheSet;

        private AnimatedFirearmViewmodel _afv;

        protected bool IsServerController;

        protected Firearm TargetFirearm
        {
            get
            {
                if (_cacheSet)
                    return _cachedFirearm;

                if (TryGetComponent<Animator>(out var component))
                    component.cullingMode = AnimatorCullingMode.AlwaysAnimate;
                else
                    Debug.LogError("Firearm " + base.name + " does not have an animator.");

                if (TryGetComponent<Firearm>(out var component2))
                {
                    TargetFirearm = component2;
                    IsServerController = true;
                }
                else
                {
                    IsServerController = false;
                    _afv = GetComponentInParent<AnimatedFirearmViewmodel>();
                    _cachedFirearm = _afv != null ? _afv.ParentItem as Firearm : null;
                    _cacheSet = true;
                }

                return _cachedFirearm;
            }
            set
            {
                _cachedFirearm = value;
                _cacheSet = true;
            }
        }

        public void InitializeFirearm(Firearm firearm)
        {
            _cachedFirearm = firearm;
            _cacheSet = true;
            IsServerController = false;

            if (_afv == null)
                _afv = GetComponentInParent<AnimatedFirearmViewmodel>();
        }

        protected void ModifyUserAmmo(int ammoToModify)
        {
            TargetFirearm.OwnerInventory.ServerAddAmmo(TargetFirearm.AmmoType, ammoToModify);
        }

        protected virtual void PlaySound(int soundId)
        {
            Firearm firearm = TargetFirearm;
            if (firearm == null || soundId >= firearm.AudioClips.Length)
                return;

            FirearmAudioClip firearmAudioClip = firearm.AudioClips[soundId];
            bool sendToPlayers = firearmAudioClip.HasFlag(FirearmAudioFlags.SendToPlayers);

            if (Mirror.NetworkServer.active && IsServerController && sendToPlayers)
                firearm.ServerSendAudioMessage((byte)soundId);

            if (IsServerController)
                return;

            if (_afv != null && _afv.IsFastForwarding)
                return;

            if (!firearm.IsLocalPlayer)
            {
                if (sendToPlayers)
                    return;

                if (firearm.Owner == null || !firearm.Owner.IsLocallySpectated())
                    return;
            }

            AudioSourcePoolManager.PlaySound(firearmAudioClip.Sound, transform, 1f, 1f,
                FalloffType.Exponential, AudioMixerChannelType.DefaultSfx, 0f, false);
        }
    }
}