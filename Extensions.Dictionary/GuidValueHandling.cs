namespace Extensions.Dictionary
{
    public enum GuidValueHandling
    {
        /// <summary>
        /// 32 digits separated by hyphens (default).
        /// <code>aa23d7c6-bd69-45cd-9890-600aa448638a</code>
        /// </summary>
        D = 0,

        /// <summary>
        /// 32 digits 
        /// <code>aa23d7c6bd6945cd9890600aa448638a</code>
        /// </summary>
        N,

        /// <summary>
        /// 32 digits separated by hyphens, enclosed in braces.
        /// <code>{aa23d7c6-bd69-45cd-9890-600aa448638a}</code>
        /// </summary>
        B,

        /// <summary>
        /// 32 digits separated by hyphens, enclosed in parentheses.
        /// <code>(aa23d7c6-bd69-45cd-9890-600aa448638a)</code>
        /// </summary>
        P,

        /// <summary>
        /// Four hexadecimal values enclosed in braces, where the fourth value is a subset of eight hexadecimal values that is also enclosed in braces.
        /// <code>{0xaa23d7c6,0xbd69,0x45cd,{0x98,0x90,0x60,0x0a,0xa4,0x48,0x63,0x8a}}</code>
        /// </summary>
        X,

        /// <summary>
        /// 32 uppercase digits separated by hyphens.
        /// <code>AA23D7C6-BD69-45CD-9890-600AA448638A</code>
        /// </summary>
        UpperD,

        /// <summary>
        /// 32 uppercase digits 
        /// <code>AA23D7C6BD6945CD9890600AA448638A</code>
        /// </summary>
        UpperN,

        /// <summary>
        /// 32 uppercase digits separated by hyphens, enclosed in braces.
        /// <code>{AA23D7C6-BD69-45CD-9890-600AA448638A}</code>
        /// </summary>
        UpperB,

        /// <summary>
        /// 32 uppercase digits separated by hyphens, enclosed in parentheses.
        /// <code>(AA23D7C6-BD69-45CD-9890-600AA448638A)</code>
        /// </summary>
        UpperP,

        /// <summary>
        /// Four uppercase hexadecimal values enclosed in braces, where the fourth value is a subset of eight hexadecimal values that is also enclosed in braces.
        /// <code>{0XAA23D7C6,0XBD69,0X45CD,{0X98,0X90,0X60,0X0A,0XA4,0X48,0X63,0X8A}}</code>
        /// </summary>
        UpperX
    }
}
