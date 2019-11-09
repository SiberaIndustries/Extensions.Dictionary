﻿// <auto-generated>
using System;
using System.Runtime.Serialization;

namespace Extensions.Dictionary.Tests
{
    [DataContract]
    public class Dummy : IEquatable<Dummy>
    {
        public string String01 { get; set; } = nameof(String01); // Public property

        public string String02 { get; protected set; } = nameof(String02); // Protected setter property

        public string String03 { get; private set; } = nameof(String03); // Private setter property

        [Newtonsoft.Json.JsonProperty(PropertyName = "Custom" + nameof(String04))]
        [DataMember(Name = "Custom" + nameof(String04), EmitDefaultValue = true)]
        public string String04 { get; set; } = nameof(String04); // Renamed property

        [Newtonsoft.Json.JsonIgnore]
        [IgnoreDataMember]
        public string String05 { get; set; } = nameof(String05); // Ignored property

        public string String06 = nameof(String06); // Public field

        public static string String07 = nameof(String07); // Public static field

        [Newtonsoft.Json.JsonIgnore]
        [IgnoreDataMember]
        public string String08 = nameof(String08); // Ignored field

        public override int GetHashCode()
        {
            return HashCode.Combine(String01, String02, String03, String04, String05, String06, String08);
        }

        public override bool Equals(object obj) =>
            Equals(obj as Dummy);

        public bool Equals(Dummy other)
        {
            return other != null
                && String01 == other.String01
                && String02 == other.String02
                && String03 == other.String03
                && String04 == other.String04
                && String05 == other.String05
                && String06 == other.String06

                && String08 == other.String08;
        }
    }
}
