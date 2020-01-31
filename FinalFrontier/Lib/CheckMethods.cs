using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Microsoft.Office.Interop.Outlook;
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
        private List<string> freemailers;
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
                freemailers = ConfigurationManager.AppSettings["freemailers"].Split(',').ToList();
                whitelist = ConfigurationManager.AppSettings["whitelist"].Split(',').ToList();
                lookalikes = ConfigurationManager.AppSettings["lookalikes"].Split(',').ToList();
                imgextensions = ConfigurationManager.AppSettings["imgextensions"].Split(',').ToList();
            }
            catch (System.Exception)
            {
                System.Windows.Forms.MessageBox.Show("Could not read configuration file app.config / " + AppDomain.CurrentDomain.SetupInformation.ConfigurationFile + "\n\nCAUTION: FINALFRONTIER WILL NOT BE FUNCTIONING PROPERLY!!!");
            }
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

        public List<CheckResult> CheckFreeMailers(string id, string instr, string senderEmailAddress)
        {
            var results = new List<CheckResult>();

            foreach (string freemailer in freemailers)
            {
                if ((instr.IndexOf(freemailer) > 0) & (senderEmailAddress.IndexOf(freemailer) < 1))
                {
                    results.Add(new CheckResult(id, freemailer, instr.Substring(0,50)+"[...]", -20));
                }
            }
            return results;
        }

        public CheckResult CheckBadTld(string id, string instr)
        {
            if (instr == null)
                return null;

            return badtlds.Where(x => instr.EndsWith(x)).Select(y => new CheckResult(id, y, instr, -20)).FirstOrDefault();
        }

        public List<CheckResult> CheckKeywords(string id, string instr)
        {
            return keywords.Where(x => instr.ToLower().Contains(x))
                .Select(x => new CheckResult(id, x, instr, -20)).ToList();
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

            var sha = new SHA256Managed();
            var result = new List<CheckResult>();
            var tmpPath = Path.GetTempPath() + "FinalFrontier\\";

            if (!Directory.Exists(tmpPath))
                Directory.CreateDirectory(tmpPath);
            tmpPath += Path.GetRandomFileName();

            try
            {
                testfile.SaveAsFile(tmpPath);

                byte[] filehash = sha.ComputeHash(File.OpenRead(tmpPath));
                string filehashstr = BitConverter.ToString(filehash).Replace("-", string.Empty);

                if (badhashessha256.Contains(filehashstr))            
                    result.Add(new CheckResult(id, "sha256", filehashstr, -100));
            }
            catch (System.Exception)
            {
                throw;
            }
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

        public string GetSenderSMTPAddress(MailItem mail)
        {
            string PR_SMTP_ADDRESS = @"http://schemas.microsoft.com/mapi/proptag/0x39FE001E";
            if (mail == null)
                throw new ArgumentNullException(nameof(mail));
            
            if (mail.SenderEmailType == "EX")
            {
                AddressEntry sender = mail.Sender;
                if (sender != null)
                {
                    //Now we have an AddressEntry representing the Sender
                    if (sender.AddressEntryUserType == OlAddressEntryUserType.olExchangeUserAddressEntry || 
                        sender.AddressEntryUserType == OlAddressEntryUserType.olExchangeRemoteUserAddressEntry)
                    {
                        //Use the ExchangeUser object PrimarySMTPAddress
                        ExchangeUser exchUser = sender.GetExchangeUser();
                        if (exchUser != null)
                        {
                            return exchUser.PrimarySmtpAddress;
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        return sender.PropertyAccessor.GetProperty(PR_SMTP_ADDRESS) as string;
                    }
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;// mail.SenderEmailAddress;
            }
        }
    }
}
