using Microsoft.Office.Interop.Outlook;
using System;
using System.Collections.Generic;
using System.Diagnostics;


namespace FinalFrontier
{
    public class Scoring
    {
        private Dictionary<string, ModelScoring> checkedMails;

        // Constructors
        public Scoring()
        {
            checkedMails = new Dictionary<string, ModelScoring>();
        }

        // Calculating method
        public ModelScoring getSummary(MailItem mailItem)
        {
            // Return if there was a calculation before
            if (checkedMails.ContainsKey(mailItem.EntryID))
                return checkedMails[mailItem.EntryID];

            var CheckResults = new List<CheckResult>();

            AnalyzerBody bodyAnalyse = new AnalyzerBody();
            AnalyzerAttachement attachmentAnalyze = new AnalyzerAttachement();
            AnalyzerMeta metaAnalyze = new AnalyzerMeta();

            Action<List<CheckResult>> addRange = x =>
            {
                if (x != null)
                {
                    x.RemoveAll(y => y == null);
                    CheckResults.AddRange(x);
                }
            };

            addRange(bodyAnalyse.Analyze(mailItem?.HTMLBody));
            addRange(attachmentAnalyze.Analyze(mailItem?.Attachments));
            addRange(metaAnalyze.Analyze(mailItem));

            Debug.WriteLine("---CHECK RESULTS---");
            foreach (CheckResult cr in CheckResults)
                Debug.WriteLine(cr);
            Debug.WriteLine("---END CHECK RESULTS---");

            // Write a new model in checked List and return it
            checkedMails.Add(mailItem.EntryID, new ModelScoring(bodyAnalyse.Score + attachmentAnalyze.Score + metaAnalyze.Score, CheckResults, mailItem.HeaderString()));

            return checkedMails[mailItem.EntryID];
        }
    }
}
