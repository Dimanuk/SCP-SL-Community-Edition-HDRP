using CustomPlayerEffects;
using SCPE;
using UnityEngine.Rendering.PostProcessing;

public class TiltShiftWave : PostProcessEffectWave
{
    private TiltShift _tiltShift;

    protected override float EffectValue
    {
        get => _tiltShift != null ? _tiltShift.amount.value : 0f;
        set
        {
            if (_tiltShift != null)
                _tiltShift.amount.value = value;
        }
    }

    protected override void SetEffectType(PostProcessProfile profile)
    {
        _tiltShift = profile?.GetSetting<TiltShift>();
    }
}