using TMPro;
using UnityEngine;

public class PrivateBetaWarning : MonoBehaviour
{
    public TextMeshProUGUI text;

    private const string ContentPrivateBeta = "<size=32>PRIVATE BETA - restricted access</size>";
    private const string DevelopmentBuild = "<size=32>DEVELOPMENT BUILD - restricted access</size>";
    private const string NightlyBuild = "<size=32>NIGHTLY BUILD - restricted access</size>";
    private const string ContentPublicBeta = "<size=32>PUBLIC BETA - all content is subject to change!</size>";
    private const string StreamingAllowedSuffix = "Sharing footage (recording or streaming) is allowed";
    private const string StreamingDisallowedSuffix = "CONFIDENTIAL - sharing or publishing any information about this version (including recording or streaming) is strictly prohibited";
    private const string DoNotShareSuffix = "Do not share or distribute any game files";

    private void Start()
    {
        var buildType = GameCore.Version.BuildType;

        if (buildType == GameCore.Version.VersionType.Development)
        {
            if (text == null || text.IsDestroyed()) return;
            text.text = $"{DevelopmentBuild}\n{DoNotShareSuffix}";
            return;
        }

        if (buildType == GameCore.Version.VersionType.Nightly)
        {
            if (text == null || text.IsDestroyed()) return;
            text.text = $"{NightlyBuild}\n{DoNotShareSuffix}";
            return;
        }

        string betaContent;
        string releaseCandidateSuffix;
        string streamingSuffix;

        if (GameCore.Version.PrivateBeta)
        {
            betaContent = ContentPrivateBeta;
            releaseCandidateSuffix = GameCore.Version.ReleaseCandidate ? " [RELEASE CANDIDATE]\n" : "\n";
            streamingSuffix = GameCore.Version.StreamingAllowed ? StreamingAllowedSuffix : StreamingDisallowedSuffix;

            var versionString = GameCore.Version.VersionString;
            var fullMessage = $"{betaContent}{releaseCandidateSuffix}Game version: {versionString}\n{streamingSuffix}\n{DoNotShareSuffix}";

            if (text != null && !text.IsDestroyed())
            {
                text.text = fullMessage;
            }
        }
        else if (GameCore.Version.PublicBeta)
        {
            betaContent = ContentPublicBeta;
            releaseCandidateSuffix = GameCore.Version.ReleaseCandidate ? " [RELEASE CANDIDATE]\n" : "\n";

            var versionString = GameCore.Version.VersionString;
            var fullMessage = $"{betaContent}{releaseCandidateSuffix}Game version: {versionString}";

            if (text != null && !text.IsDestroyed())
            {
                text.text = fullMessage;
            }
        }
    }
}