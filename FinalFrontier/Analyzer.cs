using Microsoft.Office.Interop.Outlook;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace FinalFrontier
{
    public class Analyzer
    {
        private DictionaryTools dt;
        private Dictionary<string, int> DictSenderName;
        private Dictionary<string, int> DictSenderEmail;
        private Dictionary<string, int> DictSenderCombo;
        private string senderNameDomainPart = "";
        private bool domainMismatch = false;
        private bool isWhitelisted = false;
        private bool isLookalike = false;
        private bool isBadTldSender = false;
        private bool senderNameContainsEmail = false;
        private bool hasLinksWithShorteners = false;
        private bool hasbadextensions = false;
        private bool hasdoubleextensions = false;
        private bool hasBadTldsInLinks = false;
        private string senderName;
        private string senderEmailAddress;
        private string senderCombo;
        private Attachments attachments;
        public bool isSuspicious;
        private const string HeaderRegex = @"^(?<header_key>[-A-Za-z0-9]+)(?<seperator>:[ \t]*)" +
            "(?<header_value>([^\r\n]|\r\n[ \t]+)*)(?<terminator>\r\n)";

        public int Score { get; set; }
        public string Result { get; set; }

        public Analyzer()
        {
            dt = new DictionaryTools();
            string userpath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            DictSenderName = dt.Read(userpath + "\\dict-sender-name.bin");
            DictSenderEmail = dt.Read(userpath + "\\dict-sender-email.bin");
            DictSenderCombo = dt.Read(userpath + "\\dict-sender-combo.bin");
        }

        public List<CheckResult> getSummary(MailItem mailItem)
        {
            var currentUser = mailItem.UserProperties.Session.CurrentUser.Address;
            var CheckResults = new List<CheckResult>();
            CheckMethods checkMethods = new CheckMethods();
            BodyAnalyser bodyAnalyse = new BodyAnalyser();
            
            string result = string.Empty;
            int score = 0;

            Action<CheckResult> add = x => { if (x != null) CheckResults.Add(x); };
            Action<List<CheckResult>> addRange = x => { if (x != null) CheckResults.AddRange(x); };

            isSuspicious = false;

            addRange(bodyAnalyse.AnalyzeBody(mailItem.HTMLBody));

            foreach (string entry in mailItem.Headers("Received"))
            {
                string receiveDomain = checkMethods.GetReceiveFromString(entry);
                add(checkMethods.CheckBadTld("Receive-badTLD", receiveDomain));
            }

            addRange(checkMethods.CheckSender(mailItem.SenderName, mailItem.SenderEmailAddress, GetSenderSMTPAddress(mailItem)));

            add(checkMethods.CheckRecipients(currentUser, mailItem.To?.Split(',').ToList(), mailItem.CC?.Split(',').ToList()));

            // check for suspicious sender
            senderName = mailItem.SenderName;
            senderEmailAddress = mailItem.SenderEmailAddress;

            // TODO: if senderName and SenderEmail are equal there should not be an alert!!!

            senderCombo = senderName + "/" + senderEmailAddress;

            add(checkMethods.SenderWhitelist(senderEmailAddress, senderNameDomainPart));

            // evaluate history of senderName, senderEmailAddress and their combo
            if (DictSenderName.ContainsKey(senderName))
            {
                add(new CheckResult("Meta-NameNew", "Der Name (Freitext) des Absenders ist bekannt", senderName, -40));
                score += DictSenderName[senderName];
            }
            else
            {
                add(new CheckResult("Meta-NameNew", "Der Name (Freitext) des Absenders ist neu", senderName, -10));
            }

            if (DictSenderEmail.ContainsKey(senderEmailAddress))
            {
                if (DictSenderEmail[senderEmailAddress] > 3)
                {
                    add(new CheckResult("Meta-SenderAddressSeenBefore", "Die vermeintliche Emailadresse ist bekannt.", senderEmailAddress, -30));
                }
            }
            else
            {
                add(new CheckResult("Meta-SenderNew", "Vermeintliche Emailadresse ist neu.", senderEmailAddress, -20));
            }

            if (DictSenderCombo.ContainsKey(senderCombo))
            {
                if (DictSenderCombo[senderCombo] > 3)
                {
                    add(new CheckResult("Meta-ComboSeenBefore", "Die Kombination von Absender (Freitext) und Emailadresse ist bekannt.", senderEmailAddress, 100));
                }
            }
            else
            {
                add(new CheckResult("Meta-ComboNew", "Die Kombination von Absender (Freitext) und Emailadresse ist neu.", senderEmailAddress, -40));
            }

            attachments = mailItem.Attachments;

            foreach (Attachment attachment in attachments)
            {
                addRange(checkMethods.CheckDoubleExtensions("Attachment-DoubleExtensions", attachment.FileName));

                addRange(checkMethods.CheckBadExtensions("Attachment-BadExtension", attachment.FileName));

                addRange(checkMethods.CheckKeywords("Attachment-Keyword", attachment.FileName));

                addRange(checkMethods.CheckBadHashes("Attachment-FileHash", attachment));
            }

            Debug.WriteLine("---CHECK RESULTS---");
            foreach (CheckResult cr in CheckResults)
            {
                Debug.WriteLine(cr.id + " / " + cr.ioc + " / " + cr.fragment + " / " + cr.score);
                result += cr.id + " / " + cr.ioc + " / " + cr.fragment + " / " + cr.score + Environment.NewLine;
                score += cr.score;
            }

            Score = score;
            // TODO: better result for new message box
            Result = "SCORE: " + score + Environment.NewLine + result;

            return CheckResults;
        }

        private string GetSenderSMTPAddress(MailItem mail)
        {
            string PR_SMTP_ADDRESS = @"http://schemas.microsoft.com/mapi/proptag/0x39FE001E";
            if (mail == null)
            {
                throw new ArgumentNullException();
            }
            if (mail.SenderEmailType == "EX")
            {
                Outlook.AddressEntry sender = mail.Sender;
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
