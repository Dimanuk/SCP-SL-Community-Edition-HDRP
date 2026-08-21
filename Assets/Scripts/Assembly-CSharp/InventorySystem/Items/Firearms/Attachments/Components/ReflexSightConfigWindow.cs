using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InventorySystem.Items.Firearms.Attachments.Components
{
    public class ReflexSightConfigWindow : AttachmentConfigWindow
    {
        [SerializeField]
        private RectTransform _shapeTemplate;

        [SerializeField]
        private RectTransform _colorTemplate;

        [SerializeField]
        private RectTransform _buttonReduce;

        [SerializeField]
        private RectTransform _buttonEnlarge;

        [SerializeField]
        private TextMeshProUGUI _textPercent;

        [SerializeField]
        private Color _selectedColor;

        [SerializeField]
        private Color _normalColor;

        private Image[] _shapeInstances;
        private Image[] _colorInstances;

        private ReflexSightAttachment _reflex;

        public override void Setup(AttachmentSelectorBase selector, Attachment attachment, RectTransform transformToFit)
        {
            base.Setup(selector, attachment, transformToFit);

            _reflex = (ReflexSightAttachment)attachment;
            _reflex.OnValuesChanged += UpdateValues;

            Texture[] reticles = (_reflex.TextureOptions != null) ? _reflex.TextureOptions.Reticles : null;

            _shapeInstances = GenerateOptions(reticles, _shapeTemplate,
                idx => _reflex.SetTexture(idx),
                (rect, idx) => rect.GetComponentInChildren<RawImage>().texture = reticles[idx]);

            _colorInstances = GenerateOptions(ReflexSightAttachment.Colors, _colorTemplate,
                idx => _reflex.SetColor(idx),
                (rect, idx) => rect.GetComponentInChildren<RawImage>().color = ReflexSightAttachment.Colors[idx]);

            Selector.RegisterAction(_buttonReduce, _ => _reflex.ChangeSize(-1));
            Selector.RegisterAction(_buttonEnlarge, _ => _reflex.ChangeSize(1));

            UpdateValues();
        }

        protected override void OnDestroy()
        {
            if (_reflex != null)
                _reflex.OnValuesChanged -= UpdateValues;

            base.OnDestroy();
        }

        private void UpdateValues()
        {
            if (_shapeInstances == null || _colorInstances == null || _reflex == null)
                return;

            int selectedShape = _reflex.CurTexture;
            int selectedColor = _reflex.CurColor;

            for (int i = 0; i < _shapeInstances.Length; i++)
                _shapeInstances[i].color = (i == selectedShape) ? _selectedColor : _normalColor;

            for (int i = 0; i < _colorInstances.Length; i++)
                _colorInstances[i].color = (i == selectedColor) ? _selectedColor : _normalColor;

            int sizeIdx = Mathf.Clamp(_reflex.CurSize, 0, ReflexSightAttachment.Sizes.Length - 1);
            int percent = Mathf.RoundToInt(ReflexSightAttachment.Sizes[sizeIdx] * 100);
            _textPercent.text = percent + "%";
        }

        private Image[] GenerateOptions<T>(T[] array, RectTransform template, Action<int> onSelect, Action<RectTransform, int> modify)
        {
            if (array == null || array.Length == 0)
                return Array.Empty<Image>();

            Image[] instances = new Image[array.Length];

            for (int i = 0; i < array.Length; i++)
            {
                var instance = UnityEngine.Object.Instantiate(template, template.parent);
                instance.gameObject.SetActive(true);
                modify?.Invoke(instance, i);
                instances[i] = instance.GetComponentInChildren<Image>();

                int idx = i;
                Selector.RegisterAction(instance, _ => onSelect?.Invoke(idx));
            }

            return instances;
        }
    }
}