using TMPro;
using UnityEngine;
using System;
using GameCore;

public class Welcome : MonoBehaviour
{
    public TextMeshProUGUI Text;
    internal static string CurrentNickname = string.Empty;

    private void Start()
    {
        DistributionPlatform platform = CentralAuthManager.Platform;

        if (platform == DistributionPlatform.Discord)
        {
            if (CentralAuthManager.DiscordState != "Not initialized" && CentralAuthManager.Discord != null)
            {
                var userManager = CentralAuthManager.Discord.GetUserManager();
                CurrentNickname = userManager.GetCurrentUser().Username;
            }
            else
            {
                CurrentNickname = Environment.UserName;
            }
        }
        else if (platform == DistributionPlatform.Steam)
        {
            CurrentNickname = Steamworks.SteamClient.IsValid ? Steamworks.SteamClient.Name : Environment.UserName;
        }
        else if (platform == DistributionPlatform.Dedicated)
        {
            CurrentNickname = "Dedicated Server";
        }
        else
        {
            CurrentNickname = Environment.UserName;
        }

        string safeName = CurrentNickname.Replace("<", "(").Replace(">", ")");

        string formattedText = TranslationReader.GetFormatted("NewMainMenu", 65, "Welcome back, {1}!", safeName);

        if (Text != null)
        {
            Text.SetText(formattedText);
        }
    }
}