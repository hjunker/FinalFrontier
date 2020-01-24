namespace FinalFrontier
{
    public class CheckResult
    {
        public string id { get; private set; }
        public string fragment { get; private set; }
        public string ioc { get; private set; }
        public int score { get; private set; }

        public CheckResult(string id, string fragment, string ioc, int score)
        {
            this.id = id;
            this.fragment = fragment;
            this.ioc = ioc;
            this.score = score;
        }

        public override string ToString()
        {
            return "Score: " + score + ", ID: " + id + ", FRAGMENT: " + fragment + ", IOC: " + ioc;
        }
    }
}
