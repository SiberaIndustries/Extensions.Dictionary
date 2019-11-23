using Extensions.Dictionary.Converter;
using Extensions.Dictionary.Resolver;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Extensions.Dictionary
{
    public class ConverterSettings
    {
        internal ISerializerResolver ResolverInternal = DefaultResolver.Instance;

        public ISerializerResolver Resolver
        {
            get => ResolverInternal;
            set => ResolverInternal = value ?? DefaultResolver.Instance;
        }

        public CultureInfo Culture { get; set; } = CultureInfo.InvariantCulture;

        public EnumValueHandling EnumHandling { get; set; }

        public DateValueHandling DateHandling { get; set; }

        public IList<MemberConverter> Converters { get; private set; } = new List<MemberConverter>
        {
            DefaultDictionaryConverter.Default,
            DefaultEnumerableConverter.Default,
            DateTimeOffsetConverter.Default,
            DateTimeConverter.Default,
            TimespanConverter.Default,
            VersionConverter.Default,
            GuidConverter.Default,
            UriConverter.Default,
        };

#pragma warning disable CS8625
        internal bool TryGetMatchingConverter(Type objectType, out MemberConverter converter)
        {
            for (int i = 0; i < Converters.Count; i++)
            {
                converter = Converters[i];
                if (converter.CanConvert(objectType))
                {
                    return true;
                }
            }

            converter = null;
            return false;
        }
#pragma warning restore CS8625
    }
}
