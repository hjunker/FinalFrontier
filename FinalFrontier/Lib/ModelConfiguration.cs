using Microsoft.Office.Interop.Outlook;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;


namespace FinalFrontier
{
    public sealed class ModelConfiguration
    {
        private static ModelConfiguration instance = null;

        public static ModelConfiguration Instance
        {
            get
            {
                if (instance == null)
                    instance = new ModelConfiguration();
                return instance;
            }
        }

        // All variables getting from the configuration
        public List<string> LinkShorteners { get; private set; }
        public List<string> BadTlds { get; private set; }
        public List<string> Keywords { get; private set; }
        public List<string> DocExtensions { get; private set; }
        public List<string> ExeExtensions { get; private set; }
        public List<string> ImgExtensions { get; private set; }
        public List<string> BadExtensions { get; private set; }
        public List<string> BadHashesSha256 { get; private set; }
        public List<string> Freemailers { get; private set; }
        public List<string> Whitelist { get; private set; }
        public List<string> Lookalikes { get; private set; }
        public List<string> OwnAddresses { get; private set; }
        public int SuspiciousScore { get; private set; }
        public string ReportAddress { get; private set; }
        public string[] BadFolders { get; private set; }
        public string LearningPath { get; private set; }

        // All variables from Registry
        public Dictionary<string, Object> RegistryKeys { get; private set; }

        // Some other variables mainly from current session information
        public List<string> FolderList = new List<string>();
        public string[] CurrentSessionAccounts { get; private set; }
        public string CultureInfo { get; private set; }


        private Application outlook;

        private ModelConfiguration()
        {
            try
            {
                // Get the directory paths
                LearningPath = ConfigurationManager.AppSettings["saveLearned"];
                if (LearningPath == "")
                    LearningPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\FinalFrontier";

                // Parse lists to be checked
                LinkShorteners = ConfigurationManager.AppSettings["linkshorteners"].ToLower().Replace(" ", String.Empty).Split(',').ToList();
                BadTlds = ConfigurationManager.AppSettings["badtlds"].ToLower().Replace(" ", String.Empty).Split(',').ToList();
                Keywords = ConfigurationManager.AppSettings["keywords"].ToLower().Replace(" ", String.Empty).Split(',').ToList();
                DocExtensions = ConfigurationManager.AppSettings["docextensions"].ToLower().Replace(" ", String.Empty).Split(',').ToList();
                ExeExtensions = ConfigurationManager.AppSettings["exeextensions"].ToLower().Replace(" ", String.Empty).Split(',').ToList();
                ImgExtensions = ConfigurationManager.AppSettings["imgextensions"].ToLower().Replace(" ", String.Empty).Split(',').ToList();
                BadExtensions = ConfigurationManager.AppSettings["badextensions"].ToLower().Replace(" ", String.Empty).Split(',').ToList();
                BadHashesSha256 = ConfigurationManager.AppSettings["badhashessha256"].ToLower().Replace(" ", String.Empty).Split(',').ToList();
                Freemailers = ConfigurationManager.AppSettings["freemailers"].ToLower().Replace(" ", String.Empty).Split(',').ToList();
                Whitelist = ConfigurationManager.AppSettings["whitelist"].ToLower().Replace(" ", String.Empty).Split(',').ToList();
                Lookalikes = ConfigurationManager.AppSettings["lookalikes"].ToLower().Replace(" ", String.Empty).Split(',').ToList();

                // Get checking details
                BadFolders = ConfigurationManager.AppSettings["notLearnFrom"].ToLower().Replace(" ", String.Empty).Split(',');
                OwnAddresses = ConfigurationManager.AppSettings["ownaddresses"].ToLower().Replace(" ", String.Empty).Split(',').ToList();
                ReportAddress = ConfigurationManager.AppSettings["reportAddress"];
                SuspiciousScore = int.Parse(ConfigurationManager.AppSettings["isSuspiciousScore"]);

                RegistryKeys = new Dictionary<string, object>();
            }
            catch (ConfigurationErrorsException)
            {
                WarningScreen warningSc = new WarningScreen("Could not read configuration file app.config / " + AppDomain.CurrentDomain.SetupInformation.ConfigurationFile + "\n\nCAUTION: FINALFRONTIER WILL NOT BE FUNCTIONING PROPERLY!!!");
                warningSc.Show();
            }
            catch (FormatException)
            {
                WarningScreen warningSc = new WarningScreen("Suspicoius Score value is not set properly. Only insert integer values.");
                warningSc.Show();
            }
            catch (ArgumentNullException)
            {
                WarningScreen warningSc = new WarningScreen("Some argument in the app settings is NULL. Please configure it properly.");
                warningSc.Show();
            }
        }

        public void SetStartupInformation(Application outl, string currentCulture)
        {
            outlook = outl;

            // Get the accounts of the current outlook session -- primarly for welcome
            List<string> currentSessionAccounts = new List<string>();
            Accounts accounts = outlook.Session.Accounts;
            foreach (Account account in accounts)
            {
                currentSessionAccounts.Add(account.SmtpAddress);
            }
            CurrentSessionAccounts = currentSessionAccounts.ToArray();

            CultureInfo = currentCulture; //TODO: Vergleiche mit in config gespeicherter culture info

            GetRegKeyConfig();

            foreach(Folder folder in outlook.Session.Folders)
            {
                GetFolderList(folder);
            }
        }

        public void UpdateConfigFile(List<string> ownAddresses = null, int susScore = 0, List<string> badFolders = null)
        {
            SuspiciousScore = susScore;
            OwnAddresses = ownAddresses;
            BadFolders = badFolders.ToArray();

            string badFolderString = "";
            string ownAddressesString = "";
            foreach (string one in BadFolders) badFolderString += one + ", ";
            foreach (string one in ownAddresses) ownAddressesString += one + ", ";


            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings["notLearnFrom"].Value = badFolderString;
            config.AppSettings.Settings["ownaddresses"].Value = ownAddressesString;
            config.AppSettings.Settings["isSuspiciousScore"].Value = SuspiciousScore.ToString();
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        public void UpdateRegKeyConfig(string name = null, Object regValue = null, RegistryValueKind valuekind = RegistryValueKind.Unknown)
        {
            if ((name == null) != (regValue == null))
            {
                throw new ArgumentException("You either must set both or no parameters. Just one is not supported.");
            }

            RegistryKey FinalFrontier = Registry.CurrentUser.OpenSubKey("FinalFrontier", true);

            if (name == null)
            {
                FinalFrontier.SetValue("seenWelcomeScreen", 1, RegistryValueKind.DWord);
                FinalFrontier.SetValue("isConfigBlocked", 0, RegistryValueKind.DWord);
                FinalFrontier.SetValue("advancedConfigEnabled", 1, RegistryValueKind.DWord);
                GetRegKeyConfig();
            }
            else
            {
                FinalFrontier.SetValue(name, regValue, valuekind);
                if(RegistryKeys.TryGetValue(name, out _))
                {
                    RegistryKeys.Remove(name);
                }
                RegistryKeys.Add(name, regValue);
            }
        }

        private void GetRegKeyConfig()
        {
            RegistryKey FinalFrontier = Registry.CurrentUser.OpenSubKey("FinalFrontier") == null ? Registry.CurrentUser.CreateSubKey("FinalFrontier") : Registry.CurrentUser.OpenSubKey("FinalFrontier");

            RegistryKeys = new Dictionary<string, object>();
            foreach (string subKeys in FinalFrontier.GetValueNames())
            {
                RegistryKeys.Add(subKeys, FinalFrontier.GetValue(subKeys));
            }

            //Check the correct key types
            object seen;
            RegistryKeys.TryGetValue("seenWelcomeScreen", out seen);
            if (seen == null) UpdateRegKeyConfig("seenWelcomeScreen", 1, RegistryValueKind.DWord);
            else if (!(seen is int)) throw new ArgumentException("'seenWelcomeScreen' must be an DWord integer value. To fix this, please open RegistryEditor and delete this key.");
            object blocked;
            RegistryKeys.TryGetValue("isConfigBlocked", out blocked);
            if (blocked == null) UpdateRegKeyConfig("isConfigBlocked", 0, RegistryValueKind.DWord);
            else if (!(blocked is int)) throw new ArgumentException("'isConfigBlocked' must be an DWord integer value. To fix this, please open RegistryEditor and delete this key.");
            object advanced;
            RegistryKeys.TryGetValue("advancedConfigEnabled", out advanced);
            if (advanced == null) UpdateRegKeyConfig("advancedConfigEnabled", 1, RegistryValueKind.DWord);
            else if (!(advanced is int)) throw new ArgumentException("'advancedConfigEnabled' must be an DWord integer value. To fix this, please open RegistryEditor and delete this key.");
        }
        
        

        public void GetFolderList(Folder folder)
        {

            Folders childFolders = folder.Folders;
            if (childFolders.Count > 0)
            {
                foreach (Folder childFolder in childFolders)
                {
                    FolderList.Add(childFolder.FolderPath);
                    GetFolderList(childFolder);
                }
            }
        }
    }
}
