using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VoiceChat.Networking;

namespace PlayerRoles.PlayableScps.Scp939.Mimicry
{
	public class MimicryRecordingIcon : MonoBehaviour
	{
		[SerializeField]
		private TextMeshProUGUI _nickname;

		[SerializeField]
		private TextMeshProUGUI _rolename;

		[SerializeField]
		private Image _removeProgress;

		[SerializeField]
		private HoldableButton _removeButton;

		[SerializeField]
		private HoldableButton _previewButton;

		[SerializeField]
		private HoldableButton _useButton;

		[SerializeField]
		private Sprite _startPreviewSprite;

		[SerializeField]
		private Sprite _stopPreviewSprite;

		private MimicryRecorder _assignedRecorder;

		private MimicryRecordingsMenu _menu;

		private PlaybackBuffer _buffer;

		private int _id;

		private bool _previewing;

		private bool Interactable
		{
			get
			{
				if (base.gameObject.activeInHierarchy && _menu.HighlightedSlot == _id)
				{
					return MimicryMenuController.Singleton.CurrentGroup == _menu.Fader;
				}
				return false;
			}
		}

		public Scp939HudTranslation CurDescription
		{
			get
			{
				if (!Interactable)
				{
					return Scp939HudTranslation.PressKeyToLunge;
				}
				if (_previewing)
				{
					return Scp939HudTranslation.LocalPreviewHeader;
				}
				if (_previewButton.IsHovering)
				{
					return Scp939HudTranslation.PreviewVoiceButton;
				}
				if (_removeButton.IsHovering)
				{
					return Scp939HudTranslation.RemoveVoiceButton;
				}
				if (_useButton.IsHovering)
				{
					return Scp939HudTranslation.TransmitVoiceButton;
				}
				return Scp939HudTranslation.PressKeyToLunge;
			}
		}

		public void Setup(MimicryRecordingsMenu menu, MimicryRecorder recorder, int id)
		{
			MimicryRecorder.MimicryRecording mimicryRecording = recorder.SavedVoices[id];
			_id = id;
			_menu = menu;
			_assignedRecorder = recorder;
			_buffer = mimicryRecording.Buffer;
			if (PlayerRoleLoader.TryGetRoleTemplate<PlayerRoleBase>(mimicryRecording.Owner.Role, out var result))
			{
				_nickname.text = mimicryRecording.Owner.Nickname;
				_rolename.text = result.RoleName;
				_rolename.color = result.RoleColor;
			}
		}

		private void Update()
		{
			_removeProgress.fillAmount = _removeButton.HeldPercent;
			if (_previewing && (!Interactable || _assignedRecorder.PreviewPlayback.IsEmpty))
			{
				StopPreview();
			}
		}

		public void TogglePreview()
		{
			if (Interactable)
			{
				if (_previewing)
				{
					StopPreview();
				}
				else
				{
					StartPreview();
				}
			}
		}

		public void RemoveRecording()
		{
			_assignedRecorder.SavedVoices.RemoveAt(_id);
			_assignedRecorder.SavedVoicesModified = true;
			_assignedRecorder.PreviewPlayback.StopPreview();
		}

		public void SendRecording()
		{
			_assignedRecorder.Transmitter.SendVoice(_buffer);
		}

		private void StartPreview()
		{
			_previewing = true;
			_assignedRecorder.PreviewPlayback.StartPreview(_buffer);
			_previewButton.image.sprite = _stopPreviewSprite;
		}

		private void StopPreview()
		{
			_previewing = false;
			_assignedRecorder.PreviewPlayback.StopPreview();
			_previewButton.image.sprite = _startPreviewSprite;
		}
	}
}
