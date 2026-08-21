using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropDownController : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private List<int> indexesToDisable = new List<int>();
    private TMP_Dropdown _dropdown;

    private void Awake()
    {
        _dropdown = GetComponent<TMP_Dropdown>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Canvas canvas = GetComponentInChildren<Canvas>();
        if (canvas != null)
        {
            Toggle[] toggles = canvas.GetComponentsInChildren<Toggle>(true);
            for (int i = 1; i < toggles.Length; i++)
            {
                toggles[i].interactable = !indexesToDisable.Contains(i - 1);
            }
        }
    }

    public void EnableOption(int index, bool enable)
    {
        if (index >= 1 && index < _dropdown.options.Count)
        {
            if (enable)
            {
                if (indexesToDisable.Contains(index))
                    indexesToDisable.Remove(index);
            }
            else
            {
                if (!indexesToDisable.Contains(index))
                    indexesToDisable.Add(index);
            }

            Canvas canvas = GetComponentInChildren<Canvas>();
            if (canvas != null)
            {
                Toggle[] toggles = canvas.GetComponentsInChildren<Toggle>(true);
                if (index < toggles.Length)
                {
                    toggles[index].interactable = enable;
                }
            }
        }
        else
        {
            Debug.LogWarning("Index out of range -> ignored!", this);
        }
    }

    public void EnableOption(string label, bool enable)
    {
        int foundIndex = _dropdown.options.FindIndex(o => o.text == label);
        EnableOption(foundIndex + 1, enable);
    }
}