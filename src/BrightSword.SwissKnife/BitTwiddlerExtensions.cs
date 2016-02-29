namespace BrightSword.SwissKnife
{
    public static unsafe class BitTwiddlerExtensions
    {
        private static byte[] GetReversedBytesFor64BitValue(byte* rgb)
        {
            return new[]
                   {
                       rgb[7],
                       rgb[6],
                       rgb[5],
                       rgb[4],
                       rgb[3],
                       rgb[2],
                       rgb[1],
                       rgb[0]
                   };
        }

        public static byte[] GetReversedBytes(this ulong _this)
        {
            return GetReversedBytesFor64BitValue((byte*) &_this);
        }

        public static byte[] GetReversedBytes(this long _this)
        {
            return GetReversedBytesFor64BitValue((byte*) &_this);
        }
    }
}