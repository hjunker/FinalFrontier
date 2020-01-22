using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;

namespace FinalFrontier
{
    class BodyAnalyser
    {
        public List<CheckResult> AnalyzeBody(string mailBody)
        {
            List<CheckResult> result = new List<CheckResult>();
            var links = LinksFind(mailBody);

            if (links.Count() > 0)
            {
                foreach (string link in links)
                {
                    result.AddRange(CheckMethods.checkLinkShorteners("Link-Shortener", link));

                    result.AddRange(CheckMethods.checkBadTld("Link-badTLD", link));

                    // check for keywords in links
                    result.AddRange(CheckMethods.checkKeywords("Link-Keyword", link));
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
    }
}
