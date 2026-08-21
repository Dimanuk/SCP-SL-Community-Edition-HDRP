using AudioPooling;
using UnityEngine;

namespace PlayerRoles.PlayableScps.Scp079.GUI
{
    public class Scp079GuiElementBase : MonoBehaviour
    {
        protected Scp079Role Role { get; private set; }
        protected ReferenceHub Owner { get; private set; }

        internal virtual void Init(Scp079Role role, ReferenceHub owner)
        {
            Role = role;
            Owner = owner;
        }

        protected AudioSource PlaySound(AudioClip clip)
        {
            return AudioSourcePoolManager.PlaySound(clip, null, 1f, 1f, FalloffType.Exponential, AudioMixerChannelType.DefaultSfx, 0f);
        }
    }
}