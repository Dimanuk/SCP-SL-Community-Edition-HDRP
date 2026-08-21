using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScpInterfaces : MonoBehaviour
{
    public static ScpInterfaces singleton;

    public TextMeshProUGUI Scp049_decayedPopup;
    public Image Scp049_loading;
    public Image Scp049_cooldown;

    public TextMeshProUGUI remainingTargets;

    public Image Scp173EyeIndicator;
    public Image Scp173TantrumCooldown;
    public Image Scp173BreakneckCooldown;
    public TextMeshProUGUI Scp173BlinkTimer;

    public GameObject Scp106_eq;
    public GameObject Scp049_eq;
    public GameObject Scp096_eq;
    public GameObject Scp173InterfaceObj;

    public static int remTargs;
    private void Awake()
    {
        singleton = this;
    }

    public void DecayedPopup()
    {
        StopAllCoroutines();
    
        if (Scp049_decayedPopup != null)
            Scp049_decayedPopup.gameObject.SetActive(true);
        
        StartCoroutine(HideDecayedPopup());
    }
    private IEnumerator HideDecayedPopup()
    {
        yield return new WaitForSeconds(4f);
        
        if (Scp049_decayedPopup != null)
            Scp049_decayedPopup.gameObject.SetActive(false);
    }
}