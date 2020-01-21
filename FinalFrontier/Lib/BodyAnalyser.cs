using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;

namespace FinalFrontier
{
    class BodyAnalyser
    {
        private List<string> linkshorteners;
        private List<string> badtlds;
        private List<string> keywords;

        public BodyAnalyser()
        {
            try
            {
                linkshorteners = ConfigurationManager.AppSettings["linkshorteners"].Split(',').ToList();
                badtlds = ConfigurationManager.AppSettings["badtlds"].Split(',').ToList();
                keywords = ConfigurationManager.AppSettings["keywords"].Split(',').ToList();
            }
            catch (System.Exception)
            {
                System.Windows.Forms.MessageBox.Show("Could not read configuration file app.config");
            }
        }

        public List<CheckResult> AnalyzeBody(string mailBody)
        {
            List<CheckResult> result = new List<CheckResult>();
            var links = LinksFind(mailBody);

            if (links.Count() > 0)
            {
                foreach (string link in links)
                {
                    result.AddRange(checkLinkShorteners("Link-Shortener", link));

                    result.AddRange(checkBadTld("Link-badTLD", link));

                    // check for keywords in links
                    result.AddRange(checkKeywords("Link-Keyword", link));
                }
            }
            return result;
        }

        public List<string> LinksFind(string file)
        {
            // https://www.dotnetperls.com/scraping-html
            List<string> list = new List<string>();

            // 1.
            // Find all matches in file.
            MatchCollection m1 = Regex.Matches(file, @"(<a.*?>.*?</a>)", RegexOptions.Singleline);

            // 2.
            // Loop over each match.
            foreach (Match m in m1)
            {
                string value = m.Groups[1].Value;

                // 3.
                // Get href attribute.
                Match m2 = Regex.Match(value, @"href=\""(.*?)\""", RegexOptions.Singleline);
                if (m2.Success)
                {
                    list.Add(m2.Groups[1].Value);
                }
            }
            return list;
        }

        private List<CheckResult> checkLinkShorteners(string id, string instr)
        {
            var results = new List<CheckResult>();

            foreach (string shortener in linkshorteners)
            {
                if (instr.IndexOf(shortener) > 0)
                {
                    results.Add(new CheckResult(id, shortener, instr, -20));
                }
            }
            return results;
        }

        private List<CheckResult> checkBadTld(string id, string instr)
        {
            var result = new List<CheckResult>();
            if (instr == null) 
                return null;
            foreach (string badtld in badtlds)
            {
                if (instr.EndsWith(badtld))
                {
                    result.Add(new CheckResult(id, badtld, instr, -20));
                }
            }
            return result;
        }

        private List<CheckResult> checkKeywords(string id, string instr)
        {
            var result = new List<CheckResult>();
            foreach (string key in keywords)
            {
                if (instr.EndsWith(key))
                {
                    result.Add(new CheckResult(id, key, instr, -20));
                }
            }
            return result;
        }
    }
}
