using System;
using System.Linq;
using UnityEngine;

namespace PostProcessing
{
    public class FogController : MonoBehaviour
    {
        public static FogController Singleton;

        private const FogType DefaultFog = FogType.Inside;
        private FogSetting[] _fogSettings;

        public static void EnableFogType(FogType fogType, float seconds = 0f)
        {
            FogSetting setting = Singleton.GetFogSetting(fogType);
            if (setting == null) return;

            if (!setting.IsEnabled)
            {
                setting.IsEnabled = true;
            }
            setting.BlendTime = seconds;
        }

        public static void DisableFogType(FogType fogType, float seconds = 0f)
        {
            FogSetting setting = Singleton.GetFogSetting(fogType);
            if (setting == null) return;

            if (setting.IsEnabled)
            {
                setting.IsEnabled = false;
            }
            setting.BlendTime = seconds;
        }

        public FogSetting GetFogSetting(FogType fogType)
        {
            foreach (FogSetting setting in _fogSettings)
            {
                if (setting.FogType == fogType)
                    return setting;
            }

            throw new NotImplementedException(
                string.Format(
                    "The FogSetting component for '{0}' needs to be attached to the FogController GameObject.",
                    fogType
                )
            );
        }

        private void Awake()
        {
            if (Singleton != null)
            {
                Destroy(this);
                return;
            }

            Singleton = this;

            FogSetting[] components = GetComponents<FogSetting>();
            _fogSettings = components.OrderBy(x => x.Priority).ToArray();

            foreach (FogSetting setting in _fogSettings)
            {
                setting.enabled = (setting.FogType == FogType.Inside);
            }
        }

        private void Update()
        {
            if (_fogSettings == null || _fogSettings.Length == 0)
                return;

            FogSetting baseFog = _fogSettings[0];
            Color currentColor = baseFog.Color;
            float currentEndDistance = baseFog.EndDistance;

            for (int i = 1; i < _fogSettings.Length; i++)
            {
                FogSetting setting = _fogSettings[i];

                if (!setting.IsEnabled && setting.Weight <= 0f)
                    continue;

                setting.UpdateWeight();

                float weight = setting.Weight;
                currentColor = Color.Lerp(currentColor, setting.Color, weight);
                currentEndDistance = Mathf.Lerp(currentEndDistance, setting.EndDistance, weight);
            }

            RenderSettings.fogColor = currentColor;
            RenderSettings.fogEndDistance = currentEndDistance;
        }
    }
}