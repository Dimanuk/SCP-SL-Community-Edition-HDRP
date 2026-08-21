using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MEC; 

public class ChangeKeyBinding : MonoBehaviour
{
    private static HashSet<ActionName> BlacklistedLMBs = new HashSet<ActionName>
    {
        (ActionName)4,  
        (ActionName)13,  
        (ActionName)14,  
        (ActionName)15,  
        (ActionName)19   
    };

    private static ActionCategory[] CategorySortingOrder = new ActionCategory[]
    {
        (ActionCategory)2,
        (ActionCategory)0,
        (ActionCategory)1,
        (ActionCategory)3,
        (ActionCategory)4,
        (ActionCategory)7,
        (ActionCategory)5
    };

    private static Dictionary<KeyCode, KeyValuePair<ActionName, Text>> AlreadyUsedKeycodes =
        new Dictionary<KeyCode, KeyValuePair<ActionName, Text>>();

    public Transform list_parent;
    public GameObject list_element;
    public GameObject _categoryTemplate;
    public HorizontalLayoutGroup _categoryParent;

    private ActionCategory _selectedCategory;
    private GameObject[] _categoryButtons;
    private List<GameObject> _instances = new List<GameObject>();
    private bool _working;


    private void Start()
    {
        int length = CategorySortingOrder.Length;
        _categoryButtons = new GameObject[length];

        for (int i = 0; i < length; i++)
        {
            ActionCategory cat = CategorySortingOrder[i];

            GameObject newButton = Instantiate(_categoryTemplate, _categoryTemplate.transform.parent);
            newButton.SetActive(true);

            Text textComponent = newButton.GetComponent<Text>();
            textComponent.text = Translations.Get<ActionCategory>(cat); 

            Button buttonComponent = newButton.GetComponent<Button>();
            buttonComponent.onClick.AddListener(() => SelectCategory(newButton, cat));

            newButton.transform.localScale = Vector3.one;
            _categoryButtons[i] = newButton;
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(_categoryParent.transform as RectTransform);

        if (_categoryButtons.Length > 0)
            SelectCategory(_categoryButtons[0], CategorySortingOrder[0]);
    }

    private void SelectCategory(GameObject button, ActionCategory cat)
    {
        _selectedCategory = cat;

        for (int i = 0; i < _categoryButtons.Length; i++)
        {
            GameObject btn = _categoryButtons[i];
            Image img = btn.GetComponentInChildren<Image>();
            if (img != null)
                img.enabled = (btn == button);
        }

        RefreshList();
    }

    private void RefreshList()
    {
        _working = false;

        foreach (GameObject go in _instances)
            Destroy(go);
        _instances.Clear();

        AlreadyUsedKeycodes.Clear();

        NewInput.Load();

        foreach (KeyValuePair<ActionName, KeyCode> kvp in NewInput.UserKeybinds)
        {
            if (NewInput.TryGetCategory(kvp.Key, out ActionCategory cat) && cat == _selectedCategory)
            {
                SetupBinding(kvp.Key, kvp.Value);
            }
        }
    }

    private void SetupBinding(ActionName action, KeyCode curKey)
    {
        GameObject item = Instantiate(list_element, list_parent);
        item.transform.localScale = Vector3.one;
        item.SetActive(true);
        _instances.Add(item);

        Text nameText = item.GetComponentInChildren<Text>();
        nameText.text = Translations.Get(action); 

        Button keyButton = item.GetComponentInChildren<Button>();
        Text keyText = keyButton.GetComponentInChildren<Text>();
        keyText.text = curKey.ToString();      

        KeyBindElement bindElement = keyButton.GetComponent<KeyBindElement>();
        bindElement.Action = action;

        if (AlreadyUsedKeycodes.TryGetValue(curKey, out KeyValuePair<ActionName, Text> existing))
        {
            if (existing.Key != action)
            {
                string colored = "<color=red>" + nameText.text + "</color>";
                nameText.text = colored;
                keyText.text = colored;
            }
        }

        AlreadyUsedKeycodes[curKey] = new KeyValuePair<ActionName, Text>(action, keyText);
    }

    public void ChangeKey(ActionName action)
    {
        Timing.RunCoroutine(_AwaitPress(action), Segment.Update);
    }

    private IEnumerator<float> _AwaitPress(ActionName action)
    {
        if (!_working)
            _working = true;

        while (true)
        {
            yield return float.NegativeInfinity; 

            KeyCode key = GetCurrentKey();

            if (key == KeyCode.None)
                continue;
            if (key == KeyCode.Escape)        
                break;
            if (key == KeyCode.Mouse0 && BlacklistedLMBs.Contains(action))
                continue;                   

            NewInput.UserKeybinds[action] = key;
            NewInput.Save();
            RefreshList();
            yield break;
        }

        RefreshList();
    }

    private KeyCode GetCurrentKey()
    {
        foreach (KeyCode kcode in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKey(kcode))
                return kcode;
        }
        return KeyCode.None;
    }

    public void Revent()
    {
        NewInput.ResetToDefault();
        RefreshList();
    }
}