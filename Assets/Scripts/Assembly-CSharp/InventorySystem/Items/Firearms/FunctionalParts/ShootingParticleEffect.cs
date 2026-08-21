using UnityEngine;

namespace InventorySystem.Items.Firearms.FunctionalParts
{
    public class ShootingParticleEffect : FunctionalFirearmPart
    {
        [SerializeField]
        private ParticleSystem[] _targetParticleSystems;

        private void Start()
        {
            Firearm fa = Firearm;
            if (fa != null)
                fa.OnShotCalled += OnShot;
        }

        private void OnDestroy()
        {
            Firearm fa = Firearm;
            if (fa != null)
                fa.OnShotCalled -= OnShot;
        }

        private void OnShot()
        {
            ParticleSystem[] systems = _targetParticleSystems;
            for (int i = 0; i < systems.Length; i++)
                systems[i].Play(withChildren: true);
        }
    }
}