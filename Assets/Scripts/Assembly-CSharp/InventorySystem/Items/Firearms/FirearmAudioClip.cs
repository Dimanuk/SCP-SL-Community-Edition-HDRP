namespace InventorySystem.Items.Firearms
{
    [global::System.Serializable]
    public struct FirearmAudioClip
    {
        public global::UnityEngine.AudioClip Sound;

        public float MaxDistance;

        [global::UnityEngine.SerializeField]
        private global::InventorySystem.Items.Firearms.FirearmAudioFlags _flags;

        public bool HasFlag(global::InventorySystem.Items.Firearms.FirearmAudioFlags flag)
        {
            return (_flags & flag) == flag;
        }
    }
}
