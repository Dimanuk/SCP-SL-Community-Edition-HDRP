using System;
using System.Globalization;
using Mirror;

namespace Hints
{
    public class DoubleHintParameter : FormattablePrimitiveHintParameter<double>
    {
        public static string FormatDouble(double value, string format)
        {
            if (string.IsNullOrEmpty(format))
            {
                return value.ToString(CultureInfo.CurrentCulture);
            }

            return value.ToString(format, CultureInfo.CurrentCulture);
        }

        public static DoubleHintParameter FromNetwork(NetworkReader reader)
        {
            DoubleHintParameter doubleHintParameter = new DoubleHintParameter();
            doubleHintParameter.Deserialize(reader);

            if (doubleHintParameter == null) throw new NullReferenceException();

            return doubleHintParameter;
        }

        protected DoubleHintParameter()
            : base(new Func<NetworkReader, double>(NetworkReaderExtensions.ReadDouble),
                   new Action<NetworkWriter, double>(NetworkWriterExtensions.WriteDouble))
        {
        }
        public DoubleHintParameter(double value, string format)
            : base(value, format,
                   new Func<double, string, string>(DoubleHintParameter.FormatDouble),
                   new Func<NetworkReader, double>(NetworkReaderExtensions.ReadDouble),
                   new Action<NetworkWriter, double>(NetworkWriterExtensions.WriteDouble))
        {
        }
    }
}