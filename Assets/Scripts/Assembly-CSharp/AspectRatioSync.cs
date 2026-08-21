using System;
using Mirror;
using UnityEngine;

public class AspectRatioSync : NetworkBehaviour
{
    private static float _defaultCameraFieldOfView;
    private int _savedWidth;
    private int _savedHeight;

    public static float YScreenEdge { get; private set; } = 35f;

    [SyncVar]
    public float XScreenEdge = 35f;

    public float XplusY { get; private set; } = 70f;
    public float AspectRatio { get; private set; } = 1f;

    public static event Action OnAspectRatioChanged;

    private void Start()
    {
        if (isLocalPlayer)
        {
            var hub = GetComponent<ReferenceHub>();
            if (hub != null && hub.PlayerCameraReference != null)
            {
                Camera component = hub.PlayerCameraReference.GetComponent<Camera>();
                _defaultCameraFieldOfView = (component == null) ? 70f : component.fieldOfView;
                YScreenEdge = _defaultCameraFieldOfView / 2f;
            }

            UpdateAspectRatio();
        }
    }

    private void FixedUpdate()
    {
        if (!isLocalPlayer) return;

        if (Screen.width != _savedWidth || Screen.height != _savedHeight)
        {
            UpdateAspectRatio();
            OnAspectRatioChanged?.Invoke();
        }
    }

    [Client]
    private void UpdateAspectRatio()
    {
        _savedWidth = Screen.width;
        _savedHeight = Screen.height;
        float aspectRatio = (float)Screen.width / Screen.height;

        CmdSetAspectRatio(aspectRatio);
    }

    [Command]
    private void CmdSetAspectRatio(float aspectRatio)
    {
        if (float.IsNaN(aspectRatio) || float.IsInfinity(aspectRatio))
            aspectRatio = 1f;

        if (aspectRatio < 1f)
            aspectRatio = 1f;
        else if (aspectRatio > 8f)
            aspectRatio = 8f;

        AspectRatio = aspectRatio;

        float halfFovRad = _defaultCameraFieldOfView * (Mathf.PI / 180f) * 0.5f;
        float tanHalfFov = Mathf.Tan(halfFovRad);

        XScreenEdge = Mathf.Atan(tanHalfFov * aspectRatio) * 57.29578f;

        XplusY = XScreenEdge + YScreenEdge;
    }
}