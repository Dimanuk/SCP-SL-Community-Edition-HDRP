namespace Subtitles
{
    public struct SubtitlePart
    {
        public SubtitleType Subtitle;
        public string[] OptionalData;

        public SubtitlePart(SubtitleType subtitle, params string[] optionalData)
        {
            Subtitle = subtitle;
            OptionalData = optionalData;
        }
    }
}