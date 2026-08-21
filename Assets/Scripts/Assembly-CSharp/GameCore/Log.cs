using System;
using UnityEngine;

namespace GameCore
{
    [Serializable]
    public struct Log : IEquatable<Log>
    {
        public string text;

        public Color color;

        public bool nospace;

        public Log(string t, Color c, bool b)
        {
            text = t;
            color = c;
            nospace = b;
        }

        public bool Equals(Log other)
        {
            if (text == other.text && color == other.color)
            {
                return nospace == other.nospace;
            }
            return false;
        }

        public override bool Equals(object obj)
        {
            if (obj is Log other)
            {
                return Equals(other);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return (((((text != null) ? text.GetHashCode() : 0) * 397) ^ color.GetHashCode()) * 397) ^ nospace.GetHashCode();
        }

        public override string ToString()
        {
            return text;
        }

        public static bool operator ==(Log left, Log right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Log left, Log right)
        {
            return !left.Equals(right);
        }
    }
}
