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

namespace FinalFrontier
{
    public class Analyzer
    {
        private string[] whitelist = { "gmail.com", "googlemail.com", "bsi.bund.de", "bund.de", "twitter.com" };
        private string[] linkshorteners = { "bit.ly", "goo.gl", "bit.do", "tinyurl.com", "is.gd", "cli.gs", "pic.gd", "DwarfURL.com", "ow.ly", "yfrog.com", "migre.me", "ff.im", "tiny.cc", "url4.eu", "tr.im", "twit.ac", "su.pr", "twurl.nl", "snipurl.com", "BudURL.com", "short.to", "ping.fm", "Digg.com", "post.ly", "Just.as", "bkite.com", "snipr.com", "flic.kr", "loopt.us", "doiop.com", "twitthis.com", "htxt.it", "AltURL.com", "RedirX.com", "DigBig.com", "short.ie", "u.mavrev.com", "kl.am", "wp.me", "u.nu", "rubyurl.com", "om.ly", "linkbee.com", "Yep.it", "posted.at", "xrl.us", "metamark.net", "sn.im", "hurl.ws", "eepurl.com", "idek.net", "urlpire.com", "chilp.it", "moourl.com", "snurl.com", "xr.com", "lin.cr", "EasyURI.com", "zz.gd", "ur1.ca", "URL.ie", "adjix.com", "twurl.cc", "s7y.us", "EasyURL.net", "atu.ca", "sp2.ro", "Profile.to", "ub0.cc", "minurl.fr", "cort.as", "fire.to", "2tu.us", "twiturl.de", "to.ly", "BurnURL.com", "nn.nf", "clck.ru", "notlong.com", "thrdl.es", "spedr.com", "vl.am", "miniurl.com", "virl.com", "PiURL.com", "1url.com", "gri.ms", "tr.my", "Sharein.com", "urlzen.com", "fon.gs", "Shrinkify.com", "ri.ms", "b23.ru", "Fly2.ws", "xrl.in", "Fhurl.com", "wipi.es", "korta.nu", "shortna.me", "fa.b", "WapURL.co.uk", "urlcut.com", "6url.com", "abbrr.com", "SimURL.com", "klck.me", "x.se", "2big.at", "url.co.uk", "ewerl.com", "inreply.to", "TightURL.com", "a.gg", "tinytw.it", "zi.pe", "riz.gd", "hex.io", "fwd4.me", "bacn.me", "shrt.st", "ln - s.ru", "tiny.pl", "o - x.fr", "StartURL.com", "jijr.com", "shorl.com", "icanhaz.com", "updating.me", "kissa.be", "hellotxt.com", "pnt.me", "nsfw.in", "xurl.jp", "yweb.com", "urlkiss.com", "QLNK.net", "w3t.org", "lt.tl", "twirl.at", "zipmyurl.com", "urlot.com", "a.nf", "hurl.me", "URLHawk.com", "Tnij.org", "4url.cc", "firsturl.de", "Hurl.it", "sturly.com", "shrinkster.com", "ln - s.net", "go2cut.com", "liip.to", "shw.me", "XeeURL.com", "liltext.com", "lnk.gd", "xzb.cc", "linkbun.ch", "href.in", "urlbrief.com", "2ya.com", "safe.mn", "shrunkin.com", "bloat.me", "krunchd.com", "minilien.com", "ShortLinks.co.uk", "qicute.com", "rb6.me", "urlx.ie", "pd.am", "go2.me", "tinyarro.ws", "tinyvid.io", "lurl.no", "ru.ly", "lru.jp", "rickroll.it", "togoto.us", "ClickMeter.com", "hugeurl.com", "tinyuri.ca", "shrten.com", "shorturl.com", "Quip - Art.com", "urlao.com", "a2a.me", "tcrn.ch", "goshrink.com", "DecentURL.com", "decenturl.com", "zi.ma", "1link.in", "sharetabs.com", "shoturl.us", "fff.to", "hover.com", "lnk.in", "jmp2.net", "dy.fi", "urlcover.com", "2pl.us", "tweetburner.com", "u6e.de", "xaddr.com", "gl.am", "dfl8.me", "go.9nl.com", "gurl.es", "C - O.IN", "TraceURL.com", "liurl.cn", "MyURL.in", "urlenco.de", "ne1.net", "buk.me", "rsmonkey.com", "cuturl.com", "turo.us", "sqrl.it", "iterasi.net", "tiny123.com", "EsyURL.com", "adf.ly", "urlx.org", "IsCool.net", "twitterpan.com", "GoWat.ch", "poprl.com", "njx.me", "shrinkify.info" };
        // private string[] lookalikes = { "google", "adobe", "microsoft", "twitter", "neobooks" };
        private string[] badtlds = { ".biz", ".pro", ".name", ".coop", ".mobi", ".travel", ".xxx", ".post", ".to", ".ag", ".me", ".tel", ".bid", ".ru", ".cn", ".cc", ".tk", ".date", ".ar", ".au", ".bd", ".bg", ".br", "by", ".ca", ".cf", ".cl", ".cn", ".vb", ".cr", ".cz", ".hk", ".ht", ".tk", ".pw", ".xyz", ".id", ".in", ".my", ".pa", ".pt", ".sg", ".tw", ".tr", ".ua", ".vg", ".vn" };
        private string[] badextensions = { ".vb", ".vbe", ".vbs", ".wsh", ".wsf", ".jar", ".js", ".jse" };
        private string[] docextensions = { ".csv", ".doc", ".docx", ".gif", ".pdf", ".ppt", ".pptx", ".rtf", ".xls", ".xlsx" };
        // private string[] imgextensions = { ".bmp", ".gif", ".jpg", ".jpeg", ".png", ".tif", ".tiff" };
        private string[] exeextensions = { ".cmd", ".cpl", ".exe", ".jar", ".js", ".jse", ".lnk", ".pif", ".scr", ".vbe", ".vbs", ".vb", ".wsf", ".wsh" };
        //string[] keywords = { "rechnung", "scan", "microsoft", "adobe", "update", "help", "support", "service", "hilfe", "google" };
        private string[] keywords = { "rechnung", "scan", "microsoft", "adobe", "update", "help", "support" };
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
        private Attachments attachments;
        private int score;
        public bool isSuspicious;
        public string alertContent;
        private const string HeaderRegex =
        @"^(?<header_key>[-A-Za-z0-9]+)(?<seperator>:[ \t]*)" +
            "(?<header_value>([^\r\n]|\r\n[ \t]+)*)(?<terminator>\r\n)";

        public Analyzer()
        {
            dt = new DictionaryTools();
            string userpath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
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
            alertContent = "";
            int linkcounter = 0;

            // check links within the message
            // TODO: what about non-html mails?
            string MailHtmlBody = mailItem.HTMLBody;

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(MailHtmlBody);
            links = doc.DocumentNode.SelectNodes("//a[@href]");//the parameter is use xpath see: https://www.w3schools.com/xml/xml_xpath.asp;
            if (links != null)
            {
                foreach (HtmlNode node in links)
                {
                    // check for link shorteners and redirects
                    foreach (string shortener in linkshorteners)
                    {
                        if (node.GetAttributeValue("href", null).IndexOf(shortener) > 0)
                        {
                            hasLinksWithShorteners = true;
                        }
                    }

                    // Check for unwanted TLDs (.date, ...)
                    hasBadTldsInLinks = hasBadTld(node.GetAttributeValue("href", null)); ;

                    // check for keywords in links
                    foreach (string key in keywords)
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
            string receivedBy = "";
            
            if (receivedByArray.Length > 0)
            {
                Debug.WriteLine("RECEIVE-LINES:");
                receivedBy = receivedByArray[0];
                foreach (string entry in receivedByArray)
                {
                    string receiveDomain = getReceiveFromString(entry);
                    //Debug.WriteLine(entry);
                    Debug.WriteLine(receiveDomain);
                    if (hasBadTld(receiveDomain) == true)
                    {
                        Debug.WriteLine("badTLD in MTA-Kette");
                    }
                }                
            }
            
            //int mailsize = mailItem.Size;
            //Debug.WriteLine("mailsize: " + mailsize);

            string senderenvelope = GetSenderSMTPAddress(mailItem);
            
            // check for suspicious sender
            senderName = mailItem.SenderName;
            senderEmailAddress = mailItem.SenderEmailAddress;

            string senderDomainEnvelope = getDomainFromMail(senderenvelope);
            string senderDomainHeader = getDomainFromMail(senderEmailAddress);

            // check if senderEmail has different domain than senderEnvelope
            if (senderDomainEnvelope != senderDomainHeader)
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
            
            if (senderEmailAddress != senderenvelope)
            {
                isSuspicious = true;
                alertContent += "Der Absender ist evtl. gefälscht. ";
            }

            senderCombo = senderName + "/" + senderEmailAddress;
            //result = senderName + "/" + senderEmailAddress;
            int senderNameAtPos = senderName.IndexOf("@");
            if ((senderNameAtPos != -1) & (!senderEmailAddress.Equals("")))
            {
                // senderName contains mail address
                senderNameContainsEmail = true;
                score -= 20;
                senderNameDomainPart = senderName.Substring(senderNameAtPos + 1);
                result += "senderName contains email address\n";
                isSuspicious = true;
                alertContent += "Der Absender ist evtl. gefälscht (Name soll Mailadresse suggerieren).";

                if ((senderEmailAddress.IndexOf(senderNameDomainPart) == -1) & (!senderEmailAddress.Equals("")))
                {
                    // senderName contains domain different to the one in senderEmailAddress
                    domainMismatch = true;
                    score -= 30;
                    result += "senderName contains email address with different domain than sender\n";
                    isSuspicious = true;
                    alertContent += "Die angezeigte Mailadresse entspricht vermutlich nicht dem tatsächlichen Absender";
                }
            }

            if (hasBadTld(senderEmailAddress) == true)
            {
                isSuspicious = true;
                isBadTldSender = true;
                alertContent += "Der Absender ist ggfs. nicht vertrauenswürdig (keine gängige Webadresse). ";
            }

            // check for domain in whitelist
            int senderEmailAddressAtPos = senderEmailAddress.IndexOf("@");
            string senderEmailAddressDomainPart = senderEmailAddress.Substring(senderEmailAddressAtPos + 1);
            if ((whitelist.Contains(senderEmailAddressDomainPart)) & (domainMismatch == false))
            {
                score += 80;
                isWhitelisted = true;
                result += "senderEmail is whitelisted\n";
            }
            
            // evaluate history of senderName, senderEmailAddress and their combo
            if (DictSenderName.ContainsKey(senderName))
            {
                result += "SenderName seen before " + DictSenderName[senderName] + "x.\n";
                score += DictSenderName[senderName];
            }
            else
            {
                result += "SenderName never seen before.\n";
                score -= 10;
            }

            if (DictSenderEmail.ContainsKey(senderEmailAddress))
            {
                result += "SenderEmail seen before " + DictSenderEmail[senderEmailAddress] + "x.\n";
                score += DictSenderEmail[senderEmailAddress];
            }
            else
            {
                result += "SenderEmail never seen before.\n";
                score -= 10;
                isSuspicious = true;
            }

            if (DictSenderCombo.ContainsKey(senderCombo))
            {
                result += "SenderCombo seen before " + DictSenderCombo[senderCombo] + "x.\n";
                score += DictSenderCombo[senderCombo];
            }
            else
            {
                result += "SenderCombo never seen before.\n";
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
                foreach (string docext in docextensions)
                {
                    foreach (string exeext in exeextensions)
                    {
                        if (attachment.FileName.EndsWith(docext + exeext))
                        {
                            hasdoubleextensions = true;
                        }
                    }
                }
                
                // check for badextensions
                foreach (string ext in badextensions)
                    {
                        if (attachment.FileName.EndsWith(ext))
                        {
                            hasbadextensions = true;
                        }
                    }

                foreach (string key in keywords)
                {
                    if (attachment.FileName.Contains(key))
                    {
                        Debug.WriteLine("filename contains keyword " + key + " - " + attachment.FileName);
                    }
                }
            }

            return result + "\nScore: " + score;
        }

        private bool hasBadTld(string instr)
        {
            foreach (string badtld in badtlds)
            {
                if (instr.Contains(badtld))
                {
                    return true;
                }
            }
            return false;
        }

        private string getReceiveFromString(string inline)
        {
            string res = "";
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

        private string getDomainFromMail(string inval)
        {
            string res = "";
            try
            {
                int startpos = inval.IndexOf("@") + 1;
                res = inval.Substring(startpos);
            }
            catch (System.Exception ex)
            {
                Debug.Write(ex.StackTrace);
            }
            return res;
        }

        private string GetSenderSMTPAddress(MailItem mail)
        {
            string PR_SMTP_ADDRESS =
                @"http://schemas.microsoft.com/mapi/proptag/0x39FE001E";
            if (mail == null)
            {
                throw new ArgumentNullException();
            }
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
                return mail.SenderEmailAddress;
            }
        }
    }
}
