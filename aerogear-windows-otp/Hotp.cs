using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AeroGear.OTP
{
    public class Hotp
    {
        private readonly string secret;
        private readonly long currentValue;
        private readonly Digits digits;

        /// <summary>
        /// Initialize an OTP instance with the shared secret generated on Registration process
        /// </summary>
        /// <param name="secret"> Shared secret </param>
        /// <param name="digits"> Number of digits of generated OTP codes </param>
        public Hotp(string secret, Digits digits = Digits.Six) : this(secret, 0, digits)
        {
        }

        /// <summary>
        /// Initialize an OTP instance with the shared secret generated on Registration process
        /// </summary>
        /// <param name="secret"> Shared secret </param>
        /// <param name="clock">  Clock responsible for retrieve the current interval </param>
        /// <param name="digits"> Number of digits of generated OTP codes </param>
        public Hotp(string secret, long value, Digits digits = Digits.Six)
        {
            this.secret = secret;
            this.currentValue = value;
            this.digits = digits;
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
            return leftPadding(hash(secret, currentValue));
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

            int candidate = generate(this.secret, currentValue);
            return candidate == code;
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
            return (int)(binary % digits.GetDivisor());
        }

        private string leftPadding(int otp)
        {
            return string.Format(digits.GetFormat(), otp);
        }

    }
}
