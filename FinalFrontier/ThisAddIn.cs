using System;
using System.Collections.Generic;
using Microsoft.Office.Interop.Outlook;
using System.Windows.Forms;
using System.Diagnostics;

// https://msdn.microsoft.com/en-us/library/cc668191.aspx
// https://msdn.microsoft.com/en-us/library/microsoft.office.interop.outlook.mailitem_members.aspx

// https://msdn.microsoft.com/de-de/library/ms268994.aspx


namespace FinalFrontier
{

    public partial class ThisAddIn
    {
        Inspectors inspectors;
        Explorer currentExplorer = null;
        private Dictionary<string, int> DictSenderName = new Dictionary<string, int>();
        private Dictionary<string, int> DictSenderEmail = new Dictionary<string, int>();
        private Dictionary<string, int> DictSenderCombo = new Dictionary<string, int>();
        DictionaryTools dt = new DictionaryTools();
        private String lastConversationID = "";
        private int tvcntr;
        private Scoring scoring = new Scoring();

        protected override Microsoft.Office.Core.IRibbonExtensibility CreateRibbonExtensibilityObject()
        {
            return new Ribbon1(scoring);
        }

        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            tvcntr = 0;

            Folder root = Application.Session.DefaultStore.GetRootFolder() as Folder;

            // TODO: Welcome screen

            currentExplorer = this.Application.ActiveExplorer();
            currentExplorer.SelectionChange += new ExplorerEvents_10_SelectionChangeEventHandler(CurrentExplorer_Event);

            /*
            currentExplorer.ViewSwitch += new Outlook
                .ExplorerEvents_10_ViewSwitchEventHandler
                (ExplorerWrapper_ViewSwitch);
                */

            // LEARN FROM FOLDERS
            EnumerateFolders(root);

            /*
            foreach (KeyValuePair<string, int> pair in DictSenderName)
            {
                Debug.WriteLine("{0}, {1}", pair.Key, pair.Value);
            }
            */
        }

        private void EnumerateFolders(Folder folder)
        {
            Folders childFolders = folder.Folders;
            if (childFolders.Count > 0)
            {
                foreach (Folder childFolder in childFolders)
                {
                    // Write the folder path.
                    Debug.WriteLine(childFolder.FolderPath);
                    //if (childFolder.FolderPath.EndsWith("Archive"))
                    try
                    {
                        // iterate through mails in this folder
                        Items mails = childFolder.Items;
                        foreach (MailItem mail in mails)
                        {
                            MailItem thismail = mail as MailItem;
                            string senderName = thismail.SenderName;
                            string senderEmailAddress = thismail.SenderEmailAddress;
                            string senderCombo = senderName + "/" + senderEmailAddress;
                            //Debug.WriteLine("\""  + senderName + "\" <" + senderEmailAddress + ">");
                            // if new then add; else update the three Dictionaries
                            if (DictSenderName.ContainsKey(senderName))
                            {
                                DictSenderName[senderName] = DictSenderName[senderName] + 1;
                            }
                            else
                            {
                                DictSenderName.Add(senderName, 1);
                            }
                            if (DictSenderEmail.ContainsKey(senderEmailAddress))
                            {
                                DictSenderEmail[senderEmailAddress] = DictSenderEmail[senderEmailAddress] + 1;
                            }
                            else
                            {
                                DictSenderEmail.Add(senderEmailAddress, 1);
                            }
                            if (DictSenderCombo.ContainsKey(senderCombo))
                            {
                                DictSenderCombo[senderCombo] = DictSenderCombo[senderCombo] + 1;
                            }
                            else
                            {
                                DictSenderCombo.Add(senderCombo, 1);
                            }
                        }
                    }
                    catch (System.Exception)
                    { }
                    // Call EnumerateFolders using childFolder.
                    EnumerateFolders(childFolder);
                }
            }
            // TODO: nicer user path from modelConfig
            string userpath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            dt.Write(DictSenderName, userpath + "\\dict-sender-name.bin");
            dt.Write(DictSenderEmail, userpath + "\\dict-sender-email.bin");
            dt.Write(DictSenderCombo, userpath + "\\dict-sender-combo.bin");
        }

        public void CurrentExplorer_Event()
        {
            MAPIFolder selectedFolder = Application.ActiveExplorer().CurrentFolder;
            string expMessage = ""; // "Your current folder is " + selectedFolder.Name + ".\n";
            string itemMessage = ""; // "Item is unknown.";
            try
            {
                if (Application.ActiveExplorer().Selection.Count > 0)
                {
                    MailItem mailItem = Application.ActiveExplorer().Selection[1] as MailItem;
                    if (mailItem != null)
                    {
                        try
                        {
                            // this condition should prevent the popup from showing twice
                            if (mailItem.ConversationID != lastConversationID)
                            {
                                lastConversationID = mailItem.ConversationID;
                                
                                // 
                                var scoreResult = scoring.getSummary(mailItem);
                                if (scoreResult.IsSuspicious)
                                {
                                    // TODO: Only show if Outlook is visible / has starten up
                                    VMInfoScreen.ShowScore(scoreResult);

                                    InfoScreen infoSc = new InfoScreen(scoreResult, "score");
                                    infoSc.Show();
                                }

                                tvcntr++;
                            }
                        }
                        catch (System.Exception ex)
                        {
                            Debug.Write(ex.StackTrace);
                        }
                        // remove finalfrontier p that are already present
                        //int startpos = mailItem.HTMLBody.IndexOf("<p class=\"finalfrontier");
                        //int endpos = mailItem.HTMLBody.IndexOf("</p>");
                        //if (!startpos.Equals(-1)) mailItem.HTMLBody = mailItem.HTMLBody.Remove(startpos, endpos-startpos);
                        //mailItem.HTMLBody = "<p class=\"finalfrontier\" style=\"background-color:red\"><b>FinalFrontier</b><br/>" + itemMessage + "</p>" + mailItem.HTMLBody;


                        //mailItem.Display(false);
                    }
                    /*                    else if (selObject is Outlook.ContactItem)
                                        {
                                            Outlook.ContactItem contactItem =
                                                (selObject as Outlook.ContactItem);
                                            itemMessage = "The item is a contact." +
                                                " The full name is " + contactItem.Subject + ".";
                                            contactItem.Display(false);
                                        }
                                        else if (selObject is Outlook.AppointmentItem)
                                        {
                                            Outlook.AppointmentItem apptItem =
                                                (selObject as Outlook.AppointmentItem);
                                            itemMessage = "The item is an appointment." +
                                                " The subject is " + apptItem.Subject + ".";
                                        }
                                        else if (selObject is Outlook.TaskItem)
                                        {
                                            Outlook.TaskItem taskItem =
                                                (selObject as Outlook.TaskItem);
                                            itemMessage = "The item is a task. The body is "
                                                + taskItem.Body + ".";
                                        }
                                        else if (selObject is Outlook.MeetingItem)
                                        {
                                            Outlook.MeetingItem meetingItem =
                                                (selObject as Outlook.MeetingItem);
                                            itemMessage = "The item is a meeting item. " +
                                                 "The subject is " + meetingItem.Subject + ".";
                                        }
                                        */

                    expMessage = expMessage + itemMessage;
                }
            }
            catch (System.Exception ex)
            {
                expMessage = ex.Message;
            }
            //MessageBox.Show(expMessage);
        }

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
            // Hinweis: Outlook löst dieses Ereignis nicht mehr aus. Wenn Code vorhanden ist, der 
            //    muss ausgeführt werden, wenn Outlook heruntergefahren wird. Weitere Informationen finden Sie unter https://go.microsoft.com/fwlink/?LinkId=506785.
        }

        #region Von VSTO generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }

        #endregion
    }
}
