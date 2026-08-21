using System.Collections.Generic;
using System.Runtime.CompilerServices;

using GameObjectPools;
using UnityEngine;
using VoiceChat;
using VoiceChat.Playbacks;

namespace InventorySystem.Items.Usables.Scp1576
{
	public class Scp1576Playback : SingleBufferPlayback, IGlobalPlayback, IPoolResettable, IPoolSpawnable
	{
        private static readonly global::System.Collections.Generic.Queue<global::InventorySystem.Items.Usables.Scp1576.Scp1576Playback> Pool = new global::System.Collections.Generic.Queue<global::InventorySystem.Items.Usables.Scp1576.Scp1576Playback>();

        private static readonly global::System.Collections.Generic.Dictionary<global::InventorySystem.Items.Usables.Scp1576.Scp1576Source, global::System.Collections.Generic.Dictionary<ReferenceHub, global::InventorySystem.Items.Usables.Scp1576.Scp1576Playback>> ActiveInstances = new global::System.Collections.Generic.Dictionary<global::InventorySystem.Items.Usables.Scp1576.Scp1576Source, global::System.Collections.Generic.Dictionary<ReferenceHub, global::InventorySystem.Items.Usables.Scp1576.Scp1576Playback>>();

        private static bool _anyCreated;

		private static bool _templateCacheSet;

		private static Scp1576Playback _templateCache;

		[SerializeField]
		private bool _playerMode;

		private Transform _tr;

		private Scp1576Source _source;

		private bool _spawned;

        public static global::InventorySystem.Items.Usables.Scp1576.Scp1576Playback Template
        {
            get
            {
                if (_templateCacheSet)
                {
                    return _templateCache;
                }
                if (!global::InventorySystem.InventoryItemLoader.TryGetItem<global::InventorySystem.Items.Usables.Scp1576.Scp1576Item>(ItemType.SCP1576, out var result))
                {
                    throw new global::System.InvalidOperationException("SCP-1576 template not found!");
                }
                _templateCache = result.PlaybackTemplate;
                _templateCacheSet = true;
                return _templateCache;
            }
        }

        public ReferenceHub Owner { get; set; }

        public virtual bool GlobalChatActive
        {
            get
            {
                if (!_spawned || !_source.HideGlobalIndicator)
                {
                    return MaxSamples > 0;
                }
                return false;
            }
        }

        public virtual global::UnityEngine.Color GlobalChatColor => Owner.serverRoles.GetVoiceColor();

        public virtual string GlobalChatName => Owner.nicknameSync.DisplayName;

        public virtual float GlobalChatLoudness => base.Loudness;

        public global::VoiceChat.Playbacks.GlobalChatIconType GlobalChatIcon => global::VoiceChat.Playbacks.GlobalChatIconType.Avatar;

        public void SpawnObject()
        {
            if (_playerMode && base.transform.parent.TryGetComponent<global::PlayerRoles.HumanRole>(out var component) && component.TryGetOwner(out var hub))
            {
                Owner = hub;
                global::VoiceChat.GlobalChatIndicatorManager.Subscribe(this, Owner);
            }
        }

        public void ResetObject()
		{
			if (_playerMode)
			{
				GlobalChatIndicatorManager.Unsubscribe(this);
			}
		}

        public override void Awake()
        {
            base.Awake();
            _tr = base.transform;
        }

        private void OnDestroy()
        {
            if (_anyCreated)
            {
                Pool.Clear();
                ActiveInstances.Clear();
                _anyCreated = false;
            }
        }

        private void LateUpdate()
        {
            if (_spawned)
            {
                _tr.position = _source.Position;
            }
        }

        public static void DistributeSamples(ReferenceHub speaker, float[] samples, int len)
        {
            foreach (global::InventorySystem.Items.Usables.Scp1576.Scp1576Source instance in global::InventorySystem.Items.Usables.Scp1576.Scp1576Source.Instances)
            {
                GetOrAdd(speaker, instance).Buffer.Write(samples, len);
            }
        }

        private static global::InventorySystem.Items.Usables.Scp1576.Scp1576Playback GetOrAdd(ReferenceHub player, global::InventorySystem.Items.Usables.Scp1576.Scp1576Source source)
        {
            if (ActiveInstances.TryGetValue(source, out var value) && value.TryGetValue(player, out var value2))
            {
                return value2;
            }
            if (!CollectionExtensions.TryDequeue(Pool, out value2))
            {
                value2 = global::UnityEngine.Object.Instantiate(Template);
                _anyCreated = true;
            }
            value2._spawned = true;
            value2.Owner = player;
            value2._source = source;
            ActiveInstances.GetOrAdd(source, () => new global::System.Collections.Generic.Dictionary<ReferenceHub, global::InventorySystem.Items.Usables.Scp1576.Scp1576Playback>())[player] = value2;
            global::VoiceChat.GlobalChatIndicatorManager.Subscribe(value2, player);
            return value2;
        }

        private static void ReturnToPool(global::InventorySystem.Items.Usables.Scp1576.Scp1576Playback playback)
        {
            if (ActiveInstances.TryGetValue(playback._source, out var value))
            {
                value.Remove(playback.Owner);
            }
            Pool.Enqueue(playback);
            playback._spawned = false;
            global::VoiceChat.GlobalChatIndicatorManager.Unsubscribe(playback);
        }

        [global::UnityEngine.RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            ReferenceHub.OnPlayerRemoved = (global::System.Action<ReferenceHub>)global::System.Delegate.Combine(ReferenceHub.OnPlayerRemoved, (global::System.Action<ReferenceHub>)delegate (ReferenceHub hub)
            {
                global::Utils.NonAllocLINQ.DictionaryExtensions.ForEachValue(ActiveInstances, delegate (global::System.Collections.Generic.Dictionary<ReferenceHub, global::InventorySystem.Items.Usables.Scp1576.Scp1576Playback> x)
                {
                    if (x.TryGetValue(hub, out var value))
                    {
                        x.Remove(hub);
                        ReturnToPool(value);
                    }
                });
            });
            global::InventorySystem.Items.Usables.Scp1576.Scp1576Source.OnRemoved = (global::System.Action<global::InventorySystem.Items.Usables.Scp1576.Scp1576Source>)global::System.Delegate.Combine(global::InventorySystem.Items.Usables.Scp1576.Scp1576Source.OnRemoved, (global::System.Action<global::InventorySystem.Items.Usables.Scp1576.Scp1576Source>)delegate (global::InventorySystem.Items.Usables.Scp1576.Scp1576Source src)
            {
                if (ActiveInstances.TryGetValue(src, out var value))
                {
                    ActiveInstances.Remove(src);
                    global::Utils.NonAllocLINQ.DictionaryExtensions.ForEachValue(value, ReturnToPool);
                }
            });
        }
	}
}
