using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroGear.OTP
{
    public class Base32Encoding
    {
        private const int SHIFT = 5;
        private const int MASK = 31;

        public static byte[] ToBytes(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                throw new ArgumentNullException("input");
            }

            input = input.TrimEnd('=').ToUpper();
            int byteCount = input.Length * 5 / 8;
            byte[] returnArray = new byte[byteCount];

            int buffer = 0;
            int next = 0;
            int bitsLeft = 0;

            foreach (char c in input)
            {
                buffer <<= SHIFT;
                buffer |= CharToValue(c) & MASK;
                bitsLeft += SHIFT;
                if (bitsLeft >= 8)
                {
                    returnArray[next++] = (byte)(buffer >> (bitsLeft - 8));
                    bitsLeft -= 8;
                }

                int cValue = CharToValue(c);
            }

            return returnArray;
        }

        public static string ToString(byte[] input)
        {
            if (input == null || input.Length == 0)
            {
                throw new ArgumentNullException("input");
            }

            int charCount = (int)Math.Ceiling(input.Length / 5d) * 8;
            char[] returnArray = new char[charCount];

            byte nextChar = 0, bitsRemaining = 5;
            int arrayIndex = 0;

            foreach (byte b in input)
            {
                nextChar = (byte)(nextChar | (b >> (8 - bitsRemaining)));
                returnArray[arrayIndex++] = ValueToChar(nextChar);

                if (bitsRemaining < 4)
                {
                    nextChar = (byte)((b >> (3 - bitsRemaining)) & 31);
                    returnArray[arrayIndex++] = ValueToChar(nextChar);
                    bitsRemaining += 5;
                }

                bitsRemaining -= 3;
                nextChar = (byte)((b << bitsRemaining) & 31);
            }

            //if we didn't end with a full char
            if (arrayIndex != charCount)
            {
                returnArray[arrayIndex++] = ValueToChar(nextChar);
                while (arrayIndex != charCount) returnArray[arrayIndex++] = '='; //padding
            }

            return new string(returnArray);
        }

        protected static int CharToValue(char c)
        {
            int value = (int)c;

            if (value <= 'Z' && value >= 'A')
            {
                return value - 'A';
            }
            if (value <= '7' && value >= '2')
            {
                return value - 24;
            }

            throw new ArgumentException("Character is not a Base32 character.", "" + c);
        }

        private static char ValueToChar(byte b)
        {
            if (b < 26)
            {
                return (char)(b + 65);
            }

            if (b < 32)
            {
                return (char)(b + 24);
            }

            throw new ArgumentException("Byte is not a value Base32 value.", "b");
        }

    }    
}
