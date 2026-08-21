using System;

[Serializable]
public struct AlphaWarheadSyncInfo : IEquatable<AlphaWarheadSyncInfo>
{
    public int ScenarioId;

    public bool ResumeScenario;

    public double StartTime;

    public bool InProgress => StartTime != 0.0;

    public override int GetHashCode()
    {
        return (int)(StartTime * 10.0) % 32767 * 255 + (ResumeScenario ? (ScenarioId + 1) : (-ScenarioId));
    }

    public static bool operator ==(AlphaWarheadSyncInfo left, AlphaWarheadSyncInfo right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(AlphaWarheadSyncInfo left, AlphaWarheadSyncInfo right)
    {
        return !left.Equals(right);
    }

    public readonly bool Equals(AlphaWarheadSyncInfo other)
    {
        if (other.ScenarioId == ScenarioId && other.StartTime == StartTime)
        {
            return other.ResumeScenario == ResumeScenario;
        }
        return false;
    }

    public readonly override bool Equals(object obj)
    {
        if (obj is AlphaWarheadSyncInfo other)
        {
            return Equals(other);
        }
        return false;
    }
}
