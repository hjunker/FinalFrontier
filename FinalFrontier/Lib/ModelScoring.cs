using System.Collections.Generic;
using System.Configuration;


namespace FinalFrontier
{
    public class ModelScoring : ModelBase
    {
        // Initialize public variables with getter, setter
        public bool IsSuspicious => score <= int.Parse(ConfigurationManager.AppSettings["isSuspiciousScore"]);
        public int Score { get { return score; } set { SetProperty(ref score, value); } }
        public List<CheckResult> DetailedScoreInfo { get { return detailedScoreInfo; } set { SetProperty(ref detailedScoreInfo, value); } }
        public string Header { get { return header; } set { SetProperty(ref header, value); } }

        // Initialize private fields for saving
        private int score;
        private List<CheckResult> detailedScoreInfo;
        private string header;

        // Constructor
        public ModelScoring(int sc, List<CheckResult> scoreRes, string head)
        {
            Score = sc;
            DetailedScoreInfo = scoreRes;
            Header = head;
        }
    }
}
