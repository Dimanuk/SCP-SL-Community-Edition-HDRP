using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using InventorySystem.Items.Firearms.Attachments;
using InventorySystem.Items.Firearms.Attachments.Components;
using InventorySystem.Items.Firearms.BasicMessages;
using NorthwoodLib.Pools;

namespace InventorySystem.Items.Firearms
{
    public class FirearmWorldmodel : MonoBehaviour
    {
        [Serializable]
        private struct AttachmentElement
        {
            [SerializeField]
            private GameObject[] _targetObjects;

            public void Refresh(bool state)
            {
                for (int i = 0; i < _targetObjects.Length; i++)
                {
                    _targetObjects[i].SetActive(state);
                }
            }
        }

        [Serializable]
        public class RiggedAttachmentElement
        {
            public int TargetAttachmentId;

            [SerializeField]
            private Transform _targetBone;

            [SerializeField]
            private Offset _disabled;

            [SerializeField]
            private Offset _enabled;

            [SerializeField]
            private bool _thirdpersonOnly;

            public void Refresh(bool isEnabled, bool thirdperson)
            {
                Offset offset = (isEnabled && (thirdperson || !_thirdpersonOnly)) ? _enabled : _disabled;
                _targetBone.localPosition = offset.position;
                _targetBone.localRotation = Quaternion.Euler(offset.rotation);
                _targetBone.localScale = offset.scale;
            }
        }

        [Serializable]
        private class MagazineElement
        {
            [SerializeField]
            private GameObject _targetObject;

            [SerializeField]
            private int[] _attachmentIds;

            private uint[] _binaryCodes;
            private int _prevStatusCode;

            private uint[] BinaryCodes
            {
                get
                {
                    if (_binaryCodes == null)
                        GenerateBinaryCodes();
                    return _binaryCodes;
                }
            }

            private void GenerateBinaryCodes()
            {
                _binaryCodes = new uint[_attachmentIds.Length];
                for (int i = 0; i < _attachmentIds.Length; i++)
                {
                    _binaryCodes[i] = 1u << _attachmentIds[i];
                }
            }

            public bool Refresh(uint attachmentsCode, bool hasMag)
            {
                if (!hasMag || _attachmentIds.Length == 0)
                    return ApplyStatus(hasMag);

                uint[] codes = BinaryCodes;
                for (int i = 0; i < codes.Length; i++)
                {
                    if ((attachmentsCode & codes[i]) != 0)
                        return ApplyStatus(true);
                }

                return ApplyStatus(false);
            }

            private bool ApplyStatus(bool status)
            {
                int code = status ? 1 : 2;
                bool changed = code != _prevStatusCode;
                _targetObject.SetActive(status);
                _prevStatusCode = code;
                return changed;
            }
        }

        [Serializable]
        private class FlagableElement
        {
            [SerializeField]
            private Transform _targetTransform;

            [SerializeField]
            private Offset _falsePosition;

            [SerializeField]
            private Offset _truePosition;

            [SerializeField]
            private FirearmStatusFlags[] _compatibleFlags;

            [SerializeField]
            private bool _invertFlags;

            [SerializeField]
            private bool _checkAmmo;

            [SerializeField]
            private bool _needsAmmo;

            private int _prevValue;

            public bool Refresh(FirearmStatusFlags flags, bool hasAmmo)
            {
                bool state = _invertFlags;
                for (int i = 0; i < _compatibleFlags.Length; i++)
                {
                    if (flags.HasFlagFast(_compatibleFlags[i]))
                    {
                        state = !_invertFlags;
                        break;
                    }
                }

                if (_checkAmmo && _needsAmmo != hasAmmo)
                    state = false;

                Offset targetOffset = state ? _truePosition : _falsePosition;
                _targetTransform.localPosition = targetOffset.position;
                _targetTransform.localRotation = Quaternion.Euler(targetOffset.rotation);
                _targetTransform.localScale = targetOffset.scale;

                int val = state ? 1 : 2;
                bool changed = val != _prevValue;
                _prevValue = val;
                return changed;
            }
        }

        [Serializable]
        private class AmmoCounterElement
        {
            [SerializeField]
            private Text _targetText;

            [SerializeField]
            private string _format;

            private static readonly string[] UncockedPerFormatLength = { " ", "  ", "   ", "    " };

            public void Refresh(bool cocked, byte ammo)
            {
                if (!cocked)
                {
                    int len = Mathf.Clamp(ammo.ToString().Length, 0, UncockedPerFormatLength.Length - 1);
                    _targetText.text = string.Format(_format, UncockedPerFormatLength[len]);
                    return;
                }
                _targetText.text = string.Format(_format, ammo);
            }
        }

        private FirearmStatus _prevStatus;
        private bool _alreadySetupOnce;

        [SerializeField] private bool _enableColliders;
        [SerializeField] private Collider[] _colliders;
        [SerializeField] private AttachmentElement[] _attachments;
        [SerializeField] private RiggedAttachmentElement[] _riggedElements;
        [SerializeField] private MagazineElement[] _magazineElements;
        [SerializeField] private FlagableElement[] _flagableElements;
        [SerializeField] private AmmoCounterElement[] _ammoCounterElements;
        [SerializeField] private ParticleSystem[] _shootingEffects;
        [SerializeField] private GameObject[] _bullets;

        private void Awake()
        {
            for (int i = 0; i < _colliders.Length; i++)
            {
                _colliders[i].enabled = _enableColliders;
            }

            FirearmWorldmodelInitializer[] initializers = GetComponentsInChildren<FirearmWorldmodelInitializer>(true);
            for (int i = 0; i < initializers.Length; i++)
            {
                initializers[i].Initialize(_enableColliders);
            }
        }

        public void PlayParticleEffects()
        {
            for (int i = 0; i < _shootingEffects.Length; i++)
            {
                _shootingEffects[i].Play();
            }
        }

        public bool ApplyStatus(FirearmStatus status, ItemType firearmType)
        {
            bool firstSetup = !_alreadySetupOnce;
            bool result = false;

            bool attChanged = firstSetup || _prevStatus.Attachments != status.Attachments;
            bool flagsChanged = firstSetup || status.Flags != _prevStatus.Flags;
            bool ammoChanged = firstSetup || _prevStatus.Ammo != status.Ammo;

            bool hasMag = status.Flags.HasFlagFast(FirearmStatusFlags.MagazineInserted);

            if ((attChanged || flagsChanged) && RefreshMags(status.Attachments, hasMag))
                result = true;

            if (attChanged)
            {
                RefreshAttachments(status.Attachments, firearmType);
                result = true;
            }

            if (flagsChanged || ammoChanged)
            {
                bool hasAmmo = status.Ammo > 0;
                if (RefreshFlagables(status.Flags, hasAmmo))
                    result = true;

                bool isCocked = status.Flags.HasFlagFast(FirearmStatusFlags.Cocked);
                for (int i = 0; i < _ammoCounterElements.Length; i++)
                {
                    _ammoCounterElements[i].Refresh(isCocked, status.Ammo);
                }

                for (int i = 0; i < _bullets.Length; i++)
                {
                    _bullets[i].SetActive(i < status.Ammo);
                }
            }

            _prevStatus = status;
            _alreadySetupOnce = true;
            return result;
        }

        private bool RefreshFlagables(FirearmStatusFlags flags, bool hasAmmo)
        {
            bool changed = false;
            for (int i = 0; i < _flagableElements.Length; i++)
            {
                if (_flagableElements[i].Refresh(flags, hasAmmo))
                    changed = true;
            }
            return changed;
        }

        private bool RefreshMags(uint att, bool hasMag)
        {
            bool changed = false;
            for (int i = 0; i < _magazineElements.Length; i++)
            {
                if (_magazineElements[i].Refresh(att, hasMag))
                    changed = true;
            }
            return changed;
        }

        private void RefreshAttachments(uint code, ItemType firearmType)
        {
            if (!InventoryItemLoader.AvailableItems.TryGetValue(firearmType, out var item) || !(item is Firearm firearm))
                return;

            HashSet<AttachmentSlot> activeSlots = HashSetPool<AttachmentSlot>.Shared.Rent();
            Attachment[] firearmAttachments = firearm.Attachments;

            for (int i = 0; i < firearmAttachments.Length; i++)
            {
                activeSlots.Add(firearmAttachments[i].Slot);
            }

            for (int i = 0; i < _attachments.Length; i++)
            {
                _attachments[i].Refresh(false);
            }

            uint bitmask = 1u;
            for (int i = 0; i < firearmAttachments.Length; i++)
            {
                bool isEquipped = (code & bitmask) != 0 && activeSlots.Remove(firearmAttachments[i].Slot);

                if (i < _attachments.Length)
                    _attachments[i].Refresh(isEquipped);

                for (int j = 0; j < _riggedElements.Length; j++)
                {
                    if (_riggedElements[j].TargetAttachmentId == i)
                    {
                        _riggedElements[j].Refresh(isEquipped, !_enableColliders);
                    }
                }
                bitmask <<= 1;
            }

            foreach (AttachmentSlot slot in activeSlots)
            {
                for (int i = 0; i < firearmAttachments.Length; i++)
                {
                    if (firearmAttachments[i].Slot == slot)
                    {
                        if (i < _attachments.Length)
                            _attachments[i].Refresh(true);
                        break;
                    }
                }
            }

            HashSetPool<AttachmentSlot>.Shared.Return(activeSlots);
        }
    }
}