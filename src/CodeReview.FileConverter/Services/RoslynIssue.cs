namespace ReviewItEasy.FileConverter.Services
{
    public class RoslynIssue
    {
        public string RuleId { get; set; }
        public string Level { get; set; }
        public string Message { get; set; }
        public RoslynIssueLocation[] Locations { get; set; }
    }
}