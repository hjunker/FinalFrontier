namespace FinalFrontier
{
    public class CheckResult
    {
        public string id;
        public string fragment = "";
        public string ioc = "";
        public int score = 0;

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
