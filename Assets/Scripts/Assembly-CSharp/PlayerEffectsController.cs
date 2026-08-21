using System;
using System.Collections.Generic;
using System.Text;
using CustomPlayerEffects;
using InventorySystem.Items;
using Mirror;
using NorthwoodLib.Pools;
using PlayerRoles;
using PlayerRoles.Spectating;
using UnityEngine;
using UnityEngine.Audio;
using Utils.NonAllocLINQ;

public class PlayerEffectsController : NetworkBehaviour
{
    public AudioMixer mixer;
    public GameObject effectsGameObject;

    private readonly Dictionary<Type, StatusEffectBase> _effectsByType = new();
    private readonly SyncList<byte> _syncEffectsIntensity = new SyncList<byte>();

    private bool _wasSpectated;
    private ReferenceHub _hub;

    public StatusEffectBase[] AllEffects { get; private set; }
    public int EffectsLength { get; private set; }

    public bool TryGetEffect(string effectName, out StatusEffectBase playerEffect)
    {
        StatusEffectBase[] all = AllEffects;
        for (int i = 0; i < all.Length; i++)
        {
            if (all[i].name.StartsWith(effectName, StringComparison.InvariantCultureIgnoreCase))
            {
                playerEffect = all[i];
                return true;
            }
        }
        playerEffect = null;
        return false;
    }

    public bool TryGetEffect<T>(out T playerEffect) where T : StatusEffectBase
    {
        if (_effectsByType.TryGetValue(typeof(T), out StatusEffectBase value) && value is T val)
        {
            playerEffect = val;
            return true;
        }
        playerEffect = null;
        return false;
    }

    [Server]
    public void UseMedicalItem(ItemBase item)
    {
        if (!NetworkServer.active)
        {
            Debug.LogWarning("[Server] function 'System.Void PlayerEffectsController::UseMedicalItem(InventorySystem.Items.ItemBase)' called when server was not active");
            return;
        }

        foreach (StatusEffectBase effect in AllEffects)
        {
            if (effect is IHealablePlayerEffect healable && healable.IsHealable(item.ItemTypeId))
                effect.IsEnabled = false;
        }
    }

    [Server]
    public StatusEffectBase ChangeState(string effectName, byte intensity, float duration = 0f, bool addDuration = false)
    {
        if (!NetworkServer.active)
        {
            Debug.LogWarning("[Server] function 'CustomPlayerEffects.StatusEffectBase PlayerEffectsController::ChangeState(System.String,System.Byte,System.Single,System.Boolean)' called when server was not active");
            return null;
        }

        if (TryGetEffect(effectName, out StatusEffectBase effect))
            effect.ServerSetState(intensity, duration, addDuration);

        return effect;
    }

    [Server]
    public T ChangeState<T>(byte intensity, float duration = 0f, bool addDuration = false) where T : StatusEffectBase
    {
        if (!NetworkServer.active)
        {
            Debug.LogWarning("[Server] function 'T PlayerEffectsController::ChangeState(System.Byte,System.Single,System.Boolean)' called when server was not active");
            return null;
        }

        if (TryGetEffect<T>(out T effect))
            effect.ServerSetState(intensity, duration, addDuration);

        return effect;
    }

    [Server]
    public T EnableEffect<T>(float duration = 0f, bool addDuration = false) where T : StatusEffectBase
        => ChangeState<T>(1, duration, addDuration);

    [Server]
    public T DisableEffect<T>() where T : StatusEffectBase
        => ChangeState<T>(0);

    public void DisableAllEffects()
    {
        foreach (StatusEffectBase effect in AllEffects)
            effect.ServerDisable();
    }

    public T GetEffect<T>() where T : StatusEffectBase
        => TryGetEffect<T>(out T effect) ? effect : null;

    public void GetAllSpectatorEffects(StringBuilder strb)
    {
        foreach (StatusEffectBase effect in AllEffects)
        {
            if (!effect.IsEnabled)
                continue;
            if (effect is ISpectatorDataPlayerEffect spectatorData && spectatorData.GetSpectatorText(out string display))
                strb.AppendFormat("<color=#DC143C>{0}</color>", display);
        }
    }

    [Server]
    public void ServerSyncEffect(StatusEffectBase effect)
    {
        if (!NetworkServer.active)
        {
            Debug.LogWarning("[Server] function 'System.Void PlayerEffectsController::ServerSyncEffect(CustomPlayerEffects.StatusEffectBase)' called when server was not active");
            return;
        }

        for (int i = 0; i < EffectsLength; i++)
        {
            if (AllEffects[i] == effect)
            {
                _syncEffectsIntensity[i] = effect.Intensity;
                break;
            }
        }
    }

    public void ServerSendPulse<T>() where T : IPulseEffect
    {
        for (int i = 0; i < EffectsLength; i++)
        {
            if (AllEffects[i] is T)
            {
                byte index = (byte)Mathf.Min(i, 255);

                TargetRpcReceivePulse(_hub.connectionToClient, index);

                SpectatorNetworking.ForeachSpectatorOf(_hub, spectator =>
                {
                    if (spectator != null)
                    {
                        TargetRpcReceivePulse(spectator.connectionToClient, index);
                    }
                });

                return;
            }
        }
    }


    /*
    [Client]
    private void ClientSyncEffects(PlayerEffectsController target)
    {
        if (!NetworkClient.active)
        {
            Debug.LogWarning("[Client] function 'System.Void PlayerEffectsController::ClientSyncEffects(PlayerEffectsController)' called when client was not active");
            return;
        }

        if (AllEffects == null || target == null || target._syncEffectsIntensity == null)
            return;

        for (int i = 0; i < EffectsLength; i++)
        {
            if (i >= AllEffects.Length || i >= target._syncEffectsIntensity.Count)
                break;

            StatusEffectBase effect = AllEffects[i];
            if (effect == null)
                continue;

            byte intensity = target._syncEffectsIntensity[i];
            effect.ForceIntensity(intensity);
        }
    }
    */

    [TargetRpc]
    private void TargetRpcReceivePulse(NetworkConnection _, byte effectIndex)
    {
        int idx = Mathf.Min(effectIndex, EffectsLength - 1);
        if (AllEffects[idx] is IPulseEffect pulse)
            pulse.ExecutePulse();
    }

    private void Awake()
    {
        _hub = ReferenceHub.GetHub(gameObject);

        AllEffects = effectsGameObject.GetComponentsInChildren<StatusEffectBase>();
        EffectsLength = AllEffects.Length;

        foreach (StatusEffectBase effect in AllEffects)
        {
            _effectsByType.Add(effect.GetType(), effect);
            _syncEffectsIntensity.Add(0);
        }
    }

    private void Update()
    {
        if (!NetworkClient.active) return;

        for (int i = 0; i < EffectsLength; i++)
            AllEffects[i].Intensity = _syncEffectsIntensity[i];
    }

    private void Start()
    {
        effectsGameObject.SetActive(true);
    }

    private void OnEnable() => PlayerRoleManager.OnRoleChanged += OnRoleChanged;
    private void OnDisable() => PlayerRoleManager.OnRoleChanged -= OnRoleChanged;

    private void OnRoleChanged(ReferenceHub target, PlayerRoleBase oldRole, PlayerRoleBase newRole)
    {
        if (target != _hub) return;

        bool death = oldRole.Team != Team.Dead && newRole.Team == Team.Dead;

        foreach (StatusEffectBase effect in AllEffects)
        {
            if (death)
                effect.OnDeath(oldRole);
            else
                effect.OnRoleChanged(oldRole, newRole);
        }
    }

    [RuntimeInitializeOnLoadMethod]
    private static void Init()
    {
        SpectatorTargetTracker.OnTargetChanged += () =>
        {
            if (ReferenceHub.AllHubs.TryGetFirst(h => h != null && h.playerEffectsController != null && h.playerEffectsController._wasSpectated, out ReferenceHub prev))
            {
                foreach (var e in prev.playerEffectsController.AllEffects)
                    e.OnStopSpectating();
                prev.playerEffectsController._wasSpectated = false;
            }

            if (SpectatorTargetTracker.TryGetTrackedPlayer(out ReferenceHub hub) && hub != null && hub.playerEffectsController != null)
            {
                var controller = hub.playerEffectsController;
                foreach (var e in controller.AllEffects)
                    e.OnBeginSpectating();
                controller._wasSpectated = true;
            }
        };
    }
}