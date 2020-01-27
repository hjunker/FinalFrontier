using Microsoft.Office.Interop.Outlook;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FinalFrontier
{
    public class AttachmentAnalyzer : BaseAnalyse
    {
        private int score;
        public override int Score { get { return score; } }

        public override List<CheckResult> Analyze(object data)
        {
            var attachments = data as Attachments;
            var results = new List<CheckResult>();

            CheckMethods checkMethods = new CheckMethods();

            Action<List<CheckResult>> addRange = x =>
            {
                if (x != null)
                {
                    x.RemoveAll(y => y == null);
                    results.AddRange(x);
                }
            };

            if (attachments == null)
                return null;

            foreach (Attachment attachment in attachments)
            {
                addRange(checkMethods.CheckDoubleExtensions("Attachment-DoubleExtensions", attachment.FileName));

                addRange(checkMethods.CheckBadExtensions("Attachment-BadExtension", attachment.FileName));

                addRange(checkMethods.CheckKeywords("Attachment-Keyword", attachment.FileName));

                addRange(checkMethods.CheckBadHashes("Attachment-FileHash", attachment));
            }

            score = results.Sum(x => x.score);

            return results;
        }
    }
}
