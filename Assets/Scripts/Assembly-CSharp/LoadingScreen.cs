using System;
using System.Collections;
using System.Collections.Generic;
using MEC;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Mirror;

public class LoadingScreen : MonoBehaviour
{
    public CanvasGroup selfGroup;
    public GameObject root;
    public Image image;
    public Image loadingCircle;
    public Image oldImage;
    public TextMeshProUGUI hints;
    public TextMeshProUGUI progress;
    public int framesToNext;
    public Sprite[] backgrounds;
    public string[] hintsText;
    public string description;

    private CoroutineHandle _checkLoadingScreen;
    private int _currentFrames;
    private int _lastProgress;

    private static bool _noFade;    
    private static bool _loaded;     
    private static bool _loadedSet;   

    private string _currentLoadedScene;

    private const string _facilitySceneName = "Assets/_Scenes/Facility.unity";

    public LoadingScreen()
    {
        framesToNext = 600;
    }

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        TranslationReader.OnTranslationsRefreshed += RefreshHints;
        hintsText = TranslationReader.GetKeys("LoadingHints");
        description = TranslationReader.Get("Connection_Info", 1, "NO_TRANSLATION");
        progress.SetText(description, true);
        Next();
    }

    private void OnDestroy()
    {
        TranslationReader.OnTranslationsRefreshed -= RefreshHints;
    }

    private void OnEnable()
    {
        loadingCircle.fillAmount = 0f;
        _lastProgress = 0;
        progress.SetText(description + " 0%", true);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _currentLoadedScene = scene.name.ToLower();

        if (_currentLoadedScene.Equals("Facility"))
        {
            _noFade = true;
            loadingCircle.fillAmount = 0f;
            _lastProgress = 0;
            _loaded = false;
            _loadedSet = false;
            root.SetActive(true);
            CheckIfStuck();
        }
        else
        {
            if (NetworkManager.singleton != null
                && !string.IsNullOrEmpty(NetworkManager.networkSceneName)
                && NetworkManager.networkSceneName.Equals(_facilitySceneName))
            {
                ResetToDefault();
            }
        }
    }


    private void CheckIfStuck()
    {
        if (NetworkClient.active || NetworkServer.active)
            return;

        if (_currentLoadedScene != null && _currentLoadedScene.Contains("menu") && root != null && root.activeSelf)
        {
            ResetToDefault();
            return;
        }

        if (_currentLoadedScene != null && _currentLoadedScene.Equals("facility", StringComparison.OrdinalIgnoreCase))
        {
            SceneManager.LoadSceneAsync("NewMainMenu", LoadSceneMode.Single);
        }
    }

    private IEnumerator<float> _CheckLoadingScreenStuck()
    {
        while (true)
        {
            yield return Timing.WaitForSeconds(3f);
            CheckIfStuck();
        }
    }

    private void FixedUpdate()
    {

        if (_checkLoadingScreen == default || !Timing.IsRunning(_checkLoadingScreen))
        {
            if (root != null && root.activeSelf)
            {
                var routine = _CheckLoadingScreenStuck();
                _checkLoadingScreen = Timing.RunCoroutine(routine);
            }
        }

        if (root != null && root.activeSelf)
        {
            if (selfGroup != null)
            {
                selfGroup.alpha = Mathf.Min(1f, selfGroup.alpha + Time.deltaTime);
            }
        }
        else
        {
            if (selfGroup != null)
                selfGroup.alpha = 0f;
            _currentFrames = 0;
            return;
        }

        if (selfGroup != null && selfGroup.alpha >= 0.01f)
        {
            _currentFrames++;
            if (_currentFrames > framesToNext)
            {
                _currentFrames = 0;
                Next();
            }
        }

        if (NetworkManager.singleton != null
            && !string.IsNullOrEmpty(NetworkManager.networkSceneName)
            && NetworkManager.networkSceneName.Equals(_facilitySceneName, StringComparison.OrdinalIgnoreCase))
        {
            if (NetworkManager.loadingSceneAsync != null)
            {
                float prog = NetworkManager.loadingSceneAsync.progress;
                if (loadingCircle != null)
                    loadingCircle.fillAmount = prog;

                int percent = (int)(prog * 100f);
                if (percent != _lastProgress && progress != null)
                {
                    progress.SetText($"{description} {percent}%", true);
                    _lastProgress = percent;
                }
            }
        }
        else
        {
            if (loadingCircle != null && loadingCircle.fillAmount > 0f)
            {
                _loaded = true;
                loadingCircle.fillAmount = 0f;
                if (progress != null)
                    progress.SetText(description + " 0%", true);
            }

            if (_loaded && !_loadedSet)
            {
                _loadedSet = true;
                if (root != null)
                    root.SetActive(false);

                if (_checkLoadingScreen != default)
                {
                    Timing.KillCoroutines(_checkLoadingScreen);
                }
            }
        }
    }

    private void Next()
    {
        if (oldImage != null && image != null)
        {
            oldImage.sprite = image.sprite;
            image.CrossFadeColor(Color.white, 0.5f, true, true);  
            image.CrossFadeColor(Color.clear, 0.5f, true, true);   
        }

        if (backgrounds != null && backgrounds.Length > 0 && image != null)
        {
            image.sprite = backgrounds[UnityEngine.Random.Range(0, backgrounds.Length)];
        }

        if (hints != null && hintsText != null && hintsText.Length > 0)
        {
            hints.SetText(hintsText[UnityEngine.Random.Range(0, hintsText.Length)], true);
        }
    }

    private void ResetToDefault()
    {
        _currentFrames = 0;
        _noFade = true;
        _loadedSet = false;
        _loaded = false;

        if (root != null)
            root.SetActive(false);

        if (_checkLoadingScreen != default)
        {
            Timing.KillCoroutines(_checkLoadingScreen);
        }
    }

    internal static void FinishedLoading()
    {
        _noFade = true;
        _loaded = true;
    }

    private void RefreshHints()
    {
        hintsText = TranslationReader.GetKeys("LoadingHints");
        description = TranslationReader.Get("Connection_Info", 1, "NO_TRANSLATION");
        if (progress != null)
            progress.SetText(description, true);
        Next();
    }
}