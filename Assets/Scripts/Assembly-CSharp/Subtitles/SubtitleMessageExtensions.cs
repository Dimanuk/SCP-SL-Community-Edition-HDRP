using Mirror;
using UnityEngine;

namespace Subtitles
{
    public static class SubtitleMessageExtensions
    {
        public static void Serialize(this NetworkWriter writer, SubtitleMessage value)
        {
            SubtitlePart[] subtitleParts = value.SubtitleParts;
            if (subtitleParts == null || subtitleParts.Length == 0)
            {
                return;
            }

            writer.WriteByte((byte)subtitleParts.Length);
            for (int i = 0; i < subtitleParts.Length; i++)
            {
                SubtitlePart subtitlePart = subtitleParts[i];
                writer.WriteByte((byte)((subtitlePart.OptionalData != null) ? ((byte)subtitlePart.OptionalData.Length) : 0));

                if (subtitlePart.OptionalData != null && subtitlePart.OptionalData.Length != 0)
                {
                    for (int j = 0; j < subtitlePart.OptionalData.Length; j++)
                    {
                        NetworkWriterExtensions.WriteString(writer, subtitlePart.OptionalData[j]);
                    }
                }

                writer.WriteByte((byte)subtitlePart.Subtitle);
            }
        }

        public static SubtitleMessage Deserialize(this NetworkReader reader)
        {
            int count = reader.ReadByte();
            SubtitlePart[] array = new SubtitlePart[count];

            for (int i = 0; i < count; i++)
            {
                int optionalCount = reader.ReadByte();
                string[] optionalData = new string[optionalCount];

                for (int j = 0; j < optionalCount; j++)
                {
                    optionalData[j] = NetworkReaderExtensions.ReadString(reader);
                }

                array[i] = new SubtitlePart((SubtitleType)reader.ReadByte(), optionalCount == 0 ? null : optionalData);
            }

            return new SubtitleMessage { SubtitleParts = array };
        }

        [RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            CustomNetworkManager.OnClientReady += RegisterHandlers;
        }

        private static void RegisterHandlers()
        {
            NetworkClient.ReplaceHandler<SubtitleMessage>(ClientMessageReceived);
        }

        private static void ClientMessageReceived(SubtitleMessage msg)
        {
            SubtitleController.Singleton?.SetupSubtitle(msg.SubtitleParts);
        }
    }
}