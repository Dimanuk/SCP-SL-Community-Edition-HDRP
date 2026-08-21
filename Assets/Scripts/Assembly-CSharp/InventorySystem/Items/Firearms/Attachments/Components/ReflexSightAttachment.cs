using Mirror;
using System;
using UnityEngine;

namespace InventorySystem.Items.Firearms.Attachments.Components
{
    public class ReflexSightAttachment : SerializableAttachment, ICustomizableAttachment
    {
        public static readonly float[] Sizes = { 0.5f, 0.6f, 0.7f, 0.8f, 0.9f, 1.0f, 1.1f, 1.2f, 1.3f };
        public static readonly Color[] Colors;

        public Action OnValuesChanged;
        public ReflexSightReticlePack TextureOptions;

        [SerializeField]
        private int _defaultColorId;

        [SerializeField]
        private int _defaultReticle;

        [SerializeField]
        private Vector2 _configIconOffset;

        [SerializeField]
        private float _configIconSize;

        [SerializeField]
        private AttachmentConfigWindow _configWindow;

        private bool _wasEverActive;

        public int CurTexture { get; private set; }
        public int CurSize { get; private set; }
        public int CurColor { get; private set; }

        public Vector2 ConfigIconOffset => _configIconOffset;
        public AttachmentConfigWindow ConfigWindow => _configWindow;
        public float ConfigIconScale => _configIconSize;

        public void SetValues(int texture, int color, int size)
        {
            int maxTexture = (TextureOptions != null && TextureOptions.Reticles != null)
                ? TextureOptions.Reticles.Length - 1
                : 0;

            CurTexture = Mathf.Clamp(texture, 0, maxTexture);
            CurColor = Mathf.Clamp(color, 0, Colors.Length - 1);
            CurSize = Mathf.Clamp(size, 0, Sizes.Length - 1);
            OnValuesChanged?.Invoke();
        }

        public void SaveValues()
        {
            if (!TryGetParentFirearm(out var firearm))
                return;

            int preset = AttachmentPreferences.GetPreset(firearm.ItemTypeId);

            // Always store under preset 0; a non-default preset gets its own copy too.
            for (int i = 0; ; i++)
            {
                int presetKey = i * preset;

                PlayerPrefsSl.Set(GetPrefsKey(firearm, presetKey, "Texture"), CurTexture);
                PlayerPrefsSl.Set(GetPrefsKey(firearm, presetKey, "Color"), CurColor);
                PlayerPrefsSl.Set(GetPrefsKey(firearm, presetKey, "Size"), CurSize);

                if (preset == 0 || i >= 1)
                    break;
            }

            SendNewSettings();
        }

        internal void SetTexture(int i)
        {
            SetValues(i, CurColor, CurSize);
            SaveValues();
        }

        internal void SetColor(int i)
        {
            SetValues(CurTexture, i, CurSize);
            SaveValues();
        }

        internal void ChangeSize(int i)
        {
            SetValues(CurTexture, CurColor, CurSize + i);
            SaveValues();
        }

        private void Awake()
        {
            AttachmentSelectorBase.OnPresetLoaded += LoadFromPreset;
            AttachmentSelectorBase.OnPresetSaved += SaveValues;
            AttachmentSelectorBase.OnAttachmentsReset += SetDefaults;
        }

        private void Update()
        {
            if (!_wasEverActive && IsEnabled)
            {
                LoadFirstTime();
                _wasEverActive = true;
            }
        }

        private void OnDestroy()
        {
            AttachmentSelectorBase.OnPresetLoaded -= LoadFromPreset;
            AttachmentSelectorBase.OnPresetSaved -= SaveValues;
            AttachmentSelectorBase.OnAttachmentsReset -= SetDefaults;
        }

        private string GetPrefsKey(Firearm parent, int preset, string setting)
        {
            return string.Format("ReflexSight_{0}_{1}_{2}_{3}", preset, parent.ItemTypeId, AttachmentId, setting);
        }

        private void LoadFirstTime()
        {
            if (!TryGetParentFirearm(out var firearm))
                return;

            // Simulated instances (workstation/spectator previews) skip the
            // ownership gate and the network database — they always mirror
            // the local player's saved preferences.
            if (!firearm.SimulatedInstanceMode)
            {
                if (!firearm.IsLocalPlayer && !firearm.IsSpectated)
                    return;

                if (ReflexSightDatabase.Database.TryGetValue(firearm.ItemSerial, out var dict)
                    && dict.TryGetValue(AttachmentId, out var msg))
                {
                    SetValues(msg.TextureId, msg.ColorId, msg.SizeId);
                    return;
                }

                if (!firearm.IsLocalPlayer)
                {
                    SetDefaults();
                    return;
                }
            }

            // Saved reticle prefs only apply if the weapon still carries the
            // attachment loadout they were saved with.
            uint currentCode = firearm.GetCurrentAttachmentsCode();
            uint savedPref = PlayerPrefsSl.Get(
                AttachmentPreferences.PreferencesPath(firearm.ItemTypeId),
                firearm.ValidateAttachmentsCode(0));

            if (currentCode == savedPref)
            {
                LoadFromPrefs();
                SendNewSettings();
            }
            else
            {
                SetDefaults();
            }
        }

        private void LoadFromPrefs()
        {
            if (!TryGetParentFirearm(out var firearm))
                return;

            int preset = AttachmentPreferences.GetPreset(firearm.ItemTypeId);

            int texture = PlayerPrefsSl.Get(GetPrefsKey(firearm, preset, "Texture"), _defaultReticle);
            int color = PlayerPrefsSl.Get(GetPrefsKey(firearm, preset, "Color"), _defaultColorId);
            int size = PlayerPrefsSl.Get(GetPrefsKey(firearm, preset, "Size"), 4);

            SetValues(texture, color, size);
        }

        private void LoadFromPreset()
        {
            if (!TryGetParentFirearm(out var firearm))
                return;

            int preset = AttachmentPreferences.GetPreset(firearm.ItemTypeId);
            if (preset == 0)
            {
                if (_wasEverActive)
                    SaveValues();
                return;
            }

            LoadFromPrefs();
            SendNewSettings();
        }

        private void SetDefaults()
        {
            SetValues(_defaultReticle, _defaultColorId, 4);
            SendNewSettings();
        }

        private bool TryGetDatabaseEntry(Firearm fa, out ReflexSightSyncMessage msg)
        {
            msg = default;
            if (fa.SimulatedInstanceMode != false)
                return false;

            if (ReflexSightDatabase.Database.TryGetValue(fa.ItemSerial, out var dict))
            {
                if (dict.TryGetValue(AttachmentId, out msg))
                    return true;
            }
            return false;
        }

        private void SendNewSettings()
        {
            if (!TryGetParentFirearm(out var firearm))
                return;

            if (!firearm.SimulatedInstanceMode && firearm.IsLocalPlayer)
            {
                var msg = new ReflexSightSyncMessage
                {
                    AttachmentId = AttachmentId,
                    TextureId = CurTexture,
                    ColorId = CurColor,
                    SizeId = CurSize,
                    WeaponSerial = firearm.ItemSerial
                };
                NetworkClient.Send(msg);
            }
        }

        static ReflexSightAttachment()
        {
            // Alpha stays 1: the HoloSight shader treats _RedDotColor.a as brightness.
            Colors = new Color[8]
            {
                new Color(1f, 0f, 0f),      // red
                new Color(0f, 0.3f, 1f),    // blue
                new Color(0f, 0.9f, 1f),    // cyan
                new Color(0f, 1f, 0f),      // green
                new Color(1f, 1f, 0f),      // yellow
                new Color(1f, 0.4f, 0f),    // orange
                new Color(1f, 0.3f, 0.8f),  // pink
                new Color(0.6f, 0f, 1f),    // purple
            };
        }
    }
}