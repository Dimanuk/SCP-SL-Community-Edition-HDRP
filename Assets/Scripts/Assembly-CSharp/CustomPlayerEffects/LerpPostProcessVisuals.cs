using UnityEngine.Rendering.PostProcessing;

namespace CustomPlayerEffects
{
    public class LerpPostProcessVisuals : LerpVisualsBase
    {
        protected PostProcessVolume ProcessVolume { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            ProcessVolume = GetComponent<PostProcessVolume>();
        }

        protected override void OnWeightChanged(float weight)
        {
            if (ProcessVolume != null)
                ProcessVolume.weight = weight;
        }

        protected override void OnActivated()
        {
            if (ProcessVolume != null)
                ProcessVolume.enabled = true;
        }

        protected override void OnShutdown()
        {
            if (ProcessVolume != null)
                ProcessVolume.enabled = false;
        }
    }
}
