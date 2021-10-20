using Extensions.Dictionary.Converter;
using System;
using System.Collections.Generic;
using Xunit;

namespace Extensions.Dictionary.Tests.Converter
{
    public class VersionConverterTests
    {
        private readonly ConverterSettings settings = new();
        private readonly VersionConverter converter = new();

        [Theory]
        [InlineData("1.2")]
        [InlineData("1.2.3")]
        [InlineData("1.2.3.4")]
        public void ConvertToVersion_Success(string versionString)
        {
            var version = Version.Parse(versionString);
            var expected = new Dictionary<string, object>
            {
                { nameof(Version.Major), version.Major },
                { nameof(Version.Minor), version.Minor },
                { nameof(Version.Build), version.Build },
                { nameof(Version.Revision), version.Revision },
            };
            Assert.True(converter.CanConvert(version.GetType()));

            var dict = converter.ToDictionary(version, settings);
            Assert.Equal(expected, dict);

            var result = converter.ToInstance(dict, settings);
            Assert.Equal(version, result);
        }
    }
}
