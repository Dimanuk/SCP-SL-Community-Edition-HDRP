using InventorySystem.Items.MicroHID;
using Knife.DeferredDecals;
using Mirror;
using UnityEngine;

namespace InventorySystem.Items.Firearms.BasicMessages
{
    public static class GunHitMessageExtensions
    {
        private const float HoleRayLength = 1.1f;

        private const float BloodRayLength = 5f;

        private static BloodDrawer _bloodDrawer;

        [global::UnityEngine.RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            CustomNetworkManager.OnClientReady += RegisterHandlers;
        }

        private static void RegisterHandlers()
        {
            global::Mirror.NetworkClient.ReplaceHandler<global::InventorySystem.Items.Firearms.BasicMessages.GunHitMessage>(ClientMessageReceived);
        }

        private static void ClientMessageReceived(GunHitMessage msg)
        {
            if (!ReferenceHub.TryGetLocalHub(out ReferenceHub localHub))
                return;

            Vector3 rayOrigin;
            Vector3 rayDirection;

            if (msg.ReceivedDamage > 0)
            {
                // Local player got hit — the message only carries the damage source position.
                if (msg.DamagePosition != Vector3.zero)
                    DamageIndicator.ReceiveDamageFromPosition(msg.DamagePosition, msg.ReceivedDamage);

                rayOrigin = msg.DamagePosition;
                rayDirection = (localHub.transform.position - msg.DamagePosition).normalized;
            }
            else
            {
                rayOrigin = msg.BulletholeOrigin;
                rayDirection = msg.BulletholeDirection;
            }

            if (_bloodDrawer == null && !localHub.TryGetComponent(out _bloodDrawer))
                return;

            if (DeferredDecalsSystem.Singleton == null || !DeferredDecalsSystem.Singleton.EnableDecals)
                return;

            float rayLength = msg.DrawBlood ? 5f : 1.1f;
            if (!Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, rayLength, MicroHIDItem.WallMask))
                return;

            if (msg.DrawBlood)
            {
                _bloodDrawer.DrawBlood(hit, 0.75f, 1.25f);
            }
            else if (DecalPoolManager.Singleton.TrySpawnDecal(DecalPoolType.Bullet, out Decal decal)
                     && decal.TryGetComponent(out BulletHoleController holeController))
            {
                holeController.SetupDecal(hit);
            }
        }

        public static void Serialize(this global::Mirror.NetworkWriter writer, global::InventorySystem.Items.Firearms.BasicMessages.GunHitMessage value)
        {
            int num = value.ReceivedDamage + 1;
            global::Mirror.NetworkWriterExtensions.WriteSByte(writer, (sbyte)(value.DrawBlood ? (-num) : num));
            if (num == 1)
            {
                global::Mirror.NetworkWriterExtensions.WriteVector3(writer, value.BulletholeOrigin);
                global::Mirror.NetworkWriterExtensions.WriteVector3(writer, value.BulletholeDirection);
            }
            else
            {
                global::Mirror.NetworkWriterExtensions.WriteVector3(writer, value.DamagePosition);
            }
        }

        public static global::InventorySystem.Items.Firearms.BasicMessages.GunHitMessage Deserialize(this global::Mirror.NetworkReader reader)
        {
            sbyte num = global::Mirror.NetworkReaderExtensions.ReadSByte(reader);
            bool flag = num < 0;
            int num2 = global::UnityEngine.Mathf.Abs(num) - 1;
            if (num2 != 0)
            {
                return new global::InventorySystem.Items.Firearms.BasicMessages.GunHitMessage(flag, num2, global::Mirror.NetworkReaderExtensions.ReadVector3(reader));
            }
            return new global::InventorySystem.Items.Firearms.BasicMessages.GunHitMessage(global::Mirror.NetworkReaderExtensions.ReadVector3(reader), global::Mirror.NetworkReaderExtensions.ReadVector3(reader), flag);
        }
    }
}