using PlayerRoles.PlayableScps.Scp079.GUI;
using UnityEngine;

namespace PlayerRoles.PlayableScps.Scp079
{
    public abstract class Scp079KeyAbilityBase : Scp079AbilityBase, IScp079FailMessageProvider
    {
        private enum Category
        {
            Movement = 0,
            SpecialAbility = 1,
            OverconInteraction = 2
        }

        [SerializeField]
        private Category _category;

        protected static string _translationNoAux;

        private static Object TrackedFailMessage
        {
            get => Scp079AbilityList.Singleton.TrackedFailMessage as Object;
            set => Scp079AbilityList.Singleton.TrackedFailMessage = value as IScp079FailMessageProvider;
        }

        public abstract ActionName ActivationKey { get; }
        public abstract bool IsReady { get; }
        public abstract bool IsVisible { get; }
        public abstract string AbilityName { get; }
        public abstract string FailMessage { get; }

        public int CategoryId => (int)_category;

        protected string GetNoAuxMessage(float cost)
        {
            Scp079AuxManager aux = base.AuxManager;
            string noAux = _translationNoAux;
            int tier = aux._tierManager.AccessTierIndex;

            if (cost <= aux._maxPerTier[tier])
            {
                float regen = aux.RegenSpeed;
                if (regen > 0f)
                {
                    float remaining = Mathf.Max(0f, cost - aux.CurrentAux);
                    int seconds = Mathf.CeilToInt(remaining / regen);
                    return noAux + "\n" + aux.GenerateCustomETA(seconds);
                }
                return noAux + "\n" + string.Empty;
            }

            return noAux + "\n" + Scp079AuxManager._textHigherTierRequired;
        }

        protected virtual void Start()
        {
            _translationNoAux = Translations.Get(Scp079HudTranslation.NotEnoughAux);
        }

        protected virtual void Update()
        {
            if (!base.Role.IsLocalPlayer || !IsVisible || !Input.GetKeyDown(NewInput.GetKey(ActivationKey)) || base.LostSignalHandler.Lost)
                return;

            if (IsReady)
            {
                if (TrackedFailMessage == this)
                    TrackedFailMessage = null;

                Trigger();
            }
            else
            {
                TrackedFailMessage = this;
            }
        }

        protected abstract void Trigger();

        public virtual void OnFailMessageAssigned() { }
    }
}