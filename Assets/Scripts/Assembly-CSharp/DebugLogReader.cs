using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using MEC;

public class DebugLogReader : MonoBehaviour
{
    public GameObject Parent;
    public Text Prefab;
    public ScrollRect Scroll;

    private static readonly List<Text> Lines = new List<Text>();
    private static readonly List<string> Linesstring = new List<string>();
    private static int _lineLimit;

    public static bool SuccesfullyInitialized()
    {
        string consoleLogPath = Application.consoleLogPath;
        if (string.IsNullOrEmpty(consoleLogPath))
            return false;

        string fullPath = Path.GetFullPath(consoleLogPath);
        if (!File.Exists(fullPath))
            return false;

        try
        {
            string content;
            using (var fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var reader = new StreamReader(fileStream))
            {
                content = reader.ReadToEnd();
            }

            if (string.IsNullOrWhiteSpace(content))
                return false;

            Linesstring.Clear();
            Linesstring.AddRange(content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries));
            
            return true;
        }
        catch
        {
            return false;
        }
    }

    public void OnEnable()
    {
        foreach (var line in Lines)
        {
            if (line != null)
                Destroy(line.gameObject);
        }
        Lines.Clear();

        var shared = NorthwoodLib.Pools.StringBuilderPool.Shared;
        var sb = shared.Rent(0x1194);

        foreach (var logLine in Linesstring)
        {
            sb.Clear();
            sb.AppendLine(logLine);
            
            string finalText = sb.ToString().TrimEnd();

            Text newText = Instantiate(Prefab, Parent.transform);
            newText.text = finalText;
            Lines.Add(newText);
        }

        shared.Return(sb);
        Linesstring.Clear();

        Timing.RunCoroutine(ScrollToDown(Scroll));
    }

    public void OnDisable()
    {
        foreach (var line in Lines)
        {
            if (line != null)
                Destroy(line.gameObject);
        }
        Lines.Clear();
        Linesstring.Clear();
    }

    private static IEnumerator<float> ScrollToDown(ScrollRect scrollRect)
    {
        if (scrollRect != null)
        {
            scrollRect.gameObject.SetActive(true);
            scrollRect.verticalNormalizedPosition = 0f;
        }
        yield return 0f; 
    }
}
