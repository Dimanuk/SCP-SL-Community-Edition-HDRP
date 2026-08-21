using UnityEngine;

public class BetaTextEnabler : MonoBehaviour
{
    private void Start()
    {
        gameObject.SetActive(GameCore.Version.PublicBeta);
    }
}