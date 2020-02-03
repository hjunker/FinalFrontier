using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace FinalFrontier
{
    public class VMProblemScreen : VMBase
    {
        // Initialize public variables used by the XAML-view
        #region variables for view

        // Static texts
        public static string ShortInfo { get { return "Feedback"; } }
        public static string LongInfo { get { return "Let us know your problem or any ideas."; } }
        public static string FPLabel { get { return "This Mail is not bad."; } }
        public static string BugLabel { get { return "I've discovered a bug."; } }
        public static string FeatureLabel { get { return "I have a new feature idea."; } }
        public static string SendMeta { get { return "Include the mail and results of checks."; } }
        public static string SendLabel { get { return "Send"; } }
        public static string CloseLabel { get { return "Abort"; } }

        // Fields with changes
        public string MailReport
        {
            get { return detailedText; }
            set { SetProperty(ref detailedText, value); }
        }
        public bool IncludeMail
        {
            get { return includeMail; }
            set { SetProperty(ref includeMail, value); }
        }

        // Style information
        public Visibility MailReportHeight
        {
            get { return mailReportHeight; }
            private set { SetProperty(ref mailReportHeight, value); }
        }
        public Visibility BugReportHeight
        {
            get { return bugReportHeight; }
            private set { SetProperty(ref bugReportHeight, value); }
        }
        public Visibility FeatureReportHeight
        {
            get { return featureReportHeight; }
            private set { SetProperty(ref featureReportHeight, value); }
        }

        #endregion

        // Initialize commands
        #region Command defintions

        public ICommand ReportMailCommand { get; private set; }
        public ICommand ReportBugCommand { get; private set; }
        public ICommand ReportFeatureCommand { get; private set; }
        public ICommand SendCommand { get; private set; }
        public ICommand CloseCommand { get; private set; }

        #endregion

        // Initialize local variables
        private string report;

        private string detailedText = "Put the details here.";
        private string subject;
        private bool includeMail = true;
        private Visibility mailReportHeight = Visibility.Collapsed;
        private Visibility bugReportHeight = Visibility.Collapsed;
        private Visibility featureReportHeight = Visibility.Collapsed;

        public VMProblemScreen()
        {
            // Initialize commands
            ReportMailCommand = new RelayCommand(ReportFP, null);
            ReportBugCommand = new RelayCommand(ReportBug, null);
            ReportFeatureCommand = new RelayCommand(ReportFeature, null);
            SendCommand = new RelayCommand(Send, null);
            CloseCommand = new RelayCommand(Close, null);
        }

        public void ReportFP(Object obj = null)
        {
            report = "fp";
            
            MailReportHeight = Visibility.Visible;
            BugReportHeight = Visibility.Collapsed;
            FeatureReportHeight = Visibility.Collapsed;
            subject = "[FALSE POSITIVE]";
        }

        public void ReportBug(Object obj = null)
        {
            report = "bug";
            
            MailReportHeight = Visibility.Collapsed;
            BugReportHeight = Visibility.Visible;
            FeatureReportHeight = Visibility.Collapsed;
            subject = "[BUG]";
        }

        public void ReportFeature(Object obj = null)
        {
            report = "feature";
            
            MailReportHeight = Visibility.Collapsed;
            BugReportHeight = Visibility.Collapsed;
            FeatureReportHeight = Visibility.Visible;
            subject = "[FEATURE]";
        }

        public void Send(Object obj = null)
        {
            ReportMail reportMail = new ReportMail(new ModelReportMail(detailedText, subject, includeMail), report);
            Close(obj);
        }

        public void Close(Object obj = null)
        {
            (obj as Window).Close();
        }
    }
}
