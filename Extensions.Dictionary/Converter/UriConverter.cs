using System;

namespace Extensions.Dictionary.Converter
{
    internal sealed class UriConverter : NativeConverter<Uri>
    {
        public static readonly UriConverter Default = new UriConverter();

        public override object ToDictionary(Uri value, ConverterSettings settings)
        {
            return value.ToString();
        }

        public override Uri ToInstance(object value, ConverterSettings settings)
        {
            return new Uri(value.ToString(), UriKind.RelativeOrAbsolute);
        }
    }
}
