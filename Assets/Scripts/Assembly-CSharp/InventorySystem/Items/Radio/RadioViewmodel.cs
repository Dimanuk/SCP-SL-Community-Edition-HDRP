using System;
using CameraShaking;
using InventorySystem.Items.SwayControllers;
using Mirror;
using PlayerRoles.Voice;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using VoiceChat.Playbacks;
using static UnityEngine.UI.GridLayoutGroup;

namespace InventorySystem.Items.Radio
{
    public class RadioViewmodel : AnimatedViewmodelBase
    {
        [SerializeField]
        private GameObject _panelNoBattery;

        [SerializeField]
        private GameObject _panelMain;

        [SerializeField]
        private GameObject _panelRoot;

        [SerializeField]
        private TextMeshProUGUI _textModeShort;

        [SerializeField]
        private TextMeshProUGUI _textModeFull;

        [SerializeField]
        private TextMeshProUGUI _textBatteryLevel;

        [SerializeField]
        private TextMeshProUGUI _textVolume;

        [SerializeField]
        private TextMeshProUGUI _textTime;

        [SerializeField]
        private GameObject _txOn;

        [SerializeField]
        private GameObject _txOff;

        [SerializeField]
        private GameObject _rxOn;

        [SerializeField]
        private GameObject _rxOff;

        [SerializeField]
        private RawImage _rangeIndicator;

        [SerializeField]
        private RawImage _noBatteryIndicator;

        [SerializeField]
        private Image[] _batteryLevels;

        [SerializeField]
        private AudioMixer _voicechatMixer;

        [SerializeField]
        private AudioSource _audioSource;

        [SerializeField]
        private AudioClip _clipTurnOn;

        [SerializeField]
        private AudioClip _clipTurnOff;

        [SerializeField]
        private AudioClip _clipCircleRange;

        [SerializeField]
        private Transform _cameraTrackerSource;

        [SerializeField]
        private Vector3 _cameraTrackerOffset;

        [SerializeField]
        private float _cameraTrackerIntensity;

        [SerializeField]
        private Transform _swayPivot;

        [SerializeField]
        private MeshRenderer _radioRenderer;

        [SerializeField]
        private Color _keypadEnabledColor;

        [SerializeField]
        private Color _keypadDisabledColor;

        private static readonly int IsTransmittingHash;

        private const string RadioChannelName = "AudioSettings_VoiceChat";

        private const string KeypadEmissionChannelName = "_EmissionColor";

        private const float BatteryFlashRate = 2.5f;

        private static Material _keypadMat;

        private GoopSway _goopSway;

        private float _batteryFlashTimer;

        private int _prevRange = -1;

        public override IItemSwayController SwayController => _goopSway;

        public override float ViewmodelCameraFOV => 60f;

        static RadioViewmodel()
        {
            IsTransmittingHash = Animator.StringToHash("IsTransmitting");
        }

        public override void InitSpectator(ReferenceHub ply, ItemIdentifier id, bool wasEquipped)
        {
            base.InitSpectator(ply, id, wasEquipped);
            UpdateNetwork();
        }

        internal override void OnEquipped()
        {
            base.OnEquipped();

            CameraShakeController.AddEffect(new TrackerShake(_cameraTrackerSource, Quaternion.Euler(_cameraTrackerOffset), _cameraTrackerIntensity));

            if (_panelRoot != null)
                RefreshKeypadColor(_panelRoot.activeSelf);
        }

        private void Start()
        {
            // _cameraTrackerIntensity tunes the TrackerShake camera effect and is a
            // different unit scale entirely - reusing it here as both sway rotation AND
            // position-translation intensity (with every other GoopSway parameter left at
            // its 1f default, instead of the small tuned values every other handheld
            // viewmodel uses) made the radio lurch downward hard on movement and ease back
            // slowly. Use the same tuned constants as StandardAnimatedViemodel's
            // GetNewSwayController, which every other non-firearm handheld item relies on.
            var settings = new GoopSway.GoopSwaySettings(
                targetTransform: _swayPivot,
                swayIntensity: 0.65f,
                translationIntensity: 0.0035f,
                zAxisIntensity: 0.04f,
                swaySmoothness: 7f,
                translationSmoothness: 6.5f,
                bobIntensity: 0.03f,
                centrifugalIntensity: 1.6f
            );

            _goopSway = new GoopSway(settings, Hub);

            if (_keypadMat == null && _radioRenderer != null)
            {
                _keypadMat = _radioRenderer.material;
            }
        }

        private void Update()
        {
            if (_panelMain != null && _panelMain.activeSelf && _panelRoot != null && _panelRoot.activeSelf)
            {
                RefreshVolumeAndTimeDisplay();

                GetTxRx(out bool tx, out bool rx);
                _txOn.SetActive(tx);
                _txOff.SetActive(!tx);
                _rxOn.SetActive(rx);
                _rxOff.SetActive(!rx);

                ViewmodelAnimator.SetBool(IsTransmittingHash, tx);
            }
            else if (_panelNoBattery != null && _panelNoBattery.activeSelf)
            {
                _batteryFlashTimer += Time.deltaTime;
                if (_batteryFlashTimer > 1f / BatteryFlashRate)
                {
                    _batteryFlashTimer = 0f;
                    _noBatteryIndicator.enabled = !_noBatteryIndicator.enabled;
                }
            }

            UpdateNetwork();
        }

        private void RefreshVolumeAndTimeDisplay()
        {
            int dbAbs = 0;
            if (_voicechatMixer.GetFloat(RadioChannelName, out float db))
                dbAbs = Mathf.Abs(Mathf.RoundToInt(db));

            if (_textVolume != null)
                _textVolume.text = "-" + dbAbs;

            if (_textTime != null)
                _textTime.text = DateTime.Now.ToString("HH:mm:ss");
        }

        private void UpdateNetwork()
        {
            if (!RadioMessages.SyncedRangeLevels.TryGetValue(Hub.netId, out RadioStatusMessage value))
                return;

            SetBattery(value.Battery);

            if (value.Battery == 0)
                return;

            if (value.Range == RadioMessages.RadioRangeLevel.RadioDisabled)
            {
                SetState(false);
                return;
            }

            if (_panelRoot != null && !_panelRoot.activeSelf)
            {
                RefreshKeypadColor(true);
                _panelRoot.SetActive(true);
                _audioSource.PlayOneShot(_clipTurnOn);
            }

            SetRange((int)value.Range);
        }

        private void GetTxRx(out bool tx, out bool rx)
        {
            tx = false;
            rx = false;

            foreach (ReferenceHub hub in ReferenceHub.AllHubs)
            {
                if (hub.roleManager.CurrentRole is not IVoiceRole)
                    continue;

                if (!PersonalRadioPlayback.IsTransmitting(hub))
                    continue;

                if (hub == Hub)
                {
                    tx = true;
                    continue;
                }

                if (hub.roleManager.CurrentRole is IVoiceRole { VoiceModule: IRadioVoiceModule radioModule }
                    && !radioModule.RadioPlayback.Source.mute)
                {
                    rx = true;
                }
            }
        }

        private void SetBattery(byte percent)
        {
            if (_panelMain != null)
                _panelMain.SetActive(percent != 0);
            if (_panelNoBattery != null)
                _panelNoBattery.SetActive(percent == 0);

            if (percent == 0)
            {
                ViewmodelAnimator.SetBool(IsTransmittingHash, false);
                return;
            }

            if (_textBatteryLevel != null)
                _textBatteryLevel.text = percent + "%";

            int filledCount = Mathf.FloorToInt(percent * _batteryLevels.Length / 100f);
            for (int i = 0; i < _batteryLevels.Length; i++)
            {
                _batteryLevels[i].enabled = filledCount >= i + 1;
            }
        }

        private void SetRange(int rangeId)
        {
            bool changed = _prevRange != rangeId;

            if (changed && gameObject.activeInHierarchy)
                _audioSource.PlayOneShot(_clipCircleRange);

            if (changed)
                _prevRange = rangeId;

            if (!global::InventorySystem.InventoryItemLoader.TryGetItem<RadioItem>(ItemType.Radio, out RadioItem radioCache) || radioCache.Ranges == null)
                return;

            int clampedIndex = Mathf.Clamp(rangeId, 0, radioCache.Ranges.Length - 1);
            if (clampedIndex < 0 || clampedIndex >= radioCache.Ranges.Length)
                return;

            RadioRangeMode range = radioCache.Ranges[clampedIndex];

            if (_textModeShort != null)
                _textModeShort.text = range.ShortName;
            if (_textModeFull != null)
                _textModeFull.text = range.FullName;
            if (_rangeIndicator != null)
                _rangeIndicator.texture = range.SignalTexture;
        }

        private void SetState(bool state)
        {
            if (_panelRoot == null || _panelRoot.activeSelf == state)
                return;

            RefreshKeypadColor(state);
            _panelRoot.SetActive(state);
            _audioSource.PlayOneShot(state ? _clipTurnOn : _clipTurnOff);

            if (!state)
                ViewmodelAnimator.SetBool(IsTransmittingHash, false);
        }

        private void RefreshKeypadColor(bool state)
        {
            if (_keypadMat == null && _radioRenderer != null)
                _keypadMat = _radioRenderer.material;

            if (_keypadMat != null)
                _keypadMat.SetColor(KeypadEmissionChannelName, state ? _keypadEnabledColor : _keypadDisabledColor);
        }
    }
}
