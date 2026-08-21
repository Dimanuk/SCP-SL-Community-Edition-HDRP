using UnityEngine;

public class ScopeShaderReplacement : MonoBehaviour
{
    private static Shader _replacementShader;

    private Camera _camera;

    private void Start()
    {
        if (_replacementShader == null)
        {
            _replacementShader = Shader.Find("Hidden/UnlitShaderReplacement");
        }

        _camera = GetComponent<Camera>();
        if (_camera != null)
        {
            _camera.SetReplacementShader(_replacementShader, "RenderType");
        }
    }

    // When the scope stops aiming the camera GameObject is deactivated, but its target
    // RenderTexture keeps the last rendered frame — leaving a frozen image on the scope lens.
    // Wipe the render target so the lens reads black once the scope is no longer rendering.
    private void OnDisable()
    {
        if (_camera == null)
            _camera = GetComponent<Camera>();

        RenderTexture rt = _camera != null ? _camera.targetTexture : null;
        if (rt == null)
            return;

        RenderTexture prev = RenderTexture.active;
        RenderTexture.active = rt;
        GL.Clear(true, true, Color.clear);
        RenderTexture.active = prev;
    }
}
