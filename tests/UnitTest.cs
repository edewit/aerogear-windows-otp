using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AeroGear.OTP
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Test()
        {
            String secret = "SECRETCODE";
            Totp totp = new Totp(secret, new StaticClock());

            string now = totp.now();

            Assert.AreEqual("255760", now); 
            totp.verify(now);
        }

        [TestMethod]
        public void CharAToInt()
        {
            //given
            char a = 'A';

            //when
            var value = Base32EncodingTest.testCharToValue(a);

            //then
            Assert.AreEqual(0, value);
        }

        [TestMethod]
        public void CharZToInt()
        {
            //given
            char z = 'Z';

            //when
            var value = Base32EncodingTest.testCharToValue(z);

            //then
            Assert.AreEqual(25, value);
        }

        [TestMethod]
        public void Char2ToInt()
        {
            //given
            char two = '2';

            //when
            var value = Base32EncodingTest.testCharToValue(two);

            //then
            Assert.AreEqual(26, value);
        }

        [TestMethod]
        public void EncodeDecode()
        {
            //given
            string code = "THECODE2";

            //when
            var encoded = Base32Encoding.ToBytes(code);
            var decoded = Base32Encoding.ToString(encoded);

            //then
            Assert.AreEqual(code, decoded);
        }
    }

    public class Base32EncodingTest : Base32Encoding
    {
        public static int testCharToValue(char c)
        {
            return Base32Encoding.CharToValue(c);
        }
    }

    public class StaticClock : Clock
    {
        public override long CurrentInterval
        {
            get
            {
                return 1;
            }
        }
    }
}
