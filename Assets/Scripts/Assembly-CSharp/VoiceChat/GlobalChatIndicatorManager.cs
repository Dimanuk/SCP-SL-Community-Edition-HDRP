namespace VoiceChat
{
    public class GlobalChatIndicatorManager : global::UnityEngine.MonoBehaviour
    {
        [global::UnityEngine.SerializeField]
        private global::VoiceChat.GlobalChatIndicator _template;

        [global::UnityEngine.SerializeField]
        private global::UnityEngine.RectTransform _root;

        private readonly global::System.Collections.Generic.Queue<global::VoiceChat.GlobalChatIndicator> _pool = new global::System.Collections.Generic.Queue<global::VoiceChat.GlobalChatIndicator>();

        private readonly global::System.Collections.Generic.Dictionary<global::VoiceChat.Playbacks.IGlobalPlayback, global::VoiceChat.GlobalChatIndicator> _instances = new global::System.Collections.Generic.Dictionary<global::VoiceChat.Playbacks.IGlobalPlayback, global::VoiceChat.GlobalChatIndicator>();

        private static global::VoiceChat.GlobalChatIndicatorManager _singleton;

        private static bool _singletonSet;

        private void Awake()
        {
            _singleton = this;
            _singletonSet = true;
        }

        private void OnDestroy()
        {
            _singletonSet = false;
        }

        private void LateUpdate()
        {
            foreach (global::System.Collections.Generic.KeyValuePair<global::VoiceChat.Playbacks.IGlobalPlayback, global::VoiceChat.GlobalChatIndicator> instance in _instances)
            {
                instance.Value.Refresh();
            }
        }

        private void SpawnIndicator(global::VoiceChat.Playbacks.IGlobalPlayback igp, ReferenceHub ply)
        {
            if (!CollectionExtensions.TryDequeue(_pool, out var element))
            {
                element = global::UnityEngine.Object.Instantiate(_template);
            }
            global::UnityEngine.Transform obj = element.transform;
            obj.SetParent(_root);
            obj.localScale = global::UnityEngine.Vector3.one;
            obj.SetAsLastSibling();
            element.Setup(igp, ply);
            _instances[igp] = element;
        }

        private void ReturnIndicator(global::VoiceChat.Playbacks.IGlobalPlayback igp)
        {
            if (_instances.TryGetValue(igp, out var value))
            {
                value.gameObject.SetActive(value: false);
                _pool.Enqueue(value);
                _instances.Remove(igp);
            }
        }

        public static void Subscribe(global::VoiceChat.Playbacks.IGlobalPlayback igp, ReferenceHub player)
        {
            _singleton.SpawnIndicator(igp, player);
        }

        public static void Unsubscribe(global::VoiceChat.Playbacks.IGlobalPlayback igp)
        {
            if (_singletonSet)
            {
                _singleton.ReturnIndicator(igp);
            }
        }
    }
}
