
using PlayerRoles;
using TMPro;
using ToggleableMenus;
using UnityEngine;

public class SpectatorInterface : SimpleToggleableMenu
{
    public GameObject RootPanel;

    public static SpectatorInterface Singleton;

    public TextMeshProUGUI PlayerList;

    public TextMeshProUGUI PlayerInfo;

    [SerializeField]
    private TextMeshProUGUI _attachmentsEditorText;

    private Canvas _spectatorCanvas;

    public bool SpectatorCanvasActive { get; set; }

    public override bool CanToggle =>
        ReferenceHub.TryGetLocalHub(out var hub) && PlayerRolesUtils.GetRoleId(hub) == RoleTypeId.Spectator;

    protected override void Awake()
    {
        base.Awake();
        Singleton = this;
        _spectatorCanvas = GetComponent<Canvas>();
    }

    private void OnEnable()
    {
        KeyCode key = NewInput.GetKey(ActionName.Inventory);
        if (_attachmentsEditorText != null)
            _attachmentsEditorText.text = "Press " + key + " to edit attachment preferences while spectating.";
    }

    private void Update()
    {
        if (SpectatorCanvasActive && HideHUDController.IsHUDVisible)
        {
            if (_spectatorCanvas != null)
                _spectatorCanvas.enabled = true;
            return;
        }

        if (_spectatorCanvas != null)
            _spectatorCanvas.enabled = false;

        IsEnabled = false;
    }
}
