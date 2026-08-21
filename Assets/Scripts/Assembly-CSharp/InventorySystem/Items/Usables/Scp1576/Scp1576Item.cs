using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

using InventorySystem.Items.Pickups;
using UnityEngine;

namespace InventorySystem.Items.Usables.Scp1576
{
	public class Scp1576Item : UsableItem
	{
		public const float TransmissionDuration = 30f;

		public const float UseCooldown = 120f;

		public const float HornReturnSpeed = 0.4f;

		public const float HornReturnDelay = 1.1f;

		public const float SqrAudibleRange = 110f;

		public const float WarningDuration = 2f;

        [field: SerializeField]
        public Scp1576Playback PlaybackTemplate { get; private set; }

        public static global::System.Collections.Generic.HashSet<ReferenceHub> ValidatedTransmitters = new global::System.Collections.Generic.HashSet<ReferenceHub>();

        public static global::System.Collections.Generic.HashSet<ReferenceHub> ValidatedReceivers = new global::System.Collections.Generic.HashSet<ReferenceHub>();

        private static readonly global::System.Collections.Generic.List<global::UnityEngine.Vector3> ActiveNonAllocPositions = new global::System.Collections.Generic.List<global::UnityEngine.Vector3>(8);

        private static bool _locallyUsed;

		private static bool _eventAssigned;

		[SerializeField]
		private AudioClip _warningStart;

		[SerializeField]
		private AudioClip _warningStop;

		private float _serverHornPos;

		private bool _startWarningTriggered;

		private readonly Stopwatch _useStopwatch = new Stopwatch();

        public static bool LocallyUsed
        {
            get
            {
                return _locallyUsed;
            }
            internal set
            {
                _locallyUsed = value;
                if (value != _eventAssigned)
                {
                    if (value)
                    {
                        StaticUnityMethods.OnUpdate += ContinueCheckingLocalUse;
                        _eventAssigned = true;
                    }
                    else
                    {
                        StaticUnityMethods.OnUpdate -= ContinueCheckingLocalUse;
                        _eventAssigned = false;
                    }
                }
            }
        }

        public override bool AllowHolster => true;

        public override void ServerOnUsingCompleted()
        {
            ValidatedTransmitters.Add(base.Owner);
        }

        public override void OnUsingStarted()
		{
			base.OnUsingStarted();
			_useStopwatch.Restart();
			_startWarningTriggered = true;
		}

		public override void OnUsingCancelled()
		{
			base.OnUsingCancelled();
			_useStopwatch.Reset();
		}

		public override bool ServerValidateStartRequest(PlayerHandler handler)
		{
			return !_useStopwatch.IsRunning;
		}

        public override bool ServerValidateCancelRequest(global::InventorySystem.Items.Usables.PlayerHandler handler)
        {
            if (handler.CurrentUsable.ItemSerial == base.ItemSerial || !_useStopwatch.IsRunning)
            {
                return true;
            }
            ServerStopTransmitting();
            return false;
        }

        public override void OnAdded(global::InventorySystem.Items.Pickups.ItemPickupBase pickup)
        {
            base.OnAdded(pickup);
            if (pickup is global::InventorySystem.Items.Usables.Scp1576.Scp1576Pickup scp1576Pickup && !(scp1576Pickup == null))
            {
                _serverHornPos = scp1576Pickup.HornPos;
            }
        }

        public override void OnRemoved(global::InventorySystem.Items.Pickups.ItemPickupBase pickup)
        {
            base.OnRemoved(pickup);
            if (pickup is global::InventorySystem.Items.Usables.Scp1576.Scp1576Pickup scp1576Pickup && !(scp1576Pickup == null))
            {
                scp1576Pickup.HornPos = _serverHornPos;
            }
        }

        public override void EquipUpdate()
        {
            base.EquipUpdate();
            if (!global::Mirror.NetworkServer.active)
            {
                return;
            }
            float num = 30f + UseTime;
            double totalSeconds = _useStopwatch.Elapsed.TotalSeconds;
            if (totalSeconds < 1.100000023841858)
            {
                return;
            }
            if (totalSeconds < (double)UseTime)
            {
                if (_startWarningTriggered && (double)UseTime - totalSeconds < 2.0)
                {
                    _startWarningTriggered = false;
                    global::InventorySystem.Items.Usables.Scp1576.Scp1576SpectatorWarningHandler.TriggerStart(this);
                }
                _serverHornPos = global::UnityEngine.Mathf.Max(_serverHornPos - global::UnityEngine.Time.deltaTime * 0.4f, 0f);
            }
            else if (totalSeconds < (double)num)
            {
                _serverHornPos = global::UnityEngine.Mathf.Clamp01((float)(totalSeconds - (double)UseTime) / num);
            }
            else
            {
                _serverHornPos = 1f;
                ServerStopTransmitting();
            }
        }

        public override void OnHolstered()
        {
            base.OnHolstered();
            if (_useStopwatch.IsRunning)
            {
                ServerStopTransmitting();
            }
        }

        internal override void OnTemplateReloaded(bool wasEverLoaded)
        {
            base.OnTemplateReloaded(wasEverLoaded);
            if (!wasEverLoaded)
            {
                global::InventorySystem.Items.Usables.Scp1576.Scp1576SpectatorWarningHandler.OnStop += delegate
                {
                    PlayWarningSound(_warningStop);
                };
                global::InventorySystem.Items.Usables.Scp1576.Scp1576SpectatorWarningHandler.OnStart += delegate
                {
                    PlayWarningSound(_warningStart);
                };
            }
        }

        private void ServerStopTransmitting()
        {
            _useStopwatch.Reset();
            ValidatedTransmitters.Remove(base.Owner);
            ServerSetGlobalItemCooldown(120f);
            global::InventorySystem.Items.Usables.Scp1576.Scp1576SpectatorWarningHandler.TriggerStop(this);
            global::Utils.Networking.NetworkUtils.SendToAuthenticated(new global::InventorySystem.Items.Usables.StatusMessage(global::InventorySystem.Items.Usables.StatusMessage.StatusType.Cancel, base.ItemSerial));
            base.Owner.connectionToClient.Send(new global::InventorySystem.Items.Usables.ItemCooldownMessage(base.ItemSerial, 120f));
        }

        private static void PlayWarningSound(global::UnityEngine.AudioClip sound)
        {
            if (global::PlayerRoles.Spectating.SpectatorTargetTracker.TrackerSet)
            {
                global::UnityEngine.Transform trackedObject = global::PlayerRoles.Spectating.SpectatorTargetTracker.Singleton.transform;
                global::AudioPooling.AudioSourcePoolManager.PlaySound(sound, trackedObject, 1f, 1f, FalloffType.Exponential, global::AudioPooling.AudioMixerChannelType.VoiceChat, 0f);
            }
        }

        private static void ContinueCheckingLocalUse()
        {
            if (!ReferenceHub.TryGetLocalHub(out var hub) || hub.inventory.CurInstance is not global::InventorySystem.Items.Usables.Scp1576.Scp1576Item scp1576Item || !(scp1576Item != null))
            {
                LocallyUsed = false;
            }
        }

        [global::UnityEngine.RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            StaticUnityMethods.OnUpdate += RevalidateReceivers;
            ReferenceHub.OnPlayerRemoved = (global::System.Action<ReferenceHub>)global::System.Delegate.Combine(ReferenceHub.OnPlayerRemoved, (global::System.Action<ReferenceHub>)delegate (ReferenceHub hub)
            {
                if (global::Mirror.NetworkServer.active)
                {
                    ValidatedReceivers.Remove(hub);
                    ValidatedTransmitters.Remove(hub);
                }
            });
            global::InventorySystem.Inventory.OnServerStarted += ValidatedTransmitters.Clear;
        }

        private static void RevalidateReceivers()
        {
            if (!StaticUnityMethods.IsPlaying || !global::Mirror.NetworkServer.active)
            {
                return;
            }
            ValidatedReceivers.Clear();
            ActiveNonAllocPositions.Clear();
            foreach (ReferenceHub validatedTransmitter in ValidatedTransmitters)
            {
                if (validatedTransmitter.roleManager.CurrentRole is global::PlayerRoles.FirstPersonControl.IFpcRole fpcRole)
                {
                    ActiveNonAllocPositions.Add(fpcRole.FpcModule.Position);
                }
            }
            int count = ActiveNonAllocPositions.Count;
            if (count == 0)
            {
                return;
            }
            foreach (ReferenceHub allHub in ReferenceHub.AllHubs)
            {
                if (allHub.roleManager.CurrentRole is not global::PlayerRoles.FirstPersonControl.IFpcRole fpcRole2)
                {
                    if (global::PlayerRoles.PlayerRolesUtils.IsAlive(allHub))
                    {
                        ValidatedReceivers.Add(allHub);
                    }
                    continue;
                }
                global::UnityEngine.Vector3 position = fpcRole2.FpcModule.Position;
                for (int i = 0; i < count; i++)
                {
                    if (!((position - ActiveNonAllocPositions[i]).sqrMagnitude > 110f))
                    {
                        ValidatedReceivers.Add(allHub);
                        break;
                    }
                }
            }
        }
	}
}
