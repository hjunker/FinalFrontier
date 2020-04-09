using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Office.Interop.Outlook;
using System.IO;
using System.Security.Cryptography;


namespace FinalFrontier
{
    public class CheckMethods
    {
        ModelConfiguration config;

        public CheckMethods() 
        {
            config = ModelConfiguration.Instance;
        }

        public List<CheckResult> CheckLinkShorteners(string id, string instr)
        {
            if (instr == null)
                return null;

            return (config.LinkShorteners.Where(shortener => instr.IndexOf(shortener) > 0)
                .Select(shortener => new CheckResult(id, shortener, instr, -20))).ToList();
        }

        public List<CheckResult> CheckFreeMailers(string id, string instr, string senderEmailAddress)
        {
            if (instr == null)
                return null;

            return (config.Freemailers.Where(freemailer => (instr.IndexOf(freemailer) > 0) & (senderEmailAddress.IndexOf(freemailer) < 1))
                .Select(freemailer => new CheckResult(id, freemailer, instr.Substring(0, 50) + "[...]", -20))).ToList();
        }

        public CheckResult CheckBadTld(string id, string instr)
        {
            if (instr == null)
                return null;

            return config.BadTlds.Where(x => instr.EndsWith(x)).Select(y => new CheckResult(id, y, instr, -20)).FirstOrDefault();
        }

        public List<CheckResult> CheckKeywords(string id, string instr)
        {
            if (instr == null)
                return null;

            return config.Keywords.Where(x => instr.ToLower().Contains(x))
                .Select(x => new CheckResult(id, x, instr, -20)).ToList();
        }

        public List<CheckResult> CheckDoubleExtensions(string id, string instr)
        {
            if (instr == null)
                return null;

            return (config.DocExtensions.SelectMany(docext => config.ExeExtensions.Where(exeext => instr.EndsWith(docext + exeext))
                .Select(exeext => new CheckResult(id, docext + exeext, instr, -20)))).ToList();
        }

        public List<CheckResult> CheckBadExtensions(string id, string instr)
        {
            if (instr == null)
                return null;

            return (config.BadExtensions.Where(ext => instr.EndsWith(ext)).Select(ext => new CheckResult(id, ext, instr, -20))).ToList();
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
                if (testfile.Size > 0)
                {
                    testfile.SaveAsFile(tmpPath);

                    byte[] filehash = sha.ComputeHash(File.OpenRead(tmpPath));
                    string filehashstr = BitConverter.ToString(filehash).Replace("-", string.Empty);

                    if (config.BadHashesSha256.Contains(filehashstr))
                        result.Add(new CheckResult(id, "sha256", filehashstr, -100));
                }
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
                return inval.Substring(inval.IndexOf("@") + 1).ToLower();
            else
                return "";
         }

        public string GetReceiveFromString(string inline)
        {
            if (inline.ToLower().Contains("from "))
            {
                int startpos = inline.ToLower().IndexOf("from ") + 5;
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
            if (config.Whitelist.Contains(senderEmailAddressDomainPart) && (senderEmailAddress.IndexOf(senderNameDomainPart) == -1) && !string.IsNullOrEmpty(senderEmailAddress))
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
