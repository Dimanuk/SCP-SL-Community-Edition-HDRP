namespace Subtitles
{
    public struct SubtitleMessage : global::Mirror.NetworkMessage
    {
        public global::Subtitles.SubtitlePart[] SubtitleParts;

        public SubtitleMessage(params global::Subtitles.SubtitlePart[] subtitleParts)
        {
            SubtitleParts = subtitleParts;
        }
    }
}
