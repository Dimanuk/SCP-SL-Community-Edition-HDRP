using System.Collections.Generic;
using Footprinting;
using Interactables.Verification;
using Mirror;
using UnityEngine;

namespace Interactables.Interobjects
{
	public class Scp330Interobject : NetworkBehaviour, IServerInteractable, IInteractable
    {
        private readonly List<Footprint> _takenCandies = new List<Footprint>();

        [SerializeField]
		private AudioClip _takeSound;

		private const float TakeCooldown = 0.1f;

		private const int MaxAmountPerLife = 2;

		public IVerificationRule VerificationRule => StandardDistanceVerification.Default;

        public void ServerInteract(ReferenceHub ply, byte colliderId)
        {
            if (!global::PlayerRoles.PlayerRolesUtils.IsHuman(ply))
            {
                return;
            }
            global::Footprinting.Footprint footprint = new global::Footprinting.Footprint(ply);
            float num = 0.1f;
            int num2 = 0;
            foreach (global::Footprinting.Footprint takenCandy in _takenCandies)
            {
                if (takenCandy.SameLife(footprint))
                {
                    num = global::UnityEngine.Mathf.Min(num, (float)takenCandy.Stopwatch.Elapsed.TotalSeconds);
                    num2++;
                }
            }
            if (num < 0.1f)
            {
                return;
            }
            if (global::InventorySystem.Items.Usables.Scp330.Scp330Bag.ServerProcessPickup(ply, null, out var bag))
            {
                RpcMakeSound();
                if (num2 >= 2)
                {
                    ply.playerEffectsController.EnableEffect<global::CustomPlayerEffects.SeveredHands>();
                    while (_takenCandies.Remove(footprint))
                    {
                    }
                }
                else
                {
                    _takenCandies.Add(footprint);
                }
            }
            else
            {
                global::InventorySystem.Searching.Scp330SearchCompletor.ShowOverloadHint(ply, bag != null);
            }
        }

        [ClientRpc]
		private void RpcMakeSound()
		{
            global::AudioPooling.AudioSourcePoolManager.PlaySound(_takeSound, base.transform, 10f);
        }
	}
}
