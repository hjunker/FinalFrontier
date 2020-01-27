using Microsoft.Office.Interop.Outlook;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;

namespace FinalFrontier
{
    public class Analyzer
    {
        private Dictionary<string, List<CheckResult>> checkedMails;

        // Define public variables with the results from scoring
        public bool IsSuspicious => Score <= int.Parse(ConfigurationManager.AppSettings["isSuspiciousScore"]);
        public int Score { get; private set; }
        public List<CheckResult> Result { get; private set; }
        public string Header { get; private set; }
        
        // Constructors
        public Analyzer()
        {
            checkedMails = new Dictionary<string, List<CheckResult>>();

            // Initialize default values for scoring
            Score = 0;
            Result = new List<CheckResult>();
            Header = "";
        }

        // TODO: Add Constructor with mailitem to save the result in the instance

        // Calculating methods
        public List<CheckResult> getSummary(MailItem mailItem)
        {
            if (checkedMails.ContainsKey(mailItem.EntryID))
            {
                Score = checkedMails[mailItem.EntryID].Sum(x => x.score);
                Result = checkedMails[mailItem.EntryID];
                Header = mailItem.HeaderString();

                return checkedMails[mailItem.EntryID];
            }
            var CheckResults = new List<CheckResult>();

            BodyAnalyser bodyAnalyse = new BodyAnalyser();
            AttachmentAnalyzer attachmentAnalyze = new AttachmentAnalyzer();
            MetaAnalyzer metaAnalyze = new MetaAnalyzer();

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

            // Write instance variables for later use
            Header = mailItem.HeaderString();
            Result = CheckResults;
            Score = bodyAnalyse.Score + attachmentAnalyze.Score + metaAnalyze.Score;

            if (CheckResults.Count > 0)
                checkedMails.Add(mailItem.EntryID, CheckResults);

            return CheckResults;
        }
    }
}
