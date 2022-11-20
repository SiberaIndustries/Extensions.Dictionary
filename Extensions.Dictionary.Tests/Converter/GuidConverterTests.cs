using Extensions.Dictionary.Converter;
using Xunit;

namespace Extensions.Dictionary.Tests.Converter
{
    public class GuidConverterTests
    {
        private readonly ConverterSettings settings = new();
        private readonly GuidConverter converter = new();

        [Theory]
        [InlineData(GuidValueHandling.D)]
        [InlineData(GuidValueHandling.N)]
        [InlineData(GuidValueHandling.B)]
        [InlineData(GuidValueHandling.P)]
        [InlineData(GuidValueHandling.X)]
        public void ConvertToLowerGuid_Success(GuidValueHandling valueHandling)
        {
            var format = valueHandling.ToString();
            settings.GuidHandling = valueHandling;

            var guid = Guid.NewGuid();
            Assert.True(converter.CanConvert(guid.GetType()));

            var dict = converter.Convert(guid, settings);
            Assert.Equal(guid.ToString(format), dict);

            var result = converter.ConvertBack(dict, settings);
            Assert.Equal(guid, result);
        }

        [Theory]
        [InlineData(GuidValueHandling.UpperD)]
        [InlineData(GuidValueHandling.UpperN)]
        [InlineData(GuidValueHandling.UpperB)]
        [InlineData(GuidValueHandling.UpperP)]
        [InlineData(GuidValueHandling.UpperX)]
        public void ConvertToUpperGuid_Success(GuidValueHandling valueHandling)
        {
            var format = valueHandling.ToString().Replace("Upper", "");
            settings.GuidHandling = valueHandling;

            var guid = Guid.NewGuid();
            Assert.True(converter.CanConvert(guid.GetType()));

            var dict = converter.Convert(guid, settings);
            Assert.Equal(guid.ToString(format).ToUpper(), dict);

            var result = converter.ConvertBack(dict, settings);
            Assert.Equal(guid, result);
        }
    }
}
