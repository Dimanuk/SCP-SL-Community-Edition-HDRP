namespace InventorySystem.Items.Firearms.BasicMessages
{
    public struct GunHitMessage : global::Mirror.NetworkMessage
    {
        public int ReceivedDamage;

        public global::UnityEngine.Vector3 DamagePosition;

        public bool DrawBlood;

        public global::UnityEngine.Vector3 BulletholeOrigin;

        public global::UnityEngine.Vector3 BulletholeDirection;

        public GunHitMessage(bool drawBlood, float receivedDamage, global::UnityEngine.Vector3 dmgPosition)
        {
            ReceivedDamage = global::UnityEngine.Mathf.RoundToInt(global::UnityEngine.Mathf.Clamp(receivedDamage, 0f, 100f));
            DamagePosition = dmgPosition;
            DrawBlood = drawBlood;
            BulletholeOrigin = global::UnityEngine.Vector3.zero;
            BulletholeDirection = global::UnityEngine.Vector3.zero;
        }

        public GunHitMessage(global::UnityEngine.Vector3 origin, global::UnityEngine.Vector3 direction, bool isBlood)
        {
            ReceivedDamage = 0;
            DamagePosition = global::UnityEngine.Vector3.zero;
            DrawBlood = isBlood;
            BulletholeOrigin = origin;
            BulletholeDirection = direction;
        }
    }
}
