using UnityEngine;
using UnityEngine.UI;

public class LauncherAssetScanProgressBar : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private SimpleMenu _menu;
    [SerializeField] private Text _text;

    private void Update()
    {
        if (!LauncherCommunicator.IsAvailable)
            return;

        float progress = Mathf.Clamp(LauncherCommunicator.AssetVerificationProgress, 0f, 100f);
        _slider.value = progress;
        _text.text = $"Validating - {Mathf.RoundToInt(progress)}%";
    }
}