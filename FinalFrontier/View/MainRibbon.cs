using Microsoft.Office.Core;
using Microsoft.Office.Interop.Outlook;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Office = Microsoft.Office.Core;


namespace FinalFrontier
{
    [ComVisible(true)]
    public class MainRibbon : Office.IRibbonExtensibility
    {
        private Office.IRibbonUI ribbon;
        private Scoring scoring;

        public MainRibbon(Scoring sc)
        {
            scoring = sc;

            if (Directory.Exists(Path.GetTempPath() + "FinalFrontier"))
            {
                foreach (var file in Directory.GetFiles(Path.GetTempPath() + "FinalFrontier"))
                {
                    File.Delete(file);
                }
            }
        }

        #region IRibbonExtensibility-Member

        public string GetCustomUI(string ribbonID)
        {
            return GetResourceText("FinalFrontier.View.MainRibbon.xml");
        }

        #endregion

        #region Menübandrückrufe
        //Erstellen Sie hier Rückrufmethoden. Weitere Informationen zum Hinzufügen von Rückrufmethoden finden Sie unter https://go.microsoft.com/fwlink/?LinkID=271226.

        public void Ribbon_Load(Office.IRibbonUI ribbonUI)
        {
            this.ribbon = ribbonUI;
        }

        #endregion
        public void OnSecInfoClick(IRibbonControl control)
        {
            MailItem selObject;
            if (control.Context is Inspector)
            {
                var item = control.Context as Inspector;
                selObject = item.CurrentItem as MailItem;
            }
            else if (control.Context is Explorer)
            {
                Explorer expl = control.Context as Explorer;
                selObject = expl.Application.ActiveExplorer().Selection[1] as MailItem;
            }
            else
                return;

            // Show the Info
            VMInfoScreen.ShowScore(scoring.getSummary(selObject));
        }

        public void OnShowHeaderClick(IRibbonControl control)
        {
            MailItem selObject;
            if (control.Context is Inspector)
            {
                var item = control.Context as Inspector;
                selObject = item.CurrentItem as MailItem;
            }
            else if (control.Context is Explorer)
            {
                Explorer expl = control.Context as Explorer;
                selObject = expl.Application.ActiveExplorer().Selection[1] as MailItem;
            }
            else
                return;

            // Show the Info
            VMInfoScreen.ShowHeader(scoring.getSummary(selObject));
        }

        public void OnShowSettingsClick(IRibbonControl control)
        {
            // Show the Settings screen
            SettingsScreen settingsSc = new SettingsScreen();
            settingsSc.Show();
        }

        public void OnShowInfoClick(IRibbonControl control)
        {
            // Show the Settings screen
            SettingsScreen settingsSc = new SettingsScreen();
            settingsSc.Show();
        }

        public void OnShowUpdateClick(IRibbonControl control)
        {
            // Show the Settings screen
            SettingsScreen settingsSc = new SettingsScreen();
            settingsSc.Show();
        }

        public void onFFFolderButtonClick(IRibbonControl control)
        {
            MessageBox.Show("TODO: TRIGGER LEARNING!");
        }

        public bool IsVisible(Office.IRibbonControl control)
        {
            //string foldername = ((Outlook.Folder)control.Context).Name;
            return true;
        }

        #region Hilfsprogramme

        private static string GetResourceText(string resourceName)
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            foreach(var resourcename in asm.GetManifestResourceNames())
            {
                if (string.Compare(resourceName, resourcename, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    using (StreamReader resourceReader = new StreamReader(asm.GetManifestResourceStream(resourcename)))
                    {
                        if (resourceReader != null)
                        {
                            return resourceReader.ReadToEnd();
                        }
                    }
                }
            }
            return null;
        }

        #endregion

        ~MainRibbon()
        {
            if (Directory.Exists(Path.GetTempPath() + "FinalFrontier"))
            {
                foreach (var file in Directory.GetFiles(Path.GetTempPath() + "FinalFrontier"))
                {
                    File.Delete(file);
                }
            }
        }
    }
}
