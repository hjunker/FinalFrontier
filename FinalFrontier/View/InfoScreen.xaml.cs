using System.ComponentModel;
using System.Windows;


namespace FinalFrontier
{
    /// <summary>
    /// Interaktionslogik für InfoScreen.xaml
    /// </summary>
    public partial class InfoScreen : Window
    { 
        public InfoScreen()
        {
            InitializeComponent();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            Visibility = Visibility.Collapsed;
        }
    }
}
