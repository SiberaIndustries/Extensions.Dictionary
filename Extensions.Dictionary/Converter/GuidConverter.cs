namespace Extensions.Dictionary.Converter
{
    internal sealed class GuidConverter : NativeConverter<Guid>
    {
        public static readonly GuidConverter Default = new();

        public override object Convert(Guid value, ConverterSettings settings)
        {
            return settings.GuidHandling switch
            {
                GuidValueHandling.D => value.ToString("D"),
                GuidValueHandling.N => value.ToString("N"),
                GuidValueHandling.B => value.ToString("B"),
                GuidValueHandling.P => value.ToString("P"),
                GuidValueHandling.X => value.ToString("X"),
                GuidValueHandling.UpperD => value.ToString("D").ToUpper(),
                GuidValueHandling.UpperN => value.ToString("N").ToUpper(),
                GuidValueHandling.UpperB => value.ToString("B").ToUpper(),
                GuidValueHandling.UpperP => value.ToString("P").ToUpper(),
                GuidValueHandling.UpperX => value.ToString("X").ToUpper(),
                _ => throw new NotSupportedException(settings.GuidHandling.ToString())
            };
        }

        public override Guid ConvertBack(object value, ConverterSettings settings)
        {
            return Guid.ParseExact(value.ToString()!, GetGuidFormat(settings.GuidHandling));
        }

        private static string GetGuidFormat(GuidValueHandling valueHandling)
        {
            return valueHandling switch
            {
                GuidValueHandling.D or GuidValueHandling.UpperD => "D",
                GuidValueHandling.N or GuidValueHandling.UpperN => "N",
                GuidValueHandling.B or GuidValueHandling.UpperB => "B",
                GuidValueHandling.P or GuidValueHandling.UpperP => "P",
                GuidValueHandling.X or GuidValueHandling.UpperX => "X",
                _ => throw new NotSupportedException(valueHandling.ToString())
            };

        }
    }
}
