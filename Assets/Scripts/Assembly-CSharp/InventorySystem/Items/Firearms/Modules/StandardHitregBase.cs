using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.BasicMessages;
using Mirror;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using RelativePositioning;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using Utils.Networking;

namespace InventorySystem.Items.Firearms.Modules
{
    public abstract class StandardHitregBase : IHitregModule, IFirearmModuleBase
    {
        public static readonly LayerMask HitregMask = LayerMask.GetMask(
            "Default", "Hitbox", "Glass", "CCTV", "Door", "Locker");

        private const float MinDot = 0.5f;
        private const float MaxHeightDiff = 50f;

        public bool Standby => true;

        protected NetworkConnection Conn => Hub.inventory.connectionToClient;

        protected abstract Firearm Firearm { get; set; }
        protected abstract ReferenceHub Hub { get; set; }

        public static bool DebugMode { get; internal set; }

        protected uint PrimaryTargetNetId { get; private set; }

        private void SetHitboxes(ReferenceHub target, bool state)
        {
            if (target?.roleManager?.CurrentRole is IFpcRole fpcRole)
            {
                HitboxIdentity[] hitboxes = fpcRole.FpcModule.CharacterModelInstance.Hitboxes;
                for (int i = 0; i < hitboxes.Length; i++)
                {
                    hitboxes[i].SetColliders(state);
                }
            }
        }

        protected void SendDebug(string msg)
        {
            Hub.gameConsoleTransmission.SendToClient("[HITREG DEBUG] " + msg, "gray");
        }

        public bool ClientCalculateHit(out ShotMessage message)
        {
            message = default(ShotMessage);

            Vector3 forward = Hub.PlayerCameraReference.forward;
            Vector3 position = Hub.PlayerCameraReference.position;
            float maxDistance = Firearm.BaseStats.MaxDistance();
            uint netId = Hub.inventory.netId;
            float bestDot = MinDot;
            IDestructible component = null;

            bool raycastHit = Physics.Raycast(position, forward, out RaycastHit hitInfo, maxDistance, HitregMask);
            bool hitDestructible = raycastHit && hitInfo.collider.TryGetComponent<IDestructible>(out component);

            if (!hitDestructible)
            {
                foreach (HitboxIdentity instance in HitboxIdentity.Instances)
                {
                    IDestructible destructible = instance;
                    if (destructible.NetworkId == netId)
                        continue;

                    if (Mathf.Abs(destructible.CenterOfMass.y - position.y) > MaxHeightDiff)
                        continue;

                    float dot = Vector3.Dot(forward, (destructible.CenterOfMass - position).normalized);
                    if (dot < bestDot)
                        continue;

                    if (dot == bestDot && component != null)
                    {
                        float distCurrent = Vector3.Distance(position, component.CenterOfMass);
                        float distCandidate = Vector3.Distance(position, destructible.CenterOfMass);
                        if (distCurrent < distCandidate)
                            continue;
                    }

                    bestDot = dot;
                    component = destructible;
                }
            }

            if (component != null && NetworkClient.spawned.TryGetValue(component.NetworkId, out NetworkIdentity value))
            {
                message.TargetNetId = value.netId;
                RelativePosition relativePosition = new RelativePosition(value.transform.position);
                message.TargetPosition = relativePosition;
                message.TargetRotation = WaypointBase.GetRelativeRotation(relativePosition.WaypointId, value.transform.rotation);

            }
            else
            {
                message.TargetNetId = 0u;
            }

            RelativePosition shooterPos = new RelativePosition(position);
            message.ShooterPosition = shooterPos;
            message.ShooterCameraRotation = WaypointBase.GetRelativeRotation(shooterPos.WaypointId, Hub.PlayerCameraReference.rotation);

            return true;
        }

        public void ServerProcessShot(ShotMessage message)
        {
            if (!WaypointBase.TryGetWaypoint(message.ShooterPosition.WaypointId, out WaypointBase wp))
            {
                return;
            }

            SetHitboxes(Hub, state: false);

            Vector3 worldspacePosition = wp.GetWorldspacePosition(message.ShooterPosition.Relative);
            Quaternion worldspaceRotation = wp.GetWorldspaceRotation(message.ShooterCameraRotation);

            using (FpcBacktracker fpcBacktracker = new FpcBacktracker(Hub, worldspacePosition, worldspaceRotation))
            {

                bool hasTarget = ReferenceHub.TryGetHubNetID(message.TargetNetId, out ReferenceHub targetHub);
                FpcBacktracker targetBacktracker = null;
                Quaternion originalRotation = default;
                Transform targetTransform = null;

                if (hasTarget && WaypointBase.TryGetWaypoint(message.TargetPosition.WaypointId, out WaypointBase wp2))
                {
                    targetTransform = targetHub.transform;
                    originalRotation = targetTransform.rotation;
                    targetBacktracker = new FpcBacktracker(targetHub, wp2.GetWorldspacePosition(message.TargetPosition.Relative));
                    targetTransform.rotation = wp2.GetWorldspaceRotation(message.TargetRotation);


                    if (targetHub.isLocalPlayer)
                    {
                        SetHitboxes(targetHub, state: true);
                    }
                }

                PrimaryTargetNetId = message.TargetNetId;

                ServerPerformShot(new Ray(Hub.PlayerCameraReference.position, Hub.PlayerCameraReference.forward));
                SetHitboxes(Hub, !Hub.isLocalPlayer);

                if (hasTarget && targetBacktracker != null)
                {
                    // Symmetric to the enable above — a local-player (listen-server host) target
                    // must go back to disabled hitboxes, otherwise they leak enabled until the
                    // next model spawn and mask the "SCPs can't hit the host" class of bugs.
                    if (targetHub.isLocalPlayer)
                    {
                        SetHitboxes(targetHub, state: false);
                    }

                    targetBacktracker.RestorePosition();
                    targetTransform.rotation = originalRotation;
                }
            }
        }

        protected void ShowHitIndicator(uint netId, float damage, Vector3 origin)
        {
            if (ReferenceHub.TryGetHubNetID(netId, out ReferenceHub hub))
            {
                hub.connectionToClient.Send(new GunHitMessage(drawBlood: false, damage, origin));
            }
        }

        protected void PlaceBulletholeDecal(Ray ray, RaycastHit hit)
        {
            Vector3 point = hit.point + (ray.origin - hit.point).normalized;
            NetworkUtils.SendToAuthenticated(new GunHitMessage(point, ray.direction, isBlood: false));
        }

        protected void PlaceBloodDecal(Ray ray, RaycastHit hit, IDestructible target)
        {
            if (ReferenceHub.TryGetHubNetID(target.NetworkId, out ReferenceHub hub)
                && PlayerRolesUtils.IsHuman(hub))
            {
                Vector3 point = hit.point + (ray.origin - hit.point).normalized;
                NetworkUtils.SendToAuthenticated(new GunHitMessage(point, ray.direction, isBlood: true));
            }
        }

        protected abstract void ServerPerformShot(Ray ray);
    }
}