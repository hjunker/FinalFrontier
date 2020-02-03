using FinalFrontier;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Office.Interop.Outlook;

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
        public void CheckFreemailer_withFreemailer()
        {
            var inst = "from [1.2.3.4] (helo=smtp3.gmx.de)  by mdbox59.gmx.de with esmtpa(ID exim)(Exim 4.92 #3)	id 1ix9To - 0003ij - BM    for horst.tester@01019gmx.de; Thu, 30 Jan 2020 14:03:32 + 0100";
            var result = checkMethods.CheckFreeMailers("FreemailerCheckTest", inst, "info@freenet.de");

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void CheckLinkShorteners_withBadLink()
        {
            var result = checkMethods.CheckLinkShorteners("BadLinkShotenerTest", "https://x.se/ThisIsATest");

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void CheckBadTld_withBadTld_OrNull()
        {
            var result = checkMethods.CheckBadTld("BadTldTest", "test.date");
            Assert.AreEqual("BadTldTest", result.id);

            result = checkMethods.CheckBadTld("BadTldTest", null);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void CheckKeywords_withBadKeyword()
        {
            var result = checkMethods.CheckKeywords("KeywordTest", "TestRechnungTest.dox");

            result.AddRange(checkMethods.CheckKeywords("KeywordTest", "http://microsoft.com/TestRechnungTest"));

            Assert.IsTrue(result.Count == 3);
        }

        [TestMethod]
        public void CheckDoubleExtensions_withDoubelExtension()
        {
            var result = checkMethods.CheckDoubleExtensions("CheckDoubleExtensionsTest", "TestRechnungTest.doc.exe");

             Assert.IsTrue(result.Count > 0);
        }

        [TestMethod]
        public void CheckBadExtensions_withBadExtension()
        {
            var result = checkMethods.CheckBadExtensions("BadExtensioTest", "test.vbs");

            Assert.IsTrue(result[0].id == "BadExtensioTest");
        }

        [TestMethod]
        [DeploymentItem("app.config")]
        public void CheckBadHashes_withBadHash()
        {
            //MailItem mail = new MailItem();
            //var tmpAttachment = mail.Attachments.Add("app.config");

            //var result = checkMethods.CheckBadHashes("BadHashesTest", tmpAttachment);

            //Assert.IsTrue(result.Count == 0);
        }

        [TestMethod]
        public void GetReceiveFromMail_Test()
        {
            var result = checkMethods.GetReceiveFromString("FROM [hierIst@derFromText.de] hier kommt noch Text");

            Assert.AreEqual("[hierIst@derFromText.de]", result);
        }

        [TestMethod]
        public void GetDomainFromMail_Test()
        {
            var result = checkMethods.GetDomainFromMail("notTheDomain@theDomain.net");

            Assert.AreEqual("thedomain.net", result);
        }
    }
}
