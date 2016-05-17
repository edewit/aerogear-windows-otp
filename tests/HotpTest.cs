using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AeroGear.OTP;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace tests
{
    [TestClass]
    public class HotpTest
    {
        private string secret;

        [TestInitialize]
        public void TestInitializer()
        {
            secret = "NVQJA2PKONYYZNFFJ5LBGHRVPID5R7X5G6G2CZAZSCW6TUD3KDTA";
        }

        [TestMethod]
        public void ShouldGenerateRightCode()
        {
            Hotp totp = new Hotp(secret, 1087);

            string now = totp.now();

            Assert.AreEqual("943768", now);
            totp.verify(now);
        }

    }
}
