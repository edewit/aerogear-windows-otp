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
            String secret = "NVQJA2PKONYYZNFFJ5LBGHRVPID5R7X5G6G2CZAZSCW6TUD3KDTA";
            Totp totp = new Totp(secret, new StaticClock());

            string now = totp.now();

            Assert.AreEqual("312921", now); 
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
                return 1033;
            }
        }
    }
}
