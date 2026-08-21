using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class ChangeSettingsOnOutside : MonoBehaviour
{
    public bool changeClearFlags = true;
    public bool renderSkyboxOverFogInside;

    private Camera _myCamera;
    private PostProcessLayer _myPostProcessLayer;

    private void Start()
    {
        if (Outside.Singleton == null) return;
        Outside.OnSetOutside += Singleton_OnSetOutside;

        _myCamera = GetComponent <Camera>();
        _myPostProcessLayer = GetComponent <PostProcessLayer>();

        Singleton_OnSetOutside(Outside.Singleton.LocalHostIsOutside);
    }

    private void Update()
    {
        if (ReferenceHub.LocalHub != null)
        {
            Transform root = transform.root;
            if (root.TryGetComponent <ReferenceHub>(out var hub) && hub.isLocalPlayer)
                return;
        }

        Destroy(this);
    }

    private void OnDestroy()
    {
        if (!enabled)
        {
            if (Outside.Singleton != null)
                Outside.OnSetOutside -= Singleton_OnSetOutside;
        }
    }

    private void Singleton_OnSetOutside(bool isOutside)
    {
        if (_myCamera == null)
        {
            Destroy(this);
            return;
        }

        if (_myPostProcessLayer != null)
        {
            var fog = _myPostProcessLayer.fog;
            if (fog != null)
                fog.excludeSkybox = isOutside || renderSkyboxOverFogInside;
        }

        if (changeClearFlags)
        {
            _myCamera.clearFlags = isOutside
                ? CameraClearFlags.Skybox
                : CameraClearFlags.SolidColor;
        }

        enabled = false;
    }
}