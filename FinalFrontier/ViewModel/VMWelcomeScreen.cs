using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace FinalFrontier
{
    class VMWelcomeScreen : VMBase
    {
        // Initialize public variables used by the XAML-view
        #region variables for view

        // Fields with changes
        public string MainButtonText
        {
            get { return mainButtonText; }
            set { SetProperty(ref mainButtonText, value); }
        }
        public string SecondButtonText
        {
            get { return secondButtonText; }
            set { SetProperty(ref secondButtonText, value); }
        }
        public ObservableCollection<ModelEMail> MailAddresses
        {
            get { return mailAddresses; }
            set { SetProperty(ref mailAddresses, value); }
        }
        public ModelMailFolder MailFolders
        {
            get { return mailFolders; }
            set { SetProperty(ref mailFolders, value); }
        }
        public string[] SelectableLearning
        {
            get {  string[] x = { Properties.Resources.WELCOMESC_LearningIterative, Properties.Resources.WELCOMESC_LearningRegulary, Properties.Resources.WELCOMESC_LearningNo}; return x; }
        }
        public string[] SelectableLearningTimeInterval
        {
            get { string[] x = { Properties.Resources.WELCOMESC_LearningConfTimeDay, Properties.Resources.WELCOMESC_LearningConfTimeWeek, Properties.Resources.WELCOMESC_LearningConfTimeMonth}; return x; }
        }
        public string[] SelectableWarnNiveau
        {
            get { string[] x = { Properties.Resources.WELCOMESC_LearningConfWarnLow, Properties.Resources.WELCOMESC_LearningConfWarnNormal, Properties.Resources.WELCOMESC_LearningConfWarnHigh }; return x; }
        }
        public string SelectedLearning
        {
            get { return selectedLearning; }
            set
            {
                SetProperty(ref selectedLearning, value);
                LearningSelectionWarning = value != null ? (value.Equals(Properties.Resources.WELCOMESC_LearningNo) ? true : false) : false;
                LearningConfTimeInterval = value != null ? (value.Equals(Properties.Resources.WELCOMESC_LearningRegulary) ? Visibility.Visible : Visibility.Collapsed) : Visibility.Collapsed;
            }
        }
        public string SelectedWarnNiveau
        {
            get { return selectedWarnNiveau; }
            set { SetProperty(ref selectedWarnNiveau, value); }
        }
        public string LearningTimeInterval
        {
            get { return learningTimeInterval; }
            set { SetProperty(ref learningTimeInterval, value); }
        }
        public string LearningTimeIntervalNumber
        {
            get { return learningTimeIntervalNumber; }
            set{
                int x;
                IsCorrectInput = Int32.TryParse(value, out x) ? true : false;
                SetProperty(ref learningTimeIntervalNumber, value);}
        }


        // Style information
        public Visibility InfoVisibility
        {
            get { return infoVisibility; }
            private set { SetProperty(ref infoVisibility, value); }
        }
        public Visibility ConfigVisibility
        {
            get { return configVisibility; }
            private set { SetProperty(ref configVisibility, value); }
        }
        public Visibility DevVisibility
        {
            get { return devVisibility; }
            private set { SetProperty(ref devVisibility, value); }
        }
        public Visibility HelpMailTextVisibility
        {
            get { return helpMailTextVisibility; }
            set { SetProperty(ref helpMailTextVisibility, value); }
        }
        public Visibility HelpLearningTextVisibility
        {
            get { return helpLearningTextVisibility; }
            set { SetProperty(ref helpLearningTextVisibility, value); }
        }
        public Visibility SecondButtonVisible
        {
            get { return advancedConfigEnabled; }
            set { SetProperty(ref advancedConfigEnabled, value); }
        }
        public Visibility NoConfiguration
        {
            get { return noConfiguration; }
            set { SetProperty(ref noConfiguration, value); }
        }
        public Visibility LearningConfTimeInterval
        {
            get { return learningConfTimeInterval; }
            set { SetProperty(ref learningConfTimeInterval, value); }
        }
        public bool WarningTextVisibility
        {
            get { return warningTextVisibility; }
            set { SetProperty(ref warningTextVisibility, value); }
        }
        public bool LearningSelectionWarning
        {
            get { return selectionWarning; }
            set { SetProperty(ref selectionWarning, value); }
        }
        public bool IsCorrectInput
        {
            get { return isCorrectInput; }
            set { SetProperty(ref isCorrectInput, value); }
        }


        #endregion

        // Initialize commands
        #region Command defintions

        public ICommand MainCommand { get; private set; }
        public ICommand SecondCommand { get; private set; }
        public ICommand AddMailCommand { get; private set; }
        public ICommand RemoveMailCommand { get; private set; }
        public ICommand OpenHelpCommand { get; private set; }
        public ICommand ShowWarningCommand { get; private set; }

        #endregion

        // Initialize local variables
        private ModelConfiguration config = ModelConfiguration.Instance;
        private int screen;

        private Visibility infoVisibility;
        private Visibility configVisibility;
        private Visibility devVisibility;
        private Visibility noConfiguration;
        private string mainButtonText;
        private string secondButtonText;
        private string selectedLearning;
        private string selectedWarnNiveau;
        private string learningTimeIntervalNumber;
        private Visibility helpMailTextVisibility;
        private Visibility helpLearningTextVisibility;
        private Visibility advancedConfigEnabled;
        private Visibility learningConfTimeInterval;
        private bool warningTextVisibility;
        private bool selectionWarning;
        private bool isCorrectInput;
        private string learningTimeInterval;
        private ObservableCollection<ModelEMail> mailAddresses = new ObservableCollection<ModelEMail>();
        private ModelMailFolder mailFolders = new ModelMailFolder("root");

        public VMWelcomeScreen()
        {
            screen = 0;

            // Initialize commands
            MainCommand = new RelayCommand(Main, null);
            SecondCommand = new RelayCommand(Second, null);
            AddMailCommand = new RelayCommand(AddMail, null);
            RemoveMailCommand = new RelayCommand(RemoveMail, null);
            OpenHelpCommand = new RelayCommand(OpenHelp, null);
            ShowWarningCommand = new RelayCommand(ShowWarning, null);

            // Initialize first texts
            ConfigVisibility = Visibility.Collapsed;
            DevVisibility = Visibility.Collapsed;
            InfoVisibility = Visibility.Visible;
            HelpMailTextVisibility = Visibility.Collapsed;
            HelpLearningTextVisibility = Visibility.Collapsed;
            NoConfiguration = Visibility.Collapsed;
            SecondButtonVisible = Visibility.Visible;
            MainButtonText = Properties.Resources.WELCOMESC_Next;
            SecondButtonText = Properties.Resources.WELCOMESC_Website;
            SelectedLearning = SelectableLearning[0];
            LearningTimeIntervalNumber = "1";
            LearningTimeInterval = SelectableLearningTimeInterval[1];
            SelectedWarnNiveau = SelectableWarnNiveau[1];

            // Get things from configuration
            foreach (string currentAccount in config.CurrentSessionAccounts)
            {
                MailAddresses.Add(new ModelEMail(currentAccount));
            }

            List<ModelMailFolder> alreadyKnown = new List<ModelMailFolder>();
            foreach (string oneFolder in config.FolderList)
            {
                string[] folderArray = oneFolder.Trim('\\').Split("\\".ToCharArray());

                for (int i = 1; i <= folderArray.Length-1; i++)
                {
                    int parentIndex = alreadyKnown.IndexOf(new ModelMailFolder(folderArray[i-1]));
                    if (parentIndex >= 0) 
                    {
                        ModelMailFolder temp = new ModelMailFolder(folderArray[i]);
                        if(alreadyKnown.IndexOf(temp) == -1) alreadyKnown[parentIndex].AddChild(temp);
                        alreadyKnown.Add(temp);
                    } 
                    else
                    {
                        ModelMailFolder temp = new ModelMailFolder(folderArray[i-1]);
                        alreadyKnown.Add(temp);
                        MailFolders.AddChild(temp);
                        temp.AddChild(new ModelMailFolder(folderArray[i]));
                    }

                    
                    
                }
            }
        }

        public void Main(Object obj = null)
        {
            switch (screen)
            {
                case 0:
                    ConfigVisibility = Visibility.Visible;
                    InfoVisibility = Visibility.Collapsed;
                    DevVisibility = Visibility.Collapsed;
                    MainButtonText = Properties.Resources.WELCOMESC_Finish;
                    SecondButtonText = Properties.Resources.WELCOMESC_Developer;

                    // Disable config if necessary
                    object blocked;
                    config.RegistryKeys.TryGetValue("isConfigBlocked", out blocked);
                    if ((int)blocked == 0)
                    {
                        NoConfiguration = Visibility.Collapsed; 
                        WarningTextVisibility = false;
                    }
                    else
                    {
                        NoConfiguration = Visibility.Visible;
                        WarningTextVisibility = true;
                    }
                        
                    object advanced;
                    config.RegistryKeys.TryGetValue("advancedConfigEnabled", out advanced);
                    SecondButtonVisible = ((int)advanced == 0) ? Visibility.Collapsed : Visibility.Visible;

                    screen += 1;
                    break;
                case 1:
                case 2:
                    Finish();
                    (obj as Window).Close();

                    break;
            }
        }

        public void Second(Object obj = null)
            {
            switch (screen)
            {
                case 0:
                    Process.Start("https://github.com/hjunker/FinalFrontier");

                    break;
                case 1:
                    ConfigVisibility = Visibility.Collapsed;
                    InfoVisibility = Visibility.Collapsed;
                    DevVisibility = Visibility.Visible;
                    MainButtonText = Properties.Resources.WELCOMESC_Finish;
                    SecondButtonText = Properties.Resources.WELCOMESC_Settings;

                    screen += 1;
                    break;
                case 2:
                    SettingsScreen settingsSc = new SettingsScreen();
                    settingsSc.Show();
                    (obj as Window).Close();

                    break;
            }
        }

        public void AddMail(Object obj = null)
        {
            mailAddresses.Add(new ModelEMail());
        }

        public void RemoveMail(Object obj = null)
        {
            mailAddresses.Remove(((obj as ListView).SelectedItem) as ModelEMail);
        }

        public void OpenHelp(Object obj = null)
        {
            switch(obj as string) {
                case "Mail": 
                    HelpMailTextVisibility = HelpMailTextVisibility.Equals(Visibility.Collapsed) ? Visibility.Visible : Visibility.Collapsed;
                    break;
                case "Learning":
                    HelpLearningTextVisibility = HelpLearningTextVisibility.Equals(Visibility.Collapsed) ? Visibility.Visible : Visibility.Collapsed;
                    break;
            }
        }

        public void ShowWarning(Object obj = null)
        {
            WarningTextVisibility = WarningTextVisibility ? false : true;
        }


        private List<string> badFolders = new List<string>();

        private void Finish()
        {
            if (NoConfiguration.Equals(Visibility.Visible)) return;

            // Get own addresses
            List<string> tempAddresses = new List<string>();
            foreach (ModelEMail address in MailAddresses)
            {
                if (address.IsCorrectEMail && !address.IsDefaultEMail)
                {
                    tempAddresses.Add(address.ToString());
                }
            }
            // Get bad folders
            ExtractSelectedInfo(MailFolders);

            // Get warn niveau
            int suspiciousScore = 0;
            if (SelectedWarnNiveau.Equals(SelectableWarnNiveau[0])) suspiciousScore = -20;
            else if (SelectedWarnNiveau.Equals(SelectableWarnNiveau[1])) suspiciousScore = -40;
            else if (SelectedWarnNiveau.Equals(SelectableWarnNiveau[2])) suspiciousScore = -60;

            config.UpdateConfigFile(tempAddresses, suspiciousScore, badFolders);
        }

        private void ExtractSelectedInfo(ModelMailFolder oneFolder)
        {
            if (!oneFolder.IsChecked) badFolders.Add(oneFolder.FolderName);
            foreach(ModelMailFolder child in oneFolder.Children) ExtractSelectedInfo(child);
        }
    }
}
