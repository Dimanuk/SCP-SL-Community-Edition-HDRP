namespace PlayerRoles.PlayableScps.Scp079
{
    public abstract class Scp079InteractableBase : global::UnityEngine.MonoBehaviour
    {
        public static readonly global::System.Collections.Generic.List<global::PlayerRoles.PlayableScps.Scp079.Scp079InteractableBase> OrderedInstances = new global::System.Collections.Generic.List<global::PlayerRoles.PlayableScps.Scp079.Scp079InteractableBase>();

        public static readonly global::System.Collections.Generic.HashSet<global::PlayerRoles.PlayableScps.Scp079.Scp079InteractableBase> AllInstances = new global::System.Collections.Generic.HashSet<global::PlayerRoles.PlayableScps.Scp079.Scp079InteractableBase>();

        private static int _instancesCount;

        public ushort SyncId { get; private set; }

        public global::UnityEngine.Vector3 Position { get; private set; }

        public virtual global::MapGeneration.RoomIdentifier Room { get; private set; }

        protected virtual void OnRegistered()
        {
            Room = global::MapGeneration.RoomIdUtils.RoomAtPositionRaycasts(base.transform.position);
        }

        protected virtual void Awake()
        {
            AllInstances.Add(this);
        }

        protected virtual void OnDestroy()
        {
            if (AllInstances.Remove(this))
            {
                OrderedInstances.Remove(this);
            }
        }

        public override string ToString()
        {
            string text = ((base.transform.parent == null) ? "null" : base.transform.parent.name);
            return GetType().Name + " @ (" + base.transform.root.name + "/.../" + text + "/" + base.name + ")";
        }

        public static bool TryGetInteractable(ushort syncId, out global::PlayerRoles.PlayableScps.Scp079.Scp079InteractableBase result)
        {
            if (syncId == 0 || syncId > _instancesCount || !global::MapGeneration.SeedSynchronizer.MapGenerated)
            {
                result = null;
                return false;
            }
            result = OrderedInstances[syncId - 1];
            return true;
        }

        public static bool TryGetInteractable<T>(ushort syncId, out T result) where T : global::PlayerRoles.PlayableScps.Scp079.Scp079InteractableBase
        {
            if (!TryGetInteractable(syncId, out var result2) || !(result2 is T val))
            {
                result = null;
                return false;
            }
            result = val;
            return true;
        }

        [global::UnityEngine.RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            global::MapGeneration.SeedSynchronizer.OnMapGenerated += RegisterIds;
        }

        private static void RegisterIds()
        {
            AllInstances.RemoveWhere((global::PlayerRoles.PlayableScps.Scp079.Scp079InteractableBase x) => !x.gameObject.activeInHierarchy);
            OrderedInstances.Clear();
            _instancesCount = 0;
            foreach (global::PlayerRoles.PlayableScps.Scp079.Scp079InteractableBase allInstance in AllInstances)
            {
                HandleInstance(allInstance);
            }
            for (ushort num = 1; num <= _instancesCount; num++)
            {
                global::PlayerRoles.PlayableScps.Scp079.Scp079InteractableBase scp079InteractableBase = OrderedInstances[num - 1];
                scp079InteractableBase.SyncId = num;
                scp079InteractableBase.OnRegistered();
            }
        }

        private static void HandleInstance(global::PlayerRoles.PlayableScps.Scp079.Scp079InteractableBase instance)
        {
            instance.Position = instance.transform.position;
            for (int i = 0; i < _instancesCount; i++)
            {
                if (CheckPriority(instance, OrderedInstances[i]))
                {
                    OrderedInstances.Insert(i, instance);
                    _instancesCount++;
                    return;
                }
            }
            OrderedInstances.Add(instance);
            _instancesCount++;
        }

        private static bool CheckPriority(global::PlayerRoles.PlayableScps.Scp079.Scp079InteractableBase target, global::PlayerRoles.PlayableScps.Scp079.Scp079InteractableBase other)
        {
            global::UnityEngine.Vector3 position = target.Position;
            global::UnityEngine.Vector3 position2 = other.Position;
            for (int i = 0; i < 3; i++)
            {
                if (!global::UnityEngine.Mathf.Approximately(position[i], position2[i]))
                {
                    return position[i] < position2[i];
                }
            }
            throw new global::System.InvalidOperationException($"Position signature collision detected between {target} and {other}!");
        }
    }
}
