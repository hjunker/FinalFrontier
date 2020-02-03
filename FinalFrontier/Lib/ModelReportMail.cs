using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalFrontier
{
    public class ModelReportMail
    {
        // Initialize variables with getter, setter
        public string DetailedText { get; private set; }
        public string Subject { get; private set; }
        public bool IncludeMeta { get; private set; }

        // Constructor
        public ModelReportMail(string detailText, string subject, bool includeMeta)
        {
            DetailedText = detailText;
            Subject = subject;
            IncludeMeta = includeMeta;
        }
    }
}
