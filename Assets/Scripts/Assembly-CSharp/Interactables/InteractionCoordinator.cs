using System;
using System.Collections.Generic;
using Interactables.Verification;
using InventorySystem.Items;
using Mirror;
using Mirror.RemoteCalls;
using PlayerRoles;
using UnityEngine;
using Utils.NonAllocLINQ;

namespace Interactables
{

    public class InteractionCoordinator : NetworkBehaviour
    {
        public readonly HashSet<IInteractionBlocker> _blockers = new HashSet<IInteractionBlocker>();

        public ReferenceHub _hub;

        public const float MaxRaycastRange = 50f;

        public static readonly CachedLayerMask RaycastMask = new("Default", "TransparentFX", "Player", "Pickup", "Hitbox", "Glass", "Door", "BreakableGlass", "Locker");

        public static KeyCode InteractKey { get; set; }

        public static bool CanDisarmedInteract { get; set; }

        public static event Action<InteractableCollider> OnClientInteracted;

        public void AddBlocker(IInteractionBlocker blocker)
        {
            _blockers.Add(blocker);
        }

        public bool RemoveBlocker(IInteractionBlocker blocker)
        {
            return _blockers.Remove(blocker);
        }

        public bool AnyBlocker(BlockedInteraction interactions)
        {
            return AnyBlocker((IInteractionBlocker x) => x.BlockedInteractions.HasFlagFast(interactions));
        }

        public bool AnyBlocker(Func<IInteractionBlocker, bool> func)
        {
            _blockers.RemoveWhere(x => (x is UnityEngine.Object @object && @object == null) || (x?.CanBeCleared ?? true));
            return _blockers.Any(func);
        }

        public void Start()
        {
            if (base.isLocalPlayer || NetworkServer.active)
            {
                _hub = ReferenceHub.GetHub(base.gameObject);
            }

            if (base.isLocalPlayer)
            {
                InteractKey = NewInput.GetKey(ActionName.Interact);
                NewInput.OnKeyModified += OnKeyModified;
            }
        }

        public void OnDestroy()
        {
            NewInput.OnKeyModified -= OnKeyModified;
        }

        public void Update()
        {
            if (base.isLocalPlayer && Input.GetKeyDown(InteractKey))
            {
                ClientInteract();
            }
        }

        public void OnKeyModified(ActionName actionName, KeyCode keyCode)
        {
            if (actionName == ActionName.Interact)
            {
                InteractKey = keyCode;
            }
        }

        

        public void ClientInteract()
        {
            if (!_hub.IsAlive() || !NetworkClient.ready || !Physics.Raycast(MainCameraController.LastForwardRay, out var hitInfo, 50f, RaycastMask))
            {
                return;
            }

            if (!hitInfo.collider.TryGetComponent<InteractableCollider>(out var component))
            {
                Transform parent = hitInfo.collider.transform.parent;
                if (parent == null || !parent.TryGetComponent<InteractableCollider>(out component))
                {
                    return;
                }
            }

            if (component.Target is IInteractable interactable && !(component.Target == null) && GetSafeRule(interactable).ClientCanInteract(component, hitInfo))
            {
                if (interactable is IClientInteractable clientInteractable)
                {
                    clientInteractable.ClientInteract(component);
                }

                InteractionCoordinator.OnClientInteracted?.Invoke(component);
                if (component.Target is NetworkBehaviour networkBehaviour)
                {
                    CmdServerInteract(networkBehaviour.netIdentity, component.ColliderId);
                }
            }
        }

        public static IVerificationRule GetSafeRule(IInteractable inter)
        {
            return inter.VerificationRule ?? StandardDistanceVerification.Default;
        }

        [Command(channel = 4)]
        public void CmdServerInteract(NetworkIdentity targetInteractable, byte colId)
        {
            if (!(targetInteractable == null) && !(_hub == null) && _hub.IsAlive() && targetInteractable.TryGetComponent<IServerInteractable>(out var component) && InteractableCollider.TryGetCollider(component, colId, out var res) && GetSafeRule(component).ServerCanInteract(_hub, res))
            {
                component.ServerInteract(_hub, colId);
            }
        }
    }
}