using System;
using CursorManagement;
using Interactables.Interobjects;
using MapGeneration;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerRoles.PlayableScps.Scp106
{
    public class Scp106Minimap : MonoBehaviour, ICursorOverride
    {
        [SerializeField]
        private Transform _rotorTransform;     
        [SerializeField]
        private Transform _offsetTransform;    
        [SerializeField]
        private Transform _rangeTransform;     
        [SerializeField]
        private Scp106MinimapElement _template; 
        [SerializeField]
        private float _gridScale;              
        [SerializeField]
        private float _radiusScale;             
        [SerializeField]
        private GameObject _errorSurface;     
        [SerializeField]
        private GameObject _errorElevator;     
        [SerializeField]
        private RectTransform _cursor;          

        private bool _setUp;                   
        private float _lastElevatorRide;       
        private int _poolSize;                 
        private int _usedRooms;                 
        private float _scaleCache;             
        private Transform _tr;                

        private static Scp106MinimapElement[] _pool;

        private const float SurfaceHeightThreshold = 800f;
        private const float HeightRange = 350f;
        private const float ElevatorCooldown = 0.2f;
        private const float FadeSpeed = 19f;

        private float CurTime => Time.timeSinceLevelLoad;

        private bool InElevator => CurTime - _lastElevatorRide < ElevatorCooldown;

        private float Scale
        {
            get => _scaleCache;
            set
            {
                _scaleCache = Mathf.Clamp01(value);
                if (_tr != null)
                    _tr.localScale = Vector3.one * _scaleCache;
            }
        }

        public CursorOverrideMode CursorOverride => IsVisible ? CursorOverrideMode.Free : CursorOverrideMode.NoOverride;

        public bool LockMovement => false;

        public bool IsVisible { get; set; }    

        public Vector3 LastWorldPos            
        {
            get => _lastWorldPos;
            set => _lastWorldPos = value;
        }
        private Vector3 _lastWorldPos;        

        public static Scp106Minimap Singleton { get; private set; }

        private void Setup()
        {
            _poolSize = RoomIdentifier.AllRoomIdentifiers.Count;

            if (_pool == null || _poolSize > _pool.Length)
            {
                _pool = new Scp106MinimapElement[_poolSize];
            }

            for (int i = 0; i < _poolSize; i++)
            {
                Scp106MinimapElement element = Instantiate(_template, _offsetTransform);
                if (element == null)
                    return; 
   
                if (element.Rt != null)
                    element.Rt.localScale = Vector3.one;

                _pool[i] = element;
            }

            _setUp = true;
        }

        private void OnElevatorMoved(Bounds elevatorBounds, ElevatorChamber chamber, Vector3 deltaPos, Quaternion deltaRot)
        {
            if (MainCameraController.CurrentCamera == null)
                return;

            if (elevatorBounds.Contains(MainCameraController.CurrentCamera.position))
            {
                _lastElevatorRide = CurTime;
            }
        }

        private void Awake()
        {
            CursorManager.Register(this);

            ElevatorChamber.OnElevatorMoved += OnElevatorMoved;

            Singleton = this;

            _tr = transform;
        }

        private void OnDestroy()
        {
            CursorManager.Unregister(this);

            ElevatorChamber.OnElevatorMoved -= OnElevatorMoved;
        }

        private void Update()
        {
            if (!SeedSynchronizer.MapGenerated)
                return;

            if (!_setUp)
                Setup();

            if (Scp106MinimapElement.AnyHighlighted)
            {
                Scp106MinimapElement last = Scp106MinimapElement.LastHighlighted;
                if (last != null && last.Rt != null && _cursor != null)
                {
                    _cursor.SetParent(last.Rt);
                    _cursor.position = Input.mousePosition;
                    _cursor.gameObject.SetActive(true);
                }
            }
            else
            {
                if (_cursor != null)
                    _cursor.gameObject.SetActive(false);
            }

            float target = IsVisible ? 1f : 0f;
            Scale = Mathf.MoveTowards(Scale, target, FadeSpeed * Time.deltaTime);

            if (Scale <= 0f)
                return;

            Transform currentCamera = MainCameraController.CurrentCamera;
            if (currentCamera == null)
                return;

            Vector3 camPos = currentCamera.position;
            int previouslyUsed = _usedRooms;
            _usedRooms = 0;

            if (InElevator)
            {
                if (_errorSurface != null) _errorSurface.SetActive(false);
                if (_errorElevator != null) _errorElevator.SetActive(true);
            }
            else if (camPos.y > SurfaceHeightThreshold)
            {
                if (_errorSurface != null) _errorSurface.SetActive(true);
                if (_errorElevator != null) _errorElevator.SetActive(false);
            }
            else
            {
                if (_errorSurface != null) _errorSurface.SetActive(false);
                if (_errorElevator != null) _errorElevator.SetActive(false);

                SetupRooms(currentCamera, camPos);

                RefreshRange();
            }

            for (int i = _usedRooms; i < previouslyUsed; i++)
            {
                if (_pool != null && i < _pool.Length && _pool[i] != null && _pool[i].Img != null)
                    _pool[i].Img.enabled = false;
            }

            if (_template != null && _template.Rt != null)
            {
                Vector3 localMouse = _template.Rt.InverseTransformPoint(Input.mousePosition) / _gridScale;
                LastWorldPos = new Vector3(-localMouse.x, camPos.y, -localMouse.y);
            }
        }

        private void RefreshRange()
        {
            if (!ReferenceHub.TryGetLocalHub(out ReferenceHub hub))
                return;

            if (hub.roleManager?.CurrentRole is not Scp106Role role)
                return;
            if (!role.SubroutineModule.TryGetSubroutine(out Scp106Vigor vigor))
                return;

            if (_rangeTransform != null)
            {
                float vigorValue = vigor.DisplayedVigor / 0.019f;
                _rangeTransform.localScale = Vector3.one / (_radiusScale * vigorValue);
            }
        }

        private void SetupRooms(Transform camTr, Vector3 camPos)
        {
            foreach (RoomIdentifier room in RoomIdentifier.AllRoomIdentifiers)
            {
                if (room == null)
                    continue;

                Transform roomTr = room.transform;
                if (roomTr == null)
                    continue;

                Vector3 roomPos = roomTr.position;

                if (room.Zone == FacilityZone.None)
                    continue;

                if (Mathf.Abs(roomPos.y - camPos.y) > HeightRange)
                    continue;

                if (_pool == null || _usedRooms >= _pool.Length)
                    return;

                Scp106MinimapElement element = _pool[_usedRooms];
                if (element == null)
                    return;

                element.Room = room;

                if (element.Img != null)
                    element.Img.sprite = room.Icon;

                if (element.Img != null)
                    element.Img.enabled = true;

                if (element.Rt != null)
                    element.Rt.localEulerAngles = Vector3.back * roomTr.eulerAngles.y;

                if (element.Rt != null)
                    element.Rt.localPosition = new Vector3(roomPos.x, roomPos.z, 0f) * _gridScale;

                _usedRooms++;
            }

            if (_offsetTransform != null)
            {
                Vector3 offset = new Vector3(camPos.x, camPos.z, 0f) * _gridScale;
                _offsetTransform.localPosition = -offset;
            }

            if (_rotorTransform != null)
                _rotorTransform.localEulerAngles = Vector3.forward * camTr.eulerAngles.y;
        }
    }
}