using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace AeroGear.OTP
{
    [TestClass]
    public class TotpTest
    {
        private string secret;

        [TestInitialize]
        public void TestInitializer()
        {
            secret = "R5MB5FAQNX5UIPWL";
        }

        [TestMethod]
        public void ShouldGenerateRightCode()
        {
            Totp totp = new Totp(secret, new StaticClock());

            string now = totp.now();

            Assert.AreEqual("002941", now);
            totp.verify(now);
        }

        [TestMethod]
        public void ShouldGenerateRightCodeOneDigit()
        {
            Totp totp = new Totp(secret, new StaticClock(), Digits.One);

            string now = totp.now();

            Assert.AreEqual("1", now);
            totp.verify(now);
        }

        [TestMethod]
        public void ShouldGenerateRightCodeTwoDigits()
        {
            Totp totp = new Totp(secret, new StaticClock(), Digits.Two);

            string now = totp.now();

            Assert.AreEqual("41", now);
            totp.verify(now);
        }

        [TestMethod]
        public void ShouldGenerateRightCodeThreeDigits()
        {
            Totp totp = new Totp(secret, new StaticClock(), Digits.Three);

            string now = totp.now();

            Assert.AreEqual("941", now);
            totp.verify(now);
        }

        [TestMethod]
        public void ShouldGenerateRightCodeFourDigits()
        {
            Totp totp = new Totp(secret, new StaticClock(), Digits.Four);

            string now = totp.now();

            Assert.AreEqual("2941", now);
            totp.verify(now);
        }

        [TestMethod]
        public void ShouldGenerateRightCodeFiveDigits()
        {
            Totp totp = new Totp(secret, new StaticClock(), Digits.Five);

            string now = totp.now();

            Assert.AreEqual("02941", now);
            totp.verify(now);
        }

        [TestMethod]
        public void ShouldGenerateRightCodeSixDigits()
        {
            Totp totp = new Totp(secret, new StaticClock(), Digits.Six);

            string now = totp.now();

            Assert.AreEqual("002941", now);
            totp.verify(now);
        }

        [TestMethod]
        public void ShouldGenerateRightCodeSevenDigits()
        {
            Totp totp = new Totp(secret, new StaticClock(), Digits.Seven);

            string now = totp.now();

            Assert.AreEqual("3002941", now);
            totp.verify(now);
        }

        [TestMethod]
        public void ShouldGenerateRightCodeEightDigits()
        {
            Totp totp = new Totp(secret, new StaticClock(), Digits.Eight);

            string now = totp.now();

            Assert.AreEqual("83002941", now);
            totp.verify(now);
        }

        [TestMethod]
        public void ShouldGenerateRightCodeNineDigits()
        {
            Totp totp = new Totp(secret, new StaticClock(), Digits.Nine);

            string now = totp.now();

            Assert.AreEqual("783002941", now);
            totp.verify(now);
        }

        [TestMethod]
        public void ShouldGenerateRightCodeTenDigits()
        {
            Totp totp = new Totp(secret, new StaticClock(), Digits.Ten);

            string now = totp.now();

            Assert.AreEqual("1783002941", now);
            totp.verify(now);
        }

        [TestMethod]
        public void TestClock()
        {
            //given
            var clock = new Clock(1);

            //when
            var interval = clock.CurrentInterval;

            //then
            //interval is bigger then the one from now
            Assert.IsTrue(interval > 1443013417L);
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
            return CharToValue(c);
        }
    }

    internal class StaticClock : Clock
    {
        public override long CurrentInterval
        {
            get
            {
                return 45187109L;
            }
        }
    }
}