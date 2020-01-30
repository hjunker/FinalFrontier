using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace FinalFrontier
{
    public class VMInfoScreen
    {
        // Initialize public variables used by the XAML-view
        public static string ShortInfo { get { if (scoring.IsSuspicious) return "Warnung!"; else return "Detaillierte Information"; } }
        public static string LongInfo { get { if (scoring.IsSuspicious) return "Diese Mail könnte schadhaft sein."; else return "FinalFrontier stuft diese Mail nicht als bösartig ein."; } }

        public static string ScoreLabel { get { return "Score: " + scoring.Score; } }
        public static List<CheckResult> DetailedScoreInfo
        {
            get
            {
                if (scoring.DetailedScoreInfo.Count() > 0)
                    return scoring.DetailedScoreInfo;
                else
                {
                    noScoreInfo.Clear();
                    noScoreInfo.Add((new CheckResult("", "E-Mail vermutlich nicht schadhaft.", "Keine IOCs gefunden.", 0)));
                    return noScoreInfo;
                }
            }
        }

        public static string HeaderLabel { get { return "Header-Informationen"; } }
        public static string Header { get { return scoring.Header; } }

        public static string ProblemLabel { get { return "You have a problem?"; } }
        public static string CloseLabel { get { return "Close"; } }

        // Init commands
        public ICommand ShowScoreCommand { get; private set; }
        public ICommand ShowHeaderCommand { get; private set; }
        public ICommand CloseCommand { get; private set; }

        // Initialize local variables
        private static ModelScoring scoring;
        private static InfoScreen infoSc;

        private static List<CheckResult> noScoreInfo = new List<CheckResult>();

        public VMInfoScreen()
        {
        }

        public static void ShowScore(ModelScoring sc = null)
        {
            if (sc != null)
                scoring = sc;

            Visualize();
        }

        public static void ShowHeader(ModelScoring sc = null)
        {
            if (sc != null)
                scoring = sc;
        }

        private static void Visualize()
        {
            infoSc = new InfoScreen();
            infoSc.Show();
        }

        private void Close(Object sender, RoutedEventArgs e)
        {
            infoSc.Close();
        }

        //public static void Show(string showItem = "")
        //{
        //    if (showItem.Normalize().Equals("score") || showItem.Equals(""))
        //        //ShowScore();
        //        Debug.WriteLine("Test score");
        //    else if (showItem.Normalize().Equals("header"))
        //        //ShowHeader();
        //        Debug.WriteLine("Test header");
        //    else
        //        throw new ArgumentException("Invalid argument to show. Only use 'score' or 'header' (case-insensitive).", showItem);
        //}

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
