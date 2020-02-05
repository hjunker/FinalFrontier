using System.Windows;


namespace FinalFrontier
{
    /// <summary>
    /// Interaktionslogik für ProblemScreen.xaml
    /// </summary>
    public partial class ProblemScreen : Window
    {
        public ModelScoring scoring { get; private set; }
        public ProblemScreen(ModelScoring sc)
        {
            scoring = sc;
            InitializeComponent();
        }
    }
}
