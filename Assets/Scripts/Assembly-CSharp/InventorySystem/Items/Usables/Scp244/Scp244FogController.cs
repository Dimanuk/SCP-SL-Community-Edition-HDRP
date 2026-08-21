using PostProcessing;
using UnityEngine;

namespace InventorySystem.Items.Usables.Scp244
{
    public static class Scp244FogController
    {
        private const float MaxLerpTime = 2f;
        private const float MaxFogDistance = 50f;
        private const float InstantUpdateSqrt = 60f;

        private static Vector3 _prevPos;

        [RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            StaticUnityMethods.OnUpdate += OnUpdate;
        }

        private static void OnUpdate()
        {
            Transform currentCamera = MainCameraController._currentCamera;
            if (currentCamera == null)
                return;

            if (FogController.Singleton == null)
                return;

            float lerpTime = MaxLerpTime;
            float fogEndDistance = MaxFogDistance;
            bool isInFog = false;

            Vector3 camPos = currentCamera.position;

            foreach (Scp244DeployablePickup deployable in Scp244DeployablePickup.Instances)
            {
                if (deployable == null)
                    continue;

                float fogPercent = deployable.FogPercentForPoint(camPos);
                float t = 1f - fogPercent;

                // FogLerpCurve → blend time, FogDistanceCurve → fog end distance; Min accumulates
                // across all deployed vases (the closest/strongest one wins).
                if (deployable.FogLerpCurve != null)
                    lerpTime = Mathf.Min(lerpTime, deployable.FogLerpCurve.Evaluate(t));

                if (deployable.FogDistanceCurve != null)
                    fogEndDistance = Mathf.Min(fogEndDistance, deployable.FogDistanceCurve.Evaluate(t));

                // Teleport detection must come after the curves so the instant update isn't
                // overwritten by the Min above.
                if ((_prevPos - camPos).sqrMagnitude > InstantUpdateSqrt)
                    lerpTime = 0f;

                _prevPos = camPos;

                if (fogPercent > 0f)
                    isInFog = true;
            }

            FogSetting fogSetting = FogController.Singleton.GetFogSetting(FogType.Scp244);
            if (fogSetting == null)
                return;

            fogSetting.EndDistance = fogEndDistance;

            if (isInFog)
                FogController.EnableFogType(FogType.Scp244, lerpTime);
            else
                FogController.DisableFogType(FogType.Scp244, lerpTime);
        }
    }
}