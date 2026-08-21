namespace CustomPlayerEffects
{
    public class Traumatized : global::CustomPlayerEffects.StatusEffectBase, global::CustomPlayerEffects.IHealablePlayerEffect
    {
        public bool IsHealable(ItemType item)
        {
            return item == ItemType.SCP500;
        }
    }
}
