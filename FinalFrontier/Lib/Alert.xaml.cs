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
    /// Interaktionslogik für Alert.xaml
    /// </summary>
    public partial class Alert : Window
    {
        public Alert(int score)
        {
            InitializeComponent();
            scoreLabel.Content = "Score: " + score.ToString();
        }

        private void Close(Object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ShowHeader(Object sender, RoutedEventArgs e)
        {
            // TODO
            int a = 1;
        }
    }
}
