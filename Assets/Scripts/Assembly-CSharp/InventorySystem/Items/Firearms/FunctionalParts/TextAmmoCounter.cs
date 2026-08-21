using UnityEngine;
using UnityEngine.UI;

namespace InventorySystem.Items.Firearms.FunctionalParts
{
    public class TextAmmoCounter : FunctionalFirearmPart
    {
        [SerializeField] private Text _targetText;
        [SerializeField] private int _digits;

        private string _toStringFormat;
        private string _unloaded;
        private Vector3 _targetScale;
        private byte _activeCooldown;

        private void Awake()
        {
            _targetScale = transform.localScale;
            transform.localScale = Vector3.zero;
        }

        private void Start()
        {
            _toStringFormat = new string('0', _digits);
            _unloaded = new string('-', _digits);
        }

        private void LateUpdate()
        {
            Firearm fa = Firearm;
            if (fa == null) return;

            int ammo = fa.Status.Ammo;

            _targetText.text = (ammo >= 0)
                ? ammo.ToString(_toStringFormat)
                : _unloaded;

            if (_activeCooldown > 0)
            {
                _activeCooldown--;
                transform.localScale = Vector3.zero;
            }
            else
            {
                transform.localScale = _targetScale;
            }
        }

        private void OnEnable() => _activeCooldown = 2;
        private void OnDisable() => transform.localScale = Vector3.zero;
    }
}