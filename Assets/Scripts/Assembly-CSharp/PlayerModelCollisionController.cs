using GameObjectPools;
using PlayerRoles;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModelCollisionController : MonoBehaviour, IPoolSpawnable, IPoolResettable
{
	public Collider[] MyColliders;

	public ReferenceHub Owner;

	public static PlayerModelCollisionController LocalPlayerController;

	public static List<PlayerModelCollisionController> AllControllers = new();

    public void SpawnObject()
    {
        AllControllers.Add(this);

        Owner = ReferenceHub.GetHub(transform.parent.gameObject);

        if (Owner == null)
            return; 

        if (Owner.isLocalPlayer)
        {
            LocalPlayerController = this;
            RefreshAllRelations();
        }
        else
        {
            RefreshRelation(this);
        }
    }

    public void ResetObject()
    {
        AllControllers.Remove(this);
    }

    private void RefreshAllRelations()
    {
        foreach (var controller in AllControllers)
        {
            if (controller == null) continue;
            if (controller.Owner == null) continue;
            if (controller.Owner.isLocalPlayer) continue;
            RefreshRelation(controller);
        }
    }

    private void RefreshRelation(PlayerModelCollisionController target)
    {
        if (LocalPlayerController == null) return;
        if (LocalPlayerController.Owner == null) return;

        if (target == null) return;
        if (target.Owner == null) return;

        bool isTrigger;

        if (!PlayerRolesUtils.IsHuman(LocalPlayerController.Owner))
        {
            isTrigger = true;
        }
        else
        {
            isTrigger = HitboxIdentity.CheckFriendlyFire(
                attacker: target.Owner,           
                victim: LocalPlayerController.Owner,  
                ignoreConfig: true
            );
        }

        foreach (var collider in target.MyColliders)
        {
            if (collider == null) continue;
            collider.isTrigger = !isTrigger; 
        }
    }
}
