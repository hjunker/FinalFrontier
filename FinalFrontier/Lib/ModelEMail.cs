using System;
using System.Globalization;
using System.Text.RegularExpressions;


namespace FinalFrontier
{
    class ModelEMail : VMBase
    {
        public bool IsCorrectEMail
        {
            get { return isCorrectMail; }
            set { SetProperty(ref isCorrectMail, value);}
        }
        private bool isCorrectMail = false;

        public string MailAddress
        {
            get { return mailAddress; }
            set
            {
                IsCorrectEMail = IsValidEmail(value) ? true : false;
                SetProperty(ref mailAddress, value);
            }
        }
        string mailAddress;

        public ModelEMail(string mail)
        {
            MailAddress = mail;
        }

        private bool IsValidEmail(string toCheck)
        {
            if (string.IsNullOrWhiteSpace(toCheck))
                return false;
            
            try
            { 
                toCheck = Regex.Replace(toCheck, @"(@)(.+)$", DomainMapper,
                                  RegexOptions.None, TimeSpan.FromMilliseconds(200));

                string DomainMapper(Match match)
                {
                    // Use IdnMapping class to convert Unicode domain names.
                    var idn = new IdnMapping();

                    // Pull out and process domain name (throws ArgumentException on invalid)
                    var domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException e)
            {
                return false;
            }
            catch (ArgumentException e)
            {
                return false;
            }

            try
            {
                return Regex.IsMatch(toCheck,
                    @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                    @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        public override string ToString()
        {
            return MailAddress;
        }
    }
}
