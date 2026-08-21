namespace PlayerRoles.Spectating
{
    [global::System.Serializable]
    public struct SpectatableListElementDefinition
    {
        public global::PlayerRoles.Spectating.SpectatableListElementType Type;

        public global::PlayerRoles.Spectating.SpectatableListElementBase FullSize;

        public global::PlayerRoles.Spectating.SpectatableListElementBase Compact;

        public bool TryGetFromPools(global::UnityEngine.Transform parent, out global::PlayerRoles.Spectating.SpectatableListElementBase full, out global::PlayerRoles.Spectating.SpectatableListElementBase compact)
        {
            if (TrySpawn(parent, FullSize, out full) && TrySpawn(parent, Compact, out compact))
            {
                return true;
            }
            full = null;
            compact = null;
            return false;
        }

        private bool TrySpawn(global::UnityEngine.Transform parent, global::PlayerRoles.Spectating.SpectatableListElementBase template, out global::PlayerRoles.Spectating.SpectatableListElementBase instance)
        {
            if (global::GameObjectPools.PoolManager.Singleton.TryGetPoolObject(template.gameObject, parent, out var poolObject))
            {
                instance = poolObject as global::PlayerRoles.Spectating.SpectatableListElementBase;
                return true;
            }
            instance = null;
            return false;
        }
    }
}
