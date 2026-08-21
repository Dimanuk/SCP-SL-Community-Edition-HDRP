using InventorySystem.Items.Pickups;
using Mirror;
using System.Collections.Generic;
using UnityEngine;

namespace InventorySystem.Items.Firearms.Ammo
{
    public class AmmoPickup : ItemPickupBase
    {
        [SyncVar]
        public ushort SavedAmmo;

        [SerializeField]
        private int _minDisplayedValue;

        [SerializeField]
        private int _maxDisplayedValue;

        [SerializeField]
        private int _roundingValue;

        [SerializeField]
        private bool _hideFirstDigitBelow10;

        [SerializeField]
        private Material _targetDigitMaterial;

        [SerializeField]
        private Renderer[] _firstDigits;

        [SerializeField]
        private Renderer[] _secondDigits;

        private static readonly Dictionary<ItemType, Dictionary<int, Material>> DigitMaterials = new();

        private ushort _prevAmmo;

        public int MaxAmmo => _maxDisplayedValue;

        private Material GetDigitMaterial(int digit)
        {
            if (!DigitMaterials.TryGetValue(Info.ItemId, out var innerDict))
            {
                innerDict = new Dictionary<int, Material>();
                DigitMaterials[Info.ItemId] = innerDict;
            }

            if (innerDict.TryGetValue(digit, out var cachedMaterial))
                return cachedMaterial;

            var material = new Material(_targetDigitMaterial)
            {
                mainTextureOffset = Vector2.up * digit / 10f
            };
            innerDict[digit] = material;

            return material;
        }

        private void Update()
        {
            if (_roundingValue == 0)
                return;

            if (SavedAmmo == _prevAmmo)
                return;

            int displayValue = Mathf.Clamp(SavedAmmo, _minDisplayedValue, _maxDisplayedValue);
            while (displayValue % _roundingValue != 0)
            {
                displayValue++;
            }

            int firstDigit = Mathf.FloorToInt(displayValue / 10f);
            int secondDigit = displayValue % 10;

            var mat1 = GetDigitMaterial(firstDigit);
            var mat2 = GetDigitMaterial(secondDigit);

            for (int i = 0; i < _firstDigits.Length; i++)
            {
                var renderer = _firstDigits[i];
                renderer.material = mat1;

                if (_hideFirstDigitBelow10)
                    renderer.gameObject.SetActive(displayValue >= 10);
            }

            for (int i = 0; i < _secondDigits.Length; i++)
            {
                var renderer = _secondDigits[i];
                renderer.material = mat2;
            }

            _prevAmmo = SavedAmmo;
        }
    }
}