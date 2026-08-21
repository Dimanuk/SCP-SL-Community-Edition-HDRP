using System;
using System.Globalization;
using Mirror;

namespace Hints
{
    public class FloatHintParameter : FormattablePrimitiveHintParameter<float>
    {
        private static string FormatFloat(float value, string format)
        {
            if (string.IsNullOrEmpty(format))
            {
                return value.ToString(CultureInfo.CurrentCulture);
            }

            return value.ToString(format, CultureInfo.CurrentCulture);
        }

        public static FloatHintParameter FromNetwork(NetworkReader reader)
        {
            FloatHintParameter floatHintParameter = new FloatHintParameter();
            floatHintParameter.Deserialize(reader);

            if (floatHintParameter == null) throw new NullReferenceException();

            return floatHintParameter;
        }

        public FloatHintParameter()
            : base(new Func<NetworkReader, float>(NetworkReaderExtensions.ReadFloat),
                   new Action<NetworkWriter, float>(NetworkWriterExtensions.WriteFloat))
        {
        }

        public FloatHintParameter(float value, string format)
            : base(value, format,
                   new Func<float, string, string>(FloatHintParameter.FormatFloat),
                   new Func<NetworkReader, float>(NetworkReaderExtensions.ReadFloat),
                   new Action<NetworkWriter, float>(NetworkWriterExtensions.WriteFloat))
        {
        }
    }
}