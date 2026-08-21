using Interactables.Interobjects.DoorUtils;
using Mirror;
using System.Runtime.InteropServices;
using UnityEngine;

public class AlphaWarheadNukesitePanel : NetworkBehaviour
{
    public Transform lever;
    public BlastDoor blastDoor;

    public Material led_blastdoors;
    public Material led_outsidedoor;
    public Material led_detonationinprogress;
    public Material led_cancel;
    public Material[] onOffMaterial;

    private float _leverStatus;

    [SyncVar]
    public bool nukenabled;

    private const string OutsideDoorName = "SURFACE_NUKE";

    private static readonly int _emissionColor = Shader.PropertyToID("_EmissionColor");

    private bool _doorFound;
    private DoorNametagExtension _outsideDoor;

    private bool OutsideDoorOpen
    {
        get
        {
            if (!_doorFound)
            {
                _doorFound = DoorNametagExtension.NamedDoors.TryGetValue(OutsideDoorName, out _outsideDoor);
            }

            if (_outsideDoor?.TargetDoor != null)
                return _outsideDoor.TargetDoor.TargetState;

            return false;
        }
    }

    private void Awake()
    {
        AlphaWarheadOutsitePanel.nukeside = this;
    }

    private void FixedUpdate()
    {
        UpdateLeverStatus();
    }

    public bool AllowChangeLevelState()
    {
        if (Mathf.Abs(_leverStatus) < 0.001f)
            return true;
        return Mathf.Abs(_leverStatus - 1f) < 0.001f;
    }

    private void UpdateLeverStatus()
    {
        led_detonationinprogress.SetColor(_emissionColor,
            AlphaWarheadController.Singleton != null && AlphaWarheadController.TimeUntilDetonation > 0
                ? Color.red : Color.black);

        led_outsidedoor.SetColor(_emissionColor, OutsideDoorOpen ? Color.green : Color.black);
        led_blastdoors.SetColor(_emissionColor, blastDoor != null && !blastDoor.isClosed ? Color.green : Color.black);

        led_cancel.SetColor(_emissionColor,
            AlphaWarheadController.Singleton != null && AlphaWarheadController.TimeUntilDetonation > 10f
                ? Color.red : Color.black);

        if (lever != null)
        {
            float angle = Mathf.Lerp(10f, -170f, _leverStatus);
            lever.localRotation = Quaternion.Euler(angle, -90f, 90f);
        }

        float target = nukenabled ? 1f : 0f;
        _leverStatus = Mathf.MoveTowards(_leverStatus, target, Time.fixedDeltaTime * 2f);

        if (onOffMaterial != null && onOffMaterial.Length >= 2)
        {
            int litIndex = Mathf.RoundToInt(_leverStatus);
            for (int i = 0; i < 2; i++)
            {
                onOffMaterial[i].SetColor(_emissionColor, i == litIndex ? new Color(1.2f, 1.2f, 1.2f, 1f) : Color.black);
            }
        }
    }
}