namespace Extensions.Dictionary
{
    public enum DateValueHandling
    {
        /// <summary>
        /// Use all existing property values.
        /// </summary>
        Default = 0,

        /// <summary>
        /// Use only the nessecary property values, such as Ticks and DateTimeKind.
        /// </summary>
        Minimum,
    }
}
