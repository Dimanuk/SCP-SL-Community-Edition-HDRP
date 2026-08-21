using System;
using InventorySystem.Items.Pickups;
using Mirror;
using Security;

namespace InventorySystem.Searching
{
    public class SearchSessionPipe
    {
        public enum Activity
        {
            Idle = 0,
            Requested = 1,
            Promised = 2
        }

        private readonly SearchCoordinator _owner;

        private readonly RateLimit _rateLimiter;

        private SearchRequest _request;

        private SearchSession _session;

        private SearchInvalidation Invalidation => new SearchInvalidation(_request.Id);

        public SearchRequest Request
        {
            get => _request;
            set
            {
                // Restored to the original v12 semantics (ISIL SearchSessionPipe.set_Request).
                // The old rebuild assigned the fields and fired RequestUpdated unconditionally,
                // while Raycast() ALSO called NetworkClient.Send - so on a listen server the
                // same request was processed twice: the second (network) pass demoted the pipe
                // from Promised back to Requested, the Session setter skipped the re-promise
                // (value.Equals(_session)), ContinuePickupServer (which requires Promised)
                // never ran, and the pickup stayed InUse=true forever - silently unpickable
                // for everyone. The original routes exactly once:
                //   host   -> ReceiveRequest(null, value)  (direct server dispatch, no send)
                //   client -> NetworkClient.Send(value) + Status=Requested (no local event)
                if (!_owner.isLocalPlayer)
                {
                    throw new InvalidOperationException("The request can only be set from the local player.");
                }
                if (value.Equals(_request))
                {
                    return;
                }
                if (NetworkServer.active)
                {
                    ReceiveRequest(null, value);
                }
                else
                {
                    NetworkClient.Send(value);
                    Status = Activity.Requested;
                }
                GameCore.Console.AddDebugLog("SEARCH", "Sent request.", MessageImportance.LessImportant, false);
                _request = value;
            }
        }

        public SearchSession Session
        {
            get => _session;
            set
            {
                if (!NetworkServer.active)
                {
                    throw new InvalidOperationException("The promise can only be set from the server.");
                }
                if (!value.Equals(_session))
                {
                    _owner.connectionToClient.Send(value);
                    Status = Activity.Promised;
                    _session = value;
                    SessionUpdated?.Invoke();
                }
            }
        }

        public Activity Status { get; private set; }

        public event Action RequestUpdated;

        public event Action SessionUpdated;

        public event Action SessionAborted;

        public event Action SessionInvalidated;

        public SearchSessionPipe(SearchCoordinator owner, RateLimit rateLimit)
        {
            _owner = owner;
            _rateLimiter = rateLimit;
        }

        private static void ReceiveRequest(NetworkConnection source, SearchRequest request)
        {
            SearchCoordinator searchCoordinator = source?.identity.GetComponent<SearchCoordinator>() ?? ReferenceHub.LocalHub.searchCoordinator;
            if (searchCoordinator == null)
            {
                return;
            }
            SearchSessionPipe sessionPipe = searchCoordinator.SessionPipe;
            if (request.Target == null)
            {
                sessionPipe.Invalidate();
                source?.identity.GetComponent<GameConsoleTransmission>().SendToClient("Pickup request rejected - target is null.", "red");
                return;
            }
            PickupSyncInfo info = request.Target.Info;
            if (info.Locked)
            {
                sessionPipe.Invalidate();
                source?.identity.GetComponent<GameConsoleTransmission>().SendToClient("Pickup request rejected - target is locked.", "red");
                return;
            }
            if (info.InUse)
            {
                sessionPipe.Invalidate();
                source?.identity.GetComponent<GameConsoleTransmission>().SendToClient("Pickup request rejected - target is in use.", "red");
                return;
            }
            info.InUse = true;
            request.Target.Info = info;

            if (!sessionPipe.HandleRequest(request))
            {
                info.InUse = false;
                request.Target.Info = info;
            }
        }

        private bool HandleRequest(SearchRequest request)
        {
            if (_rateLimiter.CanExecute())
            {
                _request = request;
                Status = Activity.Requested;
                RequestUpdated?.Invoke();
                return true;
            }
            return false;
        }

        private void HandlePromise(SearchSession session)
        {
            _session = session;
            Status = Activity.Promised;
            SessionUpdated?.Invoke();
        }

        private static void ReceiveAbortion(NetworkConnection source, SearchInvalidation invalidation)
        {
            SearchCoordinator searchCoordinator = source?.identity.GetComponent<SearchCoordinator>() ?? ReferenceHub.LocalHub.searchCoordinator;
            if (searchCoordinator != null)
            {
                searchCoordinator.SessionPipe.HandleAbort(invalidation);
            }
        }

        private void HandleAbort(SearchInvalidation invalidation)
        {
            if (_request.Id != invalidation.Id)
            {
                return;
            }
            if (_request.Target != null)
            {
                PickupSyncInfo info = _request.Target.Info;
                info.InUse = false;
                _request.Target.Info = info;
            }
            try
            {
                Status = Activity.Idle;
                SessionAborted?.Invoke();
            }
            finally
            {
                _request = default;
                _session = default;
            }
        }

        // Promises/invalidations are only ever sent to the owning connection, so the
        // correct recipient on this machine is always the LOCAL player's pipe. The
        // original bound these handlers to `this` of whichever SearchCoordinator
        // registered last (every coordinator calls RegisterHandlers in Start), so with
        // 2+ players the messages landed on another player's pipe object; the server
        // side survived that only because it promotes its own pipe directly. Resolving
        // through LocalHub removes the misrouting entirely.
        private static void ReceiveInvalidation(SearchInvalidation invalidation)
        {
            SearchCoordinator coordinator = ReferenceHub.LocalHub != null ? ReferenceHub.LocalHub.searchCoordinator : null;
            coordinator?.SessionPipe?.HandleInvalidate(invalidation);
        }

        private static void ReceivePromise(SearchSession session)
        {
            SearchCoordinator coordinator = ReferenceHub.LocalHub != null ? ReferenceHub.LocalHub.searchCoordinator : null;
            coordinator?.SessionPipe?.HandlePromise(session);
        }

        private void HandleInvalidate(SearchInvalidation invalidation)
        {
            if (_request.Id == invalidation.Id)
            {
                Status = Activity.Idle;
                SessionInvalidated?.Invoke();
                _request = default;
                _session = default;
            }
        }

        public void RegisterHandlers()
        {
            NetworkServer.ReplaceHandler<SearchRequest>(ReceiveRequest);
            NetworkServer.ReplaceHandler<SearchInvalidation>(ReceiveAbortion);
            NetworkClient.ReplaceHandler<SearchInvalidation>(ReceiveInvalidation);
            NetworkClient.ReplaceHandler<SearchSession>(ReceivePromise);
        }

        public void Abort()
        {
            if (!_owner.isLocalPlayer)
            {
                throw new InvalidOperationException("An abortion can only be performed by the local player.");
            }
            SearchInvalidation invalidation = Invalidation;
            if (NetworkServer.active)
            {
                HandleAbort(invalidation);
            }
            else
            {
                NetworkClient.Send(invalidation);
                Status = Activity.Idle;
                _request = default;
                _session = default;
            }
        }

        public void Invalidate()
        {
            if (!NetworkServer.active)
            {
                throw new InvalidOperationException("An invalidation can only be performed by the server.");
            }
            if (_request.Target != null)
            {
                PickupSyncInfo info = _request.Target.Info;
                info.InUse = false;
                _request.Target.Info = info;
            }
            _owner.connectionToClient.Send(Invalidation);
            Status = Activity.Idle;
            _request = default;
            _session = default;
        }

        public void Clear()
        {
            Status = Activity.Idle;
            _request = default;
            _session = default;
        }

        public void Update()
        {
            Activity current = Status;
            Activity next;
            do
            {
                next = current;
                switch (current)
                {
                    case Activity.Promised:
                        if (NetworkTime.time >= _session.FinishTime)
                        {
                            current = Activity.Requested;
                        }
                        break;
                    case Activity.Requested:
                        if (NetworkTime.time >= _request.Body.FinishTime)
                        {
                            current = Activity.Idle;
                        }
                        break;
                }
            } while (current != next);
            Status = current;
        }
    }
}