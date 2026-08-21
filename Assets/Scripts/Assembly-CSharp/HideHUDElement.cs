using UnityEngine;

public class HideHUDElement : MonoBehaviour
{
    [SerializeField]
    private Behaviour[] elementsToDisable;

    private void Awake()
    {
        HideHUDController.ToggleHUD += ToggleElement;

        if (!HideHUDController.IsHUDVisible)
        {
            ToggleElement(false);
        }
    }

    private void OnDestroy()
    {
        HideHUDController.ToggleHUD -= ToggleElement;
    }

    private void ToggleElement(bool isEnabled)
    {
        if (elementsToDisable == null)
            return;

        for (int i = 0; i < elementsToDisable.Length; i++)
        {
            if (elementsToDisable[i] != null)
            {
                elementsToDisable[i].enabled = isEnabled;
            }
        }
    }
}