using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Resources;
using System.Windows;
using System.Windows.Data;
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
        public ObservableCollection<ModelEMail>MailAddresses
        {
            get { return mailAddresses; }
            set { SetProperty(ref mailAddresses, value); }
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

        #endregion

        // Initialize commands
        #region Command defintions

        public ICommand MainCommand { get; private set; }
        public ICommand SecondCommand { get; private set; }
        public ICommand AddMailCommand { get; private set; }
        public ICommand OpenMailCommand { get; private set; }
        public ICommand OpenWarningCommand { get; private set; }

        #endregion

        // Initialize local variables
        private ModelConfiguration config = ModelConfiguration.Instance;
        private int screen;

        private Visibility infoVisibility;
        private Visibility configVisibility;
        private Visibility devVisibility;
        private string mainButtonText;
        private string secondButtonText;
        private ObservableCollection<ModelEMail> mailAddresses = new ObservableCollection<ModelEMail>();

        public VMWelcomeScreen()
        {
            screen = 0;

            // Initialize commands
            MainCommand = new RelayCommand(Main, null);
            SecondCommand = new RelayCommand(Second, null);
            AddMailCommand = new RelayCommand(AddMail, null);
            OpenMailCommand = new RelayCommand(OpenMail, null);
            OpenWarningCommand = new RelayCommand(OpenWarning, null);

            // Initialize first texts
            ConfigVisibility = Visibility.Collapsed;
            DevVisibility = Visibility.Collapsed;
            InfoVisibility = Visibility.Visible;
            MainButtonText = Properties.Resources.WELCOMESC_Next;
            SecondButtonText = Properties.Resources.WELCOMESC_Website;
            
            MailAddresses.Add(new ModelEMail("example@me.com")); //TODO: Get from Outlook 
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
            mailAddresses.Add(new ModelEMail("example@me.com"));
        }

        public void OpenMail(Object obj = null)
        {

        }

        public void OpenWarning(Object obj = null)
        {

        }

        private void Finish()
        {
            //TODO
            
            List<string> tempAddresses = new List<string>();
            foreach (ModelEMail address in MailAddresses) {
                tempAddresses.Add(address.ToString());
            }
            
            config.OwnAddresses = tempAddresses;
        }
    }


}
