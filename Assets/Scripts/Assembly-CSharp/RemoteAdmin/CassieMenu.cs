using System;
using System.Linq;
using System.Text.RegularExpressions;
using RemoteAdmin.Menus;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace RemoteAdmin
{
    public class CassieMenu : RaCommandMenu
    {
        [SerializeField]
        private Color _resetColor;

        [SerializeField]
        private Color _errorColor;

        [SerializeField]
        private TMP_Text[] _buttons;

        [SerializeField]
        private TMP_Text _pageCount;

        [SerializeField]
        private TMP_Text _elementCount;

        [SerializeField]
        private CustomSlider _pitchSlider;

        [SerializeField]
        private CustomSlider _jamDelaySlider;

        [SerializeField]
        private CustomSlider _jamStutterSlider;

        [SerializeField]
        private CustomSlider _yieldSlider;

        private readonly string[] _validSuffixes;
        private int _pageIndex;
        private int _maxPages;

        public CassieMenu()
        {
            _resetColor = Color.white;
            _errorColor = Color.red;
            _validSuffixes = new[]
            {
                "TED", "DED", "ED", "D", "ING",
                "S", "SH", "CH", "X", "Z"
            };
        }

        public void AddJam()
        {
            if (_jamDelaySlider == null || _jamStutterSlider == null)
                return;

            float delay = _jamDelaySlider.value;
            float stutter = _jamStutterSlider.value;
            AddElement(string.Format("jam_{0:000}_{1:0}", delay, stutter));
        }

        public void AddYield()
        {
            if (_yieldSlider == null)
                return;

            float val = _yieldSlider.value;
            AddElement(string.Format("yield_{0:00}", val));
        }

        public void AddPitch()
        {
            if (_pitchSlider == null)
                return;

            float val = _pitchSlider.value;
            AddElement(string.Format("pitch_{0:0.00}", val));
        }

        public void ResetPages()
        {
            _pageIndex = 0;
            JumpPages(0);
        }

        public void JumpPages(int i = 1)
        {
            _pageIndex += i;

            if (_pageIndex < 0)
                _pageIndex = 0;
            else if (_pageIndex >= _maxPages)
                _pageIndex = _maxPages;

            if (_buttons == null)
            {
                UpdateCounters();
                return;
            }

            int perPage = _buttons.Length;
            int startIndex = perPage * _pageIndex;

            for (int j = 0; j < perPage; j++)
            {
                var btn = _buttons[j];
                var announcer = NineTailedFoxAnnouncer.singleton;
                var lines = announcer?.voiceLines;

                if (lines == null)
                {
                    UpdateCounters();
                    return;
                }

                startIndex++;
                if (startIndex >= lines.Length)
                    btn.text = string.Empty;
                else
                {
                    var line = lines[startIndex];
                    btn.text = line?.GetName() ?? string.Empty;
                }
            }

            UpdateCounters();
        }

        public void RefreshButtons()
        {
            if (_buttons == null)
            {
                UpdateCounters();
                return;
            }

            int perPage = _buttons.Length;
            int startIndex = perPage * _pageIndex;

            for (int j = 0; j < perPage; j++)
            {
                var btn = _buttons[j];
                var announcer = NineTailedFoxAnnouncer.singleton;
                var lines = announcer?.voiceLines;

                if (lines == null)
                {
                    UpdateCounters();
                    return;
                }

                startIndex++;
                if (startIndex >= lines.Length)
                    btn.text = string.Empty;
                else
                {
                    var line = lines[startIndex];
                    btn.text = line?.GetName() ?? string.Empty;
                }
            }

            UpdateCounters();
        }

        public void AddElement(string text)
        {
            if (string.IsNullOrEmpty(text))
                return;

            var input = InputFieldText;
            if (input == null)
                return;

            string current = input.text.Trim();
            input.text = string.Concat(current, " ", text);
        }

        public void AddElement(TMP_Text text)
        {
            if (text == null)
                return;

            AddElement(text.text);
        }

        public void TestCassie()
        {
            var input = InputFieldText;
            if (input == null || string.IsNullOrEmpty(input.text))
                return;

            var announcer = NineTailedFoxAnnouncer.singleton;
            if (announcer == null)
                return;

            announcer.AddPhraseToQueue(input.text, false, false, false, true);
        }

        public void CheckEasterEgg()
        {
            var input = InputFieldText;
            if (input == null || string.IsNullOrEmpty(input.text))
                return;

            string upper = input.text.ToUpper();
            if (!upper.Contains("MRBOOBOOSTANK"))
                return;

            if (input.textComponent != null)
                input.textComponent.color = Color.green;

            input.SetTextWithoutNotify(
                "An incredibly well-made guide, useful to many in this community. Thank you, mrbooboostank! :)\n" +
                "https://steamcommunity.com/sharedfiles/filedetails/?id=1577299753");
        }

        public void CopyToClipboard()
        {
            var input = InputFieldText;
            if (input != null)
                GUIUtility.systemCopyBuffer = input.text;
        }

        protected override void OnStart()
        {
            var announcer = NineTailedFoxAnnouncer.singleton;
            if (announcer == null || _buttons == null || announcer.voiceLines == null)
            {
                UpdateCounters();
                return;
            }

            _maxPages = announcer.voiceLines.Length / _buttons.Length;
            UpdateCounters();

            _pageIndex = 0;
            JumpPages(0);

            if (InputFieldText != null)
                InputFieldText.onValueChanged.AddListener(OnValueChange);
        }

        private void UpdateCounters()
        {
            if (_buttons == null || _pageCount == null || _elementCount == null)
                return;

            int perPage = _buttons.Length;
            int displayed = perPage * _pageIndex + 12;

            var announcer = NineTailedFoxAnnouncer.singleton;
            int totalLines = announcer?.voiceLines?.Length ?? 0;

            if (displayed > totalLines)
                displayed = totalLines;

            int currentPage = _pageIndex + 1;
            int maxPages = _maxPages + 1;

            _pageCount.text = string.Format(
                "<color=#E9F162>Pages:</color>\n{0:00}/{1:00}",
                currentPage, maxPages);

            _elementCount.text = string.Format(
                "<color=#E9F162>Elements:</color>\n{0:000}/{1}",
                displayed, totalLines);
        }

        private int Index(int increment = 1)
        {
            _pageIndex += increment;

            if (_pageIndex < _maxPages)
            {
                _pageIndex = _maxPages;
                return _maxPages;
            }

            _pageIndex = 0;
            return 0;
        }

        private void OnValueChange(string newValue)
        {
            if (string.IsNullOrEmpty(newValue))
            {
                if (InputFieldText?.textComponent != null)
                    InputFieldText.textComponent.color = _resetColor;
                return;
            }

            string clean = Misc.RichTextRegex?.Replace(newValue, string.Empty) ?? newValue;
            clean = clean.Trim();

            var words = clean.Split(' ');
            bool hasInvalid = false;

            foreach (var word in words)
            {
                if (string.IsNullOrEmpty(word))
                    continue;

                if (IsValidWord(word))
                    continue;

                if (HasValidWord(word))
                    continue;

                hasInvalid = true;
                break;
            }

            if (InputFieldText?.textComponent != null)
                InputFieldText.textComponent.color = hasInvalid ? _errorColor : _resetColor;
        }

        private bool HasValidWord(string word)
        {
            if (_validSuffixes == null || string.IsNullOrEmpty(word))
                return false;

            foreach (var suffix in _validSuffixes)
            {
                if (string.IsNullOrEmpty(suffix))
                    continue;

                if (word.Length <= suffix.Length)
                    continue;

                int prefixLen = word.Length - suffix.Length;
                string prefix = word.Substring(0, prefixLen);
                string actualSuffix = word.Substring(prefixLen);

                if (!IsValidWord(prefix))
                    continue;

                if (actualSuffix.Equals(suffix, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }

        private bool IsValidWord(string word)
        {
            if (string.IsNullOrEmpty(word))
                return false;

            var announcer = NineTailedFoxAnnouncer.singleton;
            if (announcer?.voiceLines == null)
                return false;

            bool found = announcer.voiceLines.Any(vl =>
                vl != null &&
                vl.apiName != null &&
                vl.apiName.Equals(word, StringComparison.OrdinalIgnoreCase));

            if (found)
                return true;

            return !NineTailedFoxAnnouncer.VoiceLine.IsRegular(word);
        }

        private void OnDestroy()
        {
            if (InputFieldText != null)
                InputFieldText.onValueChanged.RemoveListener(OnValueChange);
        }
    }
}