namespace PlayerRoles.Spectating
{
    [global::System.Serializable]
    public struct SpectatableListSpawnedElement
    {
        public int Priority;

        public global::PlayerRoles.Spectating.SpectatableListElementBase FullSize;

        public global::PlayerRoles.Spectating.SpectatableListElementBase Compact;

        public global::PlayerRoles.Spectating.SpectatableModuleBase Target;

        public void ReturnToPool()
        {
            global::GameObjectPools.PoolManager.Singleton.ReturnPoolObject(FullSize);
            global::GameObjectPools.PoolManager.Singleton.ReturnPoolObject(Compact);
        }
    }
}
