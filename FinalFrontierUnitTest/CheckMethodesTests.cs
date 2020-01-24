using System;
using FinalFrontier;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FinalFrontierUnitTest
{
    [TestClass]
    public class CheckMethodesTests
    {
        CheckMethods checkMethods;

        [TestInitialize]
        public void init()
        {

            checkMethods = new CheckMethods();
        }

        [TestMethod]
        public void CheckSender_Tests()
        {
            var result = checkMethods.CheckSender("test@tester.com", "senderMail@tester.net", "senderEnvelope@tester.org");
            Assert.IsNotNull(result);
            
        }

        [TestMethod]
        public void CheckBadTld_withBadTld()
        {
            var result = checkMethods.CheckBadTld("BadTldTest", "test.date");

            Assert.IsNotNull(result);
        }
    }
}
