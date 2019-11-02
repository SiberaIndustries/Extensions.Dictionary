using System;
using System.Collections.Generic;
using Xunit;

namespace Extensions.Dictionary.Tests
{
    public class DictionaryComparer<TKey, TValue> : IEqualityComparer<IDictionary<TKey, TValue>>
    {
        public bool Equals(IDictionary<TKey, TValue> x, IDictionary<TKey, TValue> y)
        {
            Assert.NotNull(x);
            Assert.NotNull(y);

            Assert.Equal(x.Count, y.Count);

            foreach (var key in x.Keys)
            {
                Assert.Contains(key, y);

                Assert.Equal(x[key].GetType(), y[key].GetType());

                if (x[key] is IDictionary<TKey, TValue> dict)
                {
                    Assert.Equal(dict, y[key] as IDictionary<TKey, TValue>, this);
                }
                else
                {
                    Assert.StrictEqual(x[key], y[key]);
                }
            }

            return true;
        }

        public int GetHashCode(IDictionary<TKey, TValue> obj)
        {
            throw new NotImplementedException();
        }
    }
}
