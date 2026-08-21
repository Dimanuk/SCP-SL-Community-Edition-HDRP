using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using MEC;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NewsLoader : MonoBehaviour
{
    [Serializable]
    public class Announcement
    {
        public string Title;
        public string Content;
        public string Date;
        public string Link;
        public NewsElement Thumbnail;

        public Announcement(string title, string content, string date, string link, NewsElement thumbnail)
        {
            Title = title;
            Content = content;
            Date = date;
            Link = link;
            Thumbnail = thumbnail;
        }
    }

    [SerializeField] private TextMeshProUGUI ArticleText;
    [SerializeField] private RectTransform ContentParent;
    [SerializeField] private RectTransform Element;
    [SerializeField] private Button OpenNewsUrlButton;

    private List<Announcement> _announcements;
    private string _curAnncUrl;

    private void Start()
    {
        _announcements = new List<Announcement>();
        Timing.RunCoroutine(Request());
    }

    private IEnumerator<float> Request()
    {
        using UnityWebRequest www = UnityWebRequest.Get(
            "https://api.steampowered.com/ISteamNews/GetNewsForApp/v2/?appid=700330&count=5");

        yield return Timing.WaitUntilDone(www.SendWebRequest());

        if (string.IsNullOrEmpty(www.error))
        {
            TextProcessor(www.downloadHandler.text);
            Debug.Log("Web request succeeded");
        }
        else
        {
            if (ArticleText != null)
                ArticleText.text = "Web request failed: " + www.error;
            Debug.LogError("Web request failed: " + www.error);
        }
    }

    private void DetectCustomLinks(ref string content)
    {
        const string pattern = @"[ |\n]((http:\/\/www\.|https:\/\/www\.|http:\/\/|https:\/\/)?[a-z0-9]+([\-\.]{1}[a-z0-9]+)*\.[a-z]{2,5}(:[0-9]{1,5})?(\/.*)?)";
        Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
        MatchCollection matches = regex.Matches(content);

        foreach (Match match in matches)
        {
            if (!match.Success) continue;
            Group grp = match.Groups[1];
            string url = grp.Value.TrimStart(' ', '\n');
            int idx = grp.Index + (grp.Value.Length - url.Length);

            string replacement = $"<color=#87CEFA><u><link=\"{url}\">{url}</link></u></color>";
            content = content.Remove(idx, url.Length).Insert(idx, replacement);
        }
    }

    private void DetectImages(ref string content)
    {
        int safety = 0;
        const string imgTag = "[img]";

        while (content.Contains(imgTag) && safety++ < 200)
        {
            int start = content.IndexOf(imgTag, StringComparison.Ordinal);
            int end = content.IndexOf(']', start + imgTag.Length);
            if (end == -1) break;

            string urlPart = content.Substring(start + imgTag.Length, end - start - imgTag.Length);
            urlPart = urlPart.Replace("{STEAM_CLAN_IMAGE}",
                "https://steamcdn-a.akamaihd.net/steamcommunity/public/images/clans");

            string linkTag = $"<color=#87CEFA><link=\"{urlPart}\">[ OPEN IMAGE ]</link></color>";
            content = content.Remove(start, end - start + 1).Insert(start, linkTag);
        }
    }

    private void DetectYoutubePreview(ref string content)
    {
        const string startTag = "[previewyoutube=";
        const string endTag = "[/previewyoutube]";

        while (content.Contains(startTag))
        {
            int idx = content.IndexOf(startTag, StringComparison.Ordinal);
            if (idx == -1) break;

            int endIdx = content.IndexOf(']', idx + startTag.Length);
            if (endIdx == -1) break;

            int closeIdx = content.IndexOf(endTag, endIdx, StringComparison.Ordinal);
            if (closeIdx == -1) break;

            string videoId = content.Substring(idx + startTag.Length, endIdx - idx - startTag.Length);
            string replacement = $"<color=#87CEFA><link=\"https://www.youtube.com/watch?v={videoId}\">[ OPEN YOUTUBE VIDEO ]</link></color>";

            content = content.Remove(idx, closeIdx + endTag.Length - idx).Insert(idx, replacement);
        }
    }

    private void TextProcessor(string json)
    {
        NewsRaw newsRaw = JsonSerialize.FromJson<NewsRaw>(json);
        if (newsRaw.appnews.newsitems == null) return;

        foreach (var newsItem in newsRaw.appnews.newsitems)
        {
            string title = newsItem.title ?? "";
            string date = newsItem.date != 0
                ? DateTimeOffset.FromUnixTimeSeconds(newsItem.date).ToLocalTime().ToString("yyyy-MM-dd")
                : "Unknown date";
            string content = newsItem.contents ?? "";

            if (content.Length > 5000)
                content = NorthwoodLib.StringUtils.TruncateToLast(content, 5000, ' ')
                    + "...\n<i><color=#87CEFA><u><link=\"\">(Click here for full content)</link></u></color></i>";

            content = content
                .Replace("[b]", "<b>").Replace("[/b]", "</b>")
                .Replace("[i]", "<i>").Replace("[/i]", "</i>")
                .Replace("[u]", "<u>").Replace("[/u]", "</u>")
                .Replace("[h1]", "<size=21><color=#ffe2bd>").Replace("[/h1]", "</color></size>")
                .Replace("[h2]", "<size=19><color=#ffe2bd>").Replace("[/h2]", "</color></size>")
                .Replace("[h3]", "<size=17><color=#ffe2bd>").Replace("[/h3]", "</color></size>")
                .Replace("[strike]", "<s>").Replace("[/strike]", "</s>")
                .Replace("[code]", "<size=15><i>").Replace("[/code]", "</i></size>")
                .Replace("[*]", "•")
                .Replace("[list]", "").Replace("[/list]", "")
                .Replace("[olist]", "").Replace("[/olist]", "");

            DetectYoutubePreview(ref content);

            while (content.Contains("[url="))
            {
                int start = content.IndexOf("[url=", StringComparison.Ordinal);
                if (start == -1) break;
                int bracket = content.IndexOf(']', start);
                if (bracket == -1) break;
                int end = content.IndexOf("[/url]", bracket, StringComparison.Ordinal);
                if (end == -1) break;

                string url = content.Substring(start + 5, bracket - start - 5);
                string text = content.Substring(bracket + 1, end - bracket - 1);
                string replacement = $"<color=#87CEFA><u><link=\"{url}\">{text}</link></u></color>";
                content = content.Remove(start, end + 6 - start).Insert(start, replacement);
            }

            DetectCustomLinks(ref content);
            DetectImages(ref content);

            RectTransform instance = Instantiate(Element, ContentParent);
            NewsElement newsElement = instance.GetComponent<NewsElement>();
            if (newsElement == null) continue;

            if (newsElement.Title != null) newsElement.Title.text = title;
            if (newsElement.Date != null) newsElement.Date.text = date;
            if (newsElement.Content != null) newsElement.Content.text = content;

            newsElement.Id = _announcements.Count;
            newsElement.transform.localScale = Vector3.one;

            _announcements.Add(new Announcement(title, content, date, newsItem.url, newsElement));
        }

        ShowAnnouncement(0);
    }

    public void OpenAnnouncementUrl()
    {
        if (string.IsNullOrEmpty(_curAnncUrl)) return;

        if (SteamManager.IsSteamReady())
            SteamFriends.OpenWebOverlay(_curAnncUrl, false);
        else
            Application.OpenURL(_curAnncUrl);
    }

    public void ShowAnnouncement(int id)
    {
        if (_announcements == null || id < 0 || id >= _announcements.Count) return;

        Announcement ann = _announcements[id];
        _curAnncUrl = ann.Link;

        if (ArticleText != null)
        {
            ArticleText.text = string.Concat(
                "<size=30><b>", ann.Title, "</b></size>\n",
                "<size=23><i>", ann.Date, "</i></size>\n\n",
                "<size=17><color=#F0F8FF>", ann.Content, "</color></size>");
        }

        if (OpenNewsUrlButton != null)
            OpenNewsUrlButton.interactable = !string.IsNullOrEmpty(_curAnncUrl);

        for (int i = 0; i < _announcements.Count; i++)
        {
            NewsElement el = _announcements[i].Thumbnail;
            if (el == null) continue;
            el.transform.localScale = (i == id) ? Vector3.one : new Vector3(0.78125f, 0.78125f, 0.78125f);
        }
    }
}