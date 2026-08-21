[global::System.Serializable]
public struct Offset : global::System.IEquatable<Offset>
{
    public global::UnityEngine.Vector3 position;

    public global::UnityEngine.Vector3 rotation;

    public global::UnityEngine.Vector3 scale;

    public bool Equals(Offset other)
    {
        if (position == other.position && rotation == other.rotation)
        {
            return scale == other.scale;
        }
        return false;
    }

    public override bool Equals(object obj)
    {
        if (obj is Offset other)
        {
            return Equals(other);
        }
        return false;
    }

    public override int GetHashCode()
    {
        return (((position.GetHashCode() * 397) ^ rotation.GetHashCode()) * 397) ^ scale.GetHashCode();
    }

    public static bool operator ==(Offset left, Offset right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Offset left, Offset right)
    {
        return !left.Equals(right);
    }
}
