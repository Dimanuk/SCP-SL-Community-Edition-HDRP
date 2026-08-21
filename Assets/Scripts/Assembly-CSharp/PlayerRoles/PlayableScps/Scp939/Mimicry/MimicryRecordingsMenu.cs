using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerRoles.PlayableScps.Scp939.Mimicry
{
	public class MimicryRecordingsMenu : MimicryMenuBase
	{
		public enum MimicryDesc
		{
			None = 0,
			PreviewStart = 1,
			PreviewTimeline = 2,
			Remove = 3,
			Use = 4,
			Return = 20
		}

		private MimicryRecordingIcon[] _icons;

		private MimicryRecorder _recorder;

		private MimicryDesc _desc;

		[SerializeField]
		private MimicryRecordingIcon _iconTemplate;

		[SerializeField]
		private GameObject _previewRoot;

		[SerializeField]
		private TextMeshProUGUI _previewElapsed;

		[SerializeField]
		private TextMeshProUGUI _previewDuration;

		[SerializeField]
		private Slider _previewProgress;

		private bool HighlightValid
		{
			get
			{
				if (base.HighlightedSlot >= 0)
				{
					return base.HighlightedSlot < Slots;
				}
				return false;
			}
		}

		public override int Slots => _recorder.SavedVoices.Count;

		protected override void OnSlotsNumberChanged(int prev, int cur)
		{
			base.OnSlotsNumberChanged(prev, cur);
			int slots = Slots;
			for (int i = 0; i < _recorder.MaxRecordings; i++)
			{
				MimicryRecordingIcon mimicryRecordingIcon = _icons[i];
				if (i < slots)
				{
					mimicryRecordingIcon.gameObject.SetActive(value: true);
					mimicryRecordingIcon.Setup(this, _recorder, i);
					mimicryRecordingIcon.transform.localPosition = GetSlotPosition(i);
				}
				else
				{
					mimicryRecordingIcon.gameObject.SetActive(value: false);
				}
			}
			_recorder.SavedVoicesModified = false;
		}

		protected override void Setup(Scp939Role role)
		{
			base.Setup(role);
			role.SubroutineModule.TryGetSubroutine<MimicryRecorder>(out _recorder);
			int maxRecordings = _recorder.MaxRecordings;
			_icons = new MimicryRecordingIcon[maxRecordings];
			for (int i = 0; i < maxRecordings; i++)
			{
				_icons[i] = Object.Instantiate(_iconTemplate, _iconTemplate.transform.parent);
			}
		}

		protected override void Update()
		{
			base.Update();
			int slots = Slots;
			if (_recorder.SavedVoicesModified)
			{
				OnSlotsNumberChanged(slots, slots);
			}
			if (MimicryExternalButton.TryGetDescription(out var desc))
			{
				UpdateDescription(desc);
			}
			else if (HighlightValid)
			{
				UpdateDescription(_icons[base.HighlightedSlot].CurDescription);
			}
			else
			{
				UpdateDescription(Scp939HudTranslation.PressKeyToLunge);
			}
			if (_desc == MimicryDesc.PreviewTimeline && HighlightValid)
			{
				_recorder.PreviewPlayback.UpdateProgress(_previewProgress, _previewElapsed, _previewDuration);
			}
		}

		protected override void OnDescriptionUpdated(Scp939HudTranslation newDesc)
		{
			_desc = (MimicryDesc)newDesc;
			switch (_desc)
			{
			case MimicryDesc.None:
				_previewRoot.SetActive(value: false);
				base.DescriptionText = string.Empty;
				break;
			case MimicryDesc.PreviewTimeline:
				_previewRoot.SetActive(value: true);
				base.DescriptionText = string.Empty;
				break;
			default:
				_previewRoot.SetActive(value: false);
				base.OnDescriptionUpdated(newDesc);
				break;
			}
		}
	}
}
