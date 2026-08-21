using System;
using Interactables.Interobjects.DoorUtils;
using MapGeneration;
using PlayerRoles.PlayableScps.Scp079.Cameras;
using UnityEngine;
using Utils;

namespace PlayerRoles.PlayableScps.Scp079.Overcons
{
    public class CameraOvercon : StandardOvercon
    {
        [Serializable]
        private struct ZoneOverrride
        {
            public FacilityZone Zone;
            public Sprite Icon;
            public Vector3 Offset;
        }

        private const float ColorSelectorTarget = 0.3f;
        private const float ExternalCamHeight = 3.2f;

        [SerializeField]
        private Sprite _defaultIcon;

        [SerializeField]
        private Vector3 _defaultOffset;

        [SerializeField]
        private ZoneOverrride[] _zoneOverrides;

        [SerializeField]
        private GameObject _externalIcon;

        private FacilityZone _prevZone;
        private Vector3 _prevOffset;
        private Vector3 _position;

        public Scp079Camera Target { get; private set; }
        public bool IsElevator { get; private set; }

        public Vector3 Position
        {
            get => _position;
            set
            {
                _position = value;
                transform.position = value;
            }
        }

        internal void Setup(Scp079Camera newCam, Scp079Camera target, bool isElevator)
        {
            Target = target;
            IsElevator = isElevator;

            FacilityZone zone = Target.Room.Zone;
            if (zone != _prevZone)
            {
                GetZoneOverrides(zone, out Sprite icon, out _prevOffset);
                TargetSprite.sprite = icon;
                _prevZone = zone;
            }

            Vector3 position = target.transform.TransformPoint(_prevOffset);

            if (newCam.Room == Target.Room)
            {
                _externalIcon.SetActive(isElevator);
                Position = position;
                Rescale(newCam);
                return;
            }

            if (DoorVariant.DoorsByRoom.TryGetValue(Target.Room, out var doors)
                && Utils.NonAllocLINQ.HashsetExtensions.TryGetFirst(doors, x => x.Rooms.Contains(newCam.Room), out var firstDoor))
            {
                Position = firstDoor.transform.position + Vector3.up * ExternalCamHeight;
                Rescale(newCam, 255f);
            }
            else
            {
                Position = position;
                Rescale(newCam);
            }

            _externalIcon.SetActive(true);
        }

        private void GetZoneOverrides(FacilityZone zone, out Sprite icon, out Vector3 offset)
        {
            foreach (ZoneOverrride zoneOverride in _zoneOverrides)
            {
                if (zoneOverride.Zone == zone)
                {
                    icon = zoneOverride.Icon;
                    offset = zoneOverride.Offset;
                    return;
                }
            }

            icon = _defaultIcon;
            offset = _defaultOffset;
        }

        private void LateUpdate()
        {
            float t = IsHighlighted ? 1f
                : (Scp079ForwardCameraSelector.HighlightedCamera == Target ? ColorSelectorTarget : 0f);

            TargetSprite.color = Color.Lerp(NormalColor, HighlightedColor, t);
        }
    }
}