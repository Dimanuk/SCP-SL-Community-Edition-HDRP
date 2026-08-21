using TMPro;
using UnityEngine;

public class AuthStatus : MonoBehaviour
{
    public GameObject LoadingCircle;
    public TextMeshProUGUI Description;
    public NewServerBrowser ServerBrowser;

    private AuthStatusType _lastStatus;

    private void Awake()
    {
        Description.SetText(TranslationReader.Get("NewMainMenu", 68, "Connecting to central servers..."), true);
    }

    private void Update()
    {
        AuthStatusType authStatusType = CentralAuthManager.AuthStatusType;
        if (_lastStatus == authStatusType)
            return;

        _lastStatus = authStatusType;

        switch (authStatusType)
        {
            case (AuthStatusType)1:
                LoadingCircle.SetActive(false);
                Description.SetText(TranslationReader.Get("NewMainMenu", 69, "<color=green>Connection established</color>"), true);
                ServerBrowser.AuthCompleted();
                break;
            case (AuthStatusType)2:
                LoadingCircle.SetActive(false);
                Description.SetText(TranslationReader.Get("NewMainMenu", 72, "<color=red>Discord authentication failure (Check console for details)</color>"), true);
                break;
            case (AuthStatusType)3:
                LoadingCircle.SetActive(true);
                Description.SetText(TranslationReader.Get("NewMainMenu", 70, "<color=red>Connection failure (Check console for details)</color>"), true);
                break;
            default:
                LoadingCircle.SetActive(true);
                break;
        }
    }
}