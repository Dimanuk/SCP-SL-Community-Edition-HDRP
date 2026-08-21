using UnityEngine;

public static class DamageIndicator
{
    private const float PulseIntensity = 1.5f;
    private const float ScrapeIntensity = 0.02f;
    private const float EffectsIntensity = 0.025f;

    public static void ReceiveDamageFromPosition(Vector3 srcPos, float damage)
    {
        Transform currentCamera = MainCameraController._currentCamera;
        if (currentCamera == null)
            return;

        Vector3 cameraPos = currentCamera.position;

        Vector3 toSource = srcPos - cameraPos;

        Vector3 localDir = currentCamera.InverseTransformDirection(toSource);

        float magnitude = Misc.MagnitudeIgnoreY(localDir);

        if (magnitude <= 0f)
            return;

        Vector3 normalizedDir = localDir / magnitude;

        ApplyBloodEffects(normalizedDir, damage);
    }

    public static void ReceiveDamageFromDirection(Vector3 dir, float damage)
    {
        float magnitude = Misc.MagnitudeIgnoreY(dir);

        if (magnitude <= 0f)
            return;

        Vector3 normalizedDir = dir / magnitude;

        ApplyBloodEffects(normalizedDir, damage);
    }
    private static void ApplyBloodEffects(Vector3 normalizedDir, float damage)
    {
        BloodEffectsSystem bloodSystem = BloodEffectsSystem.LocalPlayerSingleton;
        if (bloodSystem == null)
            return;
        bloodSystem.AddPulseBlood(normalizedDir, damage * PulseIntensity);

        bloodSystem.AddScrapeBlood(normalizedDir, damage * ScrapeIntensity);

        bloodSystem.AddScrapes(damage * EffectsIntensity);
    }
}