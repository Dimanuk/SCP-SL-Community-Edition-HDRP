using CameraShaking;
using InventorySystem.Items.Firearms.Attachments.Components;
using InventorySystem.Items.Firearms.Attachments.Formatters;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InventorySystem.Items.Firearms.Attachments
{
    public abstract class AttachmentSelectorBase : MonoBehaviour
    {
        public static Action OnPresetLoaded;
        public static Action OnPresetSaved;
        public static Action OnAttachmentsReset;
        public Action OnSummaryToggled;

        public AttachmentSlot SelectedSlot = AttachmentSlot.Unassigned;

        [SerializeField]
        public MonoBehaviour[] SlotsPool;

        [SerializeField]
        public MonoBehaviour[] SelectableAttachmentsPool;

        [SerializeField]
        public TextMeshProUGUI _attachmentName;

        [SerializeField]
        public TextMeshProUGUI _attachmentDescription;

        [SerializeField]
        public TextMeshProUGUI _pros;

        [SerializeField]
        public TextMeshProUGUI _cons;

        [SerializeField]
        public CanvasGroup _attachmentDimmer;

        [SerializeField]
        public float _dimmerSpeed;

        [SerializeField]
        public RawImage _bodyImage;

        [SerializeField]
        public RectTransform _fullscreenRect;

        [SerializeField]
        public RectTransform _sideRect;

        [SerializeField]
        public RectTransform _selectableRect;

        [SerializeField]
        public GameObject _selectorScreen;

        [SerializeField]
        public GameObject _summaryScreen;

        [SerializeField]
        public float _selectableMaxHeight;

        [SerializeField]
        public float _selectableMaxWidth;

        [SerializeField]
        public float _selectableMaxScale;

        [SerializeField]
        public float _maxDisplayedScale = 2f;

        [SerializeField]
        public RectTransform _configIcon;

        [SerializeField]
        public HorizontalOrVerticalLayoutGroup _selectableLayoutGroup;

        public Vector3 _targetScale;
        public Vector3 _targetPosition;
        public bool _isCorrectAttachment;

        // Id of the button whose stats are currently on screen (-1 when the panel is hidden).
        private int _shownAttachmentId = -1;

        public readonly List<GameObject> _spawnedConfigButtons = new List<GameObject>();

        public const byte SlotOffset = 32;
        public const string AttParamsFilename = "AttachmentParameters";

        private static AttachmentParam[] _paramsArray;

        public Firearm SelectedFirearm { get; set; }

        public abstract bool UseLookatMode { get; set; }

        public static Vector3 SpinRotation => Vector3.back * Time.timeSinceLevelLoad * 100f;

        public AttachmentSelectorBase()
        {
            SelectedSlot = AttachmentSlot.Unassigned;
            _maxDisplayedScale = 2f;
        }

        public void ProcessCollider(byte colId)
        {
            if (SelectedFirearm == null)
                return;

            RefreshState(SelectedFirearm, colId);

            if (colId >= SlotOffset)
            {
                SelectedSlot = (AttachmentSlot)(colId - SlotOffset);
                if (UseLookatMode)
                {
                    RefreshState(SelectedFirearm, null);
                }
            }
            else
            {
                SelectAttachmentId(colId);
            }
        }

        public void ShowStats(int attachmentId)
        {
            _isCorrectAttachment = attachmentId >= 0 && attachmentId < SelectedFirearm.Attachments.Length;
            _shownAttachmentId = _isCorrectAttachment ? attachmentId : -1;
            if (!_isCorrectAttachment)
                return;

            if (_paramsArray == null || _paramsArray.Length != AttachmentsUtils.TotalNumberOfParams)
            {
                _paramsArray = new AttachmentParam[AttachmentsUtils.TotalNumberOfParams];
            }

            Attachment attachment = SelectedFirearm.Attachments[attachmentId];
            attachment.GetNameAndDescription(out var n, out var d);
            _pros.text = string.Empty;
            _cons.text = string.Empty;
            _attachmentName.text = n;
            _attachmentDescription.text = d;

            int paramCount = 0;
            for (int i = 0; i < AttachmentsUtils.TotalNumberOfParams; i++)
            {
                attachment.TryGetValue(i, out var val);
                if (!attachment.IsEnabled || attachment._activeParameters == null || i >= attachment._activeParameters.Length || !attachment._activeParameters[i])
                    continue;

                _paramsArray[paramCount++] = (AttachmentParam)i;
            }

            for (int i = 0; i < paramCount; i++)
            {
                int paramIndex = (int)_paramsArray[i];
                attachment.TryGetValue(paramIndex, out var val);
                AttachmentParam attachmentParam = (AttachmentParam)paramIndex;

                if (AttachmentParameterFormatters.Formatters.TryGetValue(attachmentParam, out var formatter) &&
                    formatter.FormatParameter(attachmentParam, SelectedFirearm, attachmentId, val, out var formattedText, out var isGood))
                {
                    bool hasTranslation = TranslationReader.TryGet("AttachmentParameters", paramIndex, out string paramName);
                    string text = "\n" + (hasTranslation ? paramName : attachmentParam.ToString()) + ": " + formattedText;

                    if (isGood)
                        _pros.text += text;
                    else
                        _cons.text += text;
                }
            }

            NonParameterFormatter.Format(SelectedFirearm, attachmentId, out var pros, out var cons);
            _pros.text += pros;
            _cons.text += cons;
        }

        public abstract void LoadPreset(uint loadedCode);

        public abstract void SelectAttachmentId(byte attachmentId);

        public abstract void RegisterAction(RectTransform t, Action<Vector2> action);

        public bool CanSaveAsPreference(int presetId)
        {
            if (presetId == 0 || SelectedFirearm == null)
                return false;

            uint currentAttachmentsCode = SelectedFirearm.GetCurrentAttachmentsCode();
            return AttachmentPreferences.GetPreferenceCodeOfPreset(SelectedFirearm.ItemTypeId, presetId) != currentAttachmentsCode;
        }

        public void SaveAsPreset(int presetId)
        {
            if (CanSaveAsPreference(presetId))
            {
                AttachmentPreferences.SetPreset(SelectedFirearm.ItemTypeId, presetId);
                SelectedFirearm.SavePreferenceCode();
                OnPresetSaved?.Invoke();
            }
        }

        public void LoadPreset(int presetId)
        {
            AttachmentPreferences.SetPreset(SelectedFirearm.ItemTypeId, presetId);
            if (presetId != 0)
            {
                uint savedCode = SelectedFirearm.GetSavedPreferenceCode();
                LoadPreset(savedCode);
                OnPresetLoaded?.Invoke();
            }
        }

        public void ResetAttachments()
        {
            AttachmentPreferences.SetPreset(SelectedFirearm.ItemTypeId, 0);
            uint validatedCode = SelectedFirearm.ValidateAttachmentsCode(0u);
            LoadPreset(validatedCode);
            OnAttachmentsReset?.Invoke();
        }

        public void ToggleSummaryScreen(bool summary)
        {
            _summaryScreen.SetActive(summary);
            _selectorScreen.SetActive(!summary);
            OnSummaryToggled?.Invoke();
        }

        public void ToggleSummaryScreen()
        {
            ToggleSummaryScreen(!_summaryScreen.activeSelf);
        }

        public void LerpRects(float lerpState)
        {
            _bodyImage.rectTransform.localScale = Vector3.Lerp(
                _bodyImage.rectTransform.localScale, _targetScale, lerpState);
            _bodyImage.rectTransform.localPosition = Vector3.Lerp(
                _bodyImage.rectTransform.localPosition, _targetPosition, lerpState);
        }

        public void Lookat(Vector3 pos)
        {
            CameraShakeController.AddEffect(new LookatShake(pos));
        }

        public void DisableAllSelectableAttachments()
        {
            foreach (var mono in SelectableAttachmentsPool)
            {
                if (mono == null)
                    continue;

                if (mono is IAttachmentSelectorButton button && button.RectTransform != null)
                {
                    button.RectTransform.gameObject.SetActive(false);
                }
            }
        }

        public bool RefreshState(Firearm firearm, byte? refreshReason)
        {
            foreach (var button in _spawnedConfigButtons)
            {
                if (button != null)
                    Destroy(button.gameObject);
            }
            _spawnedConfigButtons.Clear();

            if (firearm != SelectedFirearm)
            {
                SelectedFirearm = firearm;
                SelectedSlot = AttachmentSlot.Unassigned;
                _bodyImage.rectTransform.localScale = Vector3.zero;
                DisableAllSelectableAttachments();

                if (firearm == null)
                    return false;

                uint savedCode = SelectedFirearm.GetSavedPreferenceCode();
                uint currentCode = SelectedFirearm.GetCurrentAttachmentsCode();
                if (savedCode != currentCode)
                {
                    AttachmentPreferences.SetPreset(SelectedFirearm.ItemTypeId, 0);
                }
            }

            if (firearm == null)
                return false;

            float targetAlpha = _isCorrectAttachment ? 1f : 0f;
            _attachmentDimmer.alpha = Mathf.MoveTowards(
                _attachmentDimmer.alpha, targetAlpha, Time.deltaTime * _dimmerSpeed);

            _bodyImage.texture = firearm.BodyIconTexture;
            _bodyImage.rectTransform.sizeDelta = new Vector2(
                firearm.BodyIconTexture.width, firearm.BodyIconTexture.height);

            int slotIndex = 0;
            for (int i = 0; i < firearm.Attachments.Length; i++)
            {
                var attachment = firearm.Attachments[i];
                if (!attachment.IsEnabled || attachment is not IDisplayableAttachment displayable)
                    continue;

                Vector2 iconOffset = displayable.IconOffset;

                int parentId = Mathf.Clamp(displayable.ParentId, 0, firearm.Attachments.Length - 1);
                if (firearm.Attachments[parentId].IsEnabled)
                {
                    iconOffset += displayable.ParentOffset;
                }

                var slotButton = SlotsPool[slotIndex].GetComponent<IAttachmentSelectorButton>();
                slotButton.RectTransform.gameObject.SetActive(true);
                slotButton.Setup(displayable.Icon, attachment.Slot, iconOffset, firearm);
                slotButton.ButtonId = (byte)(SlotOffset + attachment.Slot);

                if (displayable is ICustomizableAttachment customizable)
                {
                    var configIcon = Instantiate(_configIcon, slotButton.RectTransform);
                    configIcon.localScale = Vector3.one * customizable.ConfigIconScale;
                    configIcon.localRotation = Quaternion.Euler(SpinRotation);
                    configIcon.localPosition = (Vector3)customizable.ConfigIconOffset;
                    configIcon.gameObject.SetActive(true);

                    _spawnedConfigButtons.Add(configIcon.gameObject);

                    var currentAttachment = attachment;
                    var currentSlot = slotButton.RectTransform;

                    RegisterAction(configIcon, (Vector2 pos) =>
                    {
                        var selectorRect = _selectorScreen.GetComponent<RectTransform>();
                        var configWindow = Instantiate(customizable.ConfigWindow, selectorRect.parent);
                        configWindow.Setup(this, currentAttachment, selectorRect);
                        configWindow.OnDestroyed += () => ToggleSummaryScreen(false);
                    });
                }

                slotIndex++;

                if (UseLookatMode)
                {
                    LerpRects(1f);
                    if (refreshReason.HasValue && refreshReason.Value < SlotOffset &&
                        firearm.Attachments[refreshReason.Value].Slot == attachment.Slot)
                    {
                        Lookat(slotButton.RectTransform.position);
                    }
                }
            }

            for (int j = slotIndex; j < SlotsPool.Length; j++)
            {
                SlotsPool[j].gameObject.SetActive(false);
            }

            DisableAllSelectableAttachments();

            if (SelectedSlot == AttachmentSlot.Unassigned)
            {
                FitToRect(_fullscreenRect);
            }
            else
            {
                SetupSelectableAttachments(firearm, refreshReason);
            }

            // Buttons only report un-hovering as an edge (UI pointer exit / crosshair leaving the
            // collider). A button that gets disabled while it is highlighted - slot closed, weapon
            // swapped, list rebuilt - never fires that edge, which would leave its description on
            // screen forever, so the owner of the panel is re-validated every frame.
            if (_shownAttachmentId >= 0 && !IsButtonActive(_shownAttachmentId))
            {
                ShowStats(-1);
            }

            return true;
        }

        private bool IsButtonActive(int buttonId)
        {
            return IsButtonActive(SlotsPool, buttonId) || IsButtonActive(SelectableAttachmentsPool, buttonId);
        }

        private bool IsButtonActive(MonoBehaviour[] pool, int buttonId)
        {
            foreach (var mono in pool)
            {
                if (mono is not IAttachmentSelectorButton button || button.ButtonId != buttonId)
                    continue;

                if (button.RectTransform != null && button.RectTransform.gameObject.activeInHierarchy)
                    return true;
            }

            return false;
        }

        private void SetupSelectableAttachments(Firearm firearm, byte? refreshReason)
        {
            int activeCount = 0;
            float totalWidth = 0f;
            float maxHeight = 0f;
            Transform lookatTarget = null;

            for (byte b = 0; b < firearm.Attachments.Length; b++)
            {
                if (firearm.Attachments[b].Slot != SelectedSlot)
                    continue;

                if (activeCount >= SelectableAttachmentsPool.Length)
                    break;

                var selectable = SelectableAttachmentsPool[activeCount];
                if (!selectable.TryGetComponent<IAttachmentSelectorButton>(out var button))
                    continue;

                if (!(firearm.Attachments[b] is IDisplayableAttachment displayable))
                    continue;

                button.RectTransform.gameObject.SetActive(true);
                button.ButtonId = b;
                button.Setup(displayable.Icon, AttachmentSlot.Unassigned, Vector2.zero, firearm);

                maxHeight = Mathf.Max(maxHeight, button.RectTransform.sizeDelta.y);
                totalWidth += button.RectTransform.sizeDelta.x;

                if (UseLookatMode && refreshReason.HasValue &&
                    refreshReason.Value - SlotOffset == (int)SelectedSlot &&
                    firearm.Attachments[b].IsEnabled)
                {
                    lookatTarget = button.RectTransform;
                }

                activeCount++;
            }

            float scale = Mathf.Max(1f / _selectableMaxScale,
                Mathf.Max(totalWidth / _selectableMaxWidth, maxHeight / _selectableMaxHeight));

            for (int k = 0; k < SelectableAttachmentsPool.Length; k++)
            {
                var button = SelectableAttachmentsPool[k] as IAttachmentSelectorButton;
                if (button?.RectTransform.gameObject.activeSelf == true)
                {
                    button.RectTransform.sizeDelta /= scale;
                }
            }

            FitToRect(_sideRect);

            if (lookatTarget != null)
            {
                _selectableLayoutGroup.SetLayoutVertical();
                LerpRects(1f);
                Lookat(lookatTarget.position);
            }
        }

        public virtual void OnDisable()
        {
            _isCorrectAttachment = false;
            _attachmentDimmer.alpha = 0f;
        }

        public void FitToRect(RectTransform rt)
        {
            Vector3 originalScale = _bodyImage.rectTransform.localScale;
            Vector3 originalPosition = _bodyImage.rectTransform.localPosition;

            Bounds bounds = new Bounds();
            _bodyImage.rectTransform.localScale = Vector3.one;
            _bodyImage.rectTransform.localPosition = Vector3.zero;

            Encapsulate(ref bounds, _bodyImage.rectTransform);

            foreach (var slot in SlotsPool)
            {
                var button = slot as IAttachmentSelectorButton;
                if (button?.RectTransform.gameObject.activeSelf == true)
                {
                    Encapsulate(ref bounds, button.RectTransform);
                }
            }

            Vector3 centerOffset = bounds.center - _bodyImage.rectTransform.localPosition;
            float displayScale = Mathf.Min(_maxDisplayedScale,
                Mathf.Min(rt.sizeDelta.x / bounds.size.x, rt.sizeDelta.y / bounds.size.y));

            _targetScale = Vector3.one * displayScale;
            _targetPosition = rt.localPosition - centerOffset * displayScale;

            _bodyImage.rectTransform.localScale = originalScale;
            _bodyImage.rectTransform.localPosition = originalPosition;
        }

        public void Encapsulate(ref Bounds b, RectTransform rct)
        {
            Vector2 halfSize = rct.sizeDelta / 2f;
            b.Encapsulate(rct.localPosition + Vector3.up * halfSize.y + Vector3.left * halfSize.x);
            b.Encapsulate(rct.localPosition + Vector3.down * halfSize.y + Vector3.right * halfSize.x);
        }
    }
}
