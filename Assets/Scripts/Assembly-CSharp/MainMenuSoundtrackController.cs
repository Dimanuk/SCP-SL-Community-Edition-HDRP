using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuSoundtrackController : MonoBehaviour
{
    public enum MenuSoundtrackState
    {
        MenuJustLoaded = 0,
        MenuLoop = 1,
        MenuIntensive = 2,
        Muted = 3,
        Retro = 4,
        PregameLobby = 5
    }

    public AudioSource ToposThemeSource;
    public AudioClip IntroClip;
    public AudioClip ToposClip;
    public AudioClip[] AltTracks;
    public float ToposMetric;
    public float ToposLength;
    public AudioSource IntenseThemeSource;
    public float IntenseDropTime;
    public int IntenseSequencesToDrop;
    public float IntenseOldToposFadeoffTime;
    public AudioSource RetroThemeSource;
    public MenuSoundtrackState SoundtrackState;
    public AnimationCurve FadeoffAnimationCurve;
    public AudioSource LobbyThemeSource;

    private float fadeoffAnim;
    private CustomNetworkManager cnm;

    public static bool DontPlayIntensive;

    public bool DebugMode;
    public bool DebugTrigger;
    public MenuSoundtrackState DebugOverride;

    public MainMenuSoundtrackController()
    {
        ToposLength = 5.1f;
    }

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        cnm = GetComponentInParent<CustomNetworkManager>();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

        if (!scene.name.Contains("Menu"))
        {
            DontPlayIntensive = true;
            SoundtrackState = MenuSoundtrackState.Muted;
            return;
        }


        int menuTheme = PlayerPrefsSl.Get("MenuTheme", 0);

        if (menuTheme > 0)
        {
            NewMainMenu newMainMenu = FindFirstObjectByType<NewMainMenu>();
            if (newMainMenu != null)
            {
                SoundtrackState = MenuSoundtrackState.MenuLoop;
                DontPlayIntensive = true;

                ToposThemeSource.Stop();
                ToposThemeSource.clip = AltTracks[menuTheme - 1];
                ToposThemeSource.loop = true;
                ToposThemeSource.volume = 1f;
                ToposThemeSource.Play();
                ToposThemeSource.time = 0f;
                return;
            }
        }

        SoundtrackState = MenuSoundtrackState.MenuJustLoaded;
        ToposThemeSource.Stop();
        ToposThemeSource.volume = 1f;

        if (IntenseThemeSource != null)
            IntenseThemeSource.volume = 0f;

        ToposThemeSource.loop = false;
        ToposThemeSource.clip = IntroClip;
        ToposThemeSource.PlayOneShot(IntroClip);
    }

    private void LateUpdate()
    {
        if (DebugMode && DebugTrigger)
        {
            SoundtrackState = DebugOverride;
            DebugTrigger = false;
        }

        if (ToposThemeSource != null && ToposThemeSource.clip == ToposClip)
        {
            if (ToposThemeSource.time >= ToposLength)
            {
                ToposMetric += Time.deltaTime * ToposThemeSource.pitch;
            }
            else
            {
                ToposMetric = ToposThemeSource.time;
            }

            if (ToposMetric > ToposLength)
                ToposMetric -= ToposLength;
        }

        if (SoundtrackState == MenuSoundtrackState.MenuLoop
            && !DebugMode
            && !DontPlayIntensive
            && cnm != null
            && cnm.ShouldPlayIntensive())
        {
            SoundtrackState = MenuSoundtrackState.MenuIntensive;
        }

        if (SoundtrackState == MenuSoundtrackState.MenuJustLoaded)
        {
            if (ToposThemeSource != null && !ToposThemeSource.isPlaying)
            {
                ToposThemeSource.Stop();
                ToposThemeSource.clip = ToposClip;
                ToposThemeSource.loop = true;
                ToposThemeSource.Play();
                ToposThemeSource.time = 0f;
                ToposMetric = 0f;
                SoundtrackState = MenuSoundtrackState.MenuLoop;
            }
        }

        if (SoundtrackState == MenuSoundtrackState.MenuIntensive)
        {
            if (!DebugMode && cnm != null && !cnm.ShouldPlayIntensive())
            {
                SoundtrackState = MenuSoundtrackState.MenuLoop;
            }

            if (IntenseThemeSource != null)
            {
                IntenseThemeSource.volume += Time.deltaTime * 0.5f;

                if (IntenseThemeSource.time > IntenseOldToposFadeoffTime)
                {
                    if (ToposThemeSource != null)
                        ToposThemeSource.volume -= Time.deltaTime * 0.5f;
                }

                if (IntenseThemeSource.volume > 0f)
                {
                    float sequences = (ToposMetric > ToposLength * 0.5f) ? 2f : 1f;
                    float t = IntenseDropTime
                              - sequences * ToposLength
                              + ToposMetric
                              - IntenseSequencesToDrop * ToposLength;
                    IntenseThemeSource.time = t;
                }
            }

            if (ToposThemeSource != null && ToposThemeSource.volume <= 0f)
            {
                if (IntenseThemeSource != null)
                {
                    float num = IntenseThemeSource.time - IntenseDropTime;
                    if (ToposClip != null)
                    {
                        while (num > ToposClip.length)
                            num -= ToposClip.length;
                        while (num < 0f)
                            num += ToposClip.length;
                    }
                    ToposThemeSource.time = num;
                }
            }
        }
        else if (SoundtrackState == MenuSoundtrackState.Retro)
        {
            if (RetroThemeSource != null && !RetroThemeSource.isPlaying)
            {
                if (ToposThemeSource != null) ToposThemeSource.Stop();
                if (IntenseThemeSource != null) IntenseThemeSource.Stop();
                RetroThemeSource.Play();
            }
        }
        else
        {
            if (IntenseThemeSource != null)
            {
                IntenseThemeSource.volume -= Time.deltaTime / 6f;
            }

            if (SoundtrackState == MenuSoundtrackState.Muted
                || SoundtrackState == MenuSoundtrackState.PregameLobby)
            {
                fadeoffAnim += Time.deltaTime;
                if (IntenseThemeSource != null
                    && IntenseThemeSource.volume <= 0f
                    && fadeoffAnim > 1f)
                {
                    if (FadeoffAnimationCurve != null && ToposThemeSource != null)
                    {
                        ToposThemeSource.volume = FadeoffAnimationCurve.Evaluate(fadeoffAnim);
                    }
                }
            }
            else
            {
                if (ToposThemeSource != null)
                    ToposThemeSource.volume += Time.deltaTime * 0.5f;

            }

            if (SoundtrackState == MenuSoundtrackState.PregameLobby)
            {
                if (IntenseThemeSource != null && IntenseThemeSource.volume <= 0.01f)
                {
                    if (LobbyThemeSource != null && !LobbyThemeSource.isPlaying)
                        LobbyThemeSource.Play();

                    if (LobbyThemeSource != null && ToposThemeSource != null)
                        LobbyThemeSource.volume = 1f - ToposThemeSource.volume;
                }
            }

            if (LobbyThemeSource != null)
            {
                LobbyThemeSource.volume = Mathf.Max(0f, LobbyThemeSource.volume - Time.deltaTime * 0.1f);
                if (LobbyThemeSource.volume == 0f)
                    LobbyThemeSource.Stop();
            }
        }

        if (SoundtrackState == MenuSoundtrackState.PregameLobby)
        {
            if (ReferenceHub.TryGetLocalHub(out var hub) && hub.networkIdentity != null && hub.networkIdentity.isLocalPlayer)
                SoundtrackState = MenuSoundtrackState.Muted;
        }

        if (SoundtrackState != MenuSoundtrackState.Muted
            && SoundtrackState != MenuSoundtrackState.PregameLobby)
        {
            fadeoffAnim = 0f;
        }

        if (SoundtrackState != MenuSoundtrackState.Retro)
        {
            if (RetroThemeSource != null)
                RetroThemeSource.Stop();
        }
    }
}