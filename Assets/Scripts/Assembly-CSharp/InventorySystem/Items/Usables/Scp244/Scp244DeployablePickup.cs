using System.Collections.Generic;
using System.Diagnostics;
using AudioPooling;
using InventorySystem.Items.Pickups;
using Interactables.Interobjects.DoorUtils;
using MapGeneration;
using Mirror;
using PlayerStatsSystem;
using UnityEngine;

namespace InventorySystem.Items.Usables.Scp244
{
    public class Scp244DeployablePickup : CollisionDetectionPickup, IDestructible
    {
        private const float SquaredDisUpdateDiff = 1f;
        private const float ForceBoundsUpdateSqrtDiff = 100f;
        private const float UpdateCooldownTime = 2.2f;
        private const int VertsPerFrame = 30;
        private const float ParticleSize = 3f;

        public static readonly HashSet<Scp244DeployablePickup> Instances = new HashSet<Scp244DeployablePickup>();

        public float MaxDiameter;
        public AnimationCurve FogDistanceCurve;
        public AnimationCurve FogLerpCurve;

        [SyncVar]
        private byte _syncSizePercent;

        [SyncVar]
        private byte _syncState;

        [SerializeField]
        private AnimationCurve _growSpeedOverLifetime;

        [SerializeField]
        private float _timeToDecay;

        [SerializeField]
        private float _transitionDistance;

        [SerializeField]
        private float _fullSubmergeDistance;

        [SerializeField]
        private GameObject _visibleModel;

        [SerializeField]
        private float _minimalInfluenceDistance;

        [SerializeField]
        private float _activationDot;

        [SerializeField]
        private float _health;

        [SerializeField]
        private float _deployedPickupTime;

        [SerializeField]
        private ParticleSystem _mainEffect;

        [SerializeField]
        private GameObject _destroyedModel;

        [SerializeField]
        private Mesh _referenceMesh;

        [SerializeField]
        private AnimationCurve _emissionOverPercent;

        [SerializeField]
        private AnimationCurve _sizeOverDiameter;

        [SerializeField]
        private AudioClip[] _destroyClips;

        [SerializeField]
        private AudioSource _emissionSoundSource;

        private Vector3[] _templateVertices;
        private Vector3[] _updatedVertices;
        private int _meshVertsCount;
        private int _particleTimer;

        private Mesh _generatedMesh;
        private Vector2 _initialSize;

        private Vector3 _previousPos;
        private float _lastActiveSize;
        private float _lastUpdateTime;
        private bool _conditionsSet;

        private readonly Stopwatch _lifeTime = Stopwatch.StartNew();

        private float GrowSpeed => Time.deltaTime * (MaxDiameter / TimeToGrow);

        private float TimeToGrow => 1f / _growSpeedOverLifetime.Evaluate((float)_lifeTime.Elapsed.TotalSeconds);

        private float CurTime => Time.timeSinceLevelLoad;

        public bool ModelDestroyed => State == Scp244State.Destroyed || State == Scp244State.PickedUp;

        public float CurrentDiameter
        {
            get
            {
                if (State == Scp244State.Active)
                    _lastActiveSize = CurrentSizePercent * MaxDiameter;
                return _lastActiveSize;
            }
        }

        public Bounds CurrentBounds { get; private set; }

        public float CurrentSizePercent { get; private set; }

        public Scp244TransferCondition[] Conditions { get; private set; }

        public Scp244State State
        {
            get => (Scp244State)_syncState;
            set => _syncState = (byte)value;
        }

        public uint NetworkId => netId;

        public Vector3 CenterOfMass => Rb.worldCenterOfMass;

        private void Update()
        {
            UpdateCurrentRoom();
            UpdateConditions();
            UpdateRange();
            UpdateEffects();
        }

        protected override void Start()
        {
            base.Start();
            Instances.Add(this);
            SetupEffects();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Instances.Remove(this);
        }

        private void UpdateCurrentRoom()
        {
            Vector3 position = transform.position;
            if ((position - _previousPos).sqrMagnitude >= SquaredDisUpdateDiff
                && _lastUpdateTime + UpdateCooldownTime <= CurTime
                && SeedSynchronizer.MapGenerated)
            {
                Conditions = Scp244TransferCondition.GenerateTransferConditions(this);
                _previousPos = position;
                _lastUpdateTime = CurTime;
                _conditionsSet = true;
            }
        }

        private void UpdateConditions()
        {
            if (!_conditionsSet)
                return;

            bool first = true;
            Bounds targetBounds = default;

            foreach (var condition in Conditions)
            {
                bool open = true;
                foreach (var door in condition.Doors)
                {
                    if (!door.IsConsideredOpen())
                    {
                        open = false;
                        break;
                    }
                }

                if (open)
                {
                    if (first)
                        targetBounds = condition.BoundsToEncapsulate;
                    else
                        targetBounds.Encapsulate(condition.BoundsToEncapsulate);
                    first = false;
                }
            }

            Bounds hardLimit = new Bounds(transform.position, Vector3.one * (CurrentDiameter + ParticleSize));
            targetBounds.SetMinMax(
                Vector3.Max(hardLimit.min, targetBounds.min),
                Vector3.Min(hardLimit.max, targetBounds.max));

            if ((CurrentBounds.center - targetBounds.center).sqrMagnitude < ForceBoundsUpdateSqrtDiff)
            {
                Vector3 deltaSize = CurrentBounds.size - targetBounds.size;
                float speed = GrowSpeed;
                speed = (deltaSize.x == 0f || deltaSize.z == 0f) ? (speed / 2f) : (speed * 2f);

                Vector3 center = Vector3.MoveTowards(CurrentBounds.center, targetBounds.center, speed / 2f);
                Vector3 size = Vector3.MoveTowards(CurrentBounds.size, targetBounds.size, speed);
                CurrentBounds = new Bounds(center, size);
            }
            else
            {
                CurrentBounds = targetBounds;
            }
        }

        private void UpdateRange()
        {
            if (ModelDestroyed && _visibleModel.activeSelf)
            {
                Rb.constraints = RigidbodyConstraints.FreezeAll;
                _visibleModel.SetActive(false);
                _emissionSoundSource.enabled = false;

                if (State == Scp244State.Destroyed)
                {
                    if (_destroyClips != null && _destroyClips.Length > 0)
                    {
                        var clip = RandomElement.RandomItem(_destroyClips);
                        AudioSourcePoolManager.PlaySound(clip, transform.position, 1f, 2f);
                    }

                    var destroyed = Object.Instantiate(_destroyedModel);
                    destroyed.transform.position = transform.position;
                    destroyed.transform.rotation = transform.rotation;
                    destroyed.transform.localScale = Vector3.one;

                    foreach (var rb in destroyed.GetComponentsInChildren<Rigidbody>())
                    {
                        var col = rb.GetComponent <Collider>();
                        Object.Destroy(col, _timeToDecay);
                        Object.Destroy(rb, _timeToDecay);
                    }
                }
            }

            if (!NetworkServer.active)
            {
                CurrentSizePercent = (int)_syncSizePercent / 255f;
                return;
            }

            if (State == Scp244State.Idle && Vector3.Dot(transform.up, Vector3.up) < _activationDot)
            {
                State = Scp244State.Active;
                _lifeTime.Restart();
            }

            float delta = (State == Scp244State.Active) ? TimeToGrow : (0f - _timeToDecay);
            CurrentSizePercent = Mathf.Clamp01(CurrentSizePercent + Time.deltaTime / delta);
            _syncSizePercent = (byte)Mathf.RoundToInt(CurrentSizePercent * 255f);

            if (ModelDestroyed && CurrentSizePercent <= 0f)
            {
                _timeToDecay -= Time.deltaTime;
                if (_timeToDecay <= 0f)
                    NetworkServer.Destroy(gameObject);
            }
        }

        private void SetupEffects()
        {
            _templateVertices = _referenceMesh.vertices;
            _updatedVertices = new Vector3[_templateVertices.Length];
            _meshVertsCount = _templateVertices.Length;

            var mainModule = _mainEffect.main;
            _initialSize = new Vector2(mainModule.startSizeX.constant, mainModule.startSizeY.constant);

            _generatedMesh = new Mesh
            {
                vertices = new Vector3[_meshVertsCount],
                triangles = _referenceMesh.triangles,
                normals = _referenceMesh.normals
            };

            var shapeModule = _mainEffect.shape;
            shapeModule.mesh = _generatedMesh;
        }

        private void UpdateEffects()
        {
            bool isActive = State == Scp244State.Active;

            if (_emissionSoundSource.enabled != isActive)
            {
                _emissionSoundSource.enabled = isActive;
                _lifeTime.Restart();
            }

            var emission = _mainEffect.emission;
            emission.rateOverTimeMultiplier = _emissionOverPercent.Evaluate(CurrentSizePercent);

            float currentDiameter = CurrentDiameter;
            float sizeMul = _sizeOverDiameter.Evaluate(currentDiameter);

            var main = _mainEffect.main;
            var curveX = main.startSizeX;
            curveX.constant = sizeMul * _initialSize.x;
            main.startSizeX = curveX;

            var curveY = main.startSizeY;
            curveY.constant = sizeMul * _initialSize.y;
            main.startSizeY = curveY;

            if (State == Scp244State.Idle)
                return;

            Vector3 meshOffset = CurrentBounds.center - transform.position;
            float radius = currentDiameter * 0.5f;

            for (int i = 0; i < VertsPerFrame; i++)
            {
                int index = _particleTimer;

                Vector3 scaled = _templateVertices[index] * radius;
                _updatedVertices[index] = scaled + meshOffset;

                _particleTimer++;
                if (_particleTimer >= _meshVertsCount)
                    _particleTimer = 0;
            }

            _generatedMesh.vertices = _updatedVertices;
            _mainEffect.transform.rotation = Quaternion.identity;
        }

        public float FogPercentForPoint(Vector3 worldPoint)
        {
            if (State == Scp244State.Idle)
                return 0f;

            float radius = CurrentDiameter * 0.5f;
            float sqrDist = (transform.position - worldPoint).sqrMagnitude;
            float outerRadius = radius + _transitionDistance + ParticleSize;

            if (sqrDist >= outerRadius * outerRadius)
                return 0f;

            Bounds bounds = new Bounds(CurrentBounds.center, CurrentBounds.size);
            bounds.Expand(-_fullSubmergeDistance);

            float a = Vector3.Distance(bounds.ClosestPoint(worldPoint), worldPoint);
            float b = Mathf.Sqrt(sqrDist) - outerRadius + ParticleSize + _fullSubmergeDistance;
            float fog = 1f - Mathf.Clamp01(Mathf.Max(a, b) / _transitionDistance);

            if (ModelDestroyed)
                fog *= CurrentSizePercent;

            if (radius < _minimalInfluenceDistance)
                fog *= radius / _minimalInfluenceDistance;

            return fog;
        }

        public bool Damage(float damage, DamageHandlerBase handler, Vector3 exactHitPos)
        {
            if (handler is not ExplosionDamageHandler)
                return false;

            if (_health <= 0f || ModelDestroyed)
                return false;

            _health -= damage;
            if (_health <= 0f)
                State = Scp244State.Destroyed;

            return true;
        }

        public override float SearchTimeForPlayer(ReferenceHub hub)
        {
            float time = base.SearchTimeForPlayer(hub);
            if (State == Scp244State.Active)
                time += _deployedPickupTime;
            return time;
        }
    }
}