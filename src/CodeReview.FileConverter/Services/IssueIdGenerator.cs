namespace GodelTech.CodeReview.FileConverter.Services
{
    public class IssueIdGenerator : IIssueIdGenerator
    {
        private long _currentId = 1;
        
        public long GetNext()
        {
            return _currentId++;
        }
    }
}