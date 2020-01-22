using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Microsoft.Office.Interop.Outlook;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;

namespace FinalFrontier
{
    public static class CheckMethods
    {
        private static List<string> linkshorteners;
        private static List<string> badtlds;
        private static List<string> keywords;
        private static List<string> docextensions;
        private static List<string> exeextensions;
        private static List<string> badextensions;
        private static List<string> badhashessha256;
        private static List<string> whitelist;
        // Not Used
        private static List<string> lookalikes;
        private static List<string> imgextensions;

        static CheckMethods() {
            try
            {
                linkshorteners = ConfigurationManager.AppSettings["linkshorteners"].Split(',').ToList();
                badtlds = ConfigurationManager.AppSettings["badtlds"].Split(',').ToList();
                keywords = ConfigurationManager.AppSettings["keywords"].Split(',').ToList();
                docextensions = ConfigurationManager.AppSettings["docextensions"].Split(',').ToList();
                exeextensions = ConfigurationManager.AppSettings["exeextensions"].Split(',').ToList();
                badextensions = ConfigurationManager.AppSettings["badextensions"].Split(',').ToList();
                badhashessha256 = ConfigurationManager.AppSettings["badhashessha256"].Split(',').ToList();
                whitelist = ConfigurationManager.AppSettings["whitelist"].Split(',').ToList();
                lookalikes = ConfigurationManager.AppSettings["lookalikes"].Split(',').ToList();
                imgextensions = ConfigurationManager.AppSettings["imgextensions"].Split(',').ToList();
            }
            catch (System.Exception)
            {
                System.Windows.Forms.MessageBox.Show("Could not read configuration file app.config");
            }
        }

        public static List<CheckResult> CheckLinkShorteners(string id, string instr)
        {
            var results = new List<CheckResult>();

            foreach (string shortener in linkshorteners)
            {
                if (instr.IndexOf(shortener) > 0)
                {
                    results.Add(new CheckResult(id, shortener, instr, -20));
                }
            }
            return results;
        }

        public static List<CheckResult> CheckBadTld(string id, string instr)
        {
            var result = new List<CheckResult>();
            if (instr == null)
                return null;
            foreach (string badtld in badtlds)
            {
                if (instr.EndsWith(badtld))
                {
                    result.Add(new CheckResult(id, badtld, instr, -20));
                }
            }
            return result;
        }

        public static List<CheckResult> CheckKeywords(string id, string instr)
        {
            var result = new List<CheckResult>();
            foreach (string key in keywords)
            {
                if (instr.EndsWith(key))
                {
                    result.Add(new CheckResult(id, key, instr, -20));
                }
            }
            return result;
        }

        public static List<CheckResult> CheckDoubleExtensions(string id, string instr)
        {
            var result = new List<CheckResult>();
            foreach (string docext in docextensions)
            {
                foreach (string exeext in exeextensions)
                {
                    if (instr.EndsWith(docext + exeext))
                    {
                        result.Add(new CheckResult(id, docext + exeext, instr, -20));
                    }
                }
            }
            return result;
        }

        public static List<CheckResult> CheckBadExtensions(string id, string instr)
        {
            var result = new List<CheckResult>();
            foreach (string ext in badextensions)
            {
                if (instr.EndsWith(ext))
                {
                    result.Add(new CheckResult(id, ext, instr, -20));
                }
            }
            return result;
        }

        public static List<CheckResult> CheckBadHashes(string id, Attachment testfile)
        {
            if (testfile == null) 
                return null;
            var result = new List<CheckResult>();

            string userpath = Environment.GetFolderPath(Environment.SpecialFolder.InternetCache);
            testfile.SaveAsFile(userpath + "\\testfile");
            FileStream stream = File.OpenRead(userpath + "\\testfile");
            var sha = new SHA256Managed();
            byte[] filehash = sha.ComputeHash(stream);
            string filehashstr = BitConverter.ToString(filehash).Replace("-", string.Empty);
            File.Delete(userpath + "\\testfile");

            if (badhashessha256.Contains(filehashstr))            
                result.Add(new CheckResult(id, "sha256", filehashstr, -100));
            
            return result;
        }
        
        public static string GetDomainFromMail(string inval)
        {
            if (inval != null && inval.Contains("@"))
                return inval.Substring(inval.IndexOf("@") + 1);
            else
                return "";
         }

        public static string GetReceiveFromString(string inline)
        {
            if (inline.Contains("from "))
            {
                int startpos = inline.IndexOf("from ") + 5;
                int endpos = inline.Substring(startpos).IndexOf(" ");
                return inline.Substring(startpos, endpos);
            }
            else
                return "";
        }

        public static CheckResult SenderWhitelist(string senderEmailAddress, string senderNameDomainPart)
        {
            // check for domain in whitelist
            int senderEmailAddressAtPos = senderEmailAddress.IndexOf("@");
            string senderEmailAddressDomainPart = senderEmailAddress.Substring(senderEmailAddressAtPos + 1);
            if ((whitelist.Contains(senderEmailAddressDomainPart)) && ((senderEmailAddress.IndexOf(senderNameDomainPart) == -1) && (!senderEmailAddress.Equals("")) == false))
            {
                return new CheckResult("Meta-SenderEmailWhitelisted", "Die angezeigte Mailadresse ist in der Whitelist", senderEmailAddress + " / " + senderNameDomainPart, 80);
            }
            else
                return null;
        }
    }
}
