using InventorySystem.Items;
using InventorySystem.Items.Firearms.Ammo;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InventorySystem.GUI
{
	[Serializable]
	public class AmmoElement : MonoBehaviour
	{
		[SerializeField]
		private RawImage _iconImg;

		[SerializeField]
		private TextMeshProUGUI _nameTxt;

		[SerializeField]
		private RectTransform _myTransform;

		[SerializeField]
		private float _minX;

		[SerializeField]
		private float _maxX;

		[SerializeField]
		private float _minY;

		[SerializeField]
		private float _maxY;

		[SerializeField]
		private Graphic[] _paintableParts;

		[SerializeField]
		private TextMeshProUGUI _amountIndicator;

		[SerializeField]
		private TextMeshProUGUI _lowText;

		[SerializeField]
		private TextMeshProUGUI _medText;

		[SerializeField]
		private TextMeshProUGUI _highText;

		private int _lowAmount;

		private int _medAmount;

		private int _highAmount;

		private ItemBase _targetItem;

        public void Setup(ItemType type, Color classColor)
        {
            if (InventoryItemLoader.AvailableItems.TryGetValue(type, out _targetItem))
            {
                _iconImg.texture = _targetItem.Icon;
                _nameTxt.text = type.ToString();

                foreach (Graphic graphic in _paintableParts)
                {
                    graphic.color = classColor;
                }
            }
            else
            {
                throw new InvalidOperationException($"Item {type} is not defined. Cannot create an ammo element for it.");
            }
        }

        public void UpdateAmount(int amount)
        {
            if (amount == 0)
            {
                gameObject.SetActive(false);
                return;
            }

            gameObject.SetActive(true);
            _amountIndicator.text = amount.ToString();

            if (_targetItem is AmmoItem ammoItem && ammoItem.PickupDropModel is AmmoPickup ammoPickup)
            {
                int box = ammoPickup.SavedAmmo;
                _medAmount = box;
                _highAmount = box * 2;
                _lowAmount = ((box % 2f == 0f) ? (box / 2) : (box * 2 / 3));
            }
            else
            {
                _highAmount = amount;
                _medAmount = Mathf.FloorToInt((float)amount * 0.5f);
                _lowAmount = ((_medAmount != 1) ? 1 : 0);
            }

            if (amount >= _medAmount)
            {
                if (amount < _highAmount)
                {
                    _highAmount = ((amount == _medAmount) ? 0 : amount);
                }
            }
            else if (amount > _lowAmount)
            {
                _medAmount = amount;
                _highAmount = 0;
            }
            else
            {
                _lowAmount = amount;
                _medAmount = 0;
                _highAmount = 0;
            }

            PrepButton(_lowText, _lowAmount);
            PrepButton(_medText, _medAmount);
            PrepButton(_highText, _highAmount);
        }

        private void PrepButton(TextMeshProUGUI t, int amount)
        {
            t.transform.parent.gameObject.SetActive(amount > 0);
            if (amount > 0)
            {
                t.text = $"{amount}x";
            }
        }

        public void UseButton(int type)
        {
            int amountToDrop = 0;
            switch (type)
            {
                case 0: amountToDrop = _lowAmount; break;
                case 1: amountToDrop = _medAmount; break;
                case 2: amountToDrop = _highAmount; break;
            }

            if (amountToDrop > 0)
            {
                ReferenceHub localHub = ReferenceHub.LocalHub;
                if (localHub != null && localHub.inventory != null)
                {
                    localHub.inventory.CmdDropAmmo((byte)_targetItem.ItemTypeId, (ushort)amountToDrop);
                }
            }
        }

        public bool IsHovering()
        {
            Vector2 localPoint;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_myTransform, Input.mousePosition, null, out localPoint))
            {
                return localPoint.x > _minX && localPoint.x < _maxX &&
                       localPoint.y > _minY && localPoint.y < _maxY;
            }
            return false;
        }
    }
}
