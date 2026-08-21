using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PlayerRoles.PlayableScps.Scp939.Mimicry
{
	public class MimicryExternalButton : Button
	{
		[SerializeField]
		private bool _hideDescriptionOnClick;

		private static MimicryExternalButton _highlighted;

		private MimicryMenuBase _assignedMenu;

		private bool _descriptionHidden;

		[field: SerializeField]
		public Scp939HudTranslation Description { get; internal set; }

		private bool Current => MimicryMenuController.Singleton.CurrentGroup == _assignedMenu.Fader;

		private void Update()
		{
			base.targetGraphic.raycastTarget = Current;
		}

		protected override void Awake()
		{
			base.Awake();
			_assignedMenu = GetComponentInParent<MimicryMenuBase>();
			if (_hideDescriptionOnClick)
			{
				base.onClick.AddListener(delegate
				{
					_descriptionHidden = true;
				});
			}
		}

		public override void OnPointerEnter(PointerEventData eventData)
		{
			base.OnPointerEnter(eventData);
			if (Current)
			{
				_highlighted = this;
				_descriptionHidden = false;
			}
		}

		public override void OnPointerExit(PointerEventData eventData)
		{
			base.OnPointerExit(eventData);
			if (!(_highlighted != this))
			{
				_highlighted = null;
			}
		}

		public static bool TryGetHighlighted(out MimicryExternalButton val)
		{
			val = _highlighted;
			if (val != null)
			{
				return val.Current;
			}
			return false;
		}

		public static bool TryGetDescription(out Scp939HudTranslation desc)
		{
			desc = (TryGetHighlighted(out var val) ? val.Description : Scp939HudTranslation.PressKeyToLunge);
			if (desc != Scp939HudTranslation.PressKeyToLunge)
			{
				return !val._descriptionHidden;
			}
			return false;
		}
	}
}
