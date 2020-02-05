using System;
using System.Configuration;
using Microsoft.Office.Interop.Outlook;
using System.Linq;
using System.Xml.Linq;
using System.IO;


namespace FinalFrontier
{
    public class ReportMail
    {
        public static Application OutlookApp { get; set; }

        public ReportMail(ModelReportMail repData, string reportType)
        {
            //// Create new E Mail
            MailItem mail = (MailItem)OutlookApp.CreateItem(OlItemType.olMailItem);
            mail.To = ModelConfiguration.Instance.ReportAddress;
            mail.Subject = repData.Subject;
            mail.Body = repData.DetailedText;

            // Attach the selected mail and score results if necessary
            if(repData.Scoring != null)
            { 
                // Generate XML with detailed infos
                XElement scoreDetails = new XElement("ScoreDetails",
                    (from CheckResult in repData.Scoring.DetailedScoreInfo
                        select new XElement("Result",
                            new XElement("Score", CheckResult.score),
                            new XElement("Id", CheckResult.id),
                            new XElement("Fragment", CheckResult.fragment),
                            new XElement("Ioc", CheckResult.ioc))
                    )
                );
                // Get the problematic mail 
                MailItem attMail = (MailItem)OutlookApp.GetNamespace("MAPI").GetItemFromID(repData.Scoring.MailitemEntryID);

                var tmpPath = Path.GetTempPath() + "FinalFrontier\\";

                if (!Directory.Exists(tmpPath))
                    Directory.CreateDirectory(tmpPath);
                tmpPath += Path.GetRandomFileName();
                var tmpPath2 = tmpPath + "1";
                
                scoreDetails.Save(tmpPath + ".xml");
                attMail.SaveAs(tmpPath2);

                // Generate attachements
                mail.Attachments.Add(tmpPath);
                mail.Attachments.Add(tmpPath2);
            }

            mail.Send();


            switch (reportType)
            {
                case "fp":
                    // Later use
                    break;
                case "bug":
                    // Later use
                    break;
                case "feature":
                    // Later use
                    break;
                default:
                    throw new ArgumentException("This type of report is not supportet. Only use 'fp', 'bug' oder'feature'.");
            }
        }
    }
}
