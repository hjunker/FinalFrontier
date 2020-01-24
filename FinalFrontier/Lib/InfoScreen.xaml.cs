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
    public partial class InfoScreen : Window
    {
        public int Score {get; private set;}
        public List<CheckResult> DetailedInfo { get; private set; }
        public string Header { get; private set; }
        
        private double scoreHeight = 0;

        public InfoScreen(Analyzer ana)
        {
            Score = ana.Score;
            DetailedInfo = ana.Result;
            Header = ana.Header;
            
            InitializeComponent();
            
            ScoreLabel.Content = "Score: " + ana.Score.ToString();

            ShowHeader();
            ShowScore();

        }

        private void Close(Object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ShowScore(Object sender=null, RoutedEventArgs e=null)
        {
            HeaderInfo.Height = MinHeight;
            if (scoreHeight > 0)
                ScoreInfo.Height = scoreHeight;

            // Set the detailed list of iocs
            if (scoreList.Items.Count < DetailedInfo.Count()) {
                foreach (CheckResult cr in DetailedInfo)
                    scoreList.Items.Add(new CheckResult(cr.id, cr.fragment, cr.ioc, cr.score));
            }
        }

        private void ShowHeader(Object sender=null, RoutedEventArgs e=null)
        {
            scoreHeight = ScoreInfo.ActualHeight;
            
            ScoreInfo.Height = ScoreInfo.MinHeight;
            HeaderInfo.MaxHeight = 500;

            detailedHeader.Content = Header;
        }
    }
}
