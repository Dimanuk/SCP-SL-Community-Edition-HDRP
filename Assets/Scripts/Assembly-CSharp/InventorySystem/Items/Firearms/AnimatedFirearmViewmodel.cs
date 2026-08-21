using CameraShaking;
using InventorySystem.Items.Firearms.Attachments;
using InventorySystem.Items.Firearms.BasicMessages;
using InventorySystem.Items.Firearms.Modules;
using InventorySystem.Items.SwayControllers;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace InventorySystem.Items.Firearms
{
    public class AnimatedFirearmViewmodel : AnimatedViewmodelBase
    {
        [Serializable]
        public struct ViewmodelAttachmentSettings
        {
            public GameObject[] ToggleableObjects;
            public AdsAttachmentSettings AdsSettings;
            public DualCamAttachmentSettings DualCamSettings;
        }

        [Serializable]
        public struct AdsAttachmentSettings
        {
            public float AdsFov;
            public Vector3 AdsPosition;
            public Vector3 AdsRotation;
            public float AdsScopeParameterAddition;
            public bool AdsOverride;
        }

        [Serializable]
        public struct DualCamAttachmentSettings
        {
            public GameObject TargetCameras;
            public Material DimmerMaterial;
        }

        private static readonly int AlbedoColor;
        private static readonly int EmissionColor;
        private static readonly int ScopeAds;

        [SerializeField] private GoopSway.GoopSwaySettings _regularSwaySettings;
        [SerializeField] private GoopSway.GoopSwaySettings _adsSwaySettings;

        [SerializeField] private float _fov = 50f;
        [SerializeField] private Transform _cameraTrackerSource;
        [SerializeField] private Vector3 _cameraTrackerOffset;
        [SerializeField] private float _cameraTrackerIntensity = 1f;
        [SerializeField] private bool _randomizeShootAnims;

        public ViewmodelAttachmentSettings[] Attachments;

        private Firearm _fa;

        private AdsAttachmentSettings _combinedAds;
        private DualCamAttachmentSettings _combinedDual;

        private GoopSway _regularSway;
        private GoopSway _adsSway;

        private bool _useScopeParameter;
        private bool _audioMuted;
        private bool _scopeCamerasActive;

        private const float MaxReloadTime = 15f;

        public override float ViewmodelCameraFOV
        {
            get
            {
                if (_fa?.AdsModule is IAdsModule ads && ads != null)
                    return Mathf.Lerp(_fov, _fov - _combinedAds.AdsFov, ads.ClientAdsAmount);
                return _fov;
            }
        }

        public override IItemSwayController SwayController
        {
            get
            {
                if (_fa?.AdsModule is IAdsModule ads && ads != null && ads.ClientAdsAmount > 0f)
                    return _adsSway;
                return _regularSway;
            }
        }

        public bool AudioMuted
        {
            get => _audioMuted;
            private set => _audioMuted = value;
        }

        internal override void OnEquipped()
        {
            if (_fa == null)
            {
                return;
            }

            _fa.UpdateAnims();

            var trackerShake = new TrackerShake(_cameraTrackerSource, _cameraTrackerOffset, _cameraTrackerIntensity);
            CameraShakeController.AddEffect(trackerShake);

            UpdateAttachments();
        }

        public override void InitAny()
        {
            base.InitAny();

            _adsSway = new GoopSway(_adsSwaySettings, Hub);
            _regularSway = new GoopSway(_regularSwaySettings, Hub);

            _fa = ParentItem as Firearm;

            if (_fa != null)
            {
                _fa.ViewModel = this;
                _fa.OnShotCalled += OnShot;
                _fa.OnDryfired += OnDryfired;
                var animEvents = GetComponentInChildren<FirearmAnimatorEventsBase>(true);
                if (animEvents != null)
                    animEvents.InitializeFirearm(_fa);
            }
        }

        public override void InitSpectator(ReferenceHub ply, ItemIdentifier id, bool wasEquipped)
        {
            ParentItem = ply.inventory.CreateItemInstance(id, false);

            // OnAdded is what builds the firearm's modules (action/ammo/ads/equipper/hitreg) and
            // assigns the attachment ids. Without it this client-side simulated copy has null
            // modules, so UpdateAnims bails out and the viewmodel never leaves its unequipped pose.
            ParentItem?.OnAdded(null);

            base.InitSpectator(ply, id, wasEquipped);

            _fa = ParentItem as Firearm;

            if (_fa != null)
                _fa.ViewModel = this;

            if (_fa?.transform != null)
                _fa.transform.SetParent(transform, false);

            if (!FirearmBasicMessagesHandler.ReceivedStatuses.TryGetValue(id.SerialNumber, out FirearmStatus receivedStatus))
            {
                AttachmentsUtils.ApplyAttachmentsCode(_fa, 0, true);
            }
            else if (_fa != null)
            {
                // A status for this weapon already arrived before we started spectating it. Apply it
                // now so the freshly-created viewmodel picks up the real attachment code (default or
                // otherwise) — otherwise the spectated weapon renders with no attachments.
                _fa.Status = receivedStatus;
            }

            OnEquipped();

            FirearmAudioManager.OnAudioReceived += ProcessReceivedAudio;
            FirearmBasicMessagesHandler.OnStatusMessageReceived += ProcessReceivedStatus;
            FirearmBasicMessagesHandler.OnClientConfirmationReceived += ProcessRequestMessage;

            if (wasEquipped)
            {
                AudioMuted = true;

                // Stepped fast-forward (fastMode: false), not one giant Animator.Update — a single
                // multi-second step skips right over the equip state's transitions.
                AnimatorForceUpdate(SkipEquipTime, fastMode: false);

                RequestType reloadState = FirearmClientsideStateDatabase.GetReloadStateRaw(id.SerialNumber);
                if (reloadState != RequestType.ReloadStop)
                    HandleEquipReload(reloadState);

                AudioMuted = false;
            }
        }

        private void OnDestroy()
        {
            if (_fa != null)
            {
                _fa.OnShotCalled -= OnShot;
                _fa.OnDryfired -= OnDryfired;
            }

            FirearmAudioManager.OnAudioReceived -= ProcessReceivedAudio;
            FirearmBasicMessagesHandler.OnStatusMessageReceived -= ProcessReceivedStatus;
            FirearmBasicMessagesHandler.OnClientConfirmationReceived -= ProcessRequestMessage;
        }

        private void ProcessReceivedAudio(ReferenceHub rh, ItemType it, FirearmAudioClip fac)
        {
            if (it != ItemId.TypeId || rh != Hub)
                return;

            if (!fac.HasFlag(FirearmAudioFlags.IsGunshot))
                return;

            OnShot();
        }

        private void HandleEquipReload(RequestType rq)
        {
            if (_fa == null)
                return;

            ushort serial = ItemId.SerialNumber;

            // The reload animation branches on the ammo state the weapon had when the reload
            // BEGAN (empty vs tactical), but by now the spectator may already hold a newer
            // status. Temporarily rewind to the pre-reload status so UpdateAnims pushes the
            // right animator params, then restore the live status at the end.
            FirearmStatus curStatus = _fa.Status;
            _fa.Status = FirearmClientsideStateDatabase.GetPreReloadStatus(serial);
            _fa.UpdateAnims();

            // Replay the confirmed request (sets the reload/unload trigger), THEN fast-forward
            // the animator by the time that already elapsed, in 0.07 s steps so state
            // transitions aren't skipped over.
            ProcessRequestMessage(new RequestMessage(serial, rq));

            float time = Mathf.Min(FirearmClientsideStateDatabase.ElapsedReloadState(serial), MaxReloadTime);
            while (time > 0f)
            {
                AnimatorForceUpdate(Mathf.Min(time, 0.07f), true);
                time -= 0.07f;
            }

            _fa.Status = curStatus;
        }

        private void ProcessReceivedStatus(StatusMessage msg)
        {
            if (msg.Serial != ItemId.SerialNumber || _fa == null)
                return;
            _fa.Status = msg.Status;
        }

        private void ProcessRequestMessage(RequestMessage msg)
        {
            if (msg.Serial != ItemId.SerialNumber)
                return;

            switch (msg.Request)
            {
                case RequestType.Reload:
                    (_fa?.AmmoManagerModule as IAmmoManagerModule)?.ClientReload();
                    break;
                case RequestType.Unload:
                    (_fa?.AmmoManagerModule as IAmmoManagerModule)?.ClientUnload();
                    break;
                case RequestType.ReloadStop:
                    break;
                case RequestType.Dryfire:
                    OnDryfired();
                    break;
                case RequestType.Inspect:
                    _fa?.InspectorModule?.OnInspect();
                    break;
            }
        }

        protected override void LateUpdate()
        {
            base.LateUpdate();

            if (_fa == null) return;

            // Spectated firearms are simulated copies that live outside any inventory, so
            // Inventory.UpdateObserverItems never ticks them. The viewmodel drives the tick
            // instead — EquipUpdate runs UpdateAnims and fires OnEquipUpdateCalled, which is
            // what lets StandardAds mirror the spectated player's ADS state every frame.
            if (IsSpectator)
                _fa.EquipUpdate();

            if (_fa.AdsModule is IAdsModule ads && ads != null)
            {
                float adsAmount = ads.ClientAdsAmount;
                Transform vmTransform = transform;

                vmTransform.SetLocalPositionAndRotation(Vector3.Lerp(Vector3.zero, _combinedAds.AdsPosition, adsAmount), Quaternion.Lerp(
                    Quaternion.identity,
                    Quaternion.Euler(_combinedAds.AdsRotation),
                    adsAmount
                ));
            }

            if (_combinedDual.TargetCameras != null && Hub?.PlayerCameraReference != null)
            {
                bool isAiming = _fa.AdsModule?.ClientAdsAmount > 0f;
                _combinedDual.TargetCameras.SetActive(isAiming);

                // On the aiming -> not-aiming transition, wipe EVERY scope camera's render target. The
                // dual-render scope stacks more than one camera under TargetCameras and only one carries
                // ScopeShaderReplacement (which self-clears in OnDisable), so the compositing/lens RT kept
                // the last magnified frame — a frozen picture stuck on the lens after un-ADS.
                if (!isAiming && _scopeCamerasActive)
                    ClearScopeCameraTargets();
                _scopeCamerasActive = isAiming;

                Transform dualCam = _combinedDual.TargetCameras.transform;
                Transform playerCam = Hub.PlayerCameraReference;
                dualCam.SetPositionAndRotation(playerCam.position, playerCam.rotation);
            }

            float scopeAdd = _combinedAds.AdsScopeParameterAddition;
            if (scopeAdd > 0f || _useScopeParameter)
            {
                _useScopeParameter = true;
                // The animator gets the scope offset scaled by the current ADS progress —
                // a constant scopeAdd would hold the viewmodel in the scoped pose even from the hip.
                float scopeAds = (_fa.AdsModule as IAdsModule)?.ClientAdsAmount ?? 0f;
                _fa.ClientViewmodel?.AnimatorSetFloat(ScopeAds, scopeAdd * scopeAds);
            }

            if (_combinedDual.DimmerMaterial != null)
            {
                float adsProgress = _fa.AdsModule?.ClientAdsAmount ?? 0f;

                // A dual-cam scope lights its lens up as you aim, so the render texture becomes
                // visible. Plain reflex glass does the opposite: it fades to fully clear, otherwise
                // it stays in the way and blacks out the sight picture.
                bool hasScopeCameras = _combinedDual.TargetCameras != null;
                Color dim = Color.Lerp(
                    hasScopeCameras ? Color.black : Color.white,
                    hasScopeCameras ? Color.white : Color.clear,
                    adsProgress);

                _combinedDual.DimmerMaterial.SetColor(AlbedoColor, dim);
                _combinedDual.DimmerMaterial.SetColor(EmissionColor, dim);
            }
        }

        // Clears the render targets of every camera under the dual-render scope rig so no stale
        // magnified frame is left showing on the lens once the scope stops rendering.
        private void ClearScopeCameraTargets()
        {
            if (_combinedDual.TargetCameras == null)
                return;

            RenderTexture prev = RenderTexture.active;
            foreach (Camera cam in _combinedDual.TargetCameras.GetComponentsInChildren<Camera>(true))
            {
                RenderTexture rt = cam.targetTexture;
                if (rt == null)
                    continue;

                RenderTexture.active = rt;
                GL.Clear(true, true, Color.clear);
            }
            RenderTexture.active = prev;
        }

        protected virtual void OnShot()
        {

            if (_randomizeShootAnims)
                AnimatorSetFloat(FirearmAnimatorHashes.Random, Random.value);

            AnimatorSetTrigger(FirearmAnimatorHashes.Fire);

            if (SharedHandsController.Singleton != null ? SharedHandsController.Singleton.Hands : null != null)
                SharedHandsController.Singleton.Hands.SetTrigger(FirearmAnimatorHashes.Fire);
        }

        protected virtual void OnDryfired()
        {

            AnimatorSetTrigger(FirearmAnimatorHashes.DryFire);

            if (SharedHandsController.Singleton != null ? SharedHandsController.Singleton.Hands : null != null)
                SharedHandsController.Singleton.Hands.SetTrigger(FirearmAnimatorHashes.DryFire);
        }

        public void UpdateAttachments()
        {
            if (_fa == null || _fa.Attachments == null || Attachments == null)
                return;

            if (Attachments.Length != _fa.Attachments.Length)
            {
                throw new Exception($"Attachment number mismatch for weapon {_fa} detected!");
            }

            if (_combinedDual.TargetCameras != null)
            {
                ClearScopeCameraTargets();
                _combinedDual.TargetCameras.SetActive(false);
            }
            _scopeCamerasActive = false;

            _combinedAds = default;
            _combinedDual = default;

            for (int i = 0; i < Attachments.Length; i++)
            {
                var toggleables = Attachments[i].ToggleableObjects;
                if (toggleables == null) continue;
                foreach (var obj in toggleables)
                {
                    if (obj != null)
                        obj.SetActive(false);
                }
            }

            bool adsOverrideApplied = false;

            for (int i = 0; i < Attachments.Length; i++)
            {
                var settings = Attachments[i];
                var attachment = _fa.Attachments[i];

                if (attachment == null || !attachment.IsEnabled)
                    continue;

                if (settings.ToggleableObjects != null)
                {
                    foreach (var obj in settings.ToggleableObjects)
                    {
                        if (obj != null)
                            obj.SetActive(true);
                    }
                }

                if (settings.DualCamSettings.TargetCameras != null)
                    _combinedDual.TargetCameras = settings.DualCamSettings.TargetCameras;

                if (settings.DualCamSettings.DimmerMaterial != null)
                    _combinedDual.DimmerMaterial = settings.DualCamSettings.DimmerMaterial;

                if (settings.AdsSettings.AdsOverride)
                {
                    _combinedAds = settings.AdsSettings;
                    adsOverrideApplied = true;
                }
                else if (!adsOverrideApplied)
                {
                    _combinedAds.AdsFov += settings.AdsSettings.AdsFov;
                    _combinedAds.AdsPosition += settings.AdsSettings.AdsPosition;
                    _combinedAds.AdsRotation += settings.AdsSettings.AdsRotation;
                    _combinedAds.AdsScopeParameterAddition += settings.AdsSettings.AdsScopeParameterAddition;
                }
            }
        }

        static AnimatedFirearmViewmodel()
        {
            AlbedoColor = Shader.PropertyToID("_Color");
            EmissionColor = Shader.PropertyToID("_EmissionColor");
            ScopeAds = Animator.StringToHash("ScopeAds");
        }
    }
}
