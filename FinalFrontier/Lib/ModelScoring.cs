using System.Collections.Generic;
using System.Configuration;


namespace FinalFrontier
{
    public class ModelScoring
    {
        // Initialize public variables with getter, setter
        public bool IsSuspicious => Score <= ModelConfiguration.Instance.SuspiciousScore;
        public int Score { get; private set; }
        public List<CheckResult> DetailedScoreInfo { get; private set; }
        public string Header { get; private set; }
        public string MailitemEntryID { get; private set; }

        // Constructor
        public ModelScoring(int sc, List<CheckResult> scoreRes, string head, string mailItemEntryID)
        {
            Score = sc;
            DetailedScoreInfo = scoreRes;
            Header = head;
            MailitemEntryID = mailItemEntryID;
        }
    }
}
