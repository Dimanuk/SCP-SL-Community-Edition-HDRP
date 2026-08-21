using System;

namespace Mirror
{
    public interface ISegmentTransport
    {
        bool ServerSend(int connectionId, ArraySegment<byte> data, int channelId);
        bool ClientSend(int channelId, ArraySegment<byte> data);
    }
}