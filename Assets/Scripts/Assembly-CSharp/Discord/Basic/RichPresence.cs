using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Discord.Basic
{
    public class RichPresence
    {
        private readonly List<IntPtr> _buffers = new List<IntPtr>(10);
        private RichPresenceStruct _presence;

        public string state;
        public string details;
        public long startTimestamp;
        public long endTimestamp;
        public string largeImageKey;
        public string largeImageText;
        public string smallImageKey;
        public string smallImageText;
        public string partyId;
        public int partySize;
        public int partyMax;
        public string matchSecret;
        public string joinSecret;
        public string spectateSecret;
        public bool instance;

        internal RichPresenceStruct GetStruct()
        {
            FreeMem();

            _presence.state = StrToPtr(state);
            _presence.details = StrToPtr(details);
            _presence.startTimestamp = startTimestamp;
            _presence.endTimestamp = endTimestamp;
            _presence.largeImageKey = StrToPtr(largeImageKey);
            _presence.largeImageText = StrToPtr(largeImageText);
            _presence.smallImageKey = StrToPtr(smallImageKey);
            _presence.smallImageText = StrToPtr(smallImageText);
            _presence.partyId = StrToPtr(partyId);
            _presence.partySize = partySize;
            _presence.partyMax = partyMax;
            _presence.matchSecret = StrToPtr(matchSecret);
            _presence.joinSecret = StrToPtr(joinSecret);
            _presence.spectateSecret = StrToPtr(spectateSecret);
            _presence.instance = instance;

            return _presence;
        }

        private IntPtr StrToPtr(string input)
        {
            if (input == null)
                return IntPtr.Zero;

            int byteCount = Encoding.UTF8.GetByteCount(input);
            IntPtr ptr = Marshal.AllocHGlobal(byteCount + 1);
            unsafe
            {
                fixed (char* chars = input)
                {
                    Encoding.UTF8.GetBytes(chars, input.Length, (byte*)ptr, byteCount);
                }
                *((byte*)ptr + byteCount) = 0;
            }
            _buffers.Add(ptr);
            return ptr;
        }

        internal void FreeMem()
        {
            for (int i = _buffers.Count - 1; i >= 0; i--)
            {
                Marshal.FreeHGlobal(_buffers[i]);
                _buffers.RemoveAt(i);
            }
        }
    }
}