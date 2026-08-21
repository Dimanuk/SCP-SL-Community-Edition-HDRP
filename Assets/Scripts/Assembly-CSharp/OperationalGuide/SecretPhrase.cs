using UnityEngine;

namespace OperationalGuide
{

    public class SecretPhrase : MonoBehaviour
    {

        public GameObject Secret;
        public GameObject Default;
        public GameObject DefaultPannable;
        public GameObject SecretPannable;

        public string Phrase;
        
        private int _phraseMatchIndex;

        private void Update()
        {
            string input = Input.inputString;
            if (string.IsNullOrEmpty(input)) return;


            foreach (char c in input)
            {
                CheckForSecretWord(c);
            }
        }

        private void CheckForSecretWord(char letter)
        {
            if (string.IsNullOrEmpty(Phrase)) return;

            if (_phraseMatchIndex >= Phrase.Length)
                _phraseMatchIndex = 0;

            if (Phrase[_phraseMatchIndex] == letter)
            {
                _phraseMatchIndex++;

                if (_phraseMatchIndex >= Phrase.Length)
                {
                    _phraseMatchIndex = 0;
                    SetSecretState(true);
                }
            }
            else
            {
                _phraseMatchIndex = 0; 
            }
        }

        public void SetSecretState(bool status)
        {
            if (Secret != null) Secret.SetActive(status);
            if (SecretPannable != null) SecretPannable.SetActive(status);
            if (Default != null) Default.SetActive(status);
            if (DefaultPannable != null) DefaultPannable.SetActive(status);
        }

        private void OnEnable()
        {
            if (Secret != null) Secret.SetActive(false);
            if (SecretPannable != null) SecretPannable.SetActive(false);
            if (Default != null) Default.SetActive(true);
            if (DefaultPannable != null) DefaultPannable.SetActive(true);
            
            _phraseMatchIndex = 0;
        }

        private void OnDisable()
        {
            if (Secret != null) Secret.SetActive(false);
            if (SecretPannable != null) SecretPannable.SetActive(false);
            if (Default != null) Default.SetActive(false);
            if (DefaultPannable != null) DefaultPannable.SetActive(false);
            
            _phraseMatchIndex = 0;
        }
    }
}