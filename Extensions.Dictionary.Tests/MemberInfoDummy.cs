using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Extensions.Dictionary.Tests
{
    [DataContract]
    public class MemberInfoDummy
    {
        public class MemberInfoSubDummy
        {
        }

        public string Value1 { get; set; }

        public string Value2;

        [JsonPropertyName("Value33")]
        public string Value3 { get; set; }

        [DataMember(Name = "Value44")]
        public string Value4 { get; set; }

        public MemberInfoSubDummy SubDummy { get; set; }

        public void Method1()
        {
            throw new NotImplementedException();
        }
    }
}
