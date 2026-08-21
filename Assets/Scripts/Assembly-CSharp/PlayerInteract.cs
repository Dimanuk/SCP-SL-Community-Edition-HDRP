using CustomPlayerEffects;
using Interactables.Interobjects.DoorUtils;
using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.Keycards;
using Mirror;
using Respawning;
using Security;
using UnityEngine;
using GameCore;
using System;

public class PlayerInteract : NetworkBehaviour
{
    private enum AlphaPanelOperations : byte
    {
        Cancel = 0,
        Lever = 1
    }

    internal enum Generator079Operations : byte
    {
        Door = 0,
        Tablet = 1,
        Cancel = 2
    }

    internal static bool Scp096DestroyLockedDoors;
    internal static bool CanDisarmedInteract;
    private const float ActivationTokenReward = 1f;

    public LayerMask mask;

    private ServerRoles _sr;
    private Inventory _inv;
    private string _uiToggleKey;
    private bool _enableUiToggle;
    private Invisible _invisible;
    private RateLimit _playerInteractRateLimit;
    private ReferenceHub _hub;
    private KeyCode _interactKey;

    private bool CanInteract
    {
        get
        {
            if (_playerInteractRateLimit.CanExecute(true) && (!InventorySystem.Disarming.DisarmedPlayers.IsDisarmed(_hub.inventory) || CanDisarmedInteract))
            {
                return !_hub.interCoordinator.AnyBlocker(BlockedInteraction.GeneralInteractions);
            }
            return false;
        }
    }

    private void Start()
    {
        _hub = GetComponent<ReferenceHub>();
        _playerInteractRateLimit = _hub.playerRateLimitHandler.RateLimits[0];
        _sr = _hub.serverRoles;
        _inv = _hub.inventory;
        _invisible = _hub.playerEffectsController.GetEffect<Invisible>();

        if (isLocalPlayer)
        {
            _enableUiToggle = ConfigFile.ServerConfig.GetBool("enable_ui_toggle", false);
            _uiToggleKey = ConfigFile.ServerConfig.GetString("ui_toggle_key", "numlock");

            string keyString = ConfigFile.ServerConfig.GetString("interact_key", "E");
            if (!Enum.TryParse(keyString, true, out _interactKey))
            {
                _interactKey = KeyCode.E;
            }
        }
    }

    private void Update()
    {
        if (!isLocalPlayer) return;

        if (_enableUiToggle && Input.GetKeyDown(_uiToggleKey))
        {
            ToggleUI();
        }

        if (Input.GetKeyDown(_interactKey))
        {
            if (!PlayerRoles.PlayerRolesUtils.IsAlive(_hub)) return;

            Ray ray = new(_hub.PlayerCameraReference.position, _hub.PlayerCameraReference.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, 3f, mask))
            {
                GameObject target = hit.collider.gameObject;

                if (target.CompareTag("AW_Button"))
                {
                    CmdSwitchAWButton();
                }
                else if (target.CompareTag("AW_Detonation"))
                {
                    CmdDetonateWarhead();
                }
                else if (target.CompareTag("AW_Panel"))
                {
                    AlphaPanelOperations op = target.name.ToLower().Contains("cancel")
                        ? AlphaPanelOperations.Cancel
                        : AlphaPanelOperations.Lever;
                    CmdUsePanel(op);
                }
            }
        }
    }

    private void ToggleUI()
    {
        string[] canvasNames = { "Player Crosshair Canvas", "Player Canvas" };
        foreach (string cName in canvasNames)
        {
            GameObject canvasObj = GameObject.Find(cName);
            if (canvasObj != null && canvasObj.TryGetComponent<Canvas>(out var canvas))
            {
                canvas.enabled = !canvas.enabled;
            }
        }
    }

    [Command(channel = 4)]
    private void CmdUsePanel(AlphaPanelOperations n)
    {
        if (!CanInteract) return;

        var nukeside = AlphaWarheadOutsitePanel.nukeside;
        if (nukeside == null || !ChckDis(nukeside.transform.position)) return;

        switch (n)
        {
            case AlphaPanelOperations.Cancel:
                if (AlphaWarheadController.Detonated)
                {
                    OnInteract();
                    AlphaWarheadController.Singleton.CancelDetonation(_hub);
                }
                break;

            case AlphaPanelOperations.Lever:
                if (nukeside.AllowChangeLevelState())
                {
                    OnInteract();
                    nukeside.nukenabled = !nukeside.nukenabled;
                    RpcLeverSound();
                }
                break;
        }
    }

    [ClientRpc]
    private void RpcLeverSound()
    {
        var nukeside = AlphaWarheadOutsitePanel.nukeside;
        if (nukeside != null && nukeside.lever != null)
        {
            if (nukeside.lever.TryGetComponent<AudioSource>(out var source))
            {
                source.Play();
            }
        }
    }

    [Command(channel = 4)]
    private void CmdSwitchAWButton()
    {
        if (!CanInteract) return;

        GameObject panelObj = GameObject.Find("OutsitePanelScript");
        if (panelObj == null || !ChckDis(panelObj.transform.position)) return;

        bool hasAccess = _sr.BypassMode;
        if (!hasAccess && _inv.CurInstance is KeycardItem keycard)
        {
            hasAccess = keycard.Permissions.HasFlag(KeycardPermissions.AlphaWarhead);
        }

        if (hasAccess)
        {
            var panel = panelObj.GetComponentInParent<AlphaWarheadOutsitePanel>();
            if (panel != null && !panel.keycardEntered)
            {
                OnInteract();
                panel.keycardEntered = true;

                if (_hub.TryGetAssignedSpawnableTeam(out var team))
                {
                    RespawnTokensManager.GrantTokens(team, ActivationTokenReward);
                }
            }
        }
    }

    [Command(channel = 4)]
    private void CmdDetonateWarhead()
    {
        if (!CanInteract) return;

        GameObject panelObj = GameObject.Find("OutsitePanelScript");
        var nukeside = AlphaWarheadOutsitePanel.nukeside;

        if (panelObj != null && nukeside != null && ChckDis(panelObj.transform.position))
        {
            var panel = panelObj.GetComponent<AlphaWarheadOutsitePanel>();
            if (nukeside.nukenabled && panel.keycardEntered && !AlphaWarheadController.Singleton.IsLocked)
            {
                OnInteract();
                AlphaWarheadController.Singleton.StartDetonation(false, false, _hub);
            }
        }
    }

    private bool ChckDis(Vector3 pos)
    {
        return Vector3.Distance(transform.position, pos) < 3.63f;
    }

    private void OnInteract()
    {
        if (_invisible != null)
        {
            _invisible.ServerDisable();
        }
    }
}