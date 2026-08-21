using UnityEngine;

namespace Discord.Modules
{
    public abstract class DiscordModuleBase : MonoBehaviour
    {
        [SerializeField]
        protected bool _isEnabled = true;

        public virtual bool IsEnabled
        {
            get => _isEnabled;
            set => _isEnabled = value;
        }

        public void UpdateModule()
        {
            if (IsEnabled)
            {
                OnUpdateModule();
            }
        }

        protected virtual void OnUpdateModule()
        {
        }

        protected virtual void OnDestroy()
        {
            // base.stop() - FMOD.IChannelControl::stop()
        }
    }
}
