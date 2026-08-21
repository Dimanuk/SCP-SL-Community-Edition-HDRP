using System.Runtime.InteropServices;
using Interactables.Interobjects;
using Mirror;
using UnityEngine;

public class SqueakSpawner : NetworkBehaviour
{
    [SerializeField] private int spawnChancePercent = 10;
    [SerializeField] private GameObject[] mice;

    [SyncVar(hook = nameof(SyncMouseSpawn))]
    private byte syncSpawn;

    private SqueakInteraction _spawnedMouse;

    private void Awake()
    {
        if (NetworkServer.active && Random.Range(0, 100) <= spawnChancePercent)
        {
            syncSpawn = (byte)Random.Range(1, mice.Length + 1);
            SyncMouseSpawn(0, syncSpawn);
        }
    }

    [TargetRpc]
    public void TargetHitMouse(NetworkConnection target)
    {
        if (_spawnedMouse == null)
        {
            Debug.LogWarning("[Client] TargetHitMouse: _spawnedMouse is null");
            return;
        }

        if (!_spawnedMouse.enabled)
        {
            Debug.LogWarning("[Client] TargetHitMouse: _spawnedMouse is disabled");
            return;
        }

        _spawnedMouse.ClientInteract(null);
        _spawnedMouse.gameObject.SetActive(false);
    }

    private void SyncMouseSpawn(byte oldValue, byte newValue)
    {
        if (newValue != 0 && newValue <= mice.Length)
        {
            GameObject mouseObj = mice[newValue - 1];
            mouseObj.SetActive(true);
            _spawnedMouse = mouseObj.GetComponent<SqueakInteraction>();
        }
    }
}