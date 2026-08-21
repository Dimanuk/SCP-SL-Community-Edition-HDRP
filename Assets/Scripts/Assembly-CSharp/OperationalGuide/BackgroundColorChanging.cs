using UnityEngine;
using UnityEngine.UI;

namespace OperationalGuide
{
    public class BackgroundColorChanging : MonoBehaviour
    {
        [Header("Target & Colors")]
        public Image Background;
        
        public Color ColorIni = Color.white;
        
        public Color ColorFin = Color.red;

        [Header("Trigger Settings")]
        [Range(0f, 1f)]
        public float Chance = 0.8f;

        private int _framesActive;

        private void Start()
        {

            if (ClutterSpawner.IsHolidayActive(Holidays.Christmas))
            {
                ColorIni = Color.red;    
                ColorFin = Color.green; 
            }

            if (Background != null)
                Background.color = ColorIni;
        }

        private void Update()
        {
            if (_framesActive >= 2)
            {
                _framesActive = 0;
                if (Background != null)
                    Background.color = ColorIni;
            }

            if (_framesActive != 0)
            {
                _framesActive++;
                return;
            }

            if (Random.value <= Chance) return;

            _framesActive = 1;
            if (Background != null)
                Background.color = ColorFin;
        }
    }
}