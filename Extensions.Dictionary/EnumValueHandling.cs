namespace Extensions.Dictionary
{
    public enum EnumValueHandling
    {
        /// <summary>
        /// Use the enumarator names, e.g. <c>Default</c> of <c>Default = 0</c>.
        /// </summary>
        Default = 0,

        /// <summary>
        /// Use the enumerator values, e.g. <c>0</c> of <c>Default = 0</c>.
        /// </summary>
        UnderlyingValue,
    }
}
