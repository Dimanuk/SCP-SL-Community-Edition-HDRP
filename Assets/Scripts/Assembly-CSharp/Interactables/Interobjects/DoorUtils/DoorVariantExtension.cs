namespace Interactables.Interobjects.DoorUtils
{
    public abstract class DoorVariantExtension : global::UnityEngine.MonoBehaviour
    {
        public global::Interactables.Interobjects.DoorUtils.DoorVariant TargetDoor;

        private void OnValidate()
        {
            TargetDoor = GetComponent<global::Interactables.Interobjects.DoorUtils.DoorVariant>();
        }

        private void Awake()
        {
            if (!TargetDoor)
            {
                TargetDoor = GetComponent<global::Interactables.Interobjects.DoorUtils.DoorVariant>();
            }
        }
    }
}
