using System;
using System.Linq;
using System.Security.Cryptography;

namespace AeroGear.OTP
{
    public class Totp
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

        private const int DELAY_WINDOW = 1;

        private readonly long[] DIGITS_POWER = {
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
        private readonly string secret;
        private readonly Clock clock;
        private readonly int digits;

        /// <summary>
        /// Initialize an OTP instance with the shared secret generated on Registration process
        /// </summary>
        /// <param name="secret"> Shared secret </param>
        /// <param name="digits"> Number of digits of generated OTP codes </param>
        public Totp(string secret, Digits digits = Digits.Six) : this(secret, new Clock(), digits)
        {
        }

        /// <summary>
        /// Initialize an OTP instance with the shared secret generated on Registration process
        /// </summary>
        /// <param name="secret"> Shared secret </param>
        /// <param name="clock">  Clock responsible for retrieve the current interval </param>
        /// <param name="digits"> Number of digits of generated OTP codes </param>
        public Totp(string secret, Clock clock, Digits digits = Digits.Six)
        {
            this.secret = secret;
            this.clock = clock;
            this.digits = (int)digits;
        }

        /// <summary>
        /// Retrieves the encoded URI to generated the QRCode required by Google Authenticator
        /// </summary>
        /// <param name="name"> Account name </param>
        /// <returns> Encoded URI </returns>
        public virtual string uri(string name)
        {
            return string.Format("otpauth://totp/{0}?secret={1}", Uri.EscapeUriString(name), secret);
        }

        /// <summary>
        /// Retrieves the current OTP
        /// </summary>
        /// <returns> OTP </returns>
        public virtual string now()
        {
            return leftPadding(hash(secret, clock.CurrentInterval));
        }

        /// <summary>
        /// Verifier - To be used only on the server side
        /// 
        /// Verify a timeout code. The timeout code will be valid for a time
        /// determined by the interval period and the number of adjacent intervals
        /// checked.
        /// 
        /// <param name="otp"> Timeout code </param>
        /// <returns> True if the timeout code is valid</returns>
        public virtual bool verify(string otp)
        {
            long code = long.Parse(otp);
            long currentInterval = clock.CurrentInterval;

            int pastResponse = Math.Max(DELAY_WINDOW, 0);

            for (int i = pastResponse; i >= 0; --i)
            {
                int candidate = generate(this.secret, currentInterval - i);
                if (candidate == code)
                {
                    return true;
                }
            }
            return false;
        }

        private int generate(string secret, long interval)
        {
            return hash(secret, interval);
        }

        private int hash(string secret, long interval)
        {
            byte[] data = Base32Encoding.ToBytes(secret);
            var hash = new HMACSHA1(data).ComputeHash(BitConverter.GetBytes(interval).Reverse().ToArray());

            return bytesToInt(hash);
        }

        private int bytesToInt(byte[] hash)
        {
            // put selected bytes into result int
            int offset = hash.Last() & 0x0f;

            int binary = ((hash[offset] & 0x7f) << 24)
                | ((hash[offset + 1] & 0xff) << 16)
                | ((hash[offset + 2] & 0xff) << 8)
                | (hash[offset + 3] & 0xff);

            // there should be no integer overflow here, since `binary` is an integer itself
            return (int)(binary % DIGITS_POWER[digits]);
        }

        private string leftPadding(int otp)
        {
            string formatString = $"{{0:D{ digits }}}";

            return string.Format(formatString, otp);
        }
    }
}