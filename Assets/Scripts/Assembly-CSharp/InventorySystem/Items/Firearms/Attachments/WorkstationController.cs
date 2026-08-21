using Interactables;
using Interactables.Verification;
using Mirror;
using PlayerRoles;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Serialization;

namespace InventorySystem.Items.Firearms.Attachments
{
    public class WorkstationController : NetworkBehaviour, IClientInteractable, IInteractable, IServerInteractable
    {
        public enum WorkstationStatus : byte
        {
            Offline,
            PoweringUp,
            PoweringDown,
            Online
        }

        public static readonly HashSet<WorkstationController> AllWorkstations = new HashSet<WorkstationController>();

        [SerializeField]
        public WorkstationAttachmentSelector _selector;

        [SerializeField]
        public GameObject _idleScreen;

        [SerializeField]
        public GameObject _powerupScreen;

        [SerializeField]
        public GameObject _powerdownScreen;

        [SerializeField]
        public GameObject _selectorScreen;

        [FormerlySerializedAs("_activateCollder")]
        public InteractableCollider ActivateCollider;

        [SyncVar]
        public byte Status;

        public ReferenceHub KnownUser;

        public readonly Stopwatch ServerStopwatch = new Stopwatch();

        public const float StandbyDistance = 2.4f;

        public const float UpkeepTime = 30f;

        public const float CheckTime = 5f;

        public const float PowerupTime = 1f;

        public const float PowerdownTime = 2.5f;

        public byte _prevStatus;

        public IVerificationRule VerificationRule => StandardDistanceVerification.Default;

        public void ClientInteract(InteractableCollider collider)
        {
            if (collider is WorkstationSelectorCollider workstationSelectorCollider)
            {
                _selector.ProcessCollider(workstationSelectorCollider.ColliderId);
            }
        }

        public void ServerInteract(ReferenceHub ply, byte colliderId)
        {
            if (colliderId == ActivateCollider.ColliderId && Status == 0)
            {
                Status = 1;
                ServerStopwatch.Restart();
            }
        }

        public void Start()
        {
            AllWorkstations.Add(this);
        }

        public void OnDestroy()
        {
            AllWorkstations.Remove(this);
        }

        public void Update()
        {
            if (NetworkServer.active)
            {
                if (Status == 0)
                {
                    return;
                }

                switch ((WorkstationStatus)Status)
                {
                    case WorkstationStatus.PoweringUp:
                        if (ServerStopwatch.Elapsed.TotalSeconds > 1.0)
                        {
                            Status = 3;
                            ServerStopwatch.Restart();
                        }

                        break;
                    case WorkstationStatus.PoweringDown:
                        if (ServerStopwatch.Elapsed.TotalSeconds > 2.5)
                        {
                            Status = 0;
                            ServerStopwatch.Stop();
                        }

                        break;
                    case WorkstationStatus.Online:
                        if (ServerStopwatch.Elapsed.TotalSeconds < 30.0)
                        {
                            if (!(ServerStopwatch.Elapsed.TotalSeconds > 5.0))
                            {
                                break;
                            }

                            if (IsInRange(KnownUser))
                            {
                                ServerStopwatch.Restart();
                                break;
                            }

                            foreach (ReferenceHub allHub in ReferenceHub.AllHubs)
                            {
                                if (IsInRange(allHub))
                                {
                                    KnownUser = allHub;
                                    ServerStopwatch.Restart();
                                    break;
                                }
                            }
                        }
                        else
                        {
                            Status = 2;
                            ServerStopwatch.Restart();
                        }

                        break;
                }
            }

            if (_prevStatus != Status)
            {
                WorkstationStatus status = (WorkstationStatus)Status;
                _selector.enabled = status == WorkstationStatus.Online;
                _selectorScreen.SetActive(status == WorkstationStatus.Online);
                _idleScreen.SetActive(status == WorkstationStatus.Offline);
                _powerupScreen.SetActive(status == WorkstationStatus.PoweringUp);
                _powerdownScreen.SetActive(status == WorkstationStatus.PoweringDown);
                _prevStatus = Status;
            }
        }

        public bool IsInRange(ReferenceHub hub)
        {
            if (hub != null && hub.IsAlive() && Mathf.Abs(hub.transform.position.y - base.transform.position.y) < 2.4f && Mathf.Abs(hub.transform.position.x - base.transform.position.x) < 2.4f)
            {
                return Mathf.Abs(hub.transform.position.z - base.transform.position.z) < 2.4f;
            }

            return false;
        }
    }
}
