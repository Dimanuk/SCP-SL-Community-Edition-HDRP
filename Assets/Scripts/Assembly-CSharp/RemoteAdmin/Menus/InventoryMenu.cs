using InventorySystem;
using InventorySystem.Items;
using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static RemoteAdmin.Menus.InventoryMenu;

namespace RemoteAdmin.Menus
{
    public class InventoryMenu : RaCommandMenu
    {
        public struct PrintedItem
        {
            public GameObject GameObject { get; internal set; }
            public ItemCategory Category { get; internal set; }
            public ItemType Type { get; internal set; }
            public string Name
            {
                get => SerializedString;
                internal set => SerializedString = value;
            }
            public string DisplayName { get; internal set; }
            public string SerializedString { get; private set; }

            public PrintedItem(GameObject obj, ItemType itemType, ItemCategory itemCategory, string name)
            {
                GameObject = obj;
                Type = itemType;
                Category = itemCategory;
                SerializedString = name;
                DisplayName = name;
            }

            public override string ToString() => SerializedString;
        }

        [SerializeField]
        private Transform _parent;

        [SerializeField]
        private GameObject _template;

        [SerializeField]
        private TMP_Dropdown _dropdown;

        [SerializeField]
        private TMP_InputField _itemSearchInput;

        [SerializeField]
        private float _searchAnimSpeed = 0.0185f;

        private CoroutineHandle _searchAnimHandle;

        protected List<PrintedItem> PrintedItems { get; private set; } = new List<PrintedItem>();

        public void ForceReorder()
        {
            if (_dropdown == null || PrintedItems == null || PrintedItems.Count == 0)
                return;

            int sortMode = _dropdown.value;
            Func<PrintedItem, object> keySelector = sortMode switch
            {
                0 => p => p.Type,
                1 => p => p.Category,
                2 => p => p.DisplayName,
                _ => p => p.DisplayName,
            };
            bool descending = RaSettings.Singleton?.ToggleItemOrder?.Value ?? false;
            var ordered = descending
                ? PrintedItems.OrderByDescending(keySelector)
                : PrintedItems.OrderBy(keySelector);

            foreach (var item in ordered)
            {
                item.GameObject?.transform?.SetAsFirstSibling();
            }
        }

        public void AnimatedSearch(string query)
        {
            Timing.KillCoroutines(_searchAnimHandle);

            if (string.IsNullOrEmpty(query))
                return;

            _searchAnimHandle = Timing.RunCoroutine(SearchAnimation(query.ToUpper()));
        }

        private IEnumerator<float> SearchAnimation(string query)
        {
            bool isEmpty = string.IsNullOrEmpty(query);
            int index = 0;

            foreach (var item in PrintedItems)
            {
                if (item.GameObject == null)
                    continue;

                var go = item.GameObject;
                bool wasActive = go.activeInHierarchy;

                bool shouldBeActive = isEmpty || item.SerializedString.Contains(query);
                go.SetActive(shouldBeActive);

                if (wasActive != go.activeInHierarchy)
                {
                    index++;
                    if (index >= 3)
                    {
                        index = 0;
                        yield return Timing.WaitForSeconds(_searchAnimSpeed);
                    }
                }
            }
        }

        protected override void OnStart()
        {
            if (_itemSearchInput != null)
                _itemSearchInput.onValueChanged.AddListener(OnSearchValueChanged);

            var availableItems = InventoryItemLoader.AvailableItems;
            foreach (var kvp in availableItems)
            {
                var itemType = kvp.Key;
                var itemBase = kvp.Value;

                if (_template == null || _parent == null)
                    continue;

                GameObject go = Instantiate(_template, _parent);
                go.transform.localScale = Vector3.one;

                var rawImage = go.GetComponentInChildren <RawImage>();
                if (rawImage != null && itemBase != null)
                    rawImage.texture = itemBase.Icon;

                if (go.TryGetComponent<RemoteAdmin.Elements.ValueButton>(out var valueButton))
                {
                    valueButton.Value = $"{(int)itemType}.";

                    if (itemBase != null)
                        go.name = itemBase.name;

                    // Buttons are spawned under an inactive panel, so their Awake
                    // (which normally self-registers) may not run yet.
                    if (!Options.Contains(valueButton))
                        Options.Add(valueButton);
                }

                RegisterItem(itemType, itemBase, go);
            }
        }

        private void RegisterItem(ItemType type, ItemBase itemBase, GameObject go)
        {
            if (itemBase == null)
                return;

            var translationReader = new ItemTranslationReader(itemBase.ItemTypeId);
            string displayName = translationReader.Name;
            if (string.IsNullOrEmpty(displayName))
                displayName = itemBase.name;

            string tooltip = $"<color=#ff7373>Name:</color> {displayName}" +
                           $"\n<<color=#73ff88>ID:</color> {type} (<color=#73ffff>{(int)type}</color>)" +
                           $"\n<<color=#73b2ff>Category:</color> {itemBase.Category}";

            if (TooltipManager != null)
                TooltipManager.StoredTips[go] = tooltip;

            PrintedItems.Add(new PrintedItem(go, type, itemBase.Category, displayName));
        }

        private void OnSearchValueChanged(string value)
        {
            AnimatedSearch(value);
        }
    }
}