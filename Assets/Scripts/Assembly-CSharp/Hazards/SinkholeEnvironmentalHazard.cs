using CustomPlayerEffects;
using GameCore;
using Mirror;
using PlayerRoles;
using UnityEngine;

namespace Hazards
{
    public class SinkholeEnvironmentalHazard : EnvironmentalHazard
    {
        public override void OnEnter(ReferenceHub player)
        {
            if (IsActive && !player.IsSCP())
            {
                base.OnEnter(player);
                player.playerEffectsController.EnableEffect<Sinkhole>(1f);
            }
        }

        public override void OnStay(ReferenceHub player)
        {
            player.playerEffectsController.EnableEffect<Sinkhole>(1f);
        }

        public override void OnExit(ReferenceHub player)
        {
            base.OnExit(player);
            player.playerEffectsController.EnableEffect<Sinkhole>(1f);
        }

        protected override void Start()
        {
            if (NetworkServer.active && ConfigFile.ServerConfig.GetFloat("sinkhole_spawn_chance") < (float)Random.Range(1, 100))
            {
                if (base.netId == 0)
                {
                    Object.Destroy(base.gameObject);
                }
                else
                {
                    NetworkServer.Destroy(base.gameObject);
                }
            }
            else
            {
                base.Start();
            }
        }
    }
}
