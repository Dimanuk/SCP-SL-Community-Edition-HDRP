using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;
using TMPro;
using UnityEngine;
using NorthwoodLib;
using GameCore;

public class ReportServer : MonoBehaviour
{
    private static int _reportedServersAmount;
    private static Stopwatch ReportWatch;
    private static HashSet<string> ReportedServers;
    private static Regex Re;
    private static bool _waitingForResponse;
    private static float _responseTimer;
    private static CentralResponse _response;
    private static string _responseMessage;

    public GameObject root;
    public TextMeshProUGUI ipAddress;
    public TextMeshProUGUI warningText;
    public GameObject description;
    public GameObject form;
    public GameObject report;
    public GameObject confirmation;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (confirmation != null) confirmation.SetActive(false);
            if (description != null) description.SetActive(true);
            if (report != null) report.SetActive(true);
            if (form != null) form.SetActive(false);
            if (root != null) root.SetActive(false);
            if (form != null)
            {
                var inputs = form.GetComponentsInChildren<TMP_InputField>();
                foreach (var input in inputs)
                    input.text = "";
            }
        }

        if (_waitingForResponse)
        {
            if (_response == CentralResponse.Success)
            {
                _waitingForResponse = false;
                if (confirmation != null) confirmation.SetActive(true);
            }
            else if (_response == CentralResponse.Error)
            {
                _waitingForResponse = false;
                if (warningText != null)
                    warningText.text = _responseMessage ?? "An error occurred.";
            }
            else if (_response == CentralResponse.None)
            {
                _responseTimer += Time.deltaTime;
                if (_responseTimer >= 5f)
                {
                    _waitingForResponse = false;
                    if (warningText != null)
                        warningText.text = "Request timed out. Please try again later.";
                }
            }
        }
    }

    public void Show(string ip)
    {
        if (ipAddress != null)
            ipAddress.text = ip;
        if (root != null) root.SetActive(true);
        if (report != null) report.SetActive(true);
    }

    public void Close()
    {
        if (confirmation != null) confirmation.SetActive(false);
        if (description != null) description.SetActive(true);
        if (report != null) report.SetActive(true);
        if (form != null) form.SetActive(false);
        if (root != null) root.SetActive(false);
        if (form != null)
        {
            var inputs = form.GetComponentsInChildren<TMP_InputField>();
            foreach (var input in inputs)
                input.text = "";
        }
    }

    private void ToggleForm(bool value)
    {
        if (description != null) description.SetActive(!value);
        if (report != null) report.SetActive(!value);
        if (form != null) form.SetActive(value);
    }

    public void Proceed()
    {
        if (description != null) description.SetActive(false);
        if (report != null) report.SetActive(false);
        if (form != null) form.SetActive(true);

        if (!CentralAuthManager.Authenticated == true)
        {
            if (warningText != null)
                warningText.text = "You are not authenticated!\nYou can't report servers.";
            return;
        }

        if (warningText != null)
            warningText.text = "";
    }

    public void Submit()
    {
        if (!(CentralAuthManager.Authenticated == false))
        {
            ShowWarning("You are not authenticated!\nYou can't report servers.");
            return;
        }

        var inputs = form?.GetComponentsInChildren<TMP_InputField>();
        if (inputs == null || inputs.Length < 2) return;

        string violations = inputs[0].text;
        string explanation = inputs[1].text;
        string serverIp = ipAddress?.text ?? "";

        ValidateReport(serverIp, violations, explanation);
    }

    private void ShowWarning(string message)
    {
        if (warningText != null)
            warningText.text = message;
    }

    private void ValidateReport(string serverIp, string violations, string explanation)
    {
        if (ReportedServers?.Contains(serverIp) == true)
        {
            ShowWarning("You already reported this server.");
            return;
        }

        if (string.IsNullOrWhiteSpace(violations) || string.IsNullOrWhiteSpace(explanation))
        {
            ShowWarning("Please fill out all fields.");
            return;
        }

        if (explanation.Length < 15)
        {
            ShowWarning("Explanation cannot be shorter than 15 characters.");
            return;
        }
        if (explanation.Length > 750)
        {
            ShowWarning("Explanation cannot be longer than 750 characters.");
            return;
        }

        if (Re != null && !Re.IsMatch(violations))
        {
            ShowWarning("Violations field may only contain valid VSR rule numbers.\nYou can separate numbers using commas.\nYou can mention up to 4 VSR rules in one report.");
            return;
        }

        if (ReportWatch != null && ReportWatch.IsRunning)
        {
            if (ReportWatch.Elapsed.TotalHours < 24 && _reportedServersAmount >= 3)
            {
                ShowWarning("Reached max allowed server reports.");
                return;
            }
            else if (ReportWatch.Elapsed.TotalHours >= 24)
            {
                _reportedServersAmount = 0;
                ReportWatch.Restart();
            }
        }
        else
        {
            ReportWatch?.Start();
        }

        _reportedServersAmount++;
        _responseTimer = 0f;
        _response = CentralResponse.None;
        _waitingForResponse = true;

        var closure = new __c__DisplayClass23_0
        {
            serverIp = serverIp,
            violations = violations,
            explanation = explanation
        };
        ThreadStart threadStart = closure.Execute;
        Thread thread = new Thread(threadStart) { IsBackground = true, Name = $"Reporting server - {serverIp}" };
        thread.Start();
    }

    private static void SubmitReport(ref string serverIp, ref string violations, ref string explanation)
    {
        string data = "violations=" + StringUtils.Base64Encode(violations) +
                      "&explanation=" + StringUtils.Base64Encode(explanation) +
                      "&serverIp=" + StringUtils.Base64Encode(serverIp) +
                      "&reporterToken=" + CentralAuthManager.ApiToken;

        string url = CentralServer.MasterUrl + "v5/serverreport.php";
        string response = HttpQuery.Post(url, data);

        if (response != "OK")
        {
            string errorMsg = $"[REPORTING] Error during **PROCESSING** server report:\n{response}";
            GameCore.Console.AddLog(errorMsg, Color.red, false, GameCore.Console.ConsoleLogType.Error);
            _response = CentralResponse.Error;
            _responseMessage = response;
        }
        else
        {
            ReportedServers?.Add(serverIp);
            GameCore.Console.AddLog("[REPORTING] Successfully submitted server report!", Color.green, false, GameCore.Console.ConsoleLogType.Warning);
            _response = CentralResponse.Success;
        }
    }

    private class __c__DisplayClass23_0
    {
        public string serverIp;
        public string violations;
        public string explanation;

        public void Execute()
        {
            SubmitReport(ref serverIp, ref violations, ref explanation);
        }
    }

    private enum CentralResponse
    {
        None = 0,
        Success = 1,
        Error = 2
    }

    static ReportServer()
    {
        ReportWatch = new Stopwatch();
        ReportedServers = new HashSet<string>();
        Re = new Regex(@"^((^|, |,)\d{1,2}(\.\d{1,2}){1,4}){1,4}$", RegexOptions.Compiled);
        _response = CentralResponse.None;
    }
}