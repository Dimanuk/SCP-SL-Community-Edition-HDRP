using System;
using Mirror;
using UnityEngine;

namespace PlayerRoles.PlayableScps.Scp079
{
    public class Scp079DoorLockReleaser : Scp079KeyAbilityBase
    {

        private const string ColorFormat = "<color=#ffffff{0}>{1}</color>";
        
        private const float BlinkRate = 2.8f;

        private static string _format;

        private Scp079DoorLockChanger _lockChanger;

        public override ActionName ActivationKey => ActionName.Scp079UnlockAll;

        public override bool IsReady => true;

        public override bool IsVisible => _lockChanger != null && _lockChanger.TotalLocked > 0;

        public override string AbilityName
        {
            get
            {
                string formattedText = string.Format(_format, _lockChanger.TotalLocked);
                return string.Format(ColorFormat, Transparency, formattedText);
            }
        }

        public override string FailMessage => null;

        private string Transparency
        {
            get
            {
                // time * BlinkRate * PI
                float f = Time.timeSinceLevelLoad * BlinkRate * Mathf.PI;
                
                float alpha = Mathf.InverseLerp(-1f, 1f, Mathf.Sin(f)) * 255f;
                int alphaInt = Mathf.RoundToInt(alpha);
                
                return alphaInt.ToString("X2");
            }
        }

        protected override void Trigger()
        {
            ClientSendCmd();
        }

        protected override void Start()
        {
            base.Start();

            GetSubroutine(out _lockChanger);

            _format = Translations.Get(Scp079HudTranslation.ReleaseDoorLocks);
        }

        public override void ServerProcessCmd(NetworkReader reader)
        {
            base.ServerProcessCmd(reader);

            if (_lockChanger != null)
                _lockChanger.ServerUnlockAll();
        }

    }
}
