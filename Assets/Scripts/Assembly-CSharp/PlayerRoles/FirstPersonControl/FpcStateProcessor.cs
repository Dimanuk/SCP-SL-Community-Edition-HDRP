using UnityEngine;

namespace PlayerRoles.FirstPersonControl
{
    public class FpcStateProcessor
    {
        private bool _firstUpdate;

        private bool _sprintToggled;

        private readonly global::PlayerStatsSystem.StaminaStat _stat;

        private readonly ReferenceHub _hub;

        private readonly global::PlayerRoles.FirstPersonControl.FirstPersonMovementModule _mod;

        private readonly global::UnityEngine.AnimationCurve _regenerationOverTime;

        private readonly float _useRate;

        private readonly float _respawnImmunity;

        private readonly bool _toggleSprint;

        private readonly global::System.Diagnostics.Stopwatch _regenStopwatch;

        private readonly global::UnityEngine.Transform _camPivot;

        private const float MinValueToStartSprint = 0.05f;

        private const float EyeHeight = 0.088f;

        private static global::UnityEngine.KeyCode _keySprint;

        private static global::UnityEngine.KeyCode _keySneak;

        private static global::UnityEngine.KeyCode _keyCrouch;

        private static int _layerMask;

        public float CrouchPercent { get; private set; }

        public static global::UnityEngine.LayerMask Mask
        {
            get
            {
                if (_layerMask == 0)
                {
                    int layer = global::UnityEngine.LayerMask.NameToLayer("Player");
                    for (int i = 0; i < 32; i++)
                    {
                        if (!global::UnityEngine.Physics.GetIgnoreLayerCollision(layer, i))
                        {
                            _layerMask |= 1 << i;
                        }
                    }
                }
                return _layerMask;
            }
        }

        private float ServerUseRate
        {
            get
            {
                if (_hub.roleManager.CurrentRole.ActiveTime <= _respawnImmunity)
                {
                    return 0f;
                }
                float num = _useRate * _hub.inventory.StaminaUsageMultiplier;
                for (int i = 0; i < _hub.playerEffectsController.EffectsLength; i++)
                {
                    if (_hub.playerEffectsController.AllEffects[i] is global::PlayerRoles.FirstPersonControl.IStaminaModifier staminaModifier && staminaModifier.StaminaModifierActive)
                    {
                        num *= staminaModifier.StaminaUsageMultiplier;
                    }
                }
                return num;
            }
        }

        private float ServerRegenRate
        {
            get
            {
                float num = _regenerationOverTime.Evaluate((float)_regenStopwatch.Elapsed.TotalSeconds);
                for (int i = 0; i < _hub.playerEffectsController.EffectsLength; i++)
                {
                    if (_hub.playerEffectsController.AllEffects[i] is global::PlayerRoles.FirstPersonControl.IStaminaModifier staminaModifier && staminaModifier.StaminaModifierActive)
                    {
                        num *= staminaModifier.StaminaRegenMultiplier;
                    }
                }
                return num;
            }
        }

        private bool SprintingDisabled
        {
            get
            {
                for (int i = 0; i < _hub.playerEffectsController.EffectsLength; i++)
                {
                    if (_hub.playerEffectsController.AllEffects[i] is global::PlayerRoles.FirstPersonControl.IStaminaModifier staminaModifier && staminaModifier.StaminaModifierActive && staminaModifier.SprintingDisabled)
                    {
                        return true;
                    }
                }
                return _hub.inventory.SprintingDisabled;
            }
        }

        public FpcStateProcessor(ReferenceHub hub, global::PlayerRoles.FirstPersonControl.FirstPersonMovementModule module)
            : this(hub, module, 0f, 0f, null)
        {
            _useRate = (hub.IsSCP() ? 0f : global::GameCore.ConfigFile.ServerConfig.GetFloat("stamina_balance_use", 0.05f));
            _respawnImmunity = global::GameCore.ConfigFile.ServerConfig.GetFloat("stamina_balance_immunity", 3f);
            float num = global::GameCore.ConfigFile.ServerConfig.GetFloat("stamina_balance_regen_cd", 1f);
            float num2 = global::GameCore.ConfigFile.ServerConfig.GetFloat("stamina_balance_regen_speed", 1f);
            _regenerationOverTime = new global::UnityEngine.AnimationCurve(new global::UnityEngine.Keyframe(0f, 0f, 0f, 0f, 0f, 0f), new global::UnityEngine.Keyframe(num, 0f, 0f, 0f, 0f, 0f), new global::UnityEngine.Keyframe(3.11f + num, 0.126f * num2, 0.00926f, 0.00926f, 0.1068f, 0.3f));
        }

        public FpcStateProcessor(ReferenceHub hub, global::PlayerRoles.FirstPersonControl.FirstPersonMovementModule module, float useRate, float respawnImmunity, global::UnityEngine.AnimationCurve regenCurve)
        {
            _hub = hub;
            _mod = module;
            _camPivot = _hub.PlayerCameraReference.parent;
            _stat = _hub.playerStats.GetModule<global::PlayerStatsSystem.StaminaStat>();
            _toggleSprint = PlayerPrefsSl.Get("ToggleSprint", defaultValue: false);
            _firstUpdate = global::Mirror.NetworkServer.active || _hub.isLocalPlayer;
            CrouchPercent = 0f;
            if (hub.isLocalPlayer)
            {
                ReloadInputConfigs();
            }
            _useRate = useRate;
            _respawnImmunity = respawnImmunity;
            _regenerationOverTime = regenCurve;
            if (global::Mirror.NetworkServer.active)
            {
                _regenStopwatch = global::System.Diagnostics.Stopwatch.StartNew();
            }
        }

        public virtual void ClientUpdateInput(global::PlayerRoles.FirstPersonControl.FirstPersonMovementModule moduleRef, float walkSpeed, out global::PlayerRoles.FirstPersonControl.PlayerMovementState valueToSend)
        {
            bool flag = moduleRef.CurrentMovementState == global::PlayerRoles.FirstPersonControl.PlayerMovementState.Sprinting;
            bool key = global::UnityEngine.Input.GetKey(_keyCrouch);
            if (global::UnityEngine.Input.GetKey(_keySneak) || key)
            {
                _sprintToggled = false;
                valueToSend = ((!key) ? global::PlayerRoles.FirstPersonControl.PlayerMovementState.Sneaking : global::PlayerRoles.FirstPersonControl.PlayerMovementState.Crouching);
                moduleRef.CurrentMovementState = valueToSend;
                return;
            }
            if (_toggleSprint && global::UnityEngine.Input.GetKeyDown(_keySprint))
            {
                _sprintToggled = !_sprintToggled;
            }
            if (!global::UnityEngine.Input.GetKey(_keySprint) && !_sprintToggled)
            {
                valueToSend = global::PlayerRoles.FirstPersonControl.PlayerMovementState.Walking;
                moduleRef.CurrentMovementState = valueToSend;
            }
            else if (_stat.CurValue > 0f && !SprintingDisabled && (flag || _stat.CurValue > 0.05f))
            {
                bool flag2 = _mod.Motor.Velocity.SqrMagnitudeIgnoreY() < walkSpeed * walkSpeed;
                valueToSend = (flag2 ? global::PlayerRoles.FirstPersonControl.PlayerMovementState.Walking : global::PlayerRoles.FirstPersonControl.PlayerMovementState.Sprinting);
                moduleRef.CurrentMovementState = global::PlayerRoles.FirstPersonControl.PlayerMovementState.Sprinting;
            }
            else
            {
                _sprintToggled = false;
                valueToSend = global::PlayerRoles.FirstPersonControl.PlayerMovementState.Walking;
                moduleRef.CurrentMovementState = valueToSend;
            }
        }

        public virtual global::PlayerRoles.FirstPersonControl.PlayerMovementState UpdateMovementState(global::PlayerRoles.FirstPersonControl.PlayerMovementState state)
        {
            bool isCrouching = state == global::PlayerRoles.FirstPersonControl.PlayerMovementState.Crouching;
            float height = _mod.CharacterControllerSettings.Height;
            float num = height * _mod.CrouchHeightRatio;
            if (UpdateCrouching(isCrouching, num, height) || _firstUpdate)
            {
                _firstUpdate = false;
                float num2 = global::UnityEngine.Mathf.Lerp(0f, (height - num) / 2f, CrouchPercent);
                float num3 = global::UnityEngine.Mathf.Lerp(height, num, CrouchPercent);
                float radius = _mod.CharController.radius;
                _mod.CharController.height = num3;
                _mod.CharController.center = global::UnityEngine.Vector3.down * num2;
                _camPivot.localPosition = global::UnityEngine.Vector3.up * (num3 / 2f - num2 - radius + 0.088f);
            }
            if (!global::Mirror.NetworkServer.active || _useRate == 0f)
            {
                return state;
            }
            if (state == global::PlayerRoles.FirstPersonControl.PlayerMovementState.Sprinting)
            {
                if (_stat.CurValue > 0f && !SprintingDisabled)
                {
                    float value = _stat.CurValue - global::UnityEngine.Time.deltaTime * ServerUseRate;
                    _stat.CurValue = global::UnityEngine.Mathf.Clamp01(value);
                    _regenStopwatch.Restart();
                    return global::PlayerRoles.FirstPersonControl.PlayerMovementState.Sprinting;
                }
                state = global::PlayerRoles.FirstPersonControl.PlayerMovementState.Walking;
            }
            if (_stat.CurValue >= 1f)
            {
                return state;
            }
            _stat.CurValue = global::UnityEngine.Mathf.Clamp01(_stat.CurValue + ServerRegenRate * global::UnityEngine.Time.deltaTime);
            return state;
        }

        private bool UpdateCrouching(bool isCrouching, float cH, float nH)
        {
            if (CrouchPercent <= 0f && !isCrouching)
            {
                return false;
            }
            if (isCrouching && cH < nH && _mod.CrouchSpeed != 0f)
            {
                CrouchPercent = IncreasedCrouch();
            }
            else
            {
                float maxHeight = GetMaxHeight(_hub.transform.position, cH, nH);
                CrouchPercent = global::UnityEngine.Mathf.Max(DecreasedCrouch(), global::UnityEngine.Mathf.InverseLerp(nH, cH, maxHeight));
            }
            if (!global::Mirror.NetworkServer.active)
            {
                return _hub.isLocalPlayer;
            }
            return true;
        }

        private float DecreasedCrouch()
        {
            float t = global::UnityEngine.Mathf.Abs(CrouchPercent - 0.5f) * 2f;
            float num = global::UnityEngine.Mathf.Lerp(5f, 0.4f, t);
            return global::System.Math.Max(0f, CrouchPercent - global::UnityEngine.Time.deltaTime * num);
        }

        private float IncreasedCrouch()
        {
            float num = global::UnityEngine.Mathf.SmoothStep(4.5f, 0.8f, CrouchPercent);
            return global::UnityEngine.Mathf.Min(1f, CrouchPercent + global::UnityEngine.Time.deltaTime * num);
        }

        private float GetMaxHeight(global::UnityEngine.Vector3 pos, float cH, float nH)
        {
            float radius = _mod.CharacterControllerSettings.Radius;
            pos.y -= nH / 2f - radius;
            if (!global::UnityEngine.Physics.SphereCast(pos, radius, global::UnityEngine.Vector3.up, out var hitInfo, nH, Mask))
            {
                return nH;
            }
            return hitInfo.distance + radius;
        }

        private static void ReloadInputConfigs()
        {
            _keySprint = NewInput.GetKey(ActionName.Run, KeyCode.None);
            _keySneak = NewInput.GetKey(ActionName.Sneak, KeyCode.None);
            //_keyCrouch = NewInput.GetKey(ActionName.Noclip, KeyCode.None);
        }
    }
}
