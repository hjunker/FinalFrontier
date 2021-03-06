﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Outlook;

namespace FinalFrontierLearnLib
{
    // TODO: Simplify GetDict() Functions
    public class Learn
    {
        private DictionaryTools dt = new DictionaryTools();

        public List<string> FolderList { get; } = new List<string>();

        private string userpath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\FinalFrontier";

        private string[] badFolders = { "JUNK", "UNWANTED", "TRASH", "SPAM", "POSTEINGANG", "INBOX" };

        private Dictionary<string, int> DictSenderName;
        private Dictionary<string, int> DictSenderEmail;
        private Dictionary<string, int> DictSenderCombo;
        private HashSet<string> mailId;

        public Learn()
        {
            // TODO: read the other path if it was changed

            if (!Directory.Exists(userpath))
            {
                Directory.CreateDirectory(userpath);
                DictSenderName = new Dictionary<string, int>();
                DictSenderEmail = new Dictionary<string, int>();
                DictSenderCombo = new Dictionary<string, int>();
                mailId = new HashSet<string>();
            }
            else
            {
                DictSenderName = readDictSenderName();
                DictSenderEmail = readDictSenderEmail();
                DictSenderCombo = readDictSenderCombo();
                mailId = readMailId();
            }
        }

        public Learn(string userpath, string[] badFolders)
        {
            // TODO: Copy things if path is changed and learn new if folders have changed
            // TODO: Save the path to a RegEntry(?) if it is not default

            if (this.userpath != userpath)
                this.userpath = userpath;
            if (this.badFolders != badFolders)
                this.badFolders = badFolders;

            if (!Directory.Exists(userpath))
                Directory.CreateDirectory(userpath);
        }

        public void GetFolders(Folder folder)
        {
            Folders childFolders = folder.Folders;
            if (childFolders.Count > 0)
            {
                foreach (Folder childFolder in childFolders)
                {
                    FolderList.Add(childFolder.FolderPath);
                    GetFolders(childFolder);
                }
            }
        }

        public void LearnFolders(Folder folder, int folderid = -1)
        {
            LearnFolder(folder);

            foreach (Folder childFolder in folder.Folders)
            {
                if (badFolders.Contains(childFolder.Name.ToUpper()))
                    continue;
                if (childFolder.Name.Contains("This computer only"))
                    continue;
                bool learn = true;
                if (folderid > 0)
                {
                    if (!childFolder.FolderPath.Contains(FolderList[folderid]))
                        continue;
                    foreach (string badfolder in badFolders)
                    {
                        if (childFolder.FolderPath.Contains(badfolder))
                        {
                            learn = false;
                            break;
                        }
                    }
                }
                if (learn == true)
                {
                    LearnFolder(childFolder);
                }

                LearnFolders(childFolder, folderid);
            }
        }

        public void LearnFolder(Folder folder)
        {
            foreach (object mail in folder.Items)
            {
                try
                {
                    if (mail is MailItem)
                    {
                        LearnMail(mail as MailItem, false);
                    }
                }
                catch (System.Exception ex)
                {
                    using (FileStream fs = File.OpenWrite(userpath + "\\error-log.txt"))
                    {
                        var errorLog = new UTF8Encoding(true).GetBytes("Error: Exeption in LearnFolder\n" + ex.Message);
                        fs.Write(errorLog, 0, errorLog.Length);
                    }
                }
            }
            dt.Write(DictSenderName, userpath + $"\\{folder.Name}-dict-sender-name.bin");
            dt.Write(DictSenderEmail, userpath + $"\\{folder.Name}-dict-sender-email.bin");
            dt.Write(DictSenderCombo, userpath + $"\\{folder.Name}-dict-sender-combo.bin");
            dt.WriteHashSet(mailId, userpath + "\\MailHash.bin");
        }

        public void LearnMail(MailItem mailItem, bool write)
        {            
            string senderName = mailItem.SenderName;
            string senderEmailAddress = mailItem.SenderEmailAddress;
            string senderCombo = senderName + "/" + senderEmailAddress;

            mailId.Add(mailItem.EntryID);

            if (DictSenderName.ContainsKey(senderName))
                DictSenderName[senderName] = DictSenderName[senderName] + 1;
            else
                DictSenderName.Add(senderName, 1);
            if (DictSenderEmail.ContainsKey(senderEmailAddress))
                DictSenderEmail[senderEmailAddress] = DictSenderEmail[senderEmailAddress] + 1;
            else
                DictSenderEmail.Add(senderEmailAddress, 1);
            if (DictSenderCombo.ContainsKey(senderCombo))
                DictSenderCombo[senderCombo] = DictSenderCombo[senderCombo] + 1;
            else
                DictSenderCombo.Add(senderCombo, 1);

            if (write)
            {
                dt.Write(DictSenderName, userpath + "\\Mails-dict-sender-name.bin");
                dt.Write(DictSenderEmail, userpath + "\\Mails-dict-sender-email.bin");
                dt.Write(DictSenderCombo, userpath + "\\Mails-dict-sender-combo.bin");
                dt.WriteHashSet(mailId, userpath + "\\MailHash.bin");
            }
        }

        public Dictionary<string, int> getDictSenderName()
        {
            return DictSenderName;
        }

        public Dictionary<string, int> getDictSenderEmail()
        {
            return DictSenderEmail;
        }

        public Dictionary<string, int> getDictSenderCombo()
        {
            return DictSenderCombo;
        }

        public HashSet<string> getMailId()
        {
            return mailId;
        }

        private Dictionary<string, int> readDictSenderName()
        {
            var result = new Dictionary<string, int>();

            foreach (var file in Directory.GetFiles(userpath))
            {
                if (file.EndsWith("-dict-sender-name.bin"))
                    foreach (var values in dt.Read(file))
                        if (!result.ContainsKey(values.Key))
                            result.Add(values.Key, values.Value);
            }

            return result;
        }

        private Dictionary<string, int> readDictSenderEmail()
        {
            var result = new Dictionary<string, int>();

            foreach (var file in Directory.GetFiles(userpath))
            {
                if (file.EndsWith("-dict-sender-email.bin"))
                    foreach (var values in dt.Read(file))
                        if (!result.ContainsKey(values.Key))
                            result.Add(values.Key, values.Value);
            }
            return result;
        }

        private Dictionary<string, int> readDictSenderCombo()
        {
            var result = new Dictionary<string, int>();

            foreach (var file in Directory.GetFiles(userpath))
            {
                if (file.EndsWith("-dict-sender-combo.bin"))
                    foreach (var values in dt.Read(file))
                        if (!result.ContainsKey(values.Key))
                            result.Add(values.Key, values.Value);
            }
            return result;
        }

        private HashSet<string> readMailId()
        {
            var result = new HashSet<string>();
            foreach (var value in dt.ReadHasSet(userpath + "\\MailHash.bin"))
            {
                result.Add(value);
            }
            return result;
        }
    }
}
