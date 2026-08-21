namespace Interactables.Interobjects.DoorUtils
{
    public class DoorEventOpenerExtension : global::Interactables.Interobjects.DoorUtils.DoorVariantExtension
    {
        public enum OpenerEventType
        {
            WarheadStart = 0,
            WarheadCancel = 1,
            DeconEvac = 2,
            DeconFinish = 3,
            DeconReset = 4
        }

        private static bool _lockGates;

        private static bool _isolateCheckpoints;

        private static bool _configLoaded;

        public static event global::System.Action<global::Interactables.Interobjects.DoorUtils.DoorEventOpenerExtension.OpenerEventType> OnDoorsTriggerred;

        public static void TriggerAction(global::Interactables.Interobjects.DoorUtils.DoorEventOpenerExtension.OpenerEventType eventType)
        {
            global::Interactables.Interobjects.DoorUtils.DoorEventOpenerExtension.OnDoorsTriggerred(eventType);
        }

        private void Start()
        {
            OnDoorsTriggerred += Trigger;
            _configLoaded = false;
        }

        private void OnDestroy()
        {
            OnDoorsTriggerred -= Trigger;
        }

        private void Trigger(global::Interactables.Interobjects.DoorUtils.DoorEventOpenerExtension.OpenerEventType eventType)
        {
            if (!global::Mirror.NetworkServer.active)
            {
                return;
            }
            if (!_configLoaded)
            {
                _configLoaded = true;
                _lockGates = global::GameCore.ConfigFile.ServerConfig.GetBool("lock_gates_on_countdown", def: true);
                _isolateCheckpoints = global::GameCore.ConfigFile.ServerConfig.GetBool("isolate_zones_on_countdown");
            }
            bool flag = base.transform.position.y > -100f && base.transform.position.y < 100f;
            switch ((int)eventType)
            {
                case 0:
                    {
                        global::Interactables.Interobjects.DoorUtils.DoorNametagExtension component;
                        if (_isolateCheckpoints && TargetDoor is global::Interactables.Interobjects.CheckpointDoor checkpointDoor)
                        {
                            checkpointDoor.ServerChangeLock(global::Interactables.Interobjects.DoorUtils.DoorLockReason.Isolation, newState: true);
                        }
                        else if (_lockGates || !(TargetDoor is global::Interactables.Interobjects.PryableDoor) || !TryGetComponent<global::Interactables.Interobjects.DoorUtils.DoorNametagExtension>(out component) || !component.GetName.Contains("GATE"))
                        {
                            TargetDoor.TargetState = true;
                            TargetDoor.ServerChangeLock(global::Interactables.Interobjects.DoorUtils.DoorLockReason.Warhead, newState: true);
                        }
                        break;
                    }
                case 1:
                    TargetDoor.ServerChangeLock(global::Interactables.Interobjects.DoorUtils.DoorLockReason.Warhead, newState: false);
                    TargetDoor.ServerChangeLock(global::Interactables.Interobjects.DoorUtils.DoorLockReason.Isolation, newState: false);
                    break;
                case 2:
                    if (flag)
                    {
                        TargetDoor.TargetState = true;
                        if (TargetDoor is global::Interactables.Interobjects.CheckpointDoor)
                        {
                            TargetDoor.ServerChangeLock(global::Interactables.Interobjects.DoorUtils.DoorLockReason.DecontEvacuate, newState: true);
                        }
                    }
                    break;
                case 3:
                    if (flag)
                    {
                        TargetDoor.TargetState = false;
                        TargetDoor.ServerChangeLock(global::Interactables.Interobjects.DoorUtils.DoorLockReason.DecontEvacuate, newState: false);
                        TargetDoor.ServerChangeLock(global::Interactables.Interobjects.DoorUtils.DoorLockReason.DecontLockdown, newState: true);
                    }
                    break;
                case 4:
                    if (flag)
                    {
                        TargetDoor.ServerChangeLock(global::Interactables.Interobjects.DoorUtils.DoorLockReason.DecontEvacuate, newState: false);
                        TargetDoor.ServerChangeLock(global::Interactables.Interobjects.DoorUtils.DoorLockReason.DecontLockdown, newState: false);
                    }
                    break;
            }
        }

        static DoorEventOpenerExtension()
        {
            global::Interactables.Interobjects.DoorUtils.DoorEventOpenerExtension.OnDoorsTriggerred = delegate
            {
            };
        }
    }
}
