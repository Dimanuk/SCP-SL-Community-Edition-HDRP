namespace InventorySystem.Items.Usables.Scp330
{
    public class CandyPink : global::InventorySystem.Items.Usables.Scp330.ICandy
    {
        public struct CandyExplosionMessage : global::Mirror.NetworkMessage
        {
            public global::UnityEngine.Vector3 Origin;
        }

        public global::InventorySystem.Items.Usables.Scp330.CandyKindID Kind => global::InventorySystem.Items.Usables.Scp330.CandyKindID.Pink;

        public float SpawnChanceWeight => 0f;

        public void ServerApplyEffects(ReferenceHub hub)
        {
            if (TryGetGrenade(out var grenade))
            {
                global::Utils.Networking.NetworkUtils.SendToAuthenticated(new global::InventorySystem.Items.Usables.Scp330.CandyPink.CandyExplosionMessage
                {
                    Origin = hub.transform.position
                });
                global::InventorySystem.Items.ThrowableProjectiles.ExplosionGrenade.Explode(new global::Footprinting.Footprint(hub), hub.transform.position, grenade);
            }
        }

        private static void MessageReceived(global::InventorySystem.Items.Usables.Scp330.CandyPink.CandyExplosionMessage msg)
        {
            if (!TryGetGrenade(out var grenade))
            {
                return;
            }
            global::UnityEngine.GameObject effectInstance = global::UnityEngine.Object.Instantiate(grenade.Effect);
            effectInstance.transform.position = msg.Origin;
            effectInstance.transform.rotation = grenade.Effect.transform.localRotation;
            effectInstance.transform.localScale = global::UnityEngine.Vector3.one;
            global::UnityEngine.Object.Destroy(effectInstance, 5f);
        }

        private static bool TryGetGrenade(out global::InventorySystem.Items.ThrowableProjectiles.ExplosionGrenade grenade)
        {
            grenade = null;
            if (!global::InventorySystem.InventoryItemLoader.AvailableItems.TryGetValue(ItemType.GrenadeHE, out var value))
            {
                return false;
            }
            if (!(value is global::InventorySystem.Items.ThrowableProjectiles.ThrowableItem throwableItem) || !(throwableItem.Projectile is global::InventorySystem.Items.ThrowableProjectiles.ExplosionGrenade explosionGrenade))
            {
                return false;
            }
            grenade = explosionGrenade;
            return true;
        }

        [global::UnityEngine.RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            CustomNetworkManager.OnClientReady += delegate
            {
                global::Mirror.NetworkClient.ReplaceHandler<global::InventorySystem.Items.Usables.Scp330.CandyPink.CandyExplosionMessage>(MessageReceived);
            };
        }
    }
}
