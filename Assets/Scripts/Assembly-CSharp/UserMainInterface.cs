using Mirror;
using Mirror.LiteNetLib4Mirror;
using PostProcessing;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UserMainInterface : MonoBehaviour
{
    public Text playerlistText;
    public Text voiceInfo;
    public TextMeshProUGUI hintText;

    [Space]
    [FormerlySerializedAs("fps")]
    public Text Ping;

    public GameObject noclipUI;
    public Text noclipIndicator;
    public Text fogIndicator;
    public Text noclipHelpText;

    public static UserMainInterface Singleton;

    public float LerpSpeed = 4f;

    [Header("Searching")]
    public GameObject searchObject;
    public Image searchRadial;

    [Header("Popup")]
    public CanvasGroup imageDisplayGroup;
    public Image imageToDisplay;

    public GameObject mouseGameObject;
    public GameObject PlyStats;

    private void Awake()
    {
        Singleton = this;
    }

    private void Start()
    {
        if (playerlistText != null)
        {
            KeyCode playerListKey = NewInput.GetKey(ActionName.PlayerList, KeyCode.None);
            playerlistText.text = $"PRESS <b>{playerListKey}</b> TO OPEN THE PLAYER LIST";
        }

        if (voiceInfo != null)
        {
            KeyCode voiceKey = NewInput.GetKey(ActionName.VoiceChat, KeyCode.None);
            voiceInfo.text = $"Hold <b>{voiceKey}</b> to speak";
        }
    }

    public void SetNoclipIndicator(bool state, float speed)
    {
        if (noclipUI != null)
        {
            noclipUI.SetActive(state);
        }

        if (!state) return;

        if (noclipIndicator != null)
        {
            noclipIndicator.text = $"Noclip speed: {speed:0.##} m/s";
        }

        if (fogIndicator != null)
        {
            bool fogActive = FogController.Singleton != null && FogController.Singleton.gameObject.activeSelf;
            fogIndicator.text = fogActive ? "Fog Enabled" : "Fog Disabled";
        }

        if (noclipHelpText != null)
        {
            KeyCode toggleKey = NewInput.GetKey(ActionName.Noclip, KeyCode.None);
            KeyCode fogKey = NewInput.GetKey(ActionName.NoClipFogToggle, KeyCode.None);

            noclipHelpText.text =
                $"Toggle Noclip: <b>{toggleKey}</b>\n" +
                $"Change Speed: <b>Mouse Wheel</b>\n" +
                $"Toggle Fog: <b>{fogKey}</b>";
        }
    }

    public void UpdateFogIndicator()
    {
        if (fogIndicator == null) return;

        bool fogActive = FogController.Singleton != null && FogController.Singleton.gameObject.activeSelf;
        fogIndicator.text = fogActive ? "Fog Enabled" : "Fog Disabled";
    }

    private void Update()
    {
        if (Ping != null && Ping.gameObject.activeSelf)
        {
            if (NetworkClient.activeHost)
            {
                Ping.text = "0 ms";
            }
            else if (LiteNetLib4MirrorTransport.Singleton != null && LiteNetLib4MirrorClient.IsConnected())
            {
                int currentPing = LiteNetLib4MirrorClient.GetPing() * 2;
                Ping.text = $"{currentPing} ms";
            }
            else
            {
                Ping.text = "N/A";
            }
        }
    }
}