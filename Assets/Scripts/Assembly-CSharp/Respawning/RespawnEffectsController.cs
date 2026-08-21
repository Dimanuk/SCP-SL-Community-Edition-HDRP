using System.Collections.Generic;

using Mirror;
using Subtitles;

namespace Respawning
{
	public class RespawnEffectsController : NetworkBehaviour
	{
		public enum EffectType : byte
		{
			Selection = 0,
			UponRespawn = 1
		}

        private static readonly List<global::Respawning.RespawnEffectsController> AllControllers = new();

        private static readonly int PlayKey = global::UnityEngine.Animator.StringToHash("Play");

		public RespawnEffect[] SelectionEffects;

		public RespawnEffect[] OnSpawnEffects;

        private void Awake()
        {
            while (AllControllers.Contains(null))
            {
                AllControllers.Remove(null);
            }
            AllControllers.Add(this);
        }

        public static void ExecuteAllEffects(global::Respawning.RespawnEffectsController.EffectType type, global::Respawning.SpawnableTeamType team)
        {
            foreach (global::Respawning.RespawnEffectsController allController in AllControllers)
            {
                if (allController != null)
                {
                    allController.ServerExecuteEffects(type, team);
                }
            }
        }

        [global::Mirror.Server]
        private void ServerExecuteEffects(global::Respawning.RespawnEffectsController.EffectType type, global::Respawning.SpawnableTeamType team)
        {
            if (!global::Mirror.NetworkServer.active)
            {
                global::UnityEngine.Debug.LogWarning("[Server] function 'System.Void Respawning.RespawnEffectsController::ServerExecuteEffects(Respawning.RespawnEffectsController/EffectType,Respawning.SpawnableTeamType)' called when server was not active");
                return;
            }
            global::Respawning.RespawnEffect[] array = ((type == global::Respawning.RespawnEffectsController.EffectType.Selection) ? SelectionEffects : OnSpawnEffects);
            global::System.Collections.Generic.List<byte> list = global::NorthwoodLib.Pools.ListPool<byte>.Shared.Rent();
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i].TargetTeam == team)
                {
                    int num = i + ((type == global::Respawning.RespawnEffectsController.EffectType.Selection) ? 128 : 0);
                    list.Add((byte)num);
                }
            }
            if (list.Count > 0)
            {
                RpcPlayEffects(list.ToArray());
            }
            global::NorthwoodLib.Pools.ListPool<byte>.Shared.Return(list);
        }

        [global::Mirror.ClientRpc]
        private void RpcPlayEffects(byte[] effects)
        {
            foreach (byte b in effects)
            {
                global::Respawning.RespawnEffect respawnEffect = ((b < 128) ? OnSpawnEffects[b] : SelectionEffects[b - 128]);
                if (!respawnEffect.WhitelistEnabled || respawnEffect.WhitelistedRoles.Contains(global::PlayerRoles.PlayerRolesUtils.GetRoleId(ReferenceHub.LocalHub)))
                {
                    if (respawnEffect.AnimatorEffects != null)
                    {
                        respawnEffect.AnimatorEffects.SetTrigger(PlayKey);
                    }
                    if (respawnEffect.AudioAnnouncement != null)
                    {
                        respawnEffect.AudioAnnouncement.Play();
                    }
                }
            }
        }

        public static void PlayCassieAnnouncement(string words, bool makeHold, bool makeNoise, bool customAnnouncement = false)
        {
            foreach (global::Respawning.RespawnEffectsController allController in AllControllers)
            {
                if (allController != null)
                {
                    allController.ServerPassCassie(words, makeHold, makeNoise, customAnnouncement);
                    break;
                }
            }
        }

        [global::Mirror.Server]
        private void ServerPassCassie(string words, bool makeHold, bool makeNoise, bool customAnnouncement)
        {
            if (!global::Mirror.NetworkServer.active)
            {
                global::UnityEngine.Debug.LogWarning("[Server] function 'System.Void Respawning.RespawnEffectsController::ServerPassCassie(System.String,System.Boolean,System.Boolean,System.Boolean)' called when server was not active");
            }
            else
            {
                RpcCassieAnnouncement(words, makeHold, makeNoise, customAnnouncement);
            }
        }

        [global::Mirror.ClientRpc]
        private void RpcCassieAnnouncement(string words, bool makeHold, bool makeNoise, bool customAnnouncement)
        {
            if (!string.IsNullOrEmpty(words))
            {
                NineTailedFoxAnnouncer.singleton.AddPhraseToQueue(words, makeNoise, rawNumber: false, makeHold, customAnnouncement);
            }
        }


        public static void ClearQueue()
        {
            foreach (global::Respawning.RespawnEffectsController allController in AllControllers)
            {
                if (allController != null)
                {
                    allController.ServerPassClearQueue();
                    break;
                }
            }
        }

        [global::Mirror.Server]
        private void ServerPassClearQueue()
        {
            if (!global::Mirror.NetworkServer.active)
            {
                global::UnityEngine.Debug.LogWarning("[Server] function 'System.Void Respawning.RespawnEffectsController::ServerPassClearQueue()' called when server was not active");
            }
            else
            {
                RpcClearQueue();
            }
        }

        [ClientRpc]
        public void RpcClearQueue()
        {
            NineTailedFoxAnnouncer.singleton?.ClearQueue();
            SubtitleController.Singleton?.ClearSubtitles(CassieAnnouncementType.Normal);
        }
    }
}
