using UnityEditor;

/// <summary>
/// Editor-only toggle that lets us launch the in-editor Play session as a headless
/// dedicated server instead of a client. The flag is read by <see cref="ServerStatic.Awake"/>
/// (under <c>#if UNITY_EDITOR</c>) to set <see cref="ServerStatic.IsDedicated"/>.
/// See project_editor_host_dedicated_fix for why the editor normally forces client mode.
/// </summary>
internal static class EditorDedicatedToggle
{
    private const string MenuPath = "SCP/Run As Dedicated Server";

    [MenuItem(MenuPath, false, 100)]
    private static void Toggle()
    {
        bool enabled = !EditorPrefs.GetBool(ServerStatic.EditorDedicatedPrefKey, false);
        EditorPrefs.SetBool(ServerStatic.EditorDedicatedPrefKey, enabled);
        UnityEngine.Debug.Log($"[Editor] Run As Dedicated Server: {(enabled ? "ENABLED" : "disabled")}");
    }

    [MenuItem(MenuPath, true)]
    private static bool ToggleValidate()
    {
        Menu.SetChecked(MenuPath, EditorPrefs.GetBool(ServerStatic.EditorDedicatedPrefKey, false));
        return true;
    }
}
