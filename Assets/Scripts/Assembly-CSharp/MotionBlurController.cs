public class MotionBlurController : global::UnityEngine.MonoBehaviour
{
    private int f;

    private float t;

    private bool b;

    private global::UnityEngine.Rendering.PostProcessing.PostProcessVolume _overrideVolume;

    private global::UnityEngine.Rendering.PostProcessing.MotionBlur _motionBlur;

    private void Start()
    {
        if (PlayerPrefsSl.Get(SettingsOption.gfxsets_mb.ToString(), 0) == 0)
        {
            global::UnityEngine.Object.Destroy(this);
            return;
        }
        _motionBlur = new global::UnityEngine.Rendering.PostProcessing.MotionBlur();
        _motionBlur.enabled.Override(x: false);
    }

    private void Update()
    {
        t += global::UnityEngine.Time.deltaTime;
        f++;
        if (t > 1f)
        {
            t -= 1f;
            if ((b && f < 30) || (!b && f > 50))
            {
                b = !b;
                Change();
            }
            f = 0;
        }
    }

    private void Change()
    {
        if (b && _overrideVolume != null)
        {
            global::UnityEngine.Rendering.PostProcessing.RuntimeUtilities.DestroyVolume(_overrideVolume, destroyProfile: true, destroyGameObject: true);
        }
        else if (!b)
        {
            _overrideVolume = global::UnityEngine.Rendering.PostProcessing.PostProcessManager.instance.QuickVolume(23, 100f, _motionBlur);
        }
    }
}
