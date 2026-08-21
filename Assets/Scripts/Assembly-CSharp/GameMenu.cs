using MEC;
using Mirror;
using NorthwoodLib;
using System;
using System.Collections.Generic;
using TMPro;
using ToggleableMenus;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameMenu : SimpleToggleableMenu
{
    public static GameMenu singleton;

    public GameObject[] minors;
    public Graphic[] colorableElements;
    public TextMeshProUGUI infoText;
    public GameObject hideHUDText;

    public bool _pastebinDisplayed;

    public override bool CanToggle => true;

    public override bool LockMovement
    {
        get
        {
            GameObject currentSelectedGameObject = EventSystem.current?.currentSelectedGameObject;
            if (currentSelectedGameObject != null && currentSelectedGameObject.TryGetComponent<TMP_InputField>(out var component))
            {
                return component.isFocused;
            }
            return false;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        singleton = this;

        if (hideHUDText != null)
        {
            var tmp = hideHUDText.GetComponent<TextMeshProUGUI>();
            if (tmp != null)
                tmp.text = "Warning: HUD is hidden\n" + $"Press <b>{NewInput.GetKey(ActionName.HideGUI)}</b> to enable HUD";
        }
    }

    private IEnumerator<float> _ShowServerInfo(string id)
    {
        if (!id.Contains("/"))
        {
            UnityWebRequest www = UnityWebRequest.Get("https://pastebin.com/raw/" + id);
            yield return Timing.WaitUntilDone(www.SendWebRequest());

            string error = www.error;
            string text = string.IsNullOrEmpty(error) ? www.downloadHandler.text : error;

            if (StringUtils.Contains(text, "<title>Pastebin.com - Locked Paste</title>", System.StringComparison.Ordinal))
            {
                infoText.text = "The provided paste is locked via password and cannot be displayed. Please contact the server owner.";
            }
            else
            {
                if (text.Length > 0x1388)
                {
                    text = StringUtils.TruncateToLast(text, 0x1388, '\n') + "...<<i><color=#87CEFA><u><link=\"https://pastebin.com/" + id + "\">(Click here for full content)</link></u></color></i>";
                }

                infoText.text = text;
                _pastebinDisplayed = true;
            }

            www.Dispose();
        }
        else
        {
            infoText.text = "The URL isn't directing to pastebin site. Please contact server owner.";
        }
    }

    protected override void OnToggled()
    {
        base.OnToggled();

        if (minors != null)
        {
            foreach (GameObject gameObject in minors)
            {
                if (gameObject != null && gameObject.activeSelf)
                    gameObject.SetActive(false);
            }
        }
    }

    public void SelectMinor(int id)
    {
        if (minors == null || id < 0 || id >= minors.Length) return;

        foreach (GameObject gameObject in minors)
        {
            if (gameObject != null)
                gameObject.SetActive(false);
        }

        minors[id].SetActive(true);

        if (id == 2 && !_pastebinDisplayed && ReferenceHub.TryGetHostHub(out var hub))
        {
            Timing.RunCoroutine(_ShowServerInfo(hub.characterClassManager?.Pastebin ?? string.Empty), Segment.FixedUpdate);
        }
    }

    public void Disconnect()
    {
        if (NetworkServer.active)
            NetworkManager.singleton?.StopHost();
        else
            NetworkManager.singleton?.StopClient();
    }

    public void Exit()
    {
        IsEnabled = false;
    }
}