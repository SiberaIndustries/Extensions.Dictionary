using System;

namespace Extensions.Dictionary.Converter
{
    internal sealed class GuidConverter : NativeConverter<Guid>
    {
        public static readonly GuidConverter Default = new GuidConverter();

        public override object ToDictionary(Guid value, ConverterSettings settings)
        {
            return value.ToString();
        }

        public override Guid ToInstance(object value, ConverterSettings settings)
        {
            return Guid.Parse(value.ToString());
        }
    }
}
