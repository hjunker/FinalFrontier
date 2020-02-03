using System;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.Office.Interop.Outlook;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace FinalFrontier
{
    public class ReportMail
    {
        public static Explorer Explorer { get; set; }

        public ReportMail(ModelReportMail repData, string reportType)
        {
            // Get given data
            string reportAddress = ConfigurationManager.AppSettings["reportAddress"];

            //// Create new E Mail
            MailItem mail = (MailItem)Explorer.Application.CreateItem(OlItemType.olMailItem);
            mail.To = reportAddress;
            mail.Subject = repData.Subject;
            mail.Body = repData.DetailedText;

            // Attach the selected mail and score results if necessary
            if(reportType.Equals("fp") && repData.IncludeMeta)
            {
                // TODO

                // Get problematic mail and generate attachements
                //MailItem selMail = Explorer.S as MailItem;


                //control.Context is Inspector
                //var item = Control.Context as Inspector;
                //selObject = item.CurrentItem as MailItem;


                //mail.Attachments.Add()
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
