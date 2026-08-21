using System.Runtime.InteropServices;

using InventorySystem.Items.Pickups;
using Mirror;
using Scp914;
using UnityEngine;
using VoiceChat.Playbacks;

namespace InventorySystem.Items.Radio
{
	public class RadioPickup : CollisionDetectionPickup, IUpgradeTrigger
	{
		[SyncVar]
		public bool SavedEnabled;

		[SyncVar]
		public byte SavedRange;

		public float SavedBattery;

		private static RadioItem _radioCache;

		private static bool _radioCacheSet;

		[SerializeField]
		private Material _enabledMat;

		[SerializeField]
		private Material _disabledMat;

		[SerializeField]
		private Renderer _targetRenderer;

		[SerializeField]
		private GameObject _activeObject;

		[SerializeField]
		private SpatializedRadioPlaybackBase _playback;

		private bool _prevEnabled;

		private void Update()
		{
            _playback.RangeId = SavedRange;
			if (_prevEnabled != SavedEnabled)
			{
                _prevEnabled = SavedEnabled;
                _activeObject.SetActive(SavedEnabled);
                _targetRenderer.material = SavedEnabled ? _enabledMat : _disabledMat;
			}
		}

        protected override void Awake()
        {
            base.Awake();
            if (!_radioCacheSet)
            {
                _radioCacheSet = global::InventorySystem.InventoryItemLoader.TryGetItem<global::InventorySystem.Items.Radio.RadioItem>(ItemType.Radio, out _radioCache);
            }
        }

        private void LateUpdate()
        {
            if (global::Mirror.NetworkServer.active && SavedEnabled && _radioCacheSet)
            {
                float num = _radioCache.Ranges[SavedRange].MinuteCostWhenIdle / 60f;
                float num2 = SavedBattery - global::UnityEngine.Time.deltaTime * num / 100f;
                if (num2 <= 0f)
                {
                    SavedEnabled = false;
                    num2 = 0f;
                }
                SavedBattery = num2;
            }
        }

        public void ServerOnUpgraded(Scp914KnobSetting setting)
		{
			SavedBattery = 1f;
		}
	}
}
