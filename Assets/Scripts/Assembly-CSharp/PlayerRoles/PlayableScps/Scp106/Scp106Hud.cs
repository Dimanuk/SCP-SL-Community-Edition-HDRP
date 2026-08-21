using System;
using System.Diagnostics;
using PlayerRoles.PlayableScps.HUDs;
using PlayerRoles.PlayableScps.Subroutines;
using PostProcessing;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

namespace PlayerRoles.PlayableScps.Scp106
{
    public class Scp106Hud : ScpHudBase
    {
        private readonly Stopwatch _vigorFlashSw = new Stopwatch();
        private readonly Stopwatch _cooldownFlashSw = new Stopwatch();

        [SerializeField]
        private AbilityHud _sinkholeCooldown;     
        [SerializeField]
        private Graphic _cooldownFlasher;          
        [SerializeField]
        private PostProcessVolume _vignetteVolume; 
        [SerializeField]
        private float _maxVignette;                
        [SerializeField]
        private Slider _vigorSlider;              
        [SerializeField]
        private Text _vigorPercent;                 
        [SerializeField]
        private AbilityHud _attackCooldownElement; 
        [SerializeField]
        private Color _normalColor;                
        [SerializeField]
        private float _flashSpeed;                 
        [SerializeField]
        private float _flashDuration;              
        [SerializeField]
        private Graphic[] _vigorGraphics;         
        [SerializeField]
        private GameObject _diedRoot;             

        private Scp106Role _role;                   
        private Scp106MovementModule _fpc;         
        private Scp106Vigor _vigor;              
        private Scp106SinkholeController _sinkholeController;
        private Vignette _vignette;              
        private ScreenDissolve _dissolve;         
        private float _vigorIdleTime;               
        private float _cooldownIdleTime;            

        private readonly AbilityCooldown _attackCooldown = new AbilityCooldown(); 
        private static Scp106Hud _singleton;        
        private static bool _singletonSet;          

        private static float CurTime => Time.timeSinceLevelLoad;

        private void LateUpdate()
        {
            if (_vignette != null && _vignette.intensity != null && _fpc != null)
                _vignette.intensity.value = _fpc.CurSlowdown * _maxVignette;

            if (_vigor != null)
            {
                if (_vigorSlider != null)
                    _vigorSlider.value = _vigor.DisplayedVigor;

                if (_vigorPercent != null)
                {
                    int percent = Mathf.FloorToInt(_vigor.VigorAmount * 100f);
                    _vigorPercent.text = percent + "%";
                }
            }

            if (_sinkholeCooldown != null)
                _sinkholeCooldown.Update(false);

            if (_attackCooldownElement != null)
                _attackCooldownElement.Update(false);

            if (_cooldownFlasher != null)
            {
                Color white = Color.white;
                UpdateFlash(_cooldownFlasher, _cooldownFlashSw, white, ref _cooldownIdleTime);
            }

            if (_vigorGraphics != null)
            {
                foreach (Graphic g in _vigorGraphics)
                    UpdateFlash(g, _vigorFlashSw, _normalColor, ref _vigorIdleTime);
            }
        }

        private void UpdateFlash(Graphic targetGraphic, Stopwatch sw, Color normalColor, ref float idleTime)
        {
            if (sw == null || targetGraphic == null)
                return;

            if (sw.IsRunning)
            {
                if (sw.Elapsed.TotalSeconds < _flashDuration)
                {
                    float t = Mathf.Sin((CurTime - idleTime) * _flashSpeed * Mathf.PI);
                    Color flashColor = Color.Lerp(normalColor, Color.red, Mathf.Abs(t));
                    targetGraphic.color = new Color(flashColor.r, flashColor.g, flashColor.b, targetGraphic.color.a);
                    return;
                }
            }

            Color current = targetGraphic.color;
            Color lerped = Color.Lerp(current, normalColor, Time.deltaTime * _flashSpeed);
            targetGraphic.color = new Color(lerped.r, lerped.g, lerped.b, current.a);

            idleTime = CurTime;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (this == _singleton)
            {
                _singletonSet = false;
            }
        }

        internal override void OnDied()
        {
            base.enabled = false;

            if (_diedRoot != null)
                _diedRoot.SetActive(false);
        }

        internal override void Init(ReferenceHub hub)
        {
            base.Init(hub);

            if (hub?.roleManager == null)
                return;

            _role = hub.roleManager.CurrentRole as Scp106Role;

            if (_role == null)
                return;

            _fpc = _role.FpcModule as Scp106MovementModule;

            if (_role.SubroutineModule != null)
                _role.SubroutineModule.TryGetSubroutine(out _vigor);

            if (_role.SubroutineModule != null)
                _role.SubroutineModule.TryGetSubroutine(out _sinkholeController);

            if (_vignetteVolume != null)
            {
                PostProcessProfile profile = _vignetteVolume.profile;
                if (profile != null)
                {
                    _vignette = profile.GetSetting<Vignette>();
                    _dissolve = profile.GetSetting<ScreenDissolve>();
                }
            }

            if (_attackCooldownElement != null)
                _attackCooldownElement.Setup(_attackCooldown, null);

            if (_sinkholeCooldown != null && _sinkholeController != null)
                _sinkholeCooldown.Setup(_sinkholeController.Cooldown, null);

            _singleton = this;
            _singletonSet = true;
        }

        public static void PlayCooldownAnimation(double nextTime)
        {
            if (!_singletonSet)
                return;

            if (_singleton._attackCooldown != null)
            {
                float cooldown = (float)(nextTime - Mirror.NetworkTime.time);
                _singleton._attackCooldown.Trigger(cooldown);
            }
        }

        public static void PlayFlash(bool vigor)
        {
            if (!_singletonSet)
                return;
            Stopwatch sw = vigor ? _singleton._vigorFlashSw : _singleton._cooldownFlashSw;
            if (sw != null)
                sw.Restart();
        }

        public static void SetDissolveAnimation(float amt)
        {
            if (!_singletonSet)
                return;

            if (_singleton._dissolve != null && _singleton._dissolve.DissolveAmount != null)
            {
                _singleton._dissolve.DissolveAmount.value = Mathf.Clamp01(amt);
            }
        }
    }
}
