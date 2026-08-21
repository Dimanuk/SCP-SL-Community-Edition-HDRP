using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;

namespace Subtitles
{
    public class SubtitleController : MonoBehaviour
    {
        public static SubtitleController Singleton;

        [SerializeField]
        private Subtitle[] subtitles;

        [SerializeField]
        private SubtitleCategory[] subtitleCategories;

        private Dictionary<SubtitleType, Subtitle> Subtitles = new Dictionary<SubtitleType, Subtitle>();

        private void Awake()
        {
            Singleton = this;

            bool enabled = PlayerPrefsSl.Get(Subtitles.ToString(), true);
            this.enabled = enabled;

            if (enabled)
            {
                foreach (Subtitle subtitle in subtitles)
                {
                    if (subtitle != null)
                        Subtitles.Add(subtitle.SubtitleTypeValue, subtitle);
                }
            }
        }

        public void SetupSubtitle(SubtitlePart[] subtitles)
        {
            if (!enabled || subtitles == null || subtitles.Length == 0)
                return;

            StringBuilder sb = new StringBuilder();
            float totalDuration = 0f;
            Subtitle previousSubtitle = null;
            Subtitle firstCategorizedSubtitle = null;

            for (int i = 0; i < subtitles.Length; i++)
            {
                SubtitlePart part = subtitles[i];

                if (!Subtitles.TryGetValue(part.Subtitle, out Subtitle subtitleData))
                {
                    Debug.LogError(string.Format("Unable to find serialized data for {0}", part.Subtitle));
                    continue;
                }

                totalDuration += subtitleData.Duration;

                if (previousSubtitle != null && previousSubtitle.RequestSpace)
                    sb.Append(" ");

                string translation;
                if (!TranslationReader.TryGet("Subtitles", ((int)subtitleData.SubtitleTypeValue), out translation))
                    translation = subtitleData.DefaultValue;

                string processed = ReplaceInfo(translation, part.OptionalData, subtitleData.ConvertNumbers);
                sb.Append(processed);

                if (firstCategorizedSubtitle == null && subtitleData.SubtitleCategory != CassieAnnouncementType.None)
                    firstCategorizedSubtitle = subtitleData;

                previousSubtitle = subtitleData;
            }

            string message = sb.ToString();

            if (!enabled || firstCategorizedSubtitle == null)
                return;

            int categoryIndex = firstCategorizedSubtitle.SubtitleCategory == CassieAnnouncementType.None
                ? 0
                : (int)firstCategorizedSubtitle.SubtitleCategory - 1;

            if (subtitleCategories == null || categoryIndex < 0 || categoryIndex >= subtitleCategories.Length)
                return;

            SubtitleCategory category = subtitleCategories[categoryIndex];
            if (category == null)
                return;

            if (firstCategorizedSubtitle.SubtitleCategory != (CassieAnnouncementType)1)
                category.ClearSubtitles();

            category.AddSubtitle(message, totalDuration, firstCategorizedSubtitle.Delay);
        }

        public void ClearSubtitles(CassieAnnouncementType type)
        {
            if (subtitleCategories == null)
                return;

            int index = type == CassieAnnouncementType.None ? 0 : (int)type - 1;

            if (index < 0 || index >= subtitleCategories.Length)
                return;

            SubtitleCategory category = subtitleCategories[index];
            if (category != null)
                category.ClearSubtitles();
        }

        public void AddSubtitle(string message, float duration, Subtitle masterSubtitle)
        {
            if (!enabled || masterSubtitle == null || subtitleCategories == null)
                return;

            int categoryIndex = masterSubtitle.SubtitleCategory == CassieAnnouncementType.None
                ? 0
                : (int)masterSubtitle.SubtitleCategory - 1;

            if (categoryIndex < 0 || categoryIndex >= subtitleCategories.Length)
                return;

            SubtitleCategory category = subtitleCategories[categoryIndex];
            if (category == null)
                return;

            if (masterSubtitle.SubtitleCategory != (CassieAnnouncementType)1)
                category.ClearSubtitles();

            category.AddSubtitle(message, duration, masterSubtitle.Delay);
        }


        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private string ReplaceInfo(string message, string[] optionalData, bool convertNumbers)
        {
            if (optionalData == null || optionalData.Length == 0)
                return message;

            if (convertNumbers)
            {
                for (int i = 0; i < optionalData.Length; i++)
                {
                    if (int.TryParse(optionalData[i], out int number))
                    {
                        optionalData[i] = NumberToWords(number, CultureInfo.CurrentCulture);
                    }
                }
            }

            return string.Format(message, optionalData);
        }

        private string GetTranslation(Subtitle subtitle)
        {
            string translation;
            if (TranslationReader.TryGet("Subtitles", (int)subtitle.SubtitleTypeValue, out translation))
                return translation;

            return subtitle.DefaultValue;
        }

        private static string NumberToWords(int number, CultureInfo culture)
        {
            return number.ToString(culture);
        }
    }
}