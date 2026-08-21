using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using NorthwoodLib;
using MEC;
using System;
using System.Collections.Generic;

public class CrashDetector : MonoBehaviour
{
    public static CrashDetector singleton;

    public GameObject image;
    public Button button;
    public Text text;

    private void Awake()
    {
        if (image == null)
        {
            Destroy(this);
            return;
        }

        singleton = this;

        string gpu = SystemInfo.graphicsDeviceName;
        if (StringUtils.Contains(gpu, "INTEL", StringComparison.OrdinalIgnoreCase))
        {
            if (!PlayerPrefsSl.Get("intel_warned", false))
            {
                PlayerPrefsSl.Set("intel_warned", true);
                Timing.RunCoroutine(_IShow());
                gameObject.SetActive(true);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public bool Show()
    {
        string gpu = SystemInfo.graphicsDeviceName;
        if (StringUtils.Contains(gpu, "INTEL", StringComparison.OrdinalIgnoreCase))
        {
            if (!PlayerPrefsSl.Get("intel_warned", false))
            {
                PlayerPrefsSl.Set("intel_warned", true);
                Timing.RunCoroutine(_IShow());
                return true;
            }
        }
        return false;
    }

    private IEnumerator<float> _IShow()
    {
        string okText = TranslationReader.Get("MenuWarning", 1, "OKAY");
        gameObject.SetActive(true);
        image.SetActive(true);
        button.interactable = false;

        int countdown = 15;
        while (countdown > 0)
        {
            text.text = string.Format("{0} ({1})", okText, countdown);
            yield return Timing.WaitForSeconds(1f);
            countdown--;
        }

        text.text = okText;
        button.interactable = true;
    }
}