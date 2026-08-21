using System.Diagnostics;
using Interactables.Interobjects;
using UnityEngine;

namespace CameraShaking
{
    public class ElevatorShake : IShakeEffect
    {
        public const float ShakeSpeed = 75f;

        public const float ShakeAngle = 0.12f;

        public const float PulsePrimarySpeed = 9f;

        public const float PulseSecondarySpeed = 3f;

        public static readonly Stopwatch RealTime = Stopwatch.StartNew();

        public static readonly Stopwatch LastMovement = new Stopwatch();

        public static readonly AnimationCurve FadeCurve = new AnimationCurve(new Keyframe(0f, 1f, -0.1f, -0.1f, 0f, 0.5f), new Keyframe(0.5f, 0f, 0f, 0f, 0.2f, 0f));

        [RuntimeInitializeOnLoadMethod]
        public static void Init()
        {
            ReferenceHub.OnPlayerAdded += OnPlayerAdded;
            ElevatorChamber.OnElevatorMoved += OnElevatorMoved;
        }

        public static void OnPlayerAdded(ReferenceHub hub)
        {
            if (hub.isLocalPlayer)
            {
                CameraShakeController.AddEffect(new ElevatorShake());
            }
        }

        public static void OnElevatorMoved(Bounds elevatorBounds, ElevatorChamber chamber, Vector3 deltaPos, Quaternion deltaRot)
        {
            if (elevatorBounds.Contains(MainCameraController.CurrentCamera.position))
            {
                LastMovement.Restart();
            }
        }

        public bool GetEffect(ReferenceHub ply, out ShakeEffectValues shakeValues)
        {
            float num = (LastMovement.IsRunning ? FadeCurve.Evaluate((float)LastMovement.Elapsed.TotalSeconds) : 0f);
            if (Mathf.Approximately(num, 0f))
            {
                RealTime.Restart();
                shakeValues = ShakeEffectValues.None;
            }
            else
            {
                float num2 = (float)RealTime.Elapsed.TotalSeconds;
                float num3 = Mathf.Sin(num2 * 9f);
                float num4 = Mathf.Sin(num2 * 3f);
                float num5 = Mathf.Sin(num2 * 75f) * 0.12f;
                shakeValues = new ShakeEffectValues(Quaternion.Euler(num3 * num4 * num * num5 * Vector3.up));
            }

            return true;
        }
    }
}
