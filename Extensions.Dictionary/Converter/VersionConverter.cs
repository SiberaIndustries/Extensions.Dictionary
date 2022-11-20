namespace Extensions.Dictionary.Converter
{
    internal sealed class VersionConverter : MemberConverter<Version>
    {
        public static readonly VersionConverter Default = new();

        public override IDictionary<string, object> ToDictionary(Version value, ConverterSettings settings)
        {
            return new Dictionary<string, object>(4)
            {
                { nameof(Version.Major), value.Major },
                { nameof(Version.Minor), value.Minor },
                { nameof(Version.Build), value.Build },
                { nameof(Version.Revision), value.Revision },
            };
        }

        public override Version ToInstance(IDictionary<string, object?> value, ConverterSettings settings)
        {
            if (value.TryGetValue(nameof(Version.Major), out object? majorOjb) && uint.TryParse(majorOjb?.ToString(), out uint major) &&
                value.TryGetValue(nameof(Version.Minor), out object? minorObj) && uint.TryParse(minorObj?.ToString(), out uint minor))
            {
                if (value.TryGetValue(nameof(Version.Build), out object? buildObj) && uint.TryParse(buildObj?.ToString(), out uint build))
                {
                    if (value.TryGetValue(nameof(Version.Revision), out object? revObj) && uint.TryParse(revObj?.ToString(), out uint rev))
                    {
                        return new Version((int)major, (int)minor, (int)build, (int)rev);
                    }

                    return new Version((int)major, (int)minor, (int)build);
                }

                return new Version((int)major, (int)minor);
            }

            throw new NotSupportedException();
        }
    }
}
