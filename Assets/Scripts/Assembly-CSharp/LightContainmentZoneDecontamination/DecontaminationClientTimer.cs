using System.Text;
using UnityEngine;
using NorthwoodLib.Pools;
using static LightContainmentZoneDecontamination.DecontaminationController;

namespace LightContainmentZoneDecontamination
{
    public class DecontaminationClientTimer : MonoBehaviour
    {
        public static float RemainingTimeInSeconds;
        public static string ScreenTimeString;

        public float ClientTimer;

        [SerializeField]
        private AnimationCurve deviationSpeedup;

        [SerializeField]
        private float maxDeviation = 2f;

        public AnimationCurve ScaledTimer;

        private const int DigitsSpacingSize = 20;
        private const int SeparatorLeftSpacingSize = 1;
        private const int ColonRightSpacingSize = 10;
        private const int DotRightSpacingSize = 1;

        private int _lastMinutes = -1;
        private int _lastSeconds;
        private int _lastMilliseconds = -1;

        public DecontaminationClientTimer()
        {
            maxDeviation = 2f;
            _lastMinutes = -1;
            _lastMilliseconds = -1;
        }

        public void UpdateTimer(float serverTime)
        {
            float diff = serverTime - ClientTimer;

            if (Mathf.Abs(diff) < Mathf.Epsilon)
                return;

            if (Mathf.Abs(diff) > maxDeviation)
            {
                ClientTimer = serverTime;
            }
            else
            {
                if (deviationSpeedup != null)
                    ClientTimer += deviationSpeedup.Evaluate(diff) * Time.deltaTime;
            }
        }

        private void Update()
        {
            var controller = DecontaminationController.Singleton;
            if (controller == null)
                return;

            if (controller.DecontaminationOverride == DecontaminationStatus.Disabled)
                return;

            float time;

            if (controller.RoundStartTime > 0.0)
            {
                time = ScaledTimer.Evaluate(ClientTimer);
            }
            else
            {
                time = (float)controller.RoundStartTime;
            }

            if (controller.DecontaminationOverride == DecontaminationStatus.Forced)
                return;

            RemainingTimeInSeconds = time;

            if (time == 0f && controller.RoundStartTime > 0.0)
            {
                enabled = false;
                return;
            }

            time *= 60f;

            int minutes = Mathf.FloorToInt(time / 60f);
            int seconds = Mathf.FloorToInt(time % 60f);
            int milliseconds = Mathf.RoundToInt((time - Mathf.Floor(time)) * 99f);

            if (seconds == 0 && milliseconds < 10)
                milliseconds = 0;

            if (minutes == _lastMinutes && seconds == _lastSeconds && milliseconds == _lastMilliseconds)
                return;

            StringBuilder sb = StringBuilderPool.Shared.Rent(8);

            AppendDigits(sb, minutes);
            AppendColon(sb);
            AppendDigits(sb, seconds);
            AppendDot(sb);
            AppendDigits(sb, milliseconds);

            ScreenTimeString = sb.ToString();
            StringBuilderPool.Shared.Return(sb);

            _lastMinutes = minutes;
            _lastSeconds = seconds;
            _lastMilliseconds = milliseconds;
        }

        internal static void AppendDigits(StringBuilder builder, int time)
        {
            if (builder == null)
                return;

            if (time >= 10)
            {
                builder.Append(time / 10);
                builder.Append("<size=");
                builder.Append(DigitsSpacingSize);
                builder.Append("> </size>");
                builder.Append(time % 10);
            }
            else
            {
                builder.Append("0");
                builder.Append("<size=");
                builder.Append(DigitsSpacingSize);
                builder.Append("> </size>");
                builder.Append(time);
            }
        }

        internal static void AppendColon(StringBuilder builder)
        {
            if (builder == null)
                return;

            builder.Append("<size=");
            builder.Append(SeparatorLeftSpacingSize);
            builder.Append("> </size>:<size=");
            builder.Append(ColonRightSpacingSize);
            builder.Append("> </size>");
        }

        internal static void AppendDot(StringBuilder builder)
        {
            if (builder == null)
                return;

            builder.Append("<size=");
            builder.Append(SeparatorLeftSpacingSize);
            builder.Append("> </size>.<size=");
            builder.Append(DotRightSpacingSize);
            builder.Append("> </size>");
        }
    }
}
