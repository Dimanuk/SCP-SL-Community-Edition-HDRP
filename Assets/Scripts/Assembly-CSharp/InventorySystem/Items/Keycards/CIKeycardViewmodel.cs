using System.Diagnostics;
using System.Text;
using NorthwoodLib.Pools;
using TMPro;
using UnityEngine;
using Mirror;

namespace InventorySystem.Items.Keycards
{
    public class CIKeycardViewmodel : RegularKeycardViewmodel
    {
        [SerializeField] private TMP_Text _text;
        [SerializeField] private string[] _lines;
        [SerializeField] private float _timeBetweenLines;

        private int _index;
        private Stopwatch _stopwatch = new Stopwatch();
        private bool _isRandomizing;

        private const float RandomizeTime = 0.7f;
        private const string RandomizableChars = "1234567890!@#$%^&*()QWERTYUIOPASDFGHJKLZXCVBNM";
        private const int RandomTextLength = 8;

        private void Awake()
        {
            if (_lines != null && _lines.Length > 0)
            {
                _index = 0;
                _text.text = _lines[0];
            }
            _stopwatch = Stopwatch.StartNew();
        }

        private void Update()
        {
            if (_isRandomizing)
            {
                if (_stopwatch.Elapsed.TotalSeconds < RandomizeTime)
                {
                    StringBuilder stringBuilder = StringBuilderPool.Shared.Rent();
                    for (int i = 0; i < RandomTextLength; i++)
                    {
                        char randomChar = RandomizableChars[Random.Range(0, RandomizableChars.Length)];
                        stringBuilder.Append(randomChar);
                    }
                    _text.SetText(stringBuilder);
                    StringBuilderPool.Shared.Return(stringBuilder);
                    return;
                }

                _isRandomizing = false;
                _stopwatch.Restart();

                _index = (_index + 1) % _lines.Length;
                _text.text = _lines[_index];
            }

            if (_stopwatch.Elapsed.TotalSeconds >= (double)_timeBetweenLines)
            {
                _index = (_index + 1) % _lines.Length;
                _text.text = _lines[_index];
                _stopwatch.Restart();
            }
        }

        protected override void PlayInteractAnimations()
        {
            base.PlayInteractAnimations();
            _isRandomizing = true;
            _stopwatch.Restart();
        }
    }
}