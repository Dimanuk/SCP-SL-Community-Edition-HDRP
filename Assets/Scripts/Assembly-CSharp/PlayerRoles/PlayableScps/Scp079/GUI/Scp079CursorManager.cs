using CursorManagement;
using PlayerRoles.PlayableScps.Scp079.Cameras;
using PlayerRoles.PlayableScps.Scp079.Map;
using UnityEngine;

namespace PlayerRoles.PlayableScps.Scp079.GUI
{
    public class Scp079CursorManager : Scp079GuiElementBase, ICursorOverride
    {
        [SerializeField]
        private ActionName _freeLookKey;

        [SerializeField]
        private GameObject _freeLookCursor;

        private Scp079CurrentCameraSync _curCamSync;

        private bool _freeLookToggleMode;

        private bool _mouseLook;

        public static CursorOverrideMode CurrentMode { get; private set; }

        public static bool LockCameras { get; private set; }

        public CursorOverrideMode CursorOverride => CurrentMode;

        public bool LockMovement => LockCameras;

        private void Update()
        {
            KeyCode key = NewInput.GetKey(_freeLookKey);

            if (_freeLookToggleMode)
            {
                if (Input.GetKeyDown(key))
                {
                    _mouseLook = !_mouseLook;
                }
            }
            else
            {
                _mouseLook = Input.GetKey(key);
            }

            CurrentMode = _mouseLook ? CursorOverrideMode.Centered : CursorOverrideMode.Confined;
            LockCameras = false;

            if (_curCamSync.CurClientSwitchState != Scp079CurrentCameraSync.ClientSwitchState.None || Scp079ToggleMapAbility.MapState)
            {
                LockCameras = true;
                CurrentMode = CursorOverrideMode.Centered;
            }

            if (Cursor.lockState == CursorLockMode.None || !Application.isFocused)
            {
                LockCameras = true;
            }

            _freeLookCursor.SetActive(!LockCameras && _mouseLook && HideHUDController.IsHUDVisible);
        }

        internal override void Init(Scp079Role role, ReferenceHub owner)
        {
            base.Init(role, owner);

            if (owner.isLocalPlayer)
            {
                CursorManager.Register(this);
                _freeLookToggleMode = PlayerPrefsSl.Get(SettingsOption.ModeSwitchSetting079.ToString(), false);
            }

            role.SubroutineModule.TryGetSubroutine(out _curCamSync);
        }

        private void OnDestroy()
        {
            CursorManager.Unregister(this);
        }
    }
}
