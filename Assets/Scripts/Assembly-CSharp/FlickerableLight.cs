using UnityEngine;

public class FlickerableLight : MonoBehaviour
{
    private Color _initialMaterialColor;
    private Color _initialLightColor;

    private static readonly int _emissionColor = Shader.PropertyToID("_EmissionColor");

    public AnimationCurve animationEnable;
    public AnimationCurve animationDisable;

    public int materialId;

    private Light lightSource;

    [SerializeField]
    private Renderer[] _renderers;

    private Material _copy;

    private int _rendererCount;

    private bool _enabled;
    private bool _flickering;

    private float _curAnimationProgress;

    internal bool WarheadLightOverride;
    internal Color WarheadColor = FlickerableLightController.DefaultWarheadColor;

    internal bool isColorChanged;
    internal bool HasLight;

    private bool _previousWarheadState;

    private void Awake()
    {
        lightSource = GetComponentInChildren<Light>();
        HasLight = lightSource != null;

        _renderers = GetComponentsInChildren<Renderer>();
        _rendererCount = _renderers.Length;

        if (_rendererCount > 0)
        {
            Renderer firstRenderer = _renderers[0];
            Material[] sharedMaterials = firstRenderer.sharedMaterials;

            if (materialId < sharedMaterials.Length)
            {
                _copy = new Material(sharedMaterials[materialId]);

                for (int i = 0; i < _rendererCount; i++)
                {
                    if (_renderers[i] != null)
                    {
                        Material[] mats = _renderers[i].sharedMaterials;
                        if (materialId < mats.Length)
                        {
                            mats[materialId] = _copy;
                            _renderers[i].sharedMaterials = mats;
                        }
                    }
                }

                _initialMaterialColor = _copy.GetColor(_emissionColor);
            }
        }

        if (HasLight)
        {
            _initialLightColor = lightSource.color;
        }
    }

    private void Update()
    {
        Color targetLight = _initialLightColor;
        Color targetMat = _initialMaterialColor;
        bool warheadActive = WarheadLightOverride || FlickerableLightController.WarheadEnabled;
        if (warheadActive)
        {
            targetLight = WarheadColor;
            targetMat = WarheadColor;
        }

        if (_flickering)
        {
            _enabled = true;
            _curAnimationProgress = Mathf.Clamp01(_curAnimationProgress + Time.deltaTime);
            float brightness = animationDisable != null ? animationDisable.Evaluate(_curAnimationProgress) : 0f;

            SetRendererColor(Color.Lerp(Color.black, targetMat, brightness));
            if (HasLight && lightSource != null)
                lightSource.color = Color.Lerp(Color.black, targetLight, brightness);
            return;
        }

        if (_enabled)
        {
            _curAnimationProgress = Mathf.Clamp01(_curAnimationProgress + Time.deltaTime);
            float brightness = animationEnable != null ? animationEnable.Evaluate(_curAnimationProgress) : 1f;

            SetRendererColor(Color.Lerp(Color.black, targetMat, brightness));
            if (HasLight && lightSource != null)
                lightSource.color = Color.Lerp(Color.black, targetLight, brightness);

            if (Mathf.Abs(_curAnimationProgress - 1f) < 0.005f)
                _enabled = false;
            return;
        }

        if (_previousWarheadState != warheadActive || (_previousWarheadState && isColorChanged))
        {
            UpdateBaseLight(targetLight, targetMat);
            isColorChanged = false;
            _previousWarheadState = warheadActive;
        }
    }

    private void UpdateBaseLight(Color targetLight, Color targetMat)
    {
        if (HasLight && lightSource != null)
        {
            lightSource.color = targetLight;
        }

        if (_rendererCount > 0 && _copy != null)
        {
            _copy.SetColor(_emissionColor, targetMat);
        }
    }

    private void SetRendererColor(Color color)
    {
        if (_rendererCount > 0 && _copy != null)
        {
            _copy.SetColor(_emissionColor, color);
        }
    }

    public void SetFlickering(bool state)
    {
        _flickering = state;
        _curAnimationProgress = 0f;
    }
}
