using UnityEngine;
using UnityEngine.Video;

public class VideoRepairer : MonoBehaviour
{
    private void Start()
    {
        Invoke(nameof(Repair), 5f);
    }

    private void Repair()
    {
        VideoPlayer component = GetComponent<VideoPlayer>();

        if (component != null)
        {
            component.targetMaterialProperty = "_EmissionMap";
        }
    }
}