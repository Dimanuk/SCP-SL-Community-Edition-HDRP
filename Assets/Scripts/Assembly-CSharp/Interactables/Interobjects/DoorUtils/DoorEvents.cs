namespace Interactables.Interobjects.DoorUtils
{
    public static class DoorEvents
    {
        public static event global::System.Action<global::Interactables.Interobjects.DoorUtils.DoorVariant, global::Interactables.Interobjects.DoorUtils.DoorAction, ReferenceHub> OnDoorAction;

        public static void TriggerAction(DoorVariant variant, DoorAction action, ReferenceHub user)
        {
            OnDoorAction?.Invoke(variant, action, user);
        }
    }
}
