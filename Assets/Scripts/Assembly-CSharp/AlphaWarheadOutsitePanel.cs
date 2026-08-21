using Mirror;
using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class AlphaWarheadOutsitePanel : NetworkBehaviour
{
	public Animator panelButtonCoverAnim;

	public static AlphaWarheadNukesitePanel nukeside;

	private float _refreshTimer;

	public Text[] display;

	private bool _loaded;

    private static string _textDisabled = "<size=180><color=red>DISABLED</color></size>";
    private static string _textWait = "<color=red><size=200>PLEASE WAIT</size></color>";
    private static string _textReady = "<color=lime><size=180>READY</size></color>";

    public GameObject[] inevitable;

	[SyncVar]
	public bool keycardEntered;

    private static readonly int _enabled = Animator.StringToHash("enabled");

    private void Update()
    {
        if (!_loaded)
        {
            _textDisabled = TranslationReader.Get("Nuke", 0, _textDisabled);
            _textWait = TranslationReader.Get("Nuke", 1, _textWait);
            _textReady = TranslationReader.Get("Nuke", 2, _textReady);
            _loaded = true;
        }

        base.transform.localPosition = new Vector3(0f, 0f, 9f);

        bool inProgress = AlphaWarheadController.InProgress;

        if (inProgress || _refreshTimer == 0f)
        {
            string timeString = GetTimeString();
            foreach (Text txt in display)
            {
                txt.text = timeString;
            }
        }

        _refreshTimer += Time.deltaTime;
        if (_refreshTimer > 0.35f)
        {
            _refreshTimer = 0f;
        }

        float timeUntilDetonation = AlphaWarheadController.TimeUntilDetonation;
        foreach (GameObject go in inevitable)
        {
            go.SetActive(inProgress && timeUntilDetonation <= 10f && timeUntilDetonation > 0f);
        }

        panelButtonCoverAnim.SetBool(_enabled, keycardEntered);
    }
    private static string GetTimeString()
    {
        bool inProgress = AlphaWarheadController.InProgress;

        if (inProgress)
        {
            float time = AlphaWarheadController.TimeUntilDetonation;

            if (time > 0f)
            {
                TimeSpan ts = TimeSpan.FromSeconds(time);
                string minutes = ts.Minutes.ToString("00").Substring(0, 2);
                string seconds = ts.Seconds.ToString("00").Substring(0, 2);
                string miliseconds = (ts.Milliseconds / 10).ToString("00").Substring(0, 2);

                return $"<color=orange><size=270>{minutes}:{seconds}.{miliseconds}</size></color>";
            }

            if ((int)(Time.realtimeSinceStartup * 4f) % 2 == 0)
            {
                return string.Empty;
            }
            return "<color=orange><size=270>00:00.00</size></color>";
        }

        if (nukeside != null && nukeside.nukenabled)
        {
            return AlphaWarheadController.InProgress ? _textWait : _textReady;
        }

        return _textDisabled;
    }
}
