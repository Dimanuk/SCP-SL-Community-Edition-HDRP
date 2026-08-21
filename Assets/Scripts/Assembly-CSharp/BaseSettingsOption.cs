using System;
using UnityEngine;

[Serializable]
public class BaseSettingsOption
{
    public SettingsOption SettingOption;

    public bool DependsOnLight;
    public bool DependsOnShadows;

    [SerializeField]
    protected GameObject _uiElementGameObject;

    protected virtual GameObject UIelementGameObject
    {
        get => _uiElementGameObject;
        set => _uiElementGameObject = value;
    }

    public GameObject GetRootObject
    {
        get
        {
            if (UIelementGameObject == null)
                return null;

            Transform current = UIelementGameObject.transform;

            if (current.parent != null)
            {
                Transform parent = current.parent;
                if (parent.parent != null)
                {
                    return parent.parent.gameObject;
                }
                return parent.gameObject;
            }

            return UIelementGameObject;
        }
    }
}