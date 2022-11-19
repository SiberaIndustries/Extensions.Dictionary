using Extensions.Dictionary.Converter;
using Xunit;

namespace Extensions.Dictionary.Tests.Converter
{
    public class GuidConverterTests
    {
        private readonly ConverterSettings settings = new();
        private readonly GuidConverter converter = new();

        [Fact]
        public void ConvertToGuid_Success()
        {
            var guid = Guid.NewGuid();
            Assert.True(converter.CanConvert(guid.GetType()));

            var dict = converter.Convert(guid, settings);
            Assert.Equal(guid.ToString(), dict);

            var result = converter.ConvertBack(dict, settings);
            Assert.Equal(guid, result);
        }
    }
}
