using System.Windows;
using System.Windows.Controls;

namespace FinalFrontier
{
    /// <summary>
    /// Interaktionslogik für WelcomeScreen.xaml
    /// </summary>
    public partial class WelcomeScreen : Window
    {
        public WelcomeScreen()
        {           
            typeof(GridViewRowPresenter).GetField("_defalutCellMargin", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.GetField).SetValue(null, new Thickness(0));
            InitializeComponent();
        }
    }
}
