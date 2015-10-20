namespace AeroGear.OTP
{
    public enum Digits
    {
        One = 1,
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5,
        Six = 6,
        Seven = 7,
        Eight = 8,
        Nine = 9,
        Ten = 10
    }

    public static class DigitsExtensions
    {
        private static readonly long[] DIGITS_POWER = {
            1 /*0*/,
            10 /*1*/,
            100 /*2*/,
            1000 /*3*/,
            10000 /*4*/,
            100000 /*5*/,
            1000000 /*6*/,
            10000000 /*7*/,
            100000000 /*8*/,
            1000000000 /*9*/,
            10000000000 /*10*/
        };

        /// <summary>
        /// Returns a divisor to be used to extract the given number of digits from a number
        /// </summary>
        /// <param name="digits"> Number of digits to extract from a number </param>
        /// <returns> The divisor </returns>
        public static long GetDivisor(this Digits digits)
        {
            return DIGITS_POWER[(int)digits];
        }

        /// <summary>
        /// Returns a format string to represent the given number of digits of a number
        /// </summary>
        /// <param name="digits"> Number of digits to extract from a number </param>
        /// <returns> The format string </returns>
        public static string GetFormat(this Digits digits)
        {
            return $"{{0:D{ digits.GetNumberOfDigits() }}}";
        }

        private static int GetNumberOfDigits(this Digits digits)
        {
            return (int)digits;
        }
    }
}