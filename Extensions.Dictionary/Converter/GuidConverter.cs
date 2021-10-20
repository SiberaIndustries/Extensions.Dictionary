using System;

namespace Extensions.Dictionary.Converter
{
    internal sealed class GuidConverter : NativeConverter<Guid>
    {
        public static readonly GuidConverter Default = new();

        public override object Convert(Guid value, ConverterSettings settings)
        {
            return value.ToString();
        }

        public override Guid ConvertBack(object value, ConverterSettings settings)
        {
            return Guid.Parse(value.ToString());
        }
    }
}
