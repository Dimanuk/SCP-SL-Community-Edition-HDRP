using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Subtitles
{
    public class SubtitleCategory : MonoBehaviour
    {
        private class Message
        {
            public string Text { get; private set; }
            public float Duration { get; private set; }

            public Message(string text, float duration)
            {
                Text = text;
                Duration = duration;
            }
        }

        private static readonly string[] SplitSeperator = new string[] { "<split>" };

        [SerializeField]
        private string speakerNameColor;

        public TextMeshProUGUI CategoryText;

        private float _timer;
        private Queue<Message> _messages = new Queue<Message>();
        private bool _isPlayingMessage;
        private Message _currentMessage;
        private int _currentMessageSize;
        private int _speakerStartStringSize;
        public void AddSubtitle(string message, float duration, float delay)
        {
            _messages.Enqueue(new Message(string.Empty, delay));
            string[] parts = message.Split(SplitSeperator, StringSplitOptions.None);

            int totalLength = 0;
            foreach (string part in parts)
                totalLength += part.Length;

            foreach (string part in parts)
            {
                float partDuration = Mathf.Max(0f, (part.Length / (float)totalLength) * duration);
                _messages.Enqueue(new Message(part, partDuration));
            }

            _messages.Enqueue(new Message(string.Empty, delay));
        }

        public void ClearSubtitles()
        {
            _messages.Clear();
            _isPlayingMessage = false;
            if (CategoryText != null)
                CategoryText.text = string.Empty;
        }

        private void Awake()
        {
            _speakerStartStringSize = "C.A.S.S.I.E :".Length;
        }

        private void Update()
        {
            if (!_isPlayingMessage)
            {
                CheckForMessageStart();
                return;
            }

            _timer -= Time.deltaTime;

            if (_timer <= 0f)
            {
                _isPlayingMessage = false;
                if (CategoryText != null)
                    CategoryText.text = string.Empty;

                CheckForMessageStart();
            }

            if (CategoryText != null && CategoryText.maxVisibleCharacters < _currentMessageSize)
            {
                CategoryText.maxVisibleCharacters++;
            }
        }

        private void CheckForMessageStart()
        {

            if (_messages.Count == 0)
                return;

            _currentMessage = _messages.Dequeue();
            _timer = _currentMessage.Duration;
            _isPlayingMessage = true;

            if (string.IsNullOrWhiteSpace(_currentMessage.Text))
                return;

            if (CategoryText == null)
                return;

            CategoryText.maxVisibleCharacters = _speakerStartStringSize;
            CategoryText.text = string.Concat(
                "<color=", speakerNameColor, ">C.A.S.S.I.E :</color> ",
                _currentMessage.Text
            );

            _currentMessageSize = CategoryText.text.Length;
        }
    }
}
