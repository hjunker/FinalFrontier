using System.Collections.Generic;

namespace FinalFrontier
{
    public abstract class AnalyzerBase
    {
        public abstract int Score { get; }

        public abstract List<CheckResult> Analyze(object data);
    }
}
