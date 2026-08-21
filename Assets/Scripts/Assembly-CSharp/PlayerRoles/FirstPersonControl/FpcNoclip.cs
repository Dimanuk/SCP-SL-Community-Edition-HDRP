using System.Collections.Generic;
using System.Diagnostics;
using InventorySystem;
using Mirror;
using PlayerRoles.FirstPersonControl.NetworkMessages;
using PlayerStatsSystem;
using UnityEngine;

namespace PlayerRoles.FirstPersonControl
{
    public class FpcNoclip
    {
        public static float CurSpeed = 10f;

        private const float MinNoclipSpeed = 0.1f;
        private const float MaxNoclipSpeed = 250f;
        private const float NoclipLerp = 16f;
        private const float NoclipMaxDiffSqr = 25f;
        private const float RecentTimeThreshold = 2.5f;
        private const string SpeedAxis = "Mouse ScrollWheel";

        private bool _wasEnabled;

        private readonly ReferenceHub _hub;
        private readonly FirstPersonMovementModule _fpmm;
        private readonly AdminFlagsStat _stats;
        private readonly Stopwatch _lastNcSw = new Stopwatch();

        private static readonly HashSet<uint> PermittedPlayers = new HashSet<uint>();

        private static KeyCode _keyFwd;
        private static KeyCode _keyBwd;
        private static KeyCode _keyLft;
        private static KeyCode _keyRgt;
        private static KeyCode _keyUpw;
        private static KeyCode _keyDnw;
        private static KeyCode _keyToggle;
        private static KeyCode _keyFog;

        public bool IsActive
        {
            get => _stats.HasFlag(AdminFlags.Noclip);
            set => _stats.SetFlag(AdminFlags.Noclip, value);
        }

        public bool RecentlyActive
        {
            get
            {
                if (_lastNcSw.IsRunning)
                    return _lastNcSw.Elapsed.TotalSeconds < RecentTimeThreshold;
                return false;
            }
        }

        public FpcNoclip(ReferenceHub hub, FirstPersonMovementModule fpmm)
        {
            _hub = hub;
            _fpmm = fpmm;
            _stats = hub.playerStats.GetModule<AdminFlagsStat>();

            if (hub.isLocalPlayer)
                ReloadInputConfigs();
        }

        public void UpdateNoclip()
        {
            if (_hub.isLocalPlayer && Input.GetKeyDown(_keyToggle))
            {
                NetworkClient.Send(new FpcNoclipToggleMessage());
            }

            if (!IsActive)
            {
                if (_wasEnabled)
                    DisableNoclipClientside();

                _wasEnabled = false;
                return;
            }

            _wasEnabled = true;
            _lastNcSw.Restart();

            if (NetworkServer.active)
            {
                _fpmm.Motor.ResetFallDamageCooldown();
            }

            if (_hub.isLocalPlayer)
            {
                HandleLocalNoclipMovement();
                HandleFogToggle();
                UpdateNoclipUI();
            }
            else
            {
                Vector3 targetPos = _fpmm.Motor.ReceivedPosition.Position;
                float sqrDist = (targetPos - _fpmm.Position).sqrMagnitude;
                float t = (sqrDist > NoclipMaxDiffSqr) ? 1f : (Time.deltaTime * NoclipLerp);

                _fpmm.Position = Vector3.Lerp(_fpmm.Position, targetPos, t);
            }
        }

        private void HandleLocalNoclipMovement()
        {
            if (Cursor.visible || Cursor.lockState != CursorLockMode.Locked)
                return;

            Vector3 moveDirection = Vector3.zero;
            Transform cam = _hub.PlayerCameraReference;

            if (Input.GetKey(_keyFwd)) moveDirection += cam.forward;
            if (Input.GetKey(_keyBwd)) moveDirection -= cam.forward;
            if (Input.GetKey(_keyLft)) moveDirection -= cam.right;
            if (Input.GetKey(_keyRgt)) moveDirection += cam.right;
            if (Input.GetKey(_keyUpw)) moveDirection += Vector3.up;
            if (Input.GetKey(_keyDnw)) moveDirection -= Vector3.up;

            if (moveDirection.sqrMagnitude > 0.01f)
            {
                moveDirection.Normalize();
                _fpmm.Position += moveDirection * (CurSpeed * Time.deltaTime);
            }

            float scroll = Input.GetAxis(SpeedAxis);
            if (Mathf.Abs(scroll) > 0.01f)
            {
                CurSpeed = Mathf.Clamp(CurSpeed + scroll * 5f, MinNoclipSpeed, MaxNoclipSpeed);
            }
        }

        private void HandleFogToggle()
        {
            if (Input.GetKeyDown(_keyFog))
            {
                // Deactivating the FogController object only stops it from updating
                // fog color/distance — the fog itself keeps rendering. The original
                // toggles the global fog flag directly.
                RenderSettings.fog = !RenderSettings.fog;
            }
        }

        private void UpdateNoclipUI()
        {
            UserMainInterface.Singleton?.SetNoclipIndicator(true, CurSpeed);
        }

        private void DisableNoclipClientside()
        {
            UserMainInterface.Singleton?.SetNoclipIndicator(false, 0f);
            RenderSettings.fog = true;
        }

        public void ShutdownModule()
        {
            if (NetworkServer.active)
                IsActive = false;

            DisableNoclipClientside();
        }

        public static bool IsPermitted(ReferenceHub ply) =>
            ply != null && PermittedPlayers.Contains(ply.netId);

        public static void PermitPlayer(ReferenceHub ply)
        {
            if (ply == null) return;

            PermittedPlayers.Add(ply.netId);
            ply.gameConsoleTransmission.SendToClient("Noclip is now permitted.", "green");
        }

        public static void UnpermitPlayer(ReferenceHub ply)
        {
            if (ply == null) return;

            PermittedPlayers.Remove(ply.netId);
            ply.playerStats.GetModule<AdminFlagsStat>().SetFlag(AdminFlags.Noclip, false);
            ply.gameConsoleTransmission.SendToClient("Noclip permission revoked.", "yellow");
        }

        [RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            Inventory.OnServerStarted += () => PermittedPlayers.Clear();
            NewInput.OnAnyModified += ReloadInputConfigs;
        }

        private static void ReloadInputConfigs()
        {
            _keyFwd = NewInput.GetKey(ActionName.MoveForward, KeyCode.None);
            _keyBwd = NewInput.GetKey(ActionName.MoveBackward, KeyCode.None);
            _keyLft = NewInput.GetKey(ActionName.MoveLeft, KeyCode.None);
            _keyRgt = NewInput.GetKey(ActionName.MoveRight, KeyCode.None);
            _keyUpw = NewInput.GetKey(ActionName.Jump, KeyCode.None);
            _keyDnw = NewInput.GetKey(ActionName.Sneak, KeyCode.None);
            _keyToggle = NewInput.GetKey(ActionName.Noclip, KeyCode.None);
            _keyFog = NewInput.GetKey(ActionName.NoClipFogToggle, KeyCode.None);
        }
    }
}