using System.Diagnostics;
using UnityEngine;

namespace InventorySystem.Items.Firearms.FunctionalParts
{
    public class AnimatedBelt : FunctionalFirearmPart
    {
        [SerializeField] private int _idleLayer;
        [SerializeField] private float _minimalCooldown;
        [SerializeField] private GameObject[] _bullets;

        private int _prevBullets;
        private bool _wasActive;
        private readonly Stopwatch _stopwatch = new();

        private FirearmStatus CurStatus
        {
            get
            {
                Firearm fa = Firearm;
                if (fa == null)
                    return default;

                // Local player reads the predicted (client-side) status so the belt reacts instantly;
                // spectators use the networked status.
                return fa.IsLocalPlayer ? fa.ActionModule.PredictedStatus : fa.Status;
            }
        }

        private void OnEnable()
        {
            if (!_wasActive)
                return;

            Firearm fa = Firearm;
            if (fa != null)
                Refresh(fa.Status.Ammo);
        }

        private void Start()
        {
            _wasActive = true;

            Firearm fa = Firearm;
            if (fa != null)
                fa.OnShotCalled += OnShot;

            _stopwatch.Start();

            if (fa != null)
                Refresh(fa.Status.Ammo);
        }

        private void LateUpdate()
        {
            // Suppress belt re-evaluation for a short window after every shot so the fire animation
            // can play out without the belt snapping the count around.
            if (_minimalCooldown > _stopwatch.Elapsed.TotalSeconds)
                return;

            Firearm fa = Firearm;
            if (fa == null)
                return;

            AnimatedFirearmViewmodel viewmodel = fa.ClientViewmodel;
            if (viewmodel == null)
                return;

            int curTag = viewmodel.GetAnimatorStateInfo(_idleLayer).tagHash;
            int targetAmmo = GetTargetAmmo(curTag, _prevBullets, CurStatus);
            if (targetAmmo != _prevBullets)
                Refresh(targetAmmo);
        }

        private int GetTargetAmmo(int curTag, int prev, FirearmStatus status)
        {
            Firearm fa = Firearm;

            if (curTag == FirearmAnimatorHashes.Reload)
            {
                byte maxAmmo = fa.AmmoManagerModule.MaxAmmo;

                // Spectator: show the synced ammo while it is meaningful, otherwise fill the belt.
                if (!fa.IsLocalPlayer)
                    return status.Ammo != 0 ? status.Ammo : maxAmmo;

                // Mag inserted -> the belt just mirrors the loaded ammo.
                if ((status.Flags & FirearmStatusFlags.MagazineInserted) != 0)
                    return status.Ammo;

                // Feeding from inventory: never show more rounds than are actually available.
                int inventoryAmmo = fa.OwnerInventory.GetCurAmmo(fa.AmmoType);
                return Mathf.Min(inventoryAmmo, maxAmmo);
            }

            if (curTag == FirearmAnimatorHashes.Idle)
                return status.Ammo;

            // Any other animator state (e.g. firing) keeps the currently displayed count.
            return prev;
        }

        private void OnDestroy()
        {
            Firearm fa = Firearm;
            if (fa != null)
                fa.OnShotCalled -= OnShot;
        }

        private void OnShot()
        {
            FirearmStatus status = CurStatus;

            Firearm fa = Firearm;
            if (fa == null)
                return;

            // Keep the just-fired round on the belt for the local shooter until the fire animation
            // settles; a spectator sees the plain networked count.
            int curAmmo = status.Ammo + (fa.IsSpectated ? 0 : 1);
            Refresh(curAmmo);
            _stopwatch.Restart();
        }

        private void Refresh(int curAmmo)
        {
            if (_bullets != null)
            {
                for (int i = 0; i < _bullets.Length; i++)
                {
                    GameObject bullet = _bullets[i];
                    if (bullet != null)
                        bullet.SetActive(i < curAmmo);
                }
            }

            _prevBullets = curAmmo;
        }
    }
}
