
using PlayerRoles.PlayableScps.HUDs;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

namespace PlayerRoles.PlayableScps.Scp096
{
	public class Scp096Hud : ViewmodelScpHud
    {
        [SerializeField]
        private AbilityHud _rageDuration;      

        [SerializeField]
        private AbilityHud _chargeCooldown;     

        [SerializeField]
        private Image[] _keyCircles;            

        [SerializeField]
        private Image _rageEnterSustainCircle; 

        [SerializeField]
        private GameObject _docileCircles;      

        [SerializeField]
        private ScpWarningHud _rageInfo;     

        [SerializeField]
        private PostProcessVolume _rageVolume;  

        [SerializeField]
        private float _rageVolumeDelta;        

        private Scp096Role _scp096;
        private Scp096RageCycleAbility _rageCycle;
        private Scp096RageManager _rageManager;
        private Scp096ChargeAbility _chargeAbility;

        internal override void Init(ReferenceHub hub)
        {
            base.Init(hub);

            _scp096 = hub.roleManager.CurrentRole as Scp096Role;
            if (_scp096 == null)
                return;

            _scp096.SubroutineModule.TryGetSubroutine(out _rageCycle);   
            _scp096.SubroutineModule.TryGetSubroutine(out _rageManager); 
            _scp096.SubroutineModule.TryGetSubroutine(out _chargeAbility); 

            _rageDuration.Setup(_rageManager.HudRageDuration, null);

            _chargeCooldown.Setup(_chargeAbility.Cooldown, _chargeAbility.Duration);

            _rageInfo.gameObject.SetActive(hub.isLocalPlayer);

            UpdateColorGrading(1f);
        }

        protected override void Update()
        {
            base.Update();
            UpdateRageInfo();

            UpdateColorGrading(_rageVolumeDelta * Time.deltaTime);
        }

        private void UpdateColorGrading(float maxDelta)
        {
            if (_rageVolume == null || _rageManager == null)
                return;

            float targetWeight = _rageManager.IsEnragedOrDistressed ? 1f : 0f;

            _rageVolume.weight = Mathf.MoveTowards(_rageVolume.weight, targetWeight, maxDelta);
        }

        private void UpdateRageInfo()
        {
            foreach (var circle in _keyCircles)
            {
                if (circle != null && _rageCycle != null)
                    circle.fillAmount = _rageCycle.HudEnterRageKeyProgress;
            }

            Scp096RageState rageState = _scp096.StateController.RageState;

            switch (rageState)
            {
                case Scp096RageState.Docile:
                    {
                        float hudEnterRageSustain = _rageCycle.HudEnterRageSustain;

                        _docileCircles.SetActive(hudEnterRageSustain > 0f);

                        _rageEnterSustainCircle.fillAmount = hudEnterRageSustain;

                        _rageDuration.Update(forceHidden: true);
                        _chargeCooldown.Update(forceHidden: true);

                        if (hudEnterRageSustain > 0f)
                        {
                            SetWarning(Scp096HudTranslation.EnterRageKeyInfo, ActionName.Reload, hudEnterRageSustain);
                        }
                        break;
                    }

                case Scp096RageState.Enraged:
                    _rageDuration.Update();
                    _chargeCooldown.Update();
                    _docileCircles.SetActive(false);

                    SetWarning(Scp096HudTranslation.ExitRageKeyInfo, ActionName.Reload);
                    break;

                default:
                    _rageDuration.Update(forceHidden: true);
                    _chargeCooldown.Update(forceHidden: true);
                    _docileCircles.SetActive(false);

                    _rageInfo.SetText(string.Empty);
                    break;
            }
        }

        private void SetWarning(Scp096HudTranslation key, ActionName action, float duration = 3.8f)
        {
            if (!Translations.TryGet(key, out string tr))
            {
                tr = "Hold {0} to " + key;
            }

            string formatted = string.Format(tr, $"<color=red>{new ReadableKeyCode(action)}</color>");

            _rageInfo.SetText(formatted, duration);
        }

    }

}
