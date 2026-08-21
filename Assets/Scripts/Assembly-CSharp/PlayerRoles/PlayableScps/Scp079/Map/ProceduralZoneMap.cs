using System;
using System.Collections.Generic;
using MapGeneration;
using PlayerRoles.PlayableScps.Scp079.Cameras;
using PlayerRoles.PlayableScps.Subroutines;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils.NonAllocLINQ;

namespace PlayerRoles.PlayableScps.Scp079.Map
{
    public class ProceduralZoneMap : MonoBehaviour, IZoneMap
    {
        [Serializable]
        private struct RoomIcon
        {
            public RoomName Room;
            public RoomShape Shape;
            public string Name;
            public Vector3 TextOffset;
            public Bounds IndicatorLimits;
        }

        public static readonly Dictionary<FacilityZone, Scp079HudTranslation> ZoneTranslations;
        public static readonly Color CurrentZoneColor;
        public static readonly Color OtherZoneColor;
        public static readonly Color CurrentRoomColor;
        public static readonly Color HighlightedColor;

        public readonly Dictionary<RoomIdentifier, Image> NodesByRoom = new Dictionary<RoomIdentifier, Image>();
        public readonly Dictionary<Image, RoomIdentifier> RoomsByNode = new Dictionary<Image, RoomIdentifier>();

        [SerializeField]
        protected TextMeshProUGUI ZoneLabel;

        [SerializeField]
        private FacilityZone _targetZone;

        [SerializeField]
        private Image _template;

        [SerializeField]
        private RoomIcon[] _roomIcons;

        [SerializeField]
        private float _positionScale;

        [SerializeField]
        private RectTransform _rectParent;

        private const float NonExactCooldown = 1.5f;

        private Vector2 _nodeSize;
        private Scp079InteractableBase _highlightedCamera;
        private readonly AbilityCooldown _nonExactCooldown = new AbilityCooldown();
        private readonly HashSet<TextMeshProUGUI> _spawnedTexts = new HashSet<TextMeshProUGUI>();
        private readonly Queue<ProceduralZoneMap> _queuedPostProcessing = new Queue<ProceduralZoneMap>();

        public Bounds RectBounds { get; protected set; }

        static ProceduralZoneMap()
        {
            ZoneTranslations = new Dictionary<FacilityZone, Scp079HudTranslation>
            {
                [FacilityZone.LightContainment] = Scp079HudTranslation.LightContZone,
                [FacilityZone.HeavyContainment] = Scp079HudTranslation.HeavyContZone,
                [FacilityZone.Entrance] = Scp079HudTranslation.EntranceZone,
                [FacilityZone.Surface] = Scp079HudTranslation.SurfaceZone
            };

            CurrentZoneColor = new Color(1f, 1f, 1f, 0.055f);
            OtherZoneColor = new Color(0.5f, 0.5f, 0.5f, 0.042f);
            CurrentRoomColor = new Color(0f, 1f, 0.4f, 0.15f);
            HighlightedColor = new Color(1f, 1f, 1f, 0.27f);
        }

        private void Update()
        {
            if (CollectionExtensions.TryDequeue(_queuedPostProcessing, out ProceduralZoneMap element))
                element.PostProcessRooms();
        }

        private bool TryGetIcon(RoomIdentifier room, out RoomIcon result)
        {
            RoomIcon[] roomIcons = _roomIcons;
            for (int i = 0; i < roomIcons.Length; i++)
            {
                RoomIcon roomIcon = roomIcons[i];
                if (roomIcon.Room == room.Name && roomIcon.Shape == room.Shape)
                {
                    result = roomIcon;
                    return true;
                }
            }
            result = default(RoomIcon);
            return false;
        }

        private void SpawnRoomNode(RoomIdentifier room)
        {
            Image image = Instantiate(_template, _template.rectTransform.parent);

            if (TryGetIcon(room, out RoomIcon result))
            {
                TextMeshProUGUI componentInChildren = image.GetComponentInChildren<TextMeshProUGUI>();
                componentInChildren.rectTransform.localPosition = result.TextOffset;
                componentInChildren.text = result.Name;
                _spawnedTexts.Add(componentInChildren);
                image.rectTransform.localEulerAngles = Vector3.back * room.transform.eulerAngles.y;
            }

            Vector3 position = room.transform.position;
            image.rectTransform.localPosition = new Vector3(position.x, position.z, 0f);
            NodesByRoom.Add(room, image);
            RoomsByNode.Add(image, room);
            image.sprite = room.Icon;
        }

        private bool TryGetCamOfRoom(Scp079Camera curCam, RoomIdentifier room, out Scp079InteractableBase result)
        {
            return HashsetExtensions.TryGetFirst(
                Scp079InteractableBase.AllInstances,
                x => x is Scp079Camera scp079Camera && scp079Camera.IsMain && scp079Camera.Room == room && scp079Camera != curCam,
                out result);
        }

        protected virtual void PlaceRooms()
        {
            foreach (RoomIdentifier allRoomIdentifier in RoomIdentifier.AllRoomIdentifiers)
            {
                if (allRoomIdentifier.Zone == _targetZone)
                    SpawnRoomNode(allRoomIdentifier);
            }

            foreach (Image value in NodesByRoom.Values)
                value.rectTransform.localPosition *= _positionScale;
        }

        protected virtual void PostProcessRooms()
        {
            foreach (TextMeshProUGUI spawnedText in _spawnedTexts)
            {
                RectTransform rectTransform = spawnedText.rectTransform;
                rectTransform.rotation = _rectParent.rotation;

                if (!string.IsNullOrEmpty(spawnedText.text))
                {
                    rectTransform.GetComponentInChildren<LayoutGroup>(includeInactive: true).transform.localPosition =
                        Vector3.down * rectTransform.sizeDelta.y / 2f;
                }
            }

            _nodeSize = _template.rectTransform.sizeDelta / 2f;
            Destroy(_template.gameObject);

            Bounds rectBounds = default(Bounds);
            bool flag = true;

            foreach (KeyValuePair<Image, RoomIdentifier> item in RoomsByNode)
            {
                RectTransform rectTransform2 = item.Key.rectTransform;
                Bounds bounds = new Bounds(rectTransform2.anchoredPosition, rectTransform2.sizeDelta);

                if (flag)
                    rectBounds = bounds;
                else
                    rectBounds.Encapsulate(bounds);

                flag = false;
            }

            RectBounds = rectBounds;

            RectTransform rectTransform3 = ZoneLabel.rectTransform;
            rectTransform3.anchoredPosition = (Vector2)rectBounds.center + Vector2.up * (rectBounds.extents.y + rectTransform3.sizeDelta.y);
            ZoneLabel.text = Translations.Get(ZoneTranslations[_targetZone]);
        }

        public void Generate()
        {
            PlaceRooms();
            _queuedPostProcessing.Enqueue(this);
        }

        public virtual void UpdateOpened(Scp079Camera curCam)
        {
            _highlightedCamera = null;
            Vector2 anchoredPosition = _rectParent.anchoredPosition;
            bool flag = curCam.Room.Zone == _targetZone;

            foreach (KeyValuePair<Image, RoomIdentifier> item in RoomsByNode)
            {
                Vector2 vector = item.Key.rectTransform.anchoredPosition + anchoredPosition;

                if (Mathf.Abs(vector.x) < _nodeSize.x && Mathf.Abs(vector.y) < _nodeSize.y)
                {
                    if (TryGetCamOfRoom(curCam, item.Value, out Scp079InteractableBase result))
                    {
                        item.Key.color = HighlightedColor;
                        _highlightedCamera = result as Scp079Camera;
                    }
                }
                else
                {
                    item.Key.color = flag ? CurrentZoneColor : OtherZoneColor;
                }
            }

            if (NodesByRoom.TryGetValue(curCam.Room, out Image value))
                value.color = CurrentRoomColor;
        }

        public bool TryGetCamera(out Scp079Camera target)
        {
            target = _highlightedCamera as Scp079Camera;
            return target != null;
        }

        public bool TryGetCenterTransform(Scp079Camera curCam, out Vector3 center)
        {
            if (NodesByRoom.TryGetValue(curCam.Room, out Image value))
            {
                center = -value.rectTransform.anchoredPosition;
                return true;
            }
            center = Vector3.zero;
            return false;
        }

        public bool TrySetTeammateIndicator(ReferenceHub ply, RectTransform indicator, bool exact)
        {
            if (!(ply.roleManager.CurrentRole is FpcStandardScp fpcStandardScp))
                return false;

            Vector3 position = fpcStandardScp.FpcModule.Position;
            RoomIdentifier roomIdentifier = RoomIdUtils.RoomAtPositionRaycasts(position);

            if (roomIdentifier == null || !NodesByRoom.TryGetValue(roomIdentifier, out Image value))
                return false;

            if (exact)
            {
                RectTransform rectTransform = value.rectTransform;
                Transform transform = roomIdentifier.transform;
                Vector3 vector = transform.InverseTransformPoint(position);
                Vector2 vector2 = new Vector3(vector.x, vector.z) * _positionScale;

                RoomIcon result;
                Bounds bounds = TryGetIcon(roomIdentifier, out result) ? result.IndicatorLimits : new Bounds(Vector3.zero, rectTransform.sizeDelta);

                indicator.SetParent(rectTransform);
                vector2.x = Mathf.Clamp(vector2.x, bounds.min.x, bounds.max.x);
                vector2.y = Mathf.Clamp(vector2.y, bounds.min.y, bounds.max.y);
                indicator.localPosition = vector2;
                indicator.localScale = Vector3.one;
                indicator.rotation = _rectParent.rotation;
                indicator.Rotate(Vector3.back * (fpcStandardScp.FpcModule.MouseLook.CurrentHorizontal - transform.eulerAngles.y - rectTransform.localEulerAngles.z), Space.Self);
            }
            else
            {
                if (!_nonExactCooldown.IsReady)
                    return true;

                LayoutGroup componentInChildren = value.GetComponentInChildren<LayoutGroup>();
                indicator.SetParent(componentInChildren.transform);
                indicator.localScale = Vector3.one;
                indicator.localPosition = Vector3.one;
                indicator.localEulerAngles = Vector3.zero;
                _nonExactCooldown.Trigger(NonExactCooldown);
            }

            return true;
        }
    }
}