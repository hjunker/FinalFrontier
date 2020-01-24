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
    public class CheckMethods
    {
        private List<string> linkshorteners;
        private List<string> badtlds;
        private List<string> keywords;
        private List<string> docextensions;
        private List<string> exeextensions;
        private List<string> badextensions;
        private List<string> badhashessha256;
        private List<string> whitelist;
        // Not Used
        private List<string> lookalikes;
        private List<string> imgextensions;

        public CheckMethods() {
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

        public List<CheckResult> CheckSender(string senderName, string senderEmail, string senderEnvelope)
        {
            var results = new List<CheckResult>();

            string senderDomainEnvelope = GetDomainFromMail(senderEnvelope);
            string senderDomainHeader = GetDomainFromMail(senderEmail);
            string senderDomain = GetDomainFromMail(senderName);

            // check if senderEmail has different domain than senderEnvelope
            if ((senderEnvelope != null) && (senderDomainEnvelope != senderDomainHeader))
            {
                results.Add(new CheckResult("Meta-SenderDomainMismatch", "mismatch between sender domains of envelope and header", senderDomainEnvelope + "/" + senderDomainHeader, -40));
            }
           
            // check if senderName contains email address with different domain than senderEnvelope
            if (senderName.Contains("@") && (senderDomainEnvelope != senderDomain))
            {
                results.Add(new CheckResult("Meta-SenderNameDomainMismatch", "senderName contains email address with different domain than senderEnvelope",                     senderDomainEnvelope + "/" + senderDomain, -50));
            }

            if (!string.IsNullOrEmpty(senderEnvelope) && (senderEmail!= senderEnvelope))
            {
                results.Add(new CheckResult("Meta-SenderMismatch", "Der Absender ist evtl. gefälscht (Adresse Umschlag vs. Mail)", senderEmail+ "/" + senderEnvelope, -50));
            }

            // check if senderEnvelope has badTLD
            results.Add(CheckBadTld("SenderEnvelope-badTLD", senderDomainEnvelope));
            results.Add(CheckBadTld("SenderHeader-badTLD", senderEmail));

            int senderNameAtPos = senderName.IndexOf("@");
            string senderNameDomainPart = senderName.Substring(senderNameAtPos + 1);
            if ((senderNameAtPos != -1) && (!string.IsNullOrEmpty(senderEmail)))
            {
                // senderName contains mail address
                results.Add(new CheckResult("Meta-SenderMismatch", "Der Absender ist evtl. gefälscht (Name soll Mailadresse suggerieren)", senderEmail+ "/" + senderEnvelope, -20));

                if ((senderEmail.IndexOf(senderNameDomainPart) == -1) && string.IsNullOrEmpty(senderEmail))
                {
                    // senderName contains domain different to the one in senderEmailAddress
                    results.Add(new CheckResult("Meta-SenderPhishy", "Die angezeigte Mailadresse entspricht vermutlich nicht dem tatsächlichen Absender / senderName contains email address with different domain than sender", senderEmail + " / " + senderNameDomainPart, -40));
                }
            }

            return results;
        }

        public CheckResult CheckRecipients(string mailAddress, List<string> recipients, List<string> ccRecipients)
        {
            if (recipients.Contains(mailAddress) || (ccRecipients != null && ccRecipients.Contains(mailAddress)))
                return null;

            return new CheckResult("Address-NotContained", "Emfängermailadresse ist weder in den Empfängern noch im CC", mailAddress, -40);
        }

        public List<CheckResult> CheckLinkShorteners(string id, string instr)
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

        public CheckResult CheckBadTld(string id, string instr)
        {
            if (instr == null)
                return null;

            foreach (string badtld in badtlds)
            {
                if (instr.EndsWith(badtld))
                    return new CheckResult(id, badtld, instr, -20);
            }
            return null;
        }

        public List<CheckResult> CheckKeywords(string id, string instr)
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

        public List<CheckResult> CheckDoubleExtensions(string id, string instr)
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

        public List<CheckResult> CheckBadExtensions(string id, string instr)
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

        public List<CheckResult> CheckBadHashes(string id, Attachment testfile)
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
        
        public string GetDomainFromMail(string inval)
        {
            if (inval != null && inval.Contains("@"))
                return inval.Substring(inval.IndexOf("@") + 1);
            else
                return "";
         }

        public string GetReceiveFromString(string inline)
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

        public CheckResult SenderWhitelist(string senderEmailAddress, string senderNameDomainPart)
        {
            // check for domain in whitelist
            int senderEmailAddressAtPos = senderEmailAddress.IndexOf("@");
            string senderEmailAddressDomainPart = senderEmailAddress.Substring(senderEmailAddressAtPos + 1);
            if (whitelist.Contains(senderEmailAddressDomainPart) && (senderEmailAddress.IndexOf(senderNameDomainPart) == -1) && !string.IsNullOrEmpty(senderEmailAddress))
            {
                return new CheckResult("Meta-SenderEmailWhitelisted", "Die angezeigte Mailadresse ist in der Whitelist", senderEmailAddress + " / " + senderNameDomainPart, 80);
            }
            else
                return null;
        }
    }
}
