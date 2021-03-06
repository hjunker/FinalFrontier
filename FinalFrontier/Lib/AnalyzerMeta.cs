﻿using Microsoft.Office.Interop.Outlook;
using FinalFrontierLearnLib;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;


namespace FinalFrontier
{
    public class AnalyzerMeta : AnalyzerBase
    {
        private int score;
        public override int Score { get { return score; } }
        
        private List<string> ownaddresses;
        private Learn learnLib;

        public AnalyzerMeta(Learn tmpLearnLib)
        {
            learnLib = tmpLearnLib;
        }

        public override List<CheckResult> Analyze(object data)
        {
            var mailItem = data as MailItem;
            var results = new List<CheckResult>();

            var currentUser = mailItem?.UserProperties.Session.CurrentUser.Address;
            var senderName = mailItem.SenderName;
            var senderEmailAddress = mailItem.SenderEmailAddress;
            var senderCombo = senderName + "/" + senderEmailAddress;

            // Get already known values
            Dictionary<string, int> DictSenderName = learnLib.getDictSenderName();
            Dictionary<string, int> DictSenderEmail = learnLib.getDictSenderEmail();
            Dictionary<string, int> DictSenderCombo = learnLib.getDictSenderCombo();
            
            // TODO: Get thorug app config 
            try
            {
                ownaddresses = ConfigurationManager.AppSettings["ownaddresses"].Split(',').ToList();
                ownaddresses.Add(currentUser);
            }
            catch (System.Exception)
            {
                System.Windows.Forms.MessageBox.Show("Could not read configuration file app.config / " + AppDomain.CurrentDomain.SetupInformation.ConfigurationFile + "\n\nCAUTION: FINALFRONTIER WILL NOT BE FUNCTIONING PROPERLY!!!");
            }

            var checkMethods = new CheckMethods();

            Action<CheckResult> add = x => { if (x != null) results.Add(x); };
            Action<List<CheckResult>> addRange = x =>
            {
                if (x != null)
                {
                    x.RemoveAll(y => y == null);
                    results.AddRange(x);
                }
            };

            foreach (string entry in mailItem.Headers("Received"))
            {
                string receiveDomain = checkMethods.GetReceiveFromString(entry);
                add(checkMethods.CheckBadTld("Receive-badTLD", receiveDomain));
                addRange(checkMethods.CheckFreeMailers("Receive-Freemailer", entry, senderEmailAddress));
            }

            addRange(CheckSender(senderName, senderEmailAddress, checkMethods.GetSenderSMTPAddress(mailItem)));

            add(checkRecipients(currentUser, mailItem.To?.Split(',').ToList(), mailItem.CC?.Split(',').ToList()));
            
            // evaluate history of senderName, senderEmailAddress and their combo
            if (DictSenderName.ContainsKey(senderName))
            {
                add(new CheckResult("Metadata-NewName", "Der Name (Freitext) des Absenders ist bekannt", senderName, -40));
            }
            else
            {
                add(new CheckResult("Meta-NameNew", "Der Name (Freitext) des Absenders ist neu", senderName, -10));
            }

            if (DictSenderEmail.ContainsKey(senderEmailAddress))
            {
                if (DictSenderEmail[senderEmailAddress] > 0)
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
                if (DictSenderCombo[senderCombo] > 0)
                {
                    add(new CheckResult("Meta-ComboSeenBefore", "Die Kombination von Absender (Freitext) und Emailadresse ist bekannt.", senderEmailAddress, 100));
                }
            }
            else
            {
                add(new CheckResult("Meta-ComboNew", "Die Kombination von Absender (Freitext) und Emailadresse ist neu.", senderEmailAddress, -40));
            }

            //add(checkMethods.SenderWhitelist(senderEmailAddress, senderNameDomainPart));

            foreach (string entry in mailItem.Headers("From"))
            {
                if (entry.Contains("\"\""))
                {
                    add(new CheckResult("Meta-SuspiciousSender", "Die Angabe des Absenders enthält leere Hochkommata.", senderEmailAddress, -40));
                }
                if (entry.Contains("''"))
                {
                    add(new CheckResult("Meta-SuspiciousSender", "Die Angabe des Absenders enthält leere Hochkommata.", senderEmailAddress, -40));
                }
                if (entry.StartsWith("@"))
                {
                    add(new CheckResult("Meta-SuspiciousSender", "Die Mailadresse des Absenders ist fehlerhaft / verdächtig.", senderEmailAddress, -40));
                }
            }

            if (!learnLib.getMailId().Contains(mailItem.EntryID) && !mailItem.EntryID.StartsWith("TestId"))
                learnLib.LearnMail(mailItem, true);

            score = results.Sum(x => x.score);

            return results;
        }
        
        public List<CheckResult> CheckSender(string senderName, string senderEmail, string senderEnvelope)
        {
            var results = new List<CheckResult>();
            var checkMethods = new CheckMethods();

            string senderDomainEnvelope = checkMethods.GetDomainFromMail(senderEnvelope);
            string senderDomainHeader = checkMethods.GetDomainFromMail(senderEmail);
            string senderDomain = checkMethods.GetDomainFromMail(senderName);

            // check if senderEmail has different domain than senderEnvelope
            if ((senderEnvelope != null) && (senderDomainEnvelope != senderDomainHeader))
            {
                results.Add(new CheckResult("Meta-SenderDomainMismatch", "mismatch between sender domains of envelope and header", senderDomainEnvelope + "/" + senderDomainHeader, -40));
            }
           
            // check if senderName contains email address with different domain than senderEnvelope
            if (senderName.Contains("@") && (senderDomainEnvelope != senderDomain))
            {
                results.Add(new CheckResult("Meta-SenderNameDomainMismatch", "senderName contains email address with different domain than senderEnvelope", senderDomainEnvelope + "/" + senderDomain, -50));
            }

            if (!string.IsNullOrEmpty(senderEnvelope) && (senderEmail!= senderEnvelope))
            {
                results.Add(new CheckResult("Meta-SenderMismatch", "Der Absender ist evtl. gefälscht (Adresse Umschlag vs. Mail)", senderEmail+ "/" + senderEnvelope, -50));
            }

            // check if senderEnvelope has badTLD
            results.Add(checkMethods.CheckBadTld("SenderEnvelope-badTLD", senderDomainEnvelope));
            results.Add(checkMethods.CheckBadTld("SenderHeader-badTLD", senderEmail));

            int senderNameAtPos = senderName.IndexOf("@");
            string senderNameDomainPart = senderName.Substring(senderNameAtPos + 1);
            if ((senderNameAtPos != -1) && (!string.IsNullOrEmpty(senderEmail)))
            {
                // senderName contains mail address
                results.Add(new CheckResult("Meta-SenderMismatch", "Der Absender ist evtl. gefälscht (Name soll Mailadresse suggerieren)", senderEmail+ "/" + senderEnvelope, -10));

                if ((senderEmail.IndexOf(senderNameDomainPart, StringComparison.CurrentCultureIgnoreCase) == -1) && string.IsNullOrEmpty(senderEmail))
                {
                    // senderName contains domain different to the one in senderEmailAddress
                    results.Add(new CheckResult("Meta-SenderPhishy", "Die angezeigte Mailadresse entspricht vermutlich nicht dem tatsächlichen Absender / senderName contains email address with different domain than sender", senderEmail + " / " + senderNameDomainPart, -40));
                }
            }

            return results;
        }

        private CheckResult checkRecipients(string mailAddress, List<string> recipients, List<string> ccRecipients)
        {
            foreach (string address in ownaddresses)
            {
                System.Diagnostics.Debug.WriteLine("checking own mail address: " + address);
                if (recipients.Contains(address) || (ccRecipients != null && ccRecipients.Contains(address)))
                    return null;
            }

            return new CheckResult("Address-NotContained", "Emfängermailadresse ist weder in den Empfängern noch im CC", mailAddress, -40);
        }
    }
}
