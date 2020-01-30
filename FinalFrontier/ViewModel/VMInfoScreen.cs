using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace FinalFrontier
{
    public class VMInfoScreen : VMStaticBase
    {

        // Method for notifying the view on changes
        #region Static NotifyPropertyChanged

        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;
        
        public static void NotifyStaticPropertyChanged(string propertyName)
        {
            StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        // Initialize public variables used by the XAML-view
        #region variables for view

        // Static texts
        public static string HeaderLabel { get { return "Header-Informationen"; } }
        public static string ProblemLabel { get { return "You have a problem?"; } }
        public static string CloseLabel { get { return "Close"; } }

        // Fields with changes
        public static string ShortInfo { 
            get { if (scoring.IsSuspicious) return "Warnung!"; else return "Detaillierte Information"; } 
            private set { NotifyStaticPropertyChanged("ShortInfo"); } 
        }
        public static string LongInfo
        {
            get { if (scoring.IsSuspicious) return "Diese Mail könnte schadhaft sein."; else return "FinalFrontier stuft diese Mail nicht als bösartig ein."; }
            private set { NotifyStaticPropertyChanged("LongInfo"); }
        }
        public static string ScoreLabel 
        { 
            get { return "Score: " + scoring.Score; } 
            private set { NotifyStaticPropertyChanged("ScoreLabel"); } 
        }
        public static List<CheckResult> DetailedScoreInfo { 
            get { return scoring.DetailedScoreInfo; } 
            private set 
            {
                //SetProperty<List<CheckResult>>(ref detailedScoreInfo, value); //Would be nice but not funtional
                NotifyStaticPropertyChanged("DetailedScoreInfo");
            } 
        }
        public static string Header { 
            get { return scoring.Header; }
            private set { NotifyStaticPropertyChanged("Header"); }
        }

        // Style information
        public static int ScoreHeight {
            get { return scoreHeight;  }
            private set { scoreHeight = value; NotifyStaticPropertyChanged("ScoreHeight"); }
        }
        public static int HeaderHeight
        {
            get { return headerHeight; }
            private set { headerHeight = value; NotifyStaticPropertyChanged("HeaderHeight"); }
        }
        
        #endregion

        // Initialize commands
        #region Command defintions

        public ICommand ShowScoreCommand { get; private set; }
        public ICommand ShowHeaderCommand { get; private set; }
        public ICommand ReportProblemCommand { get; private set; }
        public ICommand CloseCommand { get; private set; }

        #endregion

        // Initialize local variables
        private static InfoScreen infoSc;
        private static ModelScoring scoring;

        private static List<CheckResult> noScoreInfo = new List<CheckResult>();
        private static int scoreHeight;
        private static int headerHeight;

        public VMInfoScreen()
        {            
            // Initialize Observables
            //detailedScoreInfo = new ObservableCollection<CheckResult>(scoring.DetailedScoreInfo);
            //DetailedScoreInfoView = new ListCollectionView(detailedScoreInfo);
            
            // Initialize commands
            ShowScoreCommand = new RelayCommand(ShowScore, null);
            ShowHeaderCommand = new RelayCommand(ShowHeader, null);
            //ReportProblemCommand = new RelayCommand(ReportProblem, null);
            CloseCommand = new RelayCommand(Close, null);
    }

        public static void ShowScore(object obj)
        {
            // May write new scoring
            if (obj is ModelScoring)
                scoring = obj as ModelScoring;
            else if (obj != null)
                throw new ArgumentException("Object must be from type ModelScoring.");

            // Set the visible score
            ScoreHeight = 100;
            HeaderHeight = 0;

            Visualize();
        }

        public static void ShowHeader(object obj)
        {
            // May write new scoring
            if (obj is ModelScoring)
                scoring = obj as ModelScoring;
            else if (obj != null)
                throw new ArgumentException("Object must be from type ModelScoring.");

            // Set the visible header
            ScoreHeight = 0;
            HeaderHeight = 100;

            Visualize();
        }

        private static void Visualize()
        {
            // Update the public variables; only for triggering purposes not the actual values
            ShortInfo = "";
            LongInfo = "";
            ScoreLabel = "";
            DetailedScoreInfo = scoring.DetailedScoreInfo;
            Header = "";

            // Show the window or generate it
            if (infoSc == null)
            {
                infoSc = new InfoScreen { Topmost = true };
                infoSc.Show();
            }
            else
            {
                infoSc.Visibility = Visibility.Visible;
                infoSc.Focus();
             }
        }

        public static void Close(Object obj = null)
        {
            infoSc.Visibility = Visibility.Collapsed;
        }

        //public void ShowScore(Object sender = null, RoutedEventArgs e = null)
        //{
        //    // Update the heights of the windows


        //    headerHeight = HeaderInfo.ActualHeight;
        //    HeaderInfo.Height = HeaderInfo.MinHeight;

        //    if (scoreHeight > 0)
        //        ScoreInfo.Height = scoreHeight;
        //}

        //private void ShowHeader(Object sender = null, RoutedEventArgs e = null)
        //{
        //    // Update the heights of the windows



        //    scoreHeight = ScoreInfo.ActualHeight;
        //    ScoreInfo.Height = ScoreInfo.MinHeight;

        //    if (headerHeight > 0)
        //        HeaderInfo.Height = headerHeight;
        //}

        //private void WriteInformation()
        //{
        //    // Get the top-displayed information
        //    if (Ana.IsSuspicious == true)
        //    {
        //        ShortInfo.Content = "Warnung!";
        //        LongInfo.Content = "Diese Mail könnte schadhaft sein.";
        //    }
        //    else
        //    {
        //        ShortInfo.Content = "Detaillierte Informationen";
        //        LongInfo.Content = "FinalFrontier stuft diese Mail nicht als bösartig ein.";
        //    }

        //    // Set the score iboard with detailed list
        //    ScoreLabel.Content = "Score: " + Ana.Score.ToString();

        //    if (scoreList.Items.Count < Ana.Result.Count())
        //    {
        //        foreach (CheckResult cr in Ana.Result)
        //            scoreList.Items.Add(new CheckResult(cr.id, cr.fragment, cr.ioc, cr.score));
        //    }
        //    else if (Ana.Result.Count() == 0)
        //        scoreList.Items.Add(new CheckResult("", "E-Mail vermutlich nicht schadhaft.", "Keine IOCs gefunden.", 0));


        //    Header = Ana.Header;

        // Set the header information
        //if (detailedHeader.Content == null)
        //    detailedHeader.Content = Ana.Header;
        //}
    }
}
