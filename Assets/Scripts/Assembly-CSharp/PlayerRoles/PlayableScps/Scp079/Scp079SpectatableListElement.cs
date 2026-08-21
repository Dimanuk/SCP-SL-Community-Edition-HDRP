using System.Text;
using PlayerRoles.Spectating;
using TMPro;
using UnityEngine;

namespace PlayerRoles.PlayableScps.Scp079
{
    public class Scp079SpectatableListElement : FullSizeSpectatableListElement
    {
        [SerializeField]
        private TextMeshProUGUI _info;

        private bool _isSet;
        private string _formatTier;
        private Scp079TierManager _tierMng;
        private readonly StringBuilder _sb = new StringBuilder();

        private void Awake()
        {
            _isSet = false;
            _formatTier = Translations.Get<Scp079HudTranslation>((Scp079HudTranslation)1);
        }

        protected override void OnTargetChanged(SpectatableModuleBase prevTarget, SpectatableModuleBase newTarget)
        {
            base.OnTargetChanged(prevTarget, newTarget);

            _isSet = false;
            _tierMng = null;

            if (newTarget != null
                && newTarget.MainRole is Scp079Role mainRole
                && mainRole.SubroutineModule != null
                && mainRole.SubroutineModule.TryGetSubroutine(out _tierMng))
            {
                _isSet = true;
            }
        }

        protected override void Update()
        {
            base.Update();

            if (!_isSet)
                return;

            _sb.Clear();

            _sb.AppendFormat(_formatTier, _tierMng.AccessTierLevel);

            int nextLevelThreshold = _tierMng.NextLevelThreshold;
            if (nextLevelThreshold > 0)
            {
                _sb.Append(" (");
                _sb.Append(_tierMng.RelativeExp);
                _sb.Append("/");
                _sb.Append(nextLevelThreshold);
                _sb.Append(")");
            }

            _info.text = _sb.ToString();
        }
    }
}
