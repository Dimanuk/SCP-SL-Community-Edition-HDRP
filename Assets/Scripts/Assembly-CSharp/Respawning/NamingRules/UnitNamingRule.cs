using System.Collections.Generic;
using System.Text;

using Mirror;
using UnityEngine;

namespace Respawning.NamingRules
{
	public abstract class UnitNamingRule
	{
        private static readonly global::System.Collections.Generic.Dictionary<global::Respawning.SpawnableTeamType, global::Respawning.NamingRules.UnitNamingRule> AllNamingRules = new global::System.Collections.Generic.Dictionary<global::Respawning.SpawnableTeamType, global::Respawning.NamingRules.UnitNamingRule> { [global::Respawning.SpawnableTeamType.NineTailedFox] = new global::Respawning.NamingRules.NineTailedFoxNamingRule() };

        public static bool TryGetNamingRule(global::Respawning.SpawnableTeamType type, out global::Respawning.NamingRules.UnitNamingRule rule)
        {
            return AllNamingRules.TryGetValue(type, out rule);
        }

        public virtual string GetCassieUnitName(string regular)
        {
            return string.Empty;
        }

        public virtual void PlayEntranceAnnouncement(string regular)
        {
        }

        public abstract void GenerateNew(NetworkWriter writer);

		public abstract string ReadName(NetworkReader reader);

        internal void ConfirmAnnouncement(string completeAnnouncement)
        {
            float num = (AlphaWarheadController.Detonated ? 2.5f : 1f);
            NineTailedFoxAnnouncer.singleton.ServerOnlyAddGlitchyPhrase(completeAnnouncement, global::UnityEngine.Random.Range(0.08f, 0.1f) * num, global::UnityEngine.Random.Range(0.07f, 0.09f) * num);
        }

        internal void ConfirmAnnouncement(ref global::System.Text.StringBuilder sb)
        {
            ConfirmAnnouncement(sb.ToString());
        }
	}
}
