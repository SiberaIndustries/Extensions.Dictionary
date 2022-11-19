namespace Extensions.Dictionary.Converter
{
    internal sealed class UriConverter : NativeConverter<Uri>
    {
        public static readonly UriConverter Default = new();

        public override object Convert(Uri value, ConverterSettings settings)
        {
            return value.ToString();
        }

        public override Uri ConvertBack(object value, ConverterSettings settings)
        {
            return new Uri(value.ToString()!, UriKind.RelativeOrAbsolute);
        }
    }
}
