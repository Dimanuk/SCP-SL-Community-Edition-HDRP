using System.Collections.Generic;
using CustomPlayerEffects;
using Footprinting;
using PlayerRoles.Ragdolls;
using UnityEngine;

public class RagdollHandRemover : MonoBehaviour
{
    [SerializeField]
    private DynamicRagdoll _ragdoll;

    [SerializeField]
    private Transform[] _hands;

    private void Start()
    {
        List<Footprint> severedHands = SeveredHands.AllSeveredHands;
        
        for (int i = 0; i < severedHands.Count; i++)
        {
            Footprint footprint = severedHands[i];
            

            ReferenceHub ragdollOwner = _ragdoll.Info.OwnerHub; 
            

            if (ReferenceHub.Equals(footprint.Hub, ragdollOwner))
                continue;
            

            severedHands.RemoveAt(i);
            i--; 

            if (ReferenceHub.Equals(footprint.Hub, null))
            {
                RemoveHands();
                return;
            }
        }
        
    }

    private void RemoveHands()
    {
        if (_hands == null)
            return;

        for (int i = 0; i < _hands.Length; i++)
        {
            Transform hand = _hands[i];
            if (hand == null)
                continue;

            hand.localScale = Vector3.zero;
        }
    }
}