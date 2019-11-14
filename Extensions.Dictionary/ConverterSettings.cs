using Extensions.Dictionary.Converter;
using Extensions.Dictionary.Resolver;
using System;
using System.Collections.Generic;

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

        public IList<MemberConverter> Converters { get; private set; } = new MemberConverter[]
        {
            DefaultDictionaryConverter.Default,
            DefaultEnumerableConverter.Default,
            DateTimeOffsetConverter.Default,
            DateTimeConverter.Default
        };

        internal MemberConverter? GetMatchingConverter(Type objectType)
        {
            for (int i = 0; i < Converters.Count; i++)
            {
                var candidate = Converters[i];
                if (candidate.CanConvert(objectType))
                {
                    return candidate;
                }
            }

            return null;
        }
    }
}
