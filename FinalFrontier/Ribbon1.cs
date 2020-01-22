﻿using Microsoft.Office.Core;
using Microsoft.Office.Interop.Outlook;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Office = Microsoft.Office.Core;
using Outlook = Microsoft.Office.Interop.Outlook;

// TODO: It is not helpful to have finalfrontier have an indicator or button in the add-in tab
// we need some kind of clear and always-visible visual aid
// but this may be out of bounds regarding to what is technically possible since MS does not want add-ins to change ui look & feel
// maybe this could be helpful: https://www.add-in-express.com/add-in-net/outlook-regions.php (e.g. lets you add custom navigation pane regions by using Forms)


namespace FinalFrontier
{
    [ComVisible(true)]
    public class Ribbon1 : Office.IRibbonExtensibility
    {
        private Office.IRibbonUI ribbon;

        public Ribbon1()
        {
        }

        #region IRibbonExtensibility-Member

        public string GetCustomUI(string ribbonID)
        {
            return GetResourceText("FinalFrontier.Ribbon1.xml");
        }

        #endregion

        #region Menübandrückrufe
        //Erstellen Sie hier Rückrufmethoden. Weitere Informationen zum Hinzufügen von Rückrufmethoden finden Sie unter https://go.microsoft.com/fwlink/?LinkID=271226.

        public void Ribbon_Load(Office.IRibbonUI ribbonUI)
        {
            this.ribbon = ribbonUI;
        }

        #endregion
        public void onFFButtonClick(IRibbonControl control)
        {
            MailItem selObject;
            if (control.Context is Inspector)
            {
                var item = control.Context as Inspector;
                selObject = item.CurrentItem as MailItem;
            }
            /*Object owner = getApplication().ActiveExplorer();
            String controlId = control.getId();
            MessageBox.show(owner, "hello", "Button " + controlId + " clicked", null);
            */
            else if (control.Context is Explorer)
            {
                Explorer expl = control.Context as Explorer;
                selObject = expl.Application.ActiveExplorer().Selection[1] as MailItem;
                //MailItem selObject = (control.Context as Explorer) as MailItem;
            }
            else
                return;
            Analyzer ana = new Analyzer();
            //String itemMessage = ana.getSummary(selObject);
            String itemMessage = "TODO: ANPASSUNG NACH REFACTORING - AUSGABE AUS LIST<> ERZEUGEN!!!";
            if (ana.isSuspicious == true)
            {
                // TODO: aufräumen
                MessageBox.Show(ana.Result, "FinalFrontier - Warnung: Email könnte schadhaft sein!!!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                //Alert scAlert = new Alert(ana.Score);
                //scAlert.Show();
            }
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
            string[] resourceNames = asm.GetManifestResourceNames();
            for (int i = 0; i < resourceNames.Length; ++i)
            {
                if (string.Compare(resourceName, resourceNames[i], StringComparison.OrdinalIgnoreCase) == 0)
                {
                    using (StreamReader resourceReader = new StreamReader(asm.GetManifestResourceStream(resourceNames[i])))
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
    }
}
