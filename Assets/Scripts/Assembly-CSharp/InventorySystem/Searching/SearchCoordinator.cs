using System;
using CursorManagement;
using InventorySystem.Items.Pickups;
using Mirror;
using Mirror.LiteNetLib4Mirror;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace InventorySystem.Searching
{
    [RequireComponent(typeof(ReferenceHub))]
    public class SearchCoordinator : NetworkBehaviour, ICursorOverride
    {
        private struct ProgressBezier : IEquatable<ProgressBezier>
        {
            private readonly SearchRequest _request;
            private readonly SearchSession _session;
            private readonly double _requestProgress;
            private readonly double _sessionReceivedTime;

            public double Progress
            {
                get
                {
                    // helper 0x180538540 (ProgressBezier.get_Progress): ease from _requestProgress
                    // (the bar position when the server promise arrived) up to 1.0 over the remaining
                    // window [_sessionReceivedTime → _session.FinishTime].
                    // NOTE on the prior bug: MoreMath.BezierQuadratic(a, b, c, t) takes the curve
                    // parameter as the LAST argument. The old call BezierQuadratic(t, _requestProgress,
                    // 1.0, 1.0) passed t as `a` and 1.0 as the parameter, so it always evaluated the
                    // curve at t==1 → returned c (1.0) every frame → the Promised-phase circle was
                    // instantly full. It also ignored _session / _sessionReceivedTime entirely.
                    double t = MoreMath.InverseLerp(_sessionReceivedTime, _session.FinishTime, NetworkTime.time);
                    return MoreMath.BezierQuadratic(_requestProgress, 1.0, 1.0, t);
                }
            }

            public ProgressBezier(SearchRequest request, SearchSession session)
            {
                _request = request;
                _session = session;
                _requestProgress = MoreMath.InverseLerp(request.Body.InitialTime, request.Body.FinishTime, NetworkTime.time);
                _sessionReceivedTime = NetworkTime.time;
            }

            public bool Equals(ProgressBezier other)
            {
                return _request.Equals(other._request) &&
                       _session.Equals(other._session) &&
                       _requestProgress.Equals(other._requestProgress) &&
                       _sessionReceivedTime.Equals(other._sessionReceivedTime);
            }

            public override bool Equals(object obj)
            {
                return obj is ProgressBezier other && Equals(other);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int hash = _request.GetHashCode();
                    hash = (hash * 397) ^ _session.GetHashCode();
                    hash = (hash * 397) ^ _requestProgress.GetHashCode();
                    hash = (hash * 397) ^ _sessionReceivedTime.GetHashCode();
                    return hash;
                }
            }
        }

        public const string DebugKey = "SEARCH";

        private bool _isSearching;

        private bool _toggleSearch;

        private byte _tickedId;

        private ProgressBezier _progressBezier;

        [Header("Network Shared")]
        [SerializeField]
        [SyncVar(hook = nameof(SetRayDistance))]
        private float rayDistance = 3f;

        [Header("Server only")]
        [SerializeField]
        private float serverRayDistanceThreshold = 1.2f;

        [SerializeField]
        private double serverDelayThreshold = 1.4;

        private Image _radialImage;

        private GameObject _radialObject;

        public ReferenceHub Hub { get; private set; }

        public LayerMask InteractMask { get; private set; }

        public CursorOverrideMode CursorOverride => CursorOverrideMode.NoOverride;

        public bool LockMovement => _isSearching;

        public float MaxDistanceSqr { get; private set; }

        public float ServerMaxRayDistanceSqr { get; private set; }

        public float RayDistance
        {
            get => rayDistance;
            set
            {
                if (!NetworkServer.active)
                {
                    throw new InvalidOperationException("The ray distance can only be set by the server.");
                }
                rayDistance = value;
                UpdateMaxDistanceSqr();
            }
        }

        private byte TicketId => ++_tickedId;

        public bool IsSearching
        {
            get => _isSearching;
            private set
            {
                if (value != _isSearching)
                {
                    _isSearching = value;
                    _radialObject.SetActive(value);
                    _radialImage.fillAmount = 0f;
                }
            }
        }

        public SearchSessionPipe SessionPipe { get; private set; }

        public SearchCompletor Completor { get; private set; }

        private void Awake()
        {
            _toggleSearch = PlayerPrefsSl.Get("ToggleSearch", false);
        }

        private void SetRayDistance(float oldValue, float newValue)
        {
            UpdateMaxDistanceSqr();
        }

        private void UpdateMaxDistanceSqr()
        {
            MaxDistanceSqr = rayDistance * rayDistance;
            ServerMaxRayDistanceSqr = rayDistance * rayDistance * serverRayDistanceThreshold;
        }

        private void Start()
        {
            UpdateMaxDistanceSqr();
            Hub = ReferenceHub.GetHub(gameObject);
            _radialImage = UserMainInterface.Singleton.searchRadial;
            _radialObject = UserMainInterface.Singleton.searchObject;
            InteractMask = Hub.playerInteract.mask;
            SessionPipe = new SearchSessionPipe(this, NetworkServer.active ? Hub.playerRateLimitHandler.RateLimits[0] : null);
            SessionPipe.RequestUpdated += HandleRequest;
            SessionPipe.SessionUpdated += HandleSession;
            SessionPipe.RegisterHandlers();
            if (isLocalPlayer)
            {
                CursorManager.Register(this);
            }
        }

        private void OnDestroy()
        {
            CursorManager.Unregister(this);
        }

        private void Update()
        {
            if (isLocalPlayer)
            {
                if (SessionPipe.Status != SearchSessionPipe.Activity.Idle)
                {
                    bool isSearching = ContinuePickupClient();
                    if (isSearching != IsSearching)
                    {
                        IsSearching = isSearching;
                    }
                }
                else
                {
                    if (IsSearching)
                    {
                        IsSearching = false;
                    }
                    Raycast();
                }
            }

            // Not an "else if" of the isLocalPlayer branch above: on a listen server, the
            // host's own hub is isLocalPlayer AND NetworkServer.active at the same time.
            // ContinuePickupClient() above is purely predictive/visual (fills the radial bar);
            // this is the only place that actually calls Completor.Complete() (server-authoritative).
            // Making this an else-if (as a prior restoration pass did) meant the host's own
            // pickups were never completed: the item stayed on the floor, still flagged
            // InUse, and could never be picked up again.
            if (NetworkServer.active && SessionPipe.Status == SearchSessionPipe.Activity.Promised)
            {
                ContinuePickupServer();
            }

            SessionPipe.Update();
        }

        private void HandleRequest()
        {
            // Server-only logic: it sets the Session promise (which throws if not the
            // server) and the Completor. RequestUpdated only fires from the server-side
            // request path (SearchSessionPipe.ReceiveRequest -> HandleRequest); the guard
            // stays as a safety net.
            if (!NetworkServer.active)
            {
                return;
            }

            bool flag;
            SearchSession? session;
            SearchCompletor completor;
            try
            {
                flag = ReceiveRequestUnsafe(out session, out completor);
            }
            catch (Exception exception)
            {
                SessionPipe.Invalidate();
                GameCore.Console.AddDebugLog(DebugKey, $"Exception in HandleRequest: {exception.Message}", MessageImportance.LessImportant, false);
                return;
            }
            if (flag)
            {
                if (session.HasValue)
                {
                    SessionPipe.Session = session.Value;
                }
                else
                {
                    SessionPipe.Invalidate();
                }
            }
            Completor = completor;
        }

        private void HandleSession()
        {
            _progressBezier = new ProgressBezier(SessionPipe.Request, SessionPipe.Session);
        }

        private bool ReceiveRequestUnsafe(out SearchSession? session, out SearchCompletor completor)
        {
            SearchRequest request = SessionPipe.Request;
            completor = SearchCompletor.FromPickup(this, request.Target, ServerMaxRayDistanceSqr);
            if (!completor.ValidateStart())
            {
                session = null;
                completor = null;
                return true;
            }
            SearchSession body = request.Body;
            if (!isLocalPlayer)
            {
                double latency = NetworkTime.time - request.InitialTime;
                double pingThreshold = LiteNetLib4MirrorServer.Peers[connectionToClient.connectionId].Ping * 0.001 * serverDelayThreshold;
                float searchTime = request.Target.SearchTimeForPlayer(Hub);
                if (latency < 0.0 || latency > pingThreshold)
                {
                    body.InitialTime = NetworkTime.time - pingThreshold;
                    body.FinishTime = body.InitialTime + searchTime;
                }
                else if (Math.Abs(body.FinishTime - body.InitialTime - searchTime) > 0.001)
                {
                    body.FinishTime = body.InitialTime + searchTime;
                }
            }
            session = body;
            return true;
        }

        private void ContinuePickupServer()
        {
            // Promised state should always have a Completor, but a state desync can
            // leave it null here (NRE). Treat a missing Completor as an invalid update.
            if (Completor != null && Completor.ValidateUpdate())
            {
                if (NetworkTime.time >= SessionPipe.Session.FinishTime)
                {
                    // If Complete() throws (e.g. an exception inside ServerAddItem/OnAdded),
                    // the pickup would keep Info.InUse=true forever and become unpickable.
                    // Invalidate() releases the lock and notifies the client.
                    try
                    {
                        Completor.Complete();
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"SearchCompletor.Complete() failed for '{SessionPipe.Request.Target?.Info.ItemId}' — releasing the pickup.");
                        Debug.LogException(e);
                        SessionPipe.Invalidate();
                    }
                }
            }
            else
            {
                SessionPipe.Invalidate();
            }
        }

        private bool ContinuePickupClient()
        {
            KeyCode keyCode = NewInput.GetKey(ActionName.Interact);
            bool isKeyDown = Input.GetKeyDown(keyCode);
            bool isKeyHeld = Input.GetKey(keyCode);
            bool shouldAbort = _toggleSearch ? isKeyDown : !isKeyHeld;

            if (shouldAbort)
            {
                SessionPipe.Abort();
                return false;
            }

            // This is purely predictive/visual: it only decides whether to keep showing the
            // radial bar. It has no authority to invalidate the session or complete the
            // pickup - that is exclusively ContinuePickupServer()'s job (see Update()). If
            // ValidateUpdate() fails here for a reason other than the key being released
            // (e.g. distance), we just stop rendering the bar; the server's own independent
            // validation is what actually resets SessionPipe.Session.InUse if needed.
            double progress = SessionPipe.Status == SearchSessionPipe.Activity.Requested
                ? MoreMath.InverseLerp(SessionPipe.Request.Body.InitialTime, SessionPipe.Request.Body.FinishTime, NetworkTime.time)
                : _progressBezier.Progress;

            if (progress >= 1.0 || !Completor.ValidateUpdate())
            {
                return false;
            }

            _radialImage.fillAmount = (float)progress;
            return true;
        }

        private void Raycast()
        {
            KeyCode keyCode = NewInput.GetKey(ActionName.Interact);
            bool isKeyDown = Input.GetKeyDown(keyCode);
            if (isKeyDown)
            {
                if (Physics.Raycast(Hub.PlayerCameraReference.position, Hub.PlayerCameraReference.forward, out RaycastHit hit, rayDistance, InteractMask))
                {
                    ItemPickupBase pickup = hit.transform.GetComponentInParent<ItemPickupBase>();
                    if (pickup != null && !pickup.Info.Locked && !pickup.Info.InUse && pickup.netIdentity != null)
                    {
                        SearchCompletor completor = SearchCompletor.FromPickup(this, pickup, MaxDistanceSqr);
                        if (completor.ValidateStart())
                        {
                            Completor = completor;
                            float searchTime = pickup.SearchTimeForPlayer(Hub);
                            SearchSession session = new(NetworkTime.time, NetworkTime.time + searchTime, pickup);
                            SearchRequest request = new(TicketId, session);
                            // The Request setter does the actual dispatch (direct server call on
                            // the host, NetworkClient.Send on a remote client) - sending here as
                            // well made the host process the request twice and lock the pickup.
                            SessionPipe.Request = request;
                        }
                    }
                }
            }
        }
    }
}