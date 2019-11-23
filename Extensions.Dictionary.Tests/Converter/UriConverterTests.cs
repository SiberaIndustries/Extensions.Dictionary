using Extensions.Dictionary.Converter;
using System;
using Xunit;

namespace Extensions.Dictionary.Tests.Converter
{
    public class UriConverterTests
    {
        private readonly ConverterSettings settings = new ConverterSettings();
        private readonly UriConverter converter = new UriConverter();

        [Theory]
        [InlineData("../some/uri", UriKind.Relative)]
        [InlineData("http://some/uri.com", UriKind.Absolute)]
        public void ConvertToUri_Success(string uriString, UriKind kind)
        {
            var uri = new Uri(uriString, kind);
            Assert.True(converter.CanConvert(uri.GetType()));

            var dict = converter.Convert(uri, settings);
            Assert.Equal(uri.ToString(), dict);

            var result = converter.ConvertBack(dict, settings);
            Assert.Equal(uri, result);
        }
    }
}
