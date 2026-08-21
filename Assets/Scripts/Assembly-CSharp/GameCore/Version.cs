using System;
using UnityEngine;

namespace GameCore
{
    public static class Version
    {
        public enum VersionType : byte
        {
            Release = 0,
            PublicRC = 1,
            PublicBeta = 2,
            PrivateRC = 3,
            PrivateRCStreamingForbidden = 4,
            PrivateBeta = 5,
            PrivateBetaStreamingForbidden = 6,
            Development = 7,
            Nightly = 8
        }

        public static readonly byte Major;

        public static readonly byte Minor;

        public static readonly byte Revision;

        public static readonly bool AlwaysAcceptReleaseBuilds;

        public static readonly VersionType BuildType;

        public static readonly bool BackwardCompatibility;

        public static readonly byte BackwardRevision;

        public static readonly string DescriptionOverride;

        public static readonly string VersionString;

        public static bool PublicBeta
        {
            get
            {
                if (BuildType != VersionType.PublicBeta)
                {
                    return BuildType == VersionType.PublicRC;
                }
                return true;
            }
        }

        public static bool PrivateBeta
        {
            get
            {
                VersionType buildType = BuildType;
                return buildType == VersionType.PrivateBeta || buildType == VersionType.PrivateBetaStreamingForbidden || buildType == VersionType.PrivateRC || buildType == VersionType.PrivateRCStreamingForbidden || buildType == VersionType.Development || buildType == VersionType.Nightly;
            }
        }

        public static bool ReleaseCandidate
        {
            get
            {
                VersionType buildType = BuildType;
                return buildType == VersionType.PublicRC || buildType == VersionType.PrivateRC || buildType == VersionType.PrivateRCStreamingForbidden;
            }
        }

        public static bool StreamingAllowed
        {
            get
            {
                VersionType buildType = BuildType;
                if (buildType != VersionType.PrivateBetaStreamingForbidden && buildType != VersionType.PrivateRCStreamingForbidden && buildType != VersionType.Development)
                {
                    return buildType != VersionType.Nightly;
                }
                return false;
            }
        }

        public static bool ExtendedVersionCheckNeeded => BuildType != VersionType.Release;

        static Version()
        {
            Major = 12;
            Minor = 0;
            Revision = 2;
            AlwaysAcceptReleaseBuilds = false;
            BuildType = VersionType.PublicBeta;
            BackwardCompatibility = false;
            BackwardRevision = 0;
            DescriptionOverride = null;
            VersionString = string.Format("{0}.{1}.{2}{3}", Major, Minor, Revision, (!ExtendedVersionCheckNeeded) ? string.Empty : ("-" + (DescriptionOverride ?? "12.0.1-rc-2298ba84")));
        }

        public static bool ListedServerCompatibilityCheck(string serverVersion)
        {
            try
            {
                if (string.IsNullOrEmpty(serverVersion))
                {
                    return false;
                }
                // Fast path: an exact string match is always compatible.
                if (VersionString.Equals(serverVersion, StringComparison.Ordinal))
                {
                    return true;
                }
                // Fall back to numeric comparison so a compatible build with a
                // different or missing build suffix (e.g. "12.0.2" vs
                // "12.0.2-...") is still listed and joinable.
                if (serverVersion.Contains("-"))
                {
                    serverVersion = serverVersion.Split('-')[0];
                }
                string[] parts = serverVersion.Split('.');
                if (parts.Length != 3)
                {
                    return false;
                }
                if (!byte.TryParse(parts[0], out var sMajor) || !byte.TryParse(parts[1], out var sMinor) || !byte.TryParse(parts[2], out var sRevision))
                {
                    return false;
                }
                return CompatibilityCheck(sMajor, sMinor, sRevision);
            }
            catch (Exception ex)
            {
                Console.AddLog("Failed to process listed server version: " + ex.Message, Color.red);
                return false;
            }
        }

        public static bool CompatibilityCheck(byte sMajor, byte sMinor, byte sRevision)
        {
            return CompatibilityCheck(sMajor, sMinor, sRevision, Major, Minor, Revision, BackwardCompatibility, BackwardRevision);
        }

        public static bool CompatibilityCheck(byte sMajor, byte sMinor, byte sRevision, byte cMajor, byte cMinor, byte cRevision, bool cBackwardEnabled, byte cBackwardRevision)
        {
            if (sMajor != cMajor || sMinor != cMinor)
            {
                return false;
            }
            if (!cBackwardEnabled)
            {
                return sRevision == cRevision;
            }
            if (sRevision >= cBackwardRevision)
            {
                return sRevision <= cRevision;
            }
            return false;
        }
    }
}
