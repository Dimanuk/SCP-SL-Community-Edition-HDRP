using UnityEngine;
using Mirror;

namespace InventorySystem.Items.ThrowableProjectiles
{
    public class EffectGrenade : TimeGrenade
    {
        public GameObject Effect;

        [SerializeField]
        private float _destroyTime;

        [SerializeField]
        private AudioSource _src;

        private bool _resyncAudio = true;

        protected override void Update()
        {
            base.Update();

            // Re-sync the countdown "beep" audio so that the clip ends exactly at detonation.
            // Only resync while the fuse is still running (now <= TargetTime).
            float now = Time.timeSinceLevelLoad;
            if (!_resyncAudio || now > TargetTime)
            {
                return;
            }

            _resyncAudio = false;

            if (_src == null || _src.clip == null)
            {
                return;
            }

            float timeRemaining = TargetTime - now;
            float clipLength = _src.clip.length;
            if (timeRemaining <= clipLength)
            {
                // Fuse shorter than the clip: start playing now, offset so the clip finishes at detonation.
                _src.Play();
                _src.time = clipLength - timeRemaining;
            }
            else
            {
                // Fuse longer than the clip: delay playback so it still finishes at detonation.
                _src.PlayDelayed(timeRemaining - clipLength);
            }
        }

        protected virtual void PlayExplosionEffects()
        {
            if (Effect == null)
                return;

            GameObject effectInstance = Object.Instantiate(Effect);
            effectInstance.transform.SetPositionAndRotation(transform.position, transform.rotation);

            if (transform.parent != null)
            {
                effectInstance.transform.parent = transform.parent;
            }

            if (_src != null)
            {
                _src.Play();
            }

            if (_destroyTime > 0f)
            {
                Object.Destroy(effectInstance, _destroyTime);
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (TargetTime != 0f) 
            {
                PlayExplosionEffects();
            }
        }

        public override void ToggleRenderers(bool state)
        {
            base.ToggleRenderers(state);
            _resyncAudio = state;
        }

        protected override void ServerFuseEnd()
        {
            DestroySelf();
        }
    }
}