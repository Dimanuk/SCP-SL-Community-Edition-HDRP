using System.Collections.Generic;
using MapGeneration;
using PlayerRoles.PlayableScps.Scp079.Cameras;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerRoles.PlayableScps.Scp079.Map
{
    public class SurfaceMap : MonoBehaviour, IZoneMap
    {
        private const float ActivationMagSqr = 3500f;

        private static readonly Vector3 BottomLeftWorldspace = new Vector3(-25.168f, 0f, -82.351f);
        private static readonly Vector3 TopRightWorldspace = new Vector3(149.977f, 0f, 33.144f);

        [SerializeField]
        private RectTransform _parent;

        [SerializeField]
        private Image _template;

        [SerializeField]
        private LczMap _lczMap;

        [SerializeField]
        private Vector2 _spacing;

        [SerializeField]
        private TextMeshProUGUI _zoneLabel;

        [SerializeField]
        private RectTransform _scalerRoot;

        [SerializeField]
        private RectTransform _offsetRoot;

        [SerializeField]
        private RectTransform _indicatorsRoot;

        private Image[] _icons;
        private Image _parentImage;
        private Transform _nonExactTransform;
        private int _targetCam;
        private List<Scp079Camera> _surfaceCameras;

        private Vector2 WorldspaceToAnchored(Vector3 pos)
        {
            Vector2 sizeDelta = _parent.sizeDelta;
            return new Vector2(
                Mathf.InverseLerp(BottomLeftWorldspace.x, TopRightWorldspace.x, pos.x) * sizeDelta.x,
                Mathf.InverseLerp(BottomLeftWorldspace.z, TopRightWorldspace.z, pos.z) * sizeDelta.y
            );
        }

        public void Generate()
        {
            _parentImage = _parent.GetComponent<Image>();
            _nonExactTransform = GetComponentInChildren<LayoutGroup>(true).transform;
            _zoneLabel.text = Translations.Get(ProceduralZoneMap.ZoneTranslations[FacilityZone.Surface]);
            _surfaceCameras = new List<Scp079Camera>();

            foreach (Scp079InteractableBase allInstance in Scp079InteractableBase.AllInstances)
            {
                if (allInstance is Scp079Camera item && allInstance.Room.Zone == FacilityZone.Surface)
                    _surfaceCameras.Add(item);
            }

            int count = _surfaceCameras.Count;
            _icons = new Image[count];

            for (int i = 0; i < count; i++)
            {
                _icons[i] = Instantiate(_template, _parent);
                _icons[i].rectTransform.anchoredPosition = WorldspaceToAnchored(_surfaceCameras[i].Position);
            }

            Destroy(_template);
        }

        public bool TryGetCamera(out Scp079Camera target)
        {
            if (_targetCam < 0)
            {
                target = null;
                return false;
            }

            target = _surfaceCameras[_targetCam];
            return true;
        }

        public bool TryGetCenterTransform(Scp079Camera curCam, out Vector3 center)
        {
            center = Vector3.zero;

            if (curCam.Room.Zone != FacilityZone.Surface)
                return false;

            for (int i = 0; i < _icons.Length; i++)
            {
                if (_surfaceCameras[i] == curCam)
                {
                    center = -(Vector2)_icons[i].rectTransform.localPosition - _offsetRoot.anchoredPosition;
                    return true;
                }
            }

            return false;
        }

        public bool TrySetTeammateIndicator(ReferenceHub ply, RectTransform indicator, bool exact)
        {
            if (!(ply.roleManager.CurrentRole is FpcStandardScp fpcStandardScp))
                return false;

            Vector3 position = fpcStandardScp.FpcModule.Position;
            RoomIdentifier roomIdentifier = RoomIdUtils.RoomAtPositionRaycasts(position);

            if (roomIdentifier == null || roomIdentifier.Zone != FacilityZone.Surface)
                return false;

            indicator.SetParent(exact ? _indicatorsRoot : _nonExactTransform);
            indicator.localPosition = Vector3.zero;
            indicator.localScale = Vector3.one;

            if (exact)
            {
                indicator.anchoredPosition = WorldspaceToAnchored(position);
                indicator.localEulerAngles = Vector3.back * fpcStandardScp.FpcModule.MouseLook.CurrentHorizontal;
            }
            else
            {
                indicator.localEulerAngles = Vector3.zero;
            }

            return true;
        }

        public void UpdateOpened(Scp079Camera curCam)
        {
            Bounds rectBounds = _lczMap.RectBounds;
            Vector2 vector = (Vector2)rectBounds.center - _spacing - _parent.sizeDelta / 2f;
            _offsetRoot.localPosition = new Vector3(vector.x - rectBounds.extents.x, vector.y + rectBounds.extents.y, 0f);

            _parentImage.color = (curCam.Room.Zone == FacilityZone.Surface)
                ? ProceduralZoneMap.CurrentZoneColor
                : ProceduralZoneMap.OtherZoneColor;

            _targetCam = -1;

            for (int i = 0; i < _icons.Length; i++)
            {
                if (_surfaceCameras[i] == curCam)
                {
                    _icons[i].color = ProceduralZoneMap.CurrentRoomColor;
                }
                else if (_scalerRoot.InverseTransformPoint(_icons[i].rectTransform.position).sqrMagnitude < ActivationMagSqr)
                {
                    _targetCam = i;
                    _icons[i].color = ProceduralZoneMap.HighlightedColor;
                }
                else
                {
                    _icons[i].color = ProceduralZoneMap.CurrentZoneColor;
                }
            }
        }
    }
}