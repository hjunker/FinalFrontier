using System;
using System.Collections.Generic;
using System.Configuration;
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
        public List<string> OwnAddresses { get; set; }
        public int SuspiciousScore { get; private set; }
        public string ReportAddress { get; private set; }
        public string[] BadFodlers { get; private set; }
        public string LearningPath { get; private set; }

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
                BadFodlers = ConfigurationManager.AppSettings["notLearnFrom"].ToLower().Replace(" ", String.Empty).Split(',');
                OwnAddresses = ConfigurationManager.AppSettings["ownaddresses"].ToLower().Replace(" ", String.Empty).Split(',').ToList();
                ReportAddress = ConfigurationManager.AppSettings["reportAddress"];
                SuspiciousScore = int.Parse(ConfigurationManager.AppSettings["isSuspiciousScore"]);
            }
            catch (ConfigurationErrorsException) {
                WarningScreen warningSc = new WarningScreen("Could not read configuration file app.config / " + AppDomain.CurrentDomain.SetupInformation.ConfigurationFile + "\n\nCAUTION: FINALFRONTIER WILL NOT BE FUNCTIONING PROPERLY!!!");
                warningSc.Show();
            } 
            catch(FormatException) {
                WarningScreen warningSc = new WarningScreen("Suspicoius Score value is not set properly. Only insert integer values.");
                warningSc.Show();
            }
            catch (ArgumentNullException) {
                WarningScreen warningSc = new WarningScreen("Some argument in the app settings is NULL. Please configure it properly.");
                warningSc.Show();
            }
        }
    }
}
