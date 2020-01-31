using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Office.Interop.Outlook;
using System.IO;

namespace FinalFrontierLearnLib
{
    public class Learn
    {
        private Dictionary<string, int> DictSenderName = new Dictionary<string, int>();
        private Dictionary<string, int> DictSenderEmail = new Dictionary<string, int>();
        private Dictionary<string, int> DictSenderCombo = new Dictionary<string, int>();

        private DictionaryTools dt = new DictionaryTools();

        public List<string> FolderList { get; } = new List<string>();

        private string userpath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\FinalFrontier";

        private string[] badfolders = { "Junk", "Unwanted", "Trash", "Spam", "Posteingang", "Inbox" };

        public Learn()
        {
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
                    //Console.WriteLine(childFolder.FolderPath);
                    FolderList.Add(childFolder.FolderPath);
                    GetFolders(childFolder);
                }
            }
        }

        public void LearnFolders(Folder folder, int folderid = -1)
        {
            foreach (Folder childFolder in folder.Folders)
            {
                if (badfolders.Contains(childFolder.Name))
                    continue;
                bool learn = true;
                if (folderid > 0)
                {
                    if (!childFolder.FolderPath.Contains(FolderList[folderid]))
                    {
                        learn = false;
                        continue;
                    }
                    foreach (string badfolder in badfolders)
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
            dt.Write(DictSenderName, userpath + $"\\{folder.Name}-dict-sender-name.bin");
            dt.Write(DictSenderEmail, userpath + $"\\{folder.Name}-dict-sender-email.bin");
            dt.Write(DictSenderCombo, userpath + $"\\{folder.Name}-dict-sender-combo.bin");
        }

        public void LearnFolder(Folder folder)
        {
            foreach (object mail in folder.Items)
            {
                try
                {
                    if (mail is MailItem)
                    {
                        MailItem thismail = (mail as MailItem);
                        string senderName = thismail.SenderName;
                        string senderEmailAddress = thismail.SenderEmailAddress;
                        string senderCombo = senderName + "/" + senderEmailAddress;
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
                    }
                    dt.Write(DictSenderName, userpath + $"\\{folder.Name}-dict-sender-name.bin");
                    dt.Write(DictSenderEmail, userpath + $"\\{folder.Name}-dict-sender-email.bin");
                    dt.Write(DictSenderCombo, userpath + $"\\{folder.Name}-dict-sender-combo.bin");
                }
                catch (System.Exception ex)
                {
                }
            }
        }

        public Dictionary<string, int> getDictSenderName()
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

        public Dictionary<string, int> getDictSenderEmail()
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

        public Dictionary<string, int> getDictSenderCombo()
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
    }
}
