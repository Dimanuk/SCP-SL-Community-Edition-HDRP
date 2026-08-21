using System.Collections.Generic;
using Mirror;

namespace PlayerRoles.RoleAssign
{
    public static class ScpSpawnPreferencesExtensions
    {
        public static void WriteSpawnPreferences(this NetworkWriter writer, ScpSpawnPreferences.SpawnPreferences msg)
        {
            if (msg.Preferences == null)
            {
                writer.WriteByte(0);
                return;
            }

            writer.WriteByte((byte)msg.Preferences.Count);
            foreach (var pair in msg.Preferences)
            {
                writer.WriteRoleType(pair.Key);
                writer.WriteSByte((sbyte)pair.Value);
            }
        }

        public static ScpSpawnPreferences.SpawnPreferences ReadSpawnPreferences(this NetworkReader reader)
        {
            byte count = reader.ReadByte();
            var dict = new Dictionary<RoleTypeId, int>(count);
            for (int i = 0; i < count; i++)
            {
                dict[reader.ReadRoleType()] = reader.ReadSByte();
            }

            return new ScpSpawnPreferences.SpawnPreferences
            {
                Preferences = dict
            };
        }
    }
}