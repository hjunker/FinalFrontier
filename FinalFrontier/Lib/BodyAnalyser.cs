using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace FinalFrontier
{
    class BodyAnalyser : BaseAnalyse
    {
        private int score;
        public bool HasLink { get; set; }

        public override int Score { get { return score; }  }

        public override List<CheckResult> Analyze(object mailBody)
        {
            var results = new List<CheckResult>();
            var checkMethods = new CheckMethods();

            var links = LinksFind(mailBody as string);
            Action<CheckResult> add = x => { if (x != null) results.Add(x); };
            Action<List<CheckResult>> addRange = x =>
            {
                if (x != null)
                {
                    x.RemoveAll(y => y == null);
                    results.AddRange(x);
                }
            };

            if (links.Any())
            {
                foreach (string link in links)
                {
                    addRange(checkMethods.CheckLinkShorteners("Link-Shortener", link));

                    add(checkMethods.CheckBadTld("Link-badTLD", link));

                    // check for keywords in links
                    addRange(checkMethods.CheckKeywords("Link-Keyword", link));
                }

                HasLink = true;
                score = results.Sum(x => x.score);
            }
            return results;
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
