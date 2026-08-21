using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using InventorySystem.Items;
using InventorySystem.Items.Firearms;
using Mirror;
using PlayerRoles;
using Utils.Networking;

namespace InventorySystem.Disarming
{
    public static class DisarmingController
    {
        private const float DisarmingTime = 1.1f;
        private const float DisarmingDistanceSqrt = 10.2f;

        private static KeyCode _disarmKey;
        private static bool _hasTarget;
        private static bool _targetState;
        private static ReferenceHub _newTarget;
        private static readonly Stopwatch Progress = new Stopwatch();

        [RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            Inventory.OnLocalClientStarted += Start;
            StaticUnityMethods.OnUpdate += Update;

            NewInput.OnKeyModified += (an, kc) =>
            {
                if ((int)an == 3)
                {
                    _disarmKey = kc;
                }
            };
        }

        private static void Start()
        {
            _disarmKey = NewInput.GetKey((ActionName)3, KeyCode.None);
        }

        private static void Update()
        {
            if (!StaticUnityMethods.IsPlaying) return;

            if (Input.GetKeyDown(_disarmKey))
            {
                GetNewTarget();
            }

            float fillAmount = 0f;

            if (_hasTarget)
            {
                double elapsedSeconds = Progress.Elapsed.TotalSeconds;
                fillAmount = (float)(elapsedSeconds / DisarmingTime);

                if (fillAmount >= 1f)
                {
                    ConfirmTarget();
                }
                else
                {
                    if (!Input.GetKey(_disarmKey) || !ValidateAll(_newTarget))
                    {
                        CancelTarget();
                    }
                }
            }

            DisarmingGUI singleton = DisarmingGUI.Singleton;
            if (singleton != null && singleton.Circle != null)
            {
                singleton.Circle.fillAmount = Mathf.Clamp01(fillAmount);
                singleton.Circle.enabled = _hasTarget;
            }
        }

        private static void GetNewTarget()
        {
            if (!ReferenceHub.TryGetLocalHub(out ReferenceHub localHub)) return;

            Transform cameraTransform = localHub.PlayerCameraReference;
            Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);

            if (Physics.Raycast(ray, out RaycastHit hit, 5f))
            {
                ReferenceHub targetHub = hit.transform.root.GetComponent<ReferenceHub>();

                if (targetHub != null && targetHub != localHub)
                {
                    bool isTargetDisarmed = DisarmedPlayers.IsDisarmed(targetHub.inventory);

                    if (DisarmedPlayers.IsDisarmed(localHub.inventory)) return;

                    if (localHub.roleManager.CurrentRole.RoleTypeId == RoleTypeId.Spectator) return;
                    if (targetHub.roleManager.CurrentRole.RoleTypeId == RoleTypeId.Spectator) return;

                    if (ValidateAll(targetHub))
                    {
                        _newTarget = targetHub;
                        _hasTarget = true;
                        _targetState = isTargetDisarmed;
                        Progress.Restart();
                    }
                }
            }
        }

        private static void CancelTarget()
        {
            _hasTarget = false;
            _newTarget = null;
            Progress.Stop();
            Progress.Reset();
        }

        private static void ConfirmTarget()
        {
            Progress.Stop();

            if (_newTarget != null)
            {
                bool currentDisarmedState = DisarmedPlayers.IsDisarmed(_newTarget.inventory);

                if (_targetState == currentDisarmedState)
                {
                    NetworkClient.Send(new DisarmMessage(_newTarget, !_targetState, false));
                }
            }

            CancelTarget();
        }

        private static bool ValidateAll(ReferenceHub hub)
        {
            if (hub == null) return false;

            if (!ValidateDistance(hub)) return false;

            if (!ReferenceHub.TryGetLocalHub(out ReferenceHub localHub)) return false;
            if (!ValidateHeldItem(localHub.inventory)) return false;

            return true;
        }

        private static bool ValidateDistance(ReferenceHub hub)
        {
            if (hub == null || !ReferenceHub.TryGetLocalHub(out ReferenceHub localHub)) return false;

            float sqrDistance = (localHub.transform.position - hub.transform.position).sqrMagnitude;
            return sqrDistance <= DisarmingDistanceSqrt;
        }

        private static bool ValidateHeldItem(Inventory inv)
        {
            if (inv.CurItem.TypeId == ItemType.None) return false;

            Dictionary<ItemType, ItemBase> availableItems = InventoryItemLoader.AvailableItems;

            if (availableItems.TryGetValue(inv.CurItem.TypeId, out ItemBase item))
            {
                if (item is IDisarmingItem disarmingItem)
                {
                    return disarmingItem.AllowDisarming;
                }
            }

            return false;
        }
    }
}