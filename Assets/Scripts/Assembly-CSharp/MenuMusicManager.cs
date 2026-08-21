using UnityEngine;
public class MenuMusicManager : MonoBehaviour
{
    [Header("Fading")]
    public float curState;
    [Tooltip("Скорость плавного изменения громкости (единиц в секунду).")]
    public float lerpSpeed = 1f;
    public bool creditsChanged;

    [Header("Audio & UI")]
    public AudioSource mainSource;
    public AudioSource creditsSource;
    public GameObject creditsHolder;

    public MenuMusicManager()
    {
        lerpSpeed = 1f;
    }

    private void Update()
    {
        if (creditsHolder == null || creditsSource == null || mainSource == null)
            return;

        bool isCreditsActive = creditsHolder.activeSelf;
        float targetVolume = isCreditsActive ? 1f : 0f;

        curState = Mathf.Lerp(curState, targetVolume, lerpSpeed * Time.deltaTime);
        
        creditsSource.volume = curState;

        mainSource.mute = isCreditsActive;

        if (creditsChanged != isCreditsActive)
        {
            creditsChanged = isCreditsActive;
            if (isCreditsActive)
            {
                creditsSource.Play();
            }
        }
    }
}