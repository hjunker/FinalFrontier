using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalFrontier
{
    public abstract class BaseAnalyse
    {
        public abstract int Score { get; }

        public abstract List<CheckResult> Analyze(object data);
    }
}
