﻿using Extensions.Dictionary.Resolver;
using System;

namespace Extensions.Dictionary.Tests
{
    public class TestFixture
    {
        protected ISerializerResolver GetResolver(string resolverName, bool inspectAncestors = true)
        {
            return resolverName switch
            {
                nameof(DefaultResolver) => new DefaultResolver(),
                nameof(DataContractResolver) => new DataContractResolver { InspectAncestors = inspectAncestors },
                nameof(JsonNetSerializerResolver) => new JsonNetSerializerResolver(),
                null => null,
                _ => throw new NotSupportedException($"Unsupported resolver ${resolverName}"),
            };
        }
    }
}
