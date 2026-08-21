using Interactables.Interobjects;
using UnityEngine;

namespace PlayerRoles.PlayableScps.Scp079.Overcons
{
    public class ElevatorOvercon : StandardOvercon
    {
        private static readonly Color _busyColor = new Color(1f, 1f, 1f, 0.1f);

        public ElevatorDoor Target { get; internal set; }

        private Color TargetColor
        {
            get
            {
                if (!Target.TargetPanel.AssignedChamber.IsReady)
                    return _busyColor;

                if (!IsHighlighted)
                    return NormalColor;

                return HighlightedColor;
            }
        }

        private void LateUpdate()
        {
            TargetSprite.color = TargetColor;
        }
    }
}
