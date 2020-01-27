using Microsoft.Office.Interop.Outlook;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
 
namespace FinalFrontier
{
    public class Analyzer
    {
        private DictionaryTools dt;
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
        private const string HeaderRegex = @"^(?<header_key>[-A-Za-z0-9]+)(?<seperator>:[ \t]*)" +
            "(?<header_value>([^\r\n]|\r\n[ \t]+)*)(?<terminator>\r\n)";


        // Define public variables with the results from scoring
        public bool IsSuspicious => Score <= int.Parse(ConfigurationManager.AppSettings["isSuspiciousScore"]);
        public int Score { get; private set; }
        public List<CheckResult> Result { get; private set; }
        public string Header { get; private set; }

        
        // Constructors
        public Analyzer()
        {

            // Initialize default values for scoring
            Score = 0;
            Result = new List<CheckResult>();
            Header = "";
        }

        // TODO: Add Constructor with mailitem to save the result in the instance


        // Calculating methods
        public List<CheckResult> getSummary(MailItem mailItem)
        {
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

            return CheckResults;
        }
    }
}
