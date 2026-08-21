namespace InventorySystem.Items.Radio
{
    public class RadioThirdperson : global::InventorySystem.Items.Thirdperson.ThirdpersonItemBase
    {
        [global::UnityEngine.SerializeField]
        private global::UnityEngine.AnimationClip _animation;

        [global::UnityEngine.SerializeField]
        private global::VoiceChat.Playbacks.SpatializedRadioPlaybackBase _playback;

        private uint _netId;

        private sbyte _prevVal;

        private void Update()
        {
            if (!global::InventorySystem.Items.Radio.RadioMessages.SyncedRangeLevels.TryGetValue(_netId, out var value))
            {
                return;
            }
            sbyte range = (sbyte)value.Range;
            if (range != _prevVal)
            {
                if (range < 0)
                {
                    _playback.gameObject.SetActive(value: false);
                }
                else
                {
                    _playback.gameObject.SetActive(value: true);
                    _playback.RangeId = range;
                }
                _prevVal = range;
            }
        }

        internal override void Initialize(global::PlayerRoles.FirstPersonControl.Thirdperson.HumanCharacterModel model, global::InventorySystem.Items.ItemIdentifier id)
        {
            base.Initialize(model, id);
            global::InventorySystem.Items.Thirdperson.ThirdpersonItemAnimationManager.SetAnimation(model, global::InventorySystem.Items.Thirdperson.ThirdpersonItemAnimationName.Override0, _animation);
            _prevVal = sbyte.MinValue;
            _netId = model.OwnerHub.netId;
            _playback.IgnoredNetId = _netId;
        }
    }
}
