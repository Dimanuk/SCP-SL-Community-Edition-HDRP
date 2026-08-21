using System.Collections.Generic;

using PlayerRoles.PlayableScps.Subroutines;
using RadialMenus;
using UnityEngine;

namespace PlayerRoles.PlayableScps.Scp939.Mimicry
{
	public class EnvMimicryMenu : MimicryMenuBase
	{
        private readonly global::System.Collections.Generic.HashSet<global::PlayerRoles.RoleTypeId> _everAliveRoles = new global::System.Collections.Generic.HashSet<global::PlayerRoles.RoleTypeId>();

        private readonly global::System.Collections.Generic.List<global::UnityEngine.GameObject> _instances = new global::System.Collections.Generic.List<global::UnityEngine.GameObject>();

        private int[] _correspondingIds;

		private EnvironmentalMimicry _subroutine;

		private int _curCatIndex;

		private bool _showCooldown;

		private string _rawDesc;

		[SerializeField]
		private RectTransform _iconTemplate;
        private global::PlayerRoles.PlayableScps.Scp939.Mimicry.EnvironmentalMimicry.EnvMimicryCategory CurCategory => _subroutine.Categories[_curCatIndex];

        public override int Slots
        {
            get
            {
                global::PlayerRoles.PlayableScps.Scp939.Mimicry.EnvMimicryOption[] options = CurCategory.Options;
                int num = options.Length;
                int num2 = 0;
                for (int i = 0; i < num; i++)
                {
                    if (CheckOption(options[i]))
                    {
                        num2++;
                    }
                }
                return num2;
            }
        }

        protected override void OnSlotsNumberChanged(int prev, int cur)
        {
            base.OnSlotsNumberChanged(prev, cur);
            _instances.ForEach(delegate (global::UnityEngine.GameObject x)
            {
                global::UnityEngine.Object.Destroy(x);
            });
            _instances.Clear();
            if (_correspondingIds == null || _correspondingIds.Length < cur)
            {
                _correspondingIds = new int[cur];
            }
            int num = 0;
            for (int num2 = 0; num2 < cur; num2++)
            {
                global::UnityEngine.RectTransform rectTransform = global::UnityEngine.Object.Instantiate(_iconTemplate, _iconTemplate.parent);
                rectTransform.localPosition = GetSlotPosition(num2);
                global::PlayerRoles.PlayableScps.Scp939.Mimicry.EnvMimicryOption envMimicryOption;
                do
                {
                    envMimicryOption = CurCategory.Options[num++];
                }
                while (!CheckOption(envMimicryOption));
                _correspondingIds[num2] = num - 1;
                rectTransform.GetComponent<global::UnityEngine.UI.RawImage>().texture = envMimicryOption.MainIcon;
                rectTransform.GetChild(0).GetComponent<global::UnityEngine.UI.RawImage>().texture = envMimicryOption.CategoryIcon;
                global::UnityEngine.GameObject gameObject = rectTransform.gameObject;
                gameObject.SetActive(value: true);
                _instances.Add(gameObject);
            }
        }

        protected override void OnSelected()
        {
            base.OnSelected();
            if (base.HighlightedSlot >= 0)
            {
                _subroutine.ClientPlay(_curCatIndex, _correspondingIds[base.HighlightedSlot]);
            }
        }

        protected override void Update()
        {
            base.Update();
            if (global::PlayerRoles.PlayableScps.Scp939.Mimicry.MimicryExternalButton.TryGetDescription(out var desc))
            {
                UpdateDescription(desc);
            }
            else
            {
                UpdateDescription(CurCategory.Name);
            }
            if (_showCooldown)
            {
                base.DescriptionText = _rawDesc + "\n" + _subroutine.CooldownText;
            }
        }

        protected override void OnDescriptionUpdated(global::PlayerRoles.PlayableScps.Scp939.Scp939HudTranslation newDesc)
        {
            base.OnDescriptionUpdated(newDesc);
            _rawDesc = base.DescriptionText;
            _showCooldown = CurCategory.Name == newDesc;
        }

        protected override void Setup(global::PlayerRoles.PlayableScps.Scp939.Scp939Role role)
        {
            base.Setup(role);
            role.SubroutineModule.TryGetSubroutine<global::PlayerRoles.PlayableScps.Scp939.Mimicry.EnvironmentalMimicry>(out _subroutine);
            _subroutine.OnSoundPlayed += CloseMenu;
            global::PlayerRoles.PlayerRoleManager.OnRoleChanged += OnRoleChanged;
            global::Utils.NonAllocLINQ.HashsetExtensions.ForEach(ReferenceHub.AllHubs, delegate (ReferenceHub x)
            {
                _everAliveRoles.Add(x.GetRoleId());
            });
        }

        private void OnDestroy()
        {
            if(_subroutine != null) _subroutine.OnSoundPlayed -= CloseMenu;
            global::PlayerRoles.PlayerRoleManager.OnRoleChanged -= OnRoleChanged;
        }

        private void CloseMenu()
        {
            global::PlayerRoles.PlayableScps.Scp939.Mimicry.MimicryMenuController.Singleton.CurrentGroup = null;
        }

        private bool CheckOption(global::PlayerRoles.PlayableScps.Scp939.Mimicry.EnvMimicryOption mo)
        {
            if (mo.RoleCondition != global::PlayerRoles.RoleTypeId.None)
            {
                return _everAliveRoles.Contains(mo.RoleCondition);
            }
            return true;
        }

        private void OnRoleChanged(ReferenceHub hub, global::PlayerRoles.PlayerRoleBase prevRole, global::PlayerRoles.PlayerRoleBase newRole)
        {
            _everAliveRoles.Add(newRole.RoleTypeId);
        }

        public void NextCategory()
        {
            _curCatIndex = ++_curCatIndex % _subroutine.Categories.Length;
        }

        public void PrevCategory()
        {
            if (--_curCatIndex < 0)
            {
                _curCatIndex += _subroutine.Categories.Length;
            }
        }
    }
}
