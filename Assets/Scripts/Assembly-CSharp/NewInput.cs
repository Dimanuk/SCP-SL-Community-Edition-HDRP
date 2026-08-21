using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using NorthwoodLib.Pools;

public static class NewInput
{
    public static event Action<ActionName, KeyCode> OnKeyModified;
    public static event Action OnAnyModified;

    public class ActionDefinition
    {
        public readonly ActionName Name;
        public readonly ActionCategory Category;
        public readonly KeyCode DefaultKey;
        public readonly ActionName[] CompatibleActions;

        public static event Action<ActionName, KeyCode> OnKeyModified;
        public static event Action OnAnyModified;

        public ActionDefinition(ActionName actionName, KeyCode k, ActionCategory c, params ActionName[] compatibleActions)
        {
            Name = actionName;
            Category = c;
            DefaultKey = k;
            CompatibleActions = compatibleActions;
        }
    }

    public static readonly Dictionary<ActionName, KeyCode> UserKeybinds;
    private static readonly string SaveFilePath;
    private static readonly ActionDefinition[] DefinedActions;
    private static bool _loaded;

    static NewInput()
    {
        UserKeybinds = new Dictionary<ActionName, KeyCode>();

        SaveFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "SCP Secret Laboratory/keybinding.txt"
        );

        DefinedActions = new ActionDefinition[38];

        DefinedActions[0] = new ActionDefinition((ActionName)0, KeyCode.Mouse0, (ActionCategory)1);
        DefinedActions[1] = new ActionDefinition((ActionName)1, KeyCode.Mouse1, (ActionCategory)1);
        DefinedActions[2] = new ActionDefinition((ActionName)2, KeyCode.Space, (ActionCategory)2);
        DefinedActions[3] = new ActionDefinition((ActionName)3, KeyCode.E, (ActionCategory)0);
        DefinedActions[4] = new ActionDefinition((ActionName)4, KeyCode.Tab, (ActionCategory)0);
        DefinedActions[5] = new ActionDefinition((ActionName)5, KeyCode.R, (ActionCategory)1);
        DefinedActions[6] = new ActionDefinition((ActionName)6, KeyCode.LeftShift, (ActionCategory)2);
        DefinedActions[7] = new ActionDefinition((ActionName)7, KeyCode.Q, (ActionCategory)4);
        DefinedActions[8] = new ActionDefinition((ActionName)8, KeyCode.C, (ActionCategory)2);
        DefinedActions[9] = new ActionDefinition((ActionName)9, KeyCode.W, (ActionCategory)2);
        DefinedActions[10] = new ActionDefinition((ActionName)10, KeyCode.S, (ActionCategory)2);
        DefinedActions[11] = new ActionDefinition((ActionName)11, KeyCode.A, (ActionCategory)2);
        DefinedActions[12] = new ActionDefinition((ActionName)12, KeyCode.D, (ActionCategory)2);
        DefinedActions[13] = new ActionDefinition((ActionName)13, KeyCode.N, (ActionCategory)0);
        DefinedActions[14] = new ActionDefinition((ActionName)14, KeyCode.F1, (ActionCategory)0);
        DefinedActions[15] = new ActionDefinition((ActionName)15, KeyCode.M, (ActionCategory)5);
        DefinedActions[16] = new ActionDefinition((ActionName)16, KeyCode.F, (ActionCategory)1);
        DefinedActions[17] = new ActionDefinition((ActionName)17, KeyCode.V, (ActionCategory)4);
        DefinedActions[18] = new ActionDefinition((ActionName)18, KeyCode.LeftAlt, (ActionCategory)5);
        DefinedActions[19] = new ActionDefinition((ActionName)19, KeyCode.BackQuote, (ActionCategory)5);
        DefinedActions[20] = new ActionDefinition((ActionName)20, KeyCode.LeftControl, (ActionCategory)3);
        DefinedActions[21] = new ActionDefinition((ActionName)21, KeyCode.I, (ActionCategory)1);
        DefinedActions[22] = new ActionDefinition((ActionName)22, KeyCode.Mouse2, (ActionCategory)1);
        DefinedActions[23] = new ActionDefinition((ActionName)23, KeyCode.T, (ActionCategory)0);
        DefinedActions[24] = new ActionDefinition((ActionName)24, KeyCode.Alpha1, (ActionCategory)3);
        DefinedActions[25] = new ActionDefinition((ActionName)25, KeyCode.Alpha2, (ActionCategory)3);
        DefinedActions[26] = new ActionDefinition((ActionName)26, KeyCode.X, (ActionCategory)3);
        DefinedActions[27] = new ActionDefinition((ActionName)27, KeyCode.P, (ActionCategory)5);
        DefinedActions[28] = new ActionDefinition((ActionName)28, KeyCode.O, (ActionCategory)5);
        DefinedActions[29] = new ActionDefinition((ActionName)29, KeyCode.G, (ActionCategory)3);
        DefinedActions[30] = new ActionDefinition((ActionName)30, KeyCode.Escape, (ActionCategory)6);
        DefinedActions[31] = new ActionDefinition((ActionName)31, KeyCode.F4, (ActionCategory)6);

        DefinedActions[32] = new global::PlayerRoles.PlayableScps.Scp079.Scp079Keybinds.ActionDefinition((ActionName)32, KeyCode.Space);
        DefinedActions[33] = new global::PlayerRoles.PlayableScps.Scp079.Scp079Keybinds.ActionDefinition((ActionName)33, KeyCode.Mouse1);
        DefinedActions[34] = new global::PlayerRoles.PlayableScps.Scp079.Scp079Keybinds.ActionDefinition((ActionName)34, KeyCode.R);
        DefinedActions[35] = new global::PlayerRoles.PlayableScps.Scp079.Scp079Keybinds.ActionDefinition((ActionName)35, KeyCode.F);
        DefinedActions[36] = new global::PlayerRoles.PlayableScps.Scp079.Scp079Keybinds.ActionDefinition((ActionName)36, KeyCode.G);
        DefinedActions[37] = new global::PlayerRoles.PlayableScps.Scp079.Scp079Keybinds.ActionDefinition((ActionName)37, KeyCode.E);
    }

    public static KeyCode GetKey(ActionName actionName, KeyCode fallbackKeycode = KeyCode.None)
    {
        if (!_loaded)
            Load();

        if (UserKeybinds.TryGetValue(actionName, out KeyCode value))
            return value;

        return fallbackKeycode;
    }

    public static bool GetKeyDown(ActionName actionName)
    {
        KeyCode key = GetKey(actionName);
        return UnityEngine.Input.GetKeyDown(key);
    }

    public static bool GetKeyUp(ActionName actionName)
    {
        KeyCode key = GetKey(actionName);
        return UnityEngine.Input.GetKeyUp(key);
    }

    public static bool GetKeyHeld(ActionName actionName)
    {
        KeyCode key = GetKey(actionName);
        return UnityEngine.Input.GetKey(key);
    }

    public static void SetKey(ActionName actionName, KeyCode keyCode)
    {
        UserKeybinds[actionName] = keyCode;
        Save();

        OnKeyModified?.Invoke(actionName, keyCode);
        OnAnyModified?.Invoke();  
    }

    public static void Save()
    {
        StringBuilder sb = StringBuilderPool.Shared.Rent();

        foreach (KeyValuePair<ActionName, KeyCode> kvp in UserKeybinds)
        {
            sb.Append((int)kvp.Key);
            sb.Append(':');
            sb.Append((int)kvp.Value);
            sb.Append(';');
        }

        string content = sb.ToString(0, sb.Length - 1);
        File.WriteAllText(SaveFilePath, content);

        StringBuilderPool.Shared.Return(sb);
    }

    public static void Load()
    {
        ResetToDefault();

        if (!File.Exists(SaveFilePath))
        {
            Save();
            return;
        }

        string[] entries = File.ReadAllText(SaveFilePath).Split(';');
        for (int i = 0; i < entries.Length; i++)
        {
            string[] parts = entries[i].Split(':');
            if (parts.Length != 2)
                continue;

            if (TryParseActionName(parts[0], out ActionName action) &&
                TryParseKeycode(parts[1], out KeyCode key))
            {
                UserKeybinds[action] = key;
            }
        }

        _loaded = true;
    }

    public static bool TryParseActionName(string s, out ActionName actionName)
    {
        if (int.TryParse(s, out int result) && Enum.IsDefined(typeof(ActionName), result))
        {
            actionName = (ActionName)result;
            return true;
        }

        actionName = (ActionName)0;
        return false;
    }

    public static bool TryParseKeycode(string s, out KeyCode keyCode)
    {
        if (int.TryParse(s, out int result) && Enum.IsDefined(typeof(KeyCode), result))
        {
            keyCode = (KeyCode)result;
            return true;
        }

        keyCode = KeyCode.None;
        return false;
    }

    [RuntimeInitializeOnLoadMethod]
    public static void ResetToDefault()
    {
        UserKeybinds.Clear();

        foreach (ActionDefinition def in DefinedActions)
        {
            UserKeybinds[def.Name] = def.DefaultKey;
        }
    }

    public static bool IsCompatible(this ActionName sourceAction, ActionName actionToCompare)
    {
        foreach (ActionDefinition def in DefinedActions)
        {
            if (def.Name != sourceAction)
                continue;

            if (def.CompatibleActions == null || def.CompatibleActions.Length == 0)
                return false;

            foreach (ActionName compatible in def.CompatibleActions)
            {
                if (compatible == actionToCompare)
                    return true;
            }

            break;
        }

        return false;
    }

    public static bool TryGetCategory(this ActionName sourceAction, out ActionCategory cat)
    {
        foreach (ActionDefinition def in DefinedActions)
        {
            if (def.Name == sourceAction)
            {
                cat = def.Category;
                return true;
            }
        }

        cat = (ActionCategory)0;
        return false;
    }
}