using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FinalFrontier
{
    /// <summary>
    /// Interaktionslogik für InfoScreen.xaml
    /// </summary>
    public partial class InfoScreen2 : Window
    {
        // Initialize public variables used by the XAML-view
        public ModelScoring Ana { get; private set; }
        
        // Initialize some instance variables
        private double scoreHeight = 0;
        private double headerHeight = 0;

        public InfoScreen2(ModelScoring ana, string showItem="")
        {
            Ana = ana;

            // Set the basic view
            InitializeComponent();
            ScoreLabel.Content = "Score: " + ana.Score.ToString();

            // Initialize the specific view fields
            if (showItem.Normalize().Equals("score") || showItem.Equals(""))
                ShowScore();
            else if (showItem.Normalize().Equals("header"))
                ShowHeader();
            else
                throw new ArgumentException("Invalid argument to show. Only use 'score' or 'header' (case-insensitive).", showItem);
        }

        private void Close(Object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ShowScore(Object sender=null, RoutedEventArgs e=null)
        {
            // Get the top-displayed information
            if (Ana.IsSuspicious == true)
            {
                ShortInfo.Content = "Warnung!";
                LongInfo.Content = "Diese Mail könnte schadhaft sein.";
            } else
            {
                ShortInfo.Content = "Detaillierte Informationen";
                LongInfo.Content = "FinalFrontier stuft diese Mail nicht als bösartig ein.";
            }

            // Set the detailed list of iocs
            if (scoreList.Items.Count < Ana.DetailedScoreInfo.Count()) {
                foreach (CheckResult cr in Ana.DetailedScoreInfo)
                    scoreList.Items.Add(new CheckResult(cr.id, cr.fragment, cr.ioc, cr.score));
            } else if (Ana.DetailedScoreInfo.Count() == 0)
                scoreList.Items.Add(new CheckResult ("", "E-Mail vermutlich nicht schadhaft.", "Keine IOCs gefunden.", 0));

            // Update the heights of the windows
            headerHeight = HeaderInfo.ActualHeight;
            HeaderInfo.Height = HeaderInfo.MinHeight;
            
            if (scoreHeight > 0)
                ScoreInfo.Height = scoreHeight;
        }

        private void ShowHeader(Object sender=null, RoutedEventArgs e=null)
        {
            // Set the header information
            if (detailedHeader.Content == null)
                detailedHeader.Content = Ana.Header;

            // Update the heights of the windows
            scoreHeight = ScoreInfo.ActualHeight;
            ScoreInfo.Height = ScoreInfo.MinHeight;

            if (headerHeight > 0)
                HeaderInfo.Height = headerHeight;
        }
    }
}
