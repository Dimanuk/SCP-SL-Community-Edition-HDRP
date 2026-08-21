using System;
using PlayerRoles;
using UnityEngine;
using UnityEngine.UI;

public class StartScreen : MonoBehaviour
{
    [Serializable]
    private struct FadeElement
    {
        public AnimationCurve FadeCurve;
        public CanvasRenderer Renderer;
    }

    private static StartScreen _singleton;

    [SerializeField]
    private int _helpScreenIndex;

    [SerializeField]
    private FadeElement[] _fadeElements;

    [SerializeField]
    private AnimationCurve _fadeLimitNormal;

    [SerializeField]
    private AnimationCurve _fadeLimitFast;

    [SerializeField]
    private AnimationCurve _scaleAnim;

    [SerializeField]
    private AudioSource _bell;

    [SerializeField]
    private Text _roleNameText;

    [SerializeField]
    private Text _roleDescText;

    [SerializeField]
    private Transform _scaler;

    private float _elapsed;
    private float _length;
    private bool _hasHelpMenu;
    private AnimationCurve _limiter;

    public static void Show(PlayerRoleBase prb)
    {
        if (_singleton == null)
        {
            throw new System.NullReferenceException("StartScreen singleton is not initialized. Make sure the StartScreen GameObject is present in the scene and Awake has run.");
        }
        _singleton.Play(prb);
    }

    private void Awake()
    {
        _singleton = this;
        base.gameObject.SetActive(false);
    }

    private void Play(PlayerRoleBase prb)
    {
        bool useFastFade = false;
        _limiter = useFastFade ? _fadeLimitFast : _fadeLimitNormal;

        _length = _limiter.length > 0 ? _limiter.keys[_limiter.length - 1].time : 3f;

        _roleDescText.text = TranslationReader.Get("Class_Descriptions", (int)prb.RoleTypeId, prb.RoleName);
        _roleNameText.text = prb.RoleName;
        _roleNameText.color = prb.RoleColor;

        _hasHelpMenu = prb.RoleHelpInfo != null;

        base.gameObject.SetActive(true);

        if (!_bell.isPlaying)
        {
            _bell.Play();
        }
        _bell.timeSamples = 0;

        _elapsed = 0f;
        Update();
    }

    private void Update()
    {
        float scaleValue = _scaleAnim.Evaluate(_elapsed);
        _scaler.localScale = new Vector3(scaleValue, scaleValue, scaleValue);

        float limitAlpha = _limiter.Evaluate(_elapsed);

        for (int i = 0; i < _fadeElements.Length; i++)
        {
            FadeElement element = _fadeElements[i];
            float alpha = element.FadeCurve.Evaluate(_elapsed);

            if (!_hasHelpMenu && i == _helpScreenIndex)
            {
                alpha = 0f;
            }

            alpha = Mathf.Min(alpha, limitAlpha);

            element.Renderer.SetAlpha(alpha);
        }

        if (_elapsed >= _length)
        {
            base.gameObject.SetActive(false);
            return;
        }

        _elapsed += Time.deltaTime;
    }
}