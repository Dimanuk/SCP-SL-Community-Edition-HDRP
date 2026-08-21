using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RemoteAdmin
{
    public class PlayerRecord : MonoBehaviour, IPointerDownHandler, IEventSystemHandler
    {
        [SerializeField]
        private TMP_Text _textComponent;

        private bool _hasLink;
        private bool _isSelected;

        public static string LastSelectedId { get; internal set; } = null;
        public static List<PlayerRecord> Instances { get; private set; } = new List<PlayerRecord>();

        public string PlayerId { get; set; }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                UpdateGraphic();
            }
        }

        public void OnPointerDown(PointerEventData _)
        {
            bool newState = !_isSelected;

            bool pressingControl = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
            bool pressingShift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

            OnMouseSelect(newState, pressingControl, pressingShift);
        }

        internal void SetState(bool selected, bool allowMultipleSelection = false, bool changeLastSelected = true)
        {
            if (!allowMultipleSelection)
            {
                foreach (var other in Instances)
                {
                    if (other != this && other._isSelected)
                    {
                        other._isSelected = false;
                        other.UpdateGraphic();
                    }
                }
            }

            _isSelected = selected;
            UpdateGraphic();

            if (changeLastSelected)
                LastSelectedId = PlayerId;

            if (!string.IsNullOrEmpty(PlayerId) && PlayerId.Contains("unconnected"))
            {
                _isSelected = false;
                UpdateGraphic();
            }
        }

        internal void SetText(string content)
        {
            _textComponent.text = content;
        }

        private void OnMouseSelect(bool newState, bool pressingControl = false, bool pressingShift = false)
        {
            if (pressingShift)
            {
                if (string.IsNullOrEmpty(LastSelectedId))
                {
                    SetState(newState, pressingControl, true);
                    return;
                }

                var lastSelected = Instances.Find(x => x.PlayerId == LastSelectedId);
                int oldIndex = Instances.IndexOf(lastSelected);
                int newIndex = Instances.IndexOf(this);

                SetState(true, true, true);

                int startIndex = Math.Min(oldIndex, newIndex);
                int count = Math.Abs(oldIndex - newIndex);

                foreach (var record in Instances.Skip(startIndex).Take(count))
                {
                    if (record != null)
                        record.SetState(true, true, false);
                }
            }
            else
            {
                SetState(newState, pressingControl, true);
            }
        }

        private IEnumerable<PlayerRecord> ToSelect(int oldIndex, int newIndex)
        {
            return Instances.Skip(oldIndex).Take(newIndex - oldIndex);
        }

        private void Update()
        {
            if (ServerStatic.IsDedicated || !_hasLink)
                return;

            if (_textComponent == null)
                return;

            int linkIndex = TMP_TextUtilities.FindIntersectingLink(_textComponent, Input.mousePosition, null);
            if (linkIndex == -1)
                return;

            if (_textComponent.textInfo == null || _textComponent.textInfo.linkInfo == null || linkIndex >= _textComponent.textInfo.linkInfo.Length)
                return;

            var linkInfo = _textComponent.textInfo.linkInfo[linkIndex];
            string linkId = linkInfo.GetLinkID();

            if (linkId.Contains("RA_RaEverywhere"))
            {
                UIController.Singleton?.SetToolTip("This player is a Northwood Studios GLOBAL staff member.", 0.1f, false);
            }
            else if (linkId.Contains("RA_StudioStaff"))
            {
                UIController.Singleton?.SetToolTip("This player is a Northwood Studios staff member.", 0.1f, false);
            }
            else if (linkId.Contains("RA_Admin"))
            {
                UIController.Singleton?.SetToolTip("This player is an administrator logged in to RA.", 0.1f, false);
            }
            else if (linkId.Contains("RA_OverwatchEnabled"))
            {
                UIController.Singleton?.SetToolTip("This player has overwatch mode enabled and will not respawn at the next spawnwave.", 0.1f, false);
            }
        }

        private void UpdateGraphic()
        {
            if (_isSelected)
            {
                var mat = _textComponent?.fontMaterial;
                if (mat != null)
                {
                    if (mat.GetFloat("_OutlineWidth") == 0f)
                        _textComponent.gameObject.SetActive(false);

                    mat.SetFloat("_OutlineWidth", 0.1f);
                    _textComponent.gameObject.SetActive(true);
                }
            }
            else
            {
                var mat = _textComponent?.fontMaterial;
                if (mat != null)
                {
                    if (mat.GetFloat("_OutlineWidth") == 0.1f)
                        _textComponent.gameObject.SetActive(false);

                    mat.SetFloat("_OutlineWidth", 0f);
                    _textComponent.gameObject.SetActive(true);
                }
            }
        }

        private void Start()
        {
            if (_textComponent == null)
                return;

            _hasLink = _textComponent.text.Contains("RA_");
            Instances.Add(this);

            _textComponent.gameObject.SetActive(false);

            var mat = _textComponent.fontMaterial;
            if (mat != null)
                mat.SetFloat("_OutlineWidth", _isSelected ? 0.1f : 0f);

            var mat2 = _textComponent.fontMaterial;
            if (mat2 != null)
                mat2.SetColor("_OutlineColor", Color.white);

            _textComponent.gameObject.SetActive(true);
        }
    }
}
