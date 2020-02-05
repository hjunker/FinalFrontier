using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
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

        // Initialize Event handler for getting actual height and width of objects
        #region size changed event handler

        private static void sizeChanged(object sender, SizeChangedEventArgs e)
        {
            Resize(sender, e.NewSize.Height, e.NewSize.Width);
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
        public static double ScoreMinHeight {
            get { return scoreMinHeight;  }
            set { scoreMinHeight = value; NotifyStaticPropertyChanged("ScoreMinHeight"); }
        }
        public static double ScoreMaxHeight
        {
            get { return scoreMaxHeight; }
            set { scoreMaxHeight = value; NotifyStaticPropertyChanged("ScoreMaxHeight"); }
        }
        public static double HeaderMinHeight
        {
            get { return headerMinHeight; }
            private set { headerMinHeight = value; NotifyStaticPropertyChanged("HeaderMinHeight"); }
        }
        public static double HeaderMaxHeight
        {
            get { return headerMaxHeight; }
            private set { headerMaxHeight = value; NotifyStaticPropertyChanged("HeaderMaxHeight"); }
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

        private static double scoreMinHeight;
        private static double scoreMaxHeight = 10000;
        private static double headerMinHeight;
        private static double headerMaxHeight = 10000;
        private static bool resizeFields;
        private static double viewMainInfoField;
        private static double viewScoreHeight;
        private static double viewHeaderHeight;
        private static double viewButtonSwitchHeight;

        public VMInfoScreen()
        {            
            // Initialize commands
            ShowScoreCommand = new RelayCommand(ShowScore, null);
            ShowHeaderCommand = new RelayCommand(ShowHeader, null);
            ReportProblemCommand = new RelayCommand(ReportProblem, null);
            CloseCommand = new RelayCommand(Close, null);
    }

        public static void ShowScore(object obj)
        {
            // May write new scoring
            if (obj is ModelScoring)
            {
                scoring = obj as ModelScoring;
            }
            else if (obj != null)
                throw new ArgumentException("Object must be from type ModelScoring.");

            Visualize();

            // Set the visible score
            ScoreMaxHeight = viewScoreHeight;
            ScoreMinHeight = viewScoreHeight;
            HeaderMinHeight = 0;
            HeaderMaxHeight = viewHeaderHeight - viewScoreHeight;
        }

        public static void ShowHeader(object obj)
        {
            // May write new scoring
            if (obj is ModelScoring)
                scoring = obj as ModelScoring;
            else if (obj != null)
                throw new ArgumentException("Object must be from type ModelScoring.");

            Visualize();

            // Set the visible header
            ScoreMinHeight = 0;
            ScoreMaxHeight = 0;
            HeaderMinHeight = viewHeaderHeight;
            HeaderMaxHeight = viewHeaderHeight;
        }

        private static void Visualize()
        {
            // Activate resizing because of new content
            resizeFields = true;
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

                // Eventhandler for sizing
                infoSc.DScInfoView.SizeChanged += new SizeChangedEventHandler(sizeChanged);
                infoSc.HeaderInfoView.SizeChanged += new SizeChangedEventHandler(sizeChanged);
                infoSc.MainInfoField.SizeChanged += new SizeChangedEventHandler(sizeChanged);
                infoSc.ButtonSwitch.SizeChanged += new SizeChangedEventHandler(sizeChanged);

                infoSc.Show();
            }
            else
            {
                infoSc.Visibility = Visibility.Visible;
                infoSc.Focus();
            }
            //Deactivate resizing thorug clicking
            resizeFields = false;
        }

        public static void ReportProblem(Object obj = null)
        {
            ProblemScreen report = new ProblemScreen(scoring) { Topmost = true };
            report.Show();
        }

        public static void Close(Object obj = null)
        {
            infoSc.Visibility = Visibility.Collapsed;
        }

        private static void Resize(object sender, double height, double width)
        {
            // Do nothing if resizing not necessary
            if (!resizeFields)
                return;

            // Update view height variables
            if (sender.ToString().StartsWith("System.Windows.Controls.TextBox"))
                viewHeaderHeight = height;
            else if (sender.ToString().StartsWith("System.Windows.Controls.ListView"))
                viewScoreHeight = height;
            else if (sender.ToString().StartsWith("System.Windows.Controls.Button"))
                viewButtonSwitchHeight = height;
            else if (sender.ToString().StartsWith("System.Windows.Controls.Grid"))
                viewMainInfoField = (sender as System.Windows.Controls.Grid).DesiredSize.Height;
            else
                throw new ArgumentException("This window cannot be resized by this function.");

            // Resize them if something is too big
            viewHeaderHeight = Math.Min(viewHeaderHeight, viewMainInfoField - 2*viewButtonSwitchHeight);
        }
    }
}
