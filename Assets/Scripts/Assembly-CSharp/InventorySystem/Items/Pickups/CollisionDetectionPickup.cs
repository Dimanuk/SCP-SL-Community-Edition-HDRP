using System;
using System.Diagnostics;
using AudioPooling;
using Mirror;
using UnityEngine;

namespace InventorySystem.Items.Pickups
{
    public class CollisionDetectionPickup : ItemPickupBase
    {
        [Serializable]
        protected struct SoundOverVelocity
        {
            [SerializeField] public float MinimalVelocity;
            [SerializeField] public AudioClip[] RandomClips;
            [Range(0f, 0.5f)][SerializeField] public float RandomizePitch;
            [SerializeField] public float MaxRange;

            public bool TryPlaySound(float vel, AudioSource src)
            {
                if (vel < MinimalVelocity || RandomClips == null || RandomClips.Length == 0)
                    return false;

                src.PlayOneShot(RandomClips[UnityEngine.Random.Range(0, RandomClips.Length)]);
                src.maxDistance = MaxRange;
                src.pitch = UnityEngine.Random.Range(1f - RandomizePitch, 1f / (1f - RandomizePitch));
                return true;
            }
        }

        public event Action<Collision> OnCollided;

        [SerializeField] private SoundOverVelocity[] _soundsOverVelocity;
        [SerializeField] private AudioSource _audioSrc;

        private const float MinimalJoulesToInduceDamage = 15f;
        private const float DamagePerJoule = 0.4f;
        private const float DefaultSoundCooldown = 0.1f;

        private readonly Stopwatch _stopwatch = new Stopwatch();

        public virtual float MinSoundCooldown => DefaultSoundCooldown;

        public CollisionDetectionPickup()
        {
            _stopwatch = Stopwatch.StartNew();
        }

        private void OnCollisionEnter(Collision collision)
        {
            ProcessCollision(collision);
        }

        public float GetRangeOfCollisionVelocity(float sqrVel)
        {
            AudioSource free = AudioSourcePoolManager.GetFree(false);
            for (int i = _soundsOverVelocity.Length - 1; i >= 0; i--)
            {
                if (_soundsOverVelocity[i].TryPlaySound(sqrVel, free))
                {
                    float maxDist = free.maxDistance;
                    free.Stop();
                    return maxDist;
                }
            }
            return 0f;
        }

        protected virtual void ProcessCollision(Collision collision)
        {
            OnCollided?.Invoke(collision);

            float sqrVelocity = collision.relativeVelocity.sqrMagnitude;
            float joules = (base.Info.Weight * sqrVelocity) * 0.5f;

            if (NetworkServer.active && joules > MinimalJoulesToInduceDamage)
            {
                if (collision.collider.TryGetComponent<BreakableWindow>(out var window))
                {
                    window.Damage(joules * DamagePerJoule, null, Vector3.zero);
                }
            }

            MakeCollisionSound(sqrVelocity);
        }

        protected void MakeCollisionSound(float sqrVelocity)
        {
            if (_stopwatch.Elapsed.TotalSeconds < (double)MinSoundCooldown)
                return;

            if (_audioSrc == null || _soundsOverVelocity == null)
                return;

            _stopwatch.Restart();

            for (int i = _soundsOverVelocity.Length - 1; i >= 0; i--)
            {
                if (sqrVelocity >= _soundsOverVelocity[i].MinimalVelocity)
                {
                    if (_soundsOverVelocity[i].TryPlaySound(sqrVelocity, _audioSrc))
                        break;
                }
            }
        }
    }
}