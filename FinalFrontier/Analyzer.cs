using HtmlAgilityPack;
using Microsoft.Office.Interop.Outlook;
using Outlook = Microsoft.Office.Interop.Outlook;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Configuration;

namespace FinalFrontier
{
    public static class MailItemExtensions
    {
        private const string HeaderRegex =
            @"^(?<header_key>[-A-Za-z0-9]+)(?<seperator>:[ \t]*)" +
                "(?<header_value>([^\r\n]|\r\n[ \t]+)*)(?<terminator>\r\n)";
        private const string TransportMessageHeadersSchema =
            "http://schemas.microsoft.com/mapi/proptag/0x007D001E";

        public static string[] Headers(this MailItem mailItem, string name)
        {
            var headers = mailItem.HeaderLookup();
            if (headers.Contains(name))
                return headers[name].ToArray();
            return new string[0];
        }

        public static ILookup<string, string> HeaderLookup(this MailItem mailItem)
        {
            var headerString = mailItem.HeaderString();
            var headerMatches = Regex.Matches
                (headerString, HeaderRegex, RegexOptions.Multiline).Cast<Match>();
            return headerMatches.ToLookup(
                h => h.Groups["header_key"].Value,
                h => h.Groups["header_value"].Value);
        }

        public static string HeaderString(this MailItem mailItem)
        {
            return (string)mailItem.PropertyAccessor
                .GetProperty(TransportMessageHeadersSchema);
        }
    }


    public class Analyzer
    {
        private string[] whitelist = ConfigurationManager.AppSettings["whitelist"].Split(',');
        private string[] linkshorteners = ConfigurationManager.AppSettings["linkshorteners"].Split(',');
        private string[] lookalikes = ConfigurationManager.AppSettings["lookalikes"].Split(',');
        private string[] badtlds = ConfigurationManager.AppSettings["badtlds"].Split(',');
        private string[] badextensions = ConfigurationManager.AppSettings["badextensions"].Split(',');
        private string[] docextensions = ConfigurationManager.AppSettings["docextensions"].Split(',');
        private string[] imgextensions = ConfigurationManager.AppSettings["imgextensions"].Split(',');
        private string[] exeextensions = ConfigurationManager.AppSettings["exeextensions"].Split(',');
        private string[] keywords = ConfigurationManager.AppSettings["keywords"].Split(',');
        DictionaryTools dt;
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
        private HtmlNodeCollection links;
        private Microsoft.Office.Interop.Outlook.Attachments attachments;
        private int score;
        public bool isSuspicious;
        private const string HeaderRegex =
        @"^(?<header_key>[-A-Za-z0-9]+)(?<seperator>:[ \t]*)" +
            "(?<header_value>([^\r\n]|\r\n[ \t]+)*)(?<terminator>\r\n)";

        public Analyzer()
        {
            dt = new DictionaryTools();
            String userpath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            DictSenderName = dt.Read(userpath + "\\dict-sender-name.bin");
            DictSenderEmail = dt.Read(userpath + "\\dict-sender-email.bin");
            DictSenderCombo = dt.Read(userpath + "\\dict-sender-combo.bin");
        }


        // TODO: teilweise fehlen noch die isSuspicious=1, Ausgabe muss noch konsolidiert werden (Mehrfachstrings, unterschiedliche Variablen)
        public string getSummary(Microsoft.Office.Interop.Outlook.MailItem mailItem)
        {
            score = 0;
            isSuspicious = false;
            string result = "";
            int linkcounter = 0;

            // check links within the message
            // TODO: what about non-html mails?
            String MailHtmlBody = mailItem.HTMLBody;

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(MailHtmlBody);
            links = doc.DocumentNode.SelectNodes("//a[@href]");//the parameter is use xpath see: https://www.w3schools.com/xml/xml_xpath.asp;
            if (links != null)
            {
                foreach (HtmlNode node in links)
                {
                    // check for link shorteners and redirects
                    foreach (String shortener in linkshorteners)
                    {
                        if (node.GetAttributeValue("href", null).IndexOf(shortener) > 0) hasLinksWithShorteners = true;
                    }

                    // Check for unwanted TLDs (.date, ...)
                    hasBadTldsInLinks = hasBadTld(node.GetAttributeValue("href", null)); ;

                    // check for keywords in links
                    foreach (String key in keywords)
                    {
                        if (node.GetAttributeValue("href", null).Contains(key))
                        {
                            Debug.WriteLine("Link contains keyword " + key + " - " + node.GetAttributeValue("href", null));
                        }
                    }
                }
                linkcounter = links.Count;
            }
            
            string[] receivedByArray = mailItem.Headers("Received");
            //Debug.WriteLine("receivedByArray length: " + receivedByArray.Length);
            string receivedBy;
            
            if (receivedByArray.Length > 0)
            {
                //Debug.WriteLine("RECEIVE-LINES:");
                receivedBy = receivedByArray[0];
                foreach (String entry in receivedByArray)
                {
                    String receiveDomain = getReceiveFromString(entry);
                    //Debug.WriteLine(entry);
                    Debug.WriteLine(receiveDomain);
                    if (hasBadTld(receiveDomain) == true)
                    {
                        Debug.WriteLine("badTLD in MTA-Kette");
                    }
                }
                
            }
            else
                receivedBy = "";
            
            int mailsize = mailItem.Size;
            //Debug.WriteLine("mailsize: " + mailsize);

            String senderenvelope = GetSenderSMTPAddress(mailItem);
            
            // check for suspicious sender
            senderName = mailItem.SenderName;
            senderEmailAddress = mailItem.SenderEmailAddress;

            String senderDomainEnvelope = getDomainFromMail(senderenvelope);
            String senderDomainHeader = getDomainFromMail(senderEmailAddress);

            // check if senderEmail has different domain than senderEnvelope
            if ((senderenvelope != null) & (senderDomainEnvelope != senderDomainHeader))
            {
                Debug.WriteLine("mismatch between sender domains of envelope and header");
                isSuspicious = true;
            }

            // check if senderName contains email address with different domain than senderEnvelope
            if ((senderName.Contains("@")) & (senderDomainEnvelope != getDomainFromMail(senderName)))
            {
                Debug.WriteLine("senderName contains email address with different domain than senderEnvelope");
                isSuspicious = true;
            }

            // check if senderEnvelope has badTLD
            if (hasBadTld(senderDomainEnvelope) == true)
            {
                Debug.WriteLine("badTLD in senderEnvelope");
                isSuspicious = true;
            }

            Debug.WriteLine("senderenvelope: " + senderenvelope + " - " + senderDomainEnvelope);
            Debug.WriteLine("senderheader: " + senderEmailAddress + " - " + senderDomainHeader);
            Debug.WriteLine("sendername: " + senderName);
            
            if ((senderenvelope != null) & (senderenvelope!="") & (senderEmailAddress != senderenvelope))
            {
                isSuspicious = true;
                Debug.WriteLine("TESTTESTTEST");
                result += "Der Absender ist evtl. gefälscht. ";
            }

            // TODO: if senderName and SenderEmail are equal there should not be an alert!!!

            senderCombo = senderName + "/" + senderEmailAddress;
            //result = senderName + "/" + senderEmailAddress;
            int senderNameAtPos = senderName.IndexOf("@");
            if ((senderNameAtPos != -1) & (!senderEmailAddress.Equals("")))
            {
                // senderName contains mail address
                senderNameContainsEmail = true;
                score -= 20;
                senderNameDomainPart = senderName.Substring(senderNameAtPos + 1);
                isSuspicious = true;
                result += "Der Absender ist evtl. gefälscht (Name soll Mailadresse suggerieren).";

                if ((senderEmailAddress.IndexOf(senderNameDomainPart) == -1) & (!senderEmailAddress.Equals("")))
                {
                    // senderName contains domain different to the one in senderEmailAddress
                    domainMismatch = true;
                    score -= 30;
                    result += "senderName contains email address with different domain than sender<br/>";
                    isSuspicious = true;
                    result += "Die angezeigte Mailadresse entspricht vermutlich nicht dem tatsächlichen Absender";
                }
            }

            if (hasBadTld(senderEmailAddress) == true)
            {
                isSuspicious = true;
                isBadTldSender = true;
                result += "Der Absender ist ggfs. nicht vertrauenswürdig (keine gängige Webadresse). ";
            }

            // check for domain in whitelist
            int senderEmailAddressAtPos = senderEmailAddress.IndexOf("@");
            string senderEmailAddressDomainPart = senderEmailAddress.Substring(senderEmailAddressAtPos + 1);
            if ((whitelist.Contains(senderEmailAddressDomainPart)) & (domainMismatch == false))
            {
                score += 80;
                isWhitelisted = true;
                result += "senderEmail is whitelisted<br/>";
            }
            
            // evaluate history of senderName, senderEmailAddress and their combo
            if (DictSenderName.ContainsKey(senderName))
            {
                //result += "SenderName seen before " + DictSenderName[senderName] + "x.<br/>";
                score += DictSenderName[senderName];
            }
            else
            {
                result += "Der Name (Freitext) des Absenders ist neu.";
                isSuspicious = true;
                score -= 10;
            }

            if (DictSenderEmail.ContainsKey(senderEmailAddress))
            {
                //result += "SenderEmail seen before " + DictSenderEmail[senderEmailAddress] + "x.<br/>";
                score += DictSenderEmail[senderEmailAddress];
            }
            else
            {
                result += "Vermeintliche Emailadresse ist neu.";
                score -= 10;
                isSuspicious = true;
            }

            if (DictSenderCombo.ContainsKey(senderCombo))
            {
                //result += "SenderCombo seen before " + DictSenderCombo[senderCombo] + "x.<br/>";
                score += DictSenderCombo[senderCombo];
            }
            else
            {
                result += "Die Kombination von Absender (Freitext) und Emailadresse ist neu.";
                score -= 10;
                isSuspicious = true;
            }

            //Debug.WriteLine("LOOKING FOR ATTACHMENTS");
            attachments = mailItem.Attachments;
            Debug.WriteLine(attachments.Count + " attachments.");
            foreach (Attachment attachment in attachments)
            {
                //Debug.WriteLine(attachment.FileName + " - " + attachment.Type + " - " + attachment.Size);
                // check for double extensions using docextensions and exeextensions
                foreach (String docext in docextensions)
                {
                    foreach (String exeext in exeextensions)
                    {
                        if (attachment.FileName.EndsWith(docext + exeext))
                        {
                            hasdoubleextensions = true;
                        }
                    }
                }
                
                // check for badextensions
                foreach (String ext in badextensions)
                    {
                        if (attachment.FileName.EndsWith(ext))
                        {
                            hasbadextensions = true;
                        }
                    }

                foreach (String key in keywords)
                {
                    if (attachment.FileName.Contains(key))
                    {
                        Debug.WriteLine("filename contains keyword " + key + " - " + attachment.FileName);
                    }
                }
            }

            return result;// + "<br/>Score: " + score;
        }

        private Boolean hasBadTld(String instr)
        {
            foreach (String badtld in badtlds)
            {
                //if (instr.Contains(badtld))
                if (instr.EndsWith(badtld))
                {
                    return true;
                }
            }
            return false;
        }

        private String getReceiveFromString(String inline)
        {
            String res = "";
            try
            {
                int startpos = inline.IndexOf("from ") + 5;
                int endpos = inline.Substring(startpos).IndexOf(" ");
                res = inline.Substring(startpos, endpos);
            }
            catch (System.Exception ex)
            {
                Debug.Write(ex.StackTrace);
            }
            return res;
        }

        private String getDomainFromMail(String inval)
        {
            String res = "";
            try
            {
                // TODO: Null pointer exception!!!
                int startpos = inval.IndexOf("@") + 1;
                res = inval.Substring(startpos);
            }
            catch (System.Exception ex)
            {
                Debug.Write(ex.StackTrace);
            }
            return res;
        }

        private string GetSenderSMTPAddress(Outlook.MailItem mail)
        {

            string PR_SMTP_ADDRESS =
                @"http://schemas.microsoft.com/mapi/proptag/0x39FE001E";
            if (mail == null)
            {
                throw new ArgumentNullException();
            }
            if (mail.SenderEmailType == "EX")
            {
                Outlook.AddressEntry sender =
                    mail.Sender;
                if (sender != null)
                {
                    //Now we have an AddressEntry representing the Sender
                    if (sender.AddressEntryUserType ==
                        Outlook.OlAddressEntryUserType.
                        olExchangeUserAddressEntry
                        || sender.AddressEntryUserType ==
                        Outlook.OlAddressEntryUserType.
                        olExchangeRemoteUserAddressEntry)
                    {
                        //Use the ExchangeUser object PrimarySMTPAddress
                        Outlook.ExchangeUser exchUser =
                            sender.GetExchangeUser();
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
                        return sender.PropertyAccessor.GetProperty(
                            PR_SMTP_ADDRESS) as string;
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
