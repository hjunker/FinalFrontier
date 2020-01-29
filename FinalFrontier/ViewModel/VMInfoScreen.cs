using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FinalFrontier
{
    public static class VMInfoScreen
    {
        // Initialize public variables used by the XAML-view
        public static string ShortInfo { get; private set; }
        public static string LongInfo { get; private set; }

        public static string ScoreLabel { get; private set; }
        public static List<CheckResult> DetailedScoreInfo { get; private set; }

        public static string Header { get; private set; }

        // Initialize some instance variables
        //private Scoring ana;

        //public VMInfoScreen()
        //{

        //}
        
        //public VMInfoScreen(Scoring ana)
        //{
        //    // Get the top-displayed information
        //    if (ana.IsSuspicious == true)
        //    {
        //        ShortInfo = "Warnung!";
        //        LongInfo = "Diese Mail könnte schadhaft sein.";
        //    }
        //    else
        //    {
        //        ShortInfo = "Detaillierte Informationen";
        //        LongInfo = "FinalFrontier stuft diese Mail nicht als bösartig ein.";
        //    }

        //    // Set the score board with detailed list
        //    ScoreLabel = "Score: " + ana.Score;

        //    if (DetailedScoreInfo.Count() < ana.Result.Count())
        //        DetailedScoreInfo.AddRange(ana.Result);
        //    else if (ana.Result.Count() == 0)
        //        DetailedScoreInfo.Add(new CheckResult("", "E-Mail vermutlich nicht schadhaft.", "Keine IOCs gefunden.", 0));

        //    //Set the header information
        //    if (Header == null)
        //        Header = ana.Header;
        //}



        public static void Show(string showItem = "")
        {
            if (showItem.Normalize().Equals("score") || showItem.Equals(""))
                //ShowScore();
                Debug.WriteLine("Test score");
            else if (showItem.Normalize().Equals("header"))
                //ShowHeader();
                Debug.WriteLine("Test header");
            else
                throw new ArgumentException("Invalid argument to show. Only use 'score' or 'header' (case-insensitive).", showItem);
        }

        public static void ShowScore(ModelScoring ana)
        {
            //DO
        }

        public static void ShowHeader(ModelScoring ana)
        {
            //DO
        }

        //private void Close(Object sender, RoutedEventArgs e)
        //{
        //    Close();
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
